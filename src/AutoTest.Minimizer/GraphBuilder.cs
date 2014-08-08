using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Graphs;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    class GraphBuilder : ITestSearchStrategy
    {
        private readonly bool _debugTreeWalks;

        public event EventHandler<MessageArgs> DebugMessage;

        private readonly CouplingCache _cache;
        private readonly IEnumerable<ITestIdentifier> _testStrategies;
        private readonly IInterfaceFollowingStrategy _interfaceFollowingStrategy;
        private readonly int _numthreads;

        public GraphBuilder(CouplingCache cache, IEnumerable<ITestIdentifier> testStrategies, IInterfaceFollowingStrategy interfaceFollowingStrategy, bool isDebug, int numThreads)
        {
            _debugTreeWalks = isDebug;
            _numthreads = numThreads;
            if (isDebug) _numthreads = 1;
            _cache = cache;
            _interfaceFollowingStrategy = interfaceFollowingStrategy;
            _testStrategies = testStrategies;
        }

        protected void InvokeMinimizerMessage(MessageType m, string message)
        {
            var handler = DebugMessage;
            if (handler != null) handler(this, new MessageArgs(m, message));
        }

        public AffectedGraph GetAffectedGraphForChangeSet(IEnumerable<Change<MethodReference>> changes)
        {
            InvokeMinimizerMessage(MessageType.Debug, "getting tests for changes");
            var start = DateTime.Now;
            var graph = new AffectedGraph();
            var breadCrumbs = new Dictionary<string, bool>();
            var affectedItems = GetAffectedItems(changes, graph);
            var rets = affectedItems.ForkAndJoinTo(_numthreads, x =>
                                                                    {
                                                                        var tmp = new AffectedGraph();
                                                                        FillAfferentGraph(x.Member, breadCrumbs,
                                                                                          tmp, null, 0, null, false,
                                                                                          new List<string>(),
                                                                                          GenericContext.Empty(), false);
                                                                        return tmp;
                                                                    });
            InvokeMinimizerMessage(MessageType.Debug, "there are " + rets.Count() + " graphs returned. Combining.");
            foreach(var g in rets)
            {
                if (g != null)
                {
                    InvokeMinimizerMessage(MessageType.Debug,
                                           "there are " + g.AllNodes().Count() + " Nodes in graph. TOTAL=" +
                                           graph.AllNodes().Count());
                    graph = graph.Merge(g);
                }
            }
            InvokeMinimizerMessage(MessageType.Debug, "there are " + rets.Count() + " nodes in combined graph."); 
            var end = DateTime.Now;
            InvokeMinimizerMessage(MessageType.Debug, "took " + (end - start) + " to walk graph");
            return graph;
        }

        public AffectedGraph GetCouplingGraphFor(string cacheName)
        {
            var affected = new List<ChangeContext>();
            var graph = new AffectedGraph();
            var forwardCutOffPoints = new Dictionary<string, bool>();
            var backCutOffPoints = new Dictionary<string, bool>();
            TypeDefinition scope = null;
            GetEfferentGraph(null, cacheName, forwardCutOffPoints, affected, graph, true, scope);
            WriteWalkerDebug("got " + affected + " nodes for " + cacheName, 0);
            var rets = affected.ForkAndJoinTo(_numthreads, x =>
            {
                FillAfferentGraph(x.Member, backCutOffPoints, graph, null, 0, null, false, new List<string>(), GenericContext.Empty(), false); //gets all tests Ca
                return true;
            });
            return graph;
        }

        static IEnumerable<TestDescriptor> GetTestsInGraph(AffectedGraph graph)
        {
            return graph.AllNodes().Where(entry => entry.IsTest).SelectMany(entry => entry.TestDescriptors);
        }

        private IEnumerable<ChangeContext> GetAffectedItems(IEnumerable<Change<MethodReference>> changes, AffectedGraph graph)
        {
            var affected = new List<ChangeContext>();
            foreach (var change in changes)
            {
                var forwardCutOffPoints = new Dictionary<string, bool>();
                if (change.ChangeType == ChangeType.Remove) continue;
                change.ItemChanged.GetCacheName();
                var scope = change.ItemChanged.DeclaringType.ThreadSafeResolve();
                GetEfferentGraph(null, change.ItemChanged.GetCacheName(), forwardCutOffPoints, affected, graph, true, scope);
            }
            return affected;
        }

        private void GetEfferentGraph(string parent, string fullName, IDictionary<string, bool> cutOffs, List<ChangeContext> found, AffectedGraph graph, bool isRootNode, TypeDefinition originalContext)
        {
            var entry = _cache.TryGetEfferentCouplingNode(fullName);
            var afferent = _cache.TryGetAfferentCouplingNode(fullName);
            if (isRootNode)
            {
                entry = entry ?? afferent; 
                found.Add(new ChangeContext(fullName, null));
                graph.AddNode(GetGraphNode(fullName, entry, true, true));
                if (entry != null && entry.MemberReference != null)
                {
                    var refer = entry.MemberReference as MethodReference;
                    if (refer != null && _testStrategies.IsTest(refer))
                    {
                        return;
                    }
                }
            }
            //TODO Greg this hamstrings some stuff but should work better in most cases
            //I think we only really need statics past here but need to look it over a bit.
            return;

            WriteWalkerDebug("checking " + fullName, 0);
            if (entry == null)
            {
                WriteWalkerDebug("efferent entry is null", 0);
                if (afferent != null && afferent.MemberReference is MethodReference) return; //Leaf node to a method call
            }
            if(afferent == null)
            {
                WriteWalkerDebug("afferent entry is null", 0);
            }
            if (afferent != null)
            {
                var memref = afferent.MemberReference as MethodReference;
                MethodDefinition def = null;
                if (memref != null)
                {
                    def = memref.ThreadSafeResolve();
                }
                if (!isRootNode && def != null)
                {
                    if (def.IsGetter) return;
                    if (def.DeclaringType.IsInterface) return; //don't recurse forward on interface calls
                    if (!def.HasThis) return;
                }
                var reference = afferent.MemberReference as FieldReference; //Leaf node to a field reference
                if (reference != null)
                {
                    graph.AddNode(GetGraphNode(fullName, afferent, isRootNode, false));
                    if (parent != null)
                        graph.AddConnection(parent, fullName, true);

                    found.Add(new ChangeContext(fullName, originalContext));
                }                    
            }

            if (entry == null) return;
            graph.AddNode(GetGraphNode(fullName, entry, isRootNode, false));
            if (parent != null)
                graph.AddConnection(parent, fullName, true);
            foreach (var current in entry.Couplings)
            {
                if (cutOffs.ContainsKey(current.To)) continue;
                if (current.IgnoreWalk) continue;
                if (current.ActualReference is MethodReference && !current.IsSelfCall) continue;
                cutOffs.Add(current.To, true);
                GetEfferentGraph(fullName, current.To, cutOffs, found, graph, false, originalContext);
            }
        }

        private AffectedGraphNode GetGraphNode(string fullName, CouplingCacheNode afferent, bool isRootNode, bool isChange)
        {
            if (afferent == null) return new AffectedGraphNode(fullName, false, false, false, "", fullName, "", "", new List<TestDescriptor>(), false, false, 0);
            var r = afferent.MemberReference;
            var type = r.DeclaringType.ThreadSafeResolve();
            var isTest = false;
            List<TestDescriptor> descriptors = null;
            isTest = _testStrategies.IsTest(r);
            var shortName = r.DeclaringType.Name + "::" + r.Name;
            if (isTest)
            {
                var method = r as MethodReference;
                if (method != null)
                {
                    var tmp = _testStrategies.GetSpecificallyMangledName(method);
                    if (tmp != null)
                    {
                        fullName = tmp;
                        shortName = HackName(fullName);
                    }
                }
                descriptors = new List<TestDescriptor>(_testStrategies.GetDescriptorsFor(r));
            }
            
            var inTestAssembly = _cache.AssemblyHasTests(type.Module.FullyQualifiedName);
            return new AffectedGraphNode(shortName, type.IsInterface, isTest, isRootNode, r.Name, fullName, type.Module.FullyQualifiedName, type.FullName, descriptors, isChange, inTestAssembly, afferent.Complexity);
        }

        private string HackName(string fullName)
        {
            if (String.IsNullOrEmpty(fullName)) return "";
            var idx = fullName.IndexOf("::");
            if(idx == -1) return fullName;
            var tmp = fullName.Substring(idx + 2, fullName.Length - idx- 2);
            tmp = "It " + tmp.Replace("_", " ");
            return tmp;
        }

        private void FillAfferentGraph(string fullName, Dictionary<string, bool> cutPoints, AffectedGraph graph, MemberReference parent, int depth, TypeDefinition inheritedTypeContext, bool inVirtual, List<string> breadCrumbs, GenericContext genericContext, bool inSelf)
        { 
            WriteWalkerDebug(fullName, depth);
            if (TryBreadCrumbsPrune(fullName, depth, breadCrumbs))
            {
                if (parent != null)
                {
                    WriteWalkerDebug("truncating but adding connection to " + fullName, depth);
                    graph.AddConnection(parent.GetCacheName(), fullName, false);
                }
                return;
            }
            breadCrumbs.Add(fullName);
            if (TryCutPointPrune(fullName, depth, cutPoints))
            {
                if (parent != null)
                {

                    WriteWalkerDebug("bread crumbs truncating but adding connection to " + fullName, depth);
                    graph.AddConnection(parent.GetCacheName(), fullName, false);
                }
                return;
            }
            
            var efferentEntry = _cache.TryGetEfferentCouplingNode(fullName);
            var afferentEntry = _cache.TryGetEfferentCouplingNode(fullName);
            MethodDefinition currentDefinition = null;
            if (efferentEntry != null)
            {
                TypeReference parentType = null;
                if (parent != null) parentType = parent.DeclaringType;
                var definition = efferentEntry.MemberReference.DeclaringType.ThreadSafeResolve();
                if (TryInterfacePrune(fullName, depth, breadCrumbs, efferentEntry, parentType, definition)) return;
                if (TryInheritancePrune(fullName, depth, breadCrumbs, inVirtual, definition, inheritedTypeContext)) {return;}
                inheritedTypeContext = GetNewTypeContext(inheritedTypeContext, definition);
                currentDefinition = efferentEntry.MemberReference as MethodDefinition;
                if (currentDefinition != null)
                {
                    if(currentDefinition.DeclaringType == null) {}
                    if (parent is FieldReference && currentDefinition.IsConstructor)
                    {
                        breadCrumbs.Remove(fullName);
                        return;
                    }
                }
            }
            var touse = afferentEntry ?? efferentEntry;
            WriteWalkerDebug("adding node " + fullName, depth);
            var newnode = GetGraphNode(fullName, touse, parent == null, false);
            graph.AddNode(newnode);
            if (parent != null)
            {
                 graph.AddConnection(parent.GetCacheName(), newnode.FullName, false);    
            }
            if(currentDefinition != null)
                RecurseSynonyms(depth, cutPoints, inheritedTypeContext, currentDefinition, graph, breadCrumbs, genericContext);
            RecurseAfferentCouplings(fullName, depth, cutPoints, inVirtual, inheritedTypeContext, graph, parent, breadCrumbs, genericContext);
            breadCrumbs.Remove(fullName);
        }

        private void RecurseAfferentCouplings(string fullName, int depth, Dictionary<string, bool> history, bool inInheritanceChain, TypeDefinition inheritedTypeContext, AffectedGraph graph, MemberReference parent, List<string> breadCrumbs, GenericContext genericContext)
        {
            var afferentEntry = _cache.TryGetAfferentCouplingNode(fullName);

            if (afferentEntry == null) return;
            genericContext.SetIndirectConstraintsOn(afferentEntry.MemberReference, parent, inInheritanceChain);
            if (!inInheritanceChain && genericContext.IsClear())
            {
                lock (history) //double lock, TryCutPointPrune already did this
                {
                    if (!history.ContainsKey(fullName))
                    {
                        history.Add(fullName, true);
                    }
                }
            }
            foreach (var current in afferentEntry.Couplings)
            {
                if (current.IgnoreWalk) continue;
                var proposed = genericContext.GetGenericContextOf(current.ActualReference);
                if (!genericContext.CanTransitionTo(proposed))
                {
                    WriteWalkerDebug("Generics truncate", depth);
                    continue;
                }
                var newContext = genericContext.TransitionTo(proposed);
                FillAfferentGraph(current.To, history, graph, afferentEntry.MemberReference, depth + 1, inheritedTypeContext, inInheritanceChain && current.IsSelfCall, breadCrumbs, newContext, current.IsSelfCall);
            }
        }

        private bool TryBreadCrumbsPrune(string fullName, int depth, List<string> breadCrumbs)
        {
            if (breadCrumbs.Contains(fullName))
            {
                WriteWalkerDebug("Bread Crumbs contains, truncating.", depth);
                return true;
            }
            return false;
        }

        private bool TryCutPointPrune(string fullName, int depth, Dictionary<string, bool> cutPoints)
        {
            if (cutPoints.ContainsKey(fullName))
            {
                WriteWalkerDebug("Cut offs contains, truncating.", depth);
                return true;
            }
            return false;
        }

        private bool TryInterfacePrune(string fullName, int depth, List<string> breadCrumbs, CouplingCacheNode efferentEntry, TypeReference parentType, TypeDefinition definition)
        {
            var def = parentType.ThreadSafeResolve();
            if (parentType != null && !definition.IsInterface && def!=null && def.IsInterface &&
                !_interfaceFollowingStrategy.ShouldContinueAfter(efferentEntry.MemberReference))
            {
                WriteWalkerDebug("I should not follow interface to here.", depth);
                breadCrumbs.Remove(fullName);
                return true;
            }
            return false;
        }

        private bool TryInheritancePrune(string fullName, int depth, List<string> breadCrumbs, bool inVirtual, TypeDefinition definition, TypeDefinition inheritedTypeContext)
        {
            if (inVirtual && !definition.IsBaseTypeOf(inheritedTypeContext)) { 
                WriteWalkerDebug("Can't reach here with inheritance", depth);
                breadCrumbs.Remove(fullName);
                return true;
            }
            return false;
        }

        private void WriteWalkerDebug(string message, int depth)
        {
             if(_debugTreeWalks) InvokeMinimizerMessage(MessageType.Debug, new string(' ', depth) + message);
        }

        private void RecurseSynonyms(int depth, Dictionary<string, bool> history, TypeDefinition inheritedTypeContext, MethodDefinition currentDefinition, AffectedGraph graph, List<string> breadCrumbs, GenericContext genericContext)
        {
            var synonyms = SynonymFinder.FindSynonymsFor(currentDefinition);

            foreach (var current in synonyms)
            {
                var c = current as MethodDefinition;
                if (c == null) continue; //shouldn't happen
                if (!c.DeclaringType.IsInterface)
                {
                    FillAfferentGraph(current.GetCacheName(), history, graph, currentDefinition, depth + 1,
                                    inheritedTypeContext, true, breadCrumbs, genericContext, true); //always a base
                }
                else
                {
                    FillAfferentGraph(current.GetCacheName(), history, graph, currentDefinition, depth + 1, 
                        inheritedTypeContext, false, breadCrumbs, genericContext, false);
                }
            }
        }


        private static TypeDefinition GetNewTypeContext(TypeDefinition typeContext, TypeDefinition definition)
        {
            if (typeContext == null)
            {
                typeContext = definition;
            }
            else
            {
                if (!definition.IsBaseTypeOf(typeContext))
                {
                    typeContext = definition;
                }
            }
            return typeContext;
        }

        public IEnumerable<TestDescriptor> GetTestsForChanges(IEnumerable<Change<MethodReference>> changes)
        {
            return null;
        }
    }
}

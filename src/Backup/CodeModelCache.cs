using System;
using System.Collections.Generic;
using System.Threading;
using AutoTest.Client.Logging;
using AutoTest.VS.Util;
using EnvDTE;
using Microsoft.VisualStudio.Text;

namespace AutoTest.VS.RiskClassifier
{
    public class CodeModelCache
    {
        private static List<Signature> BuildCodeCache(string file)
        {
            var sigs = new List<Signature>();
            
            var projectItem = GetProjectItem(file); //can cache this too?
            if (projectItem == null) return sigs ;
            FileCodeModel fcm;
            try
            {
                fcm = projectItem.FileCodeModel;
                if (fcm == null) return sigs;
            }
            catch (Exception ex)
            {
                return sigs;
            }
            RecurseElements(fcm.CodeElements, sigs);
            return sigs;
        }

        static readonly Dictionary<string, ProjectItem> projectitemcache = new Dictionary<string, ProjectItem>();

        private static ProjectItem GetProjectItem(string file)
        {
            if (projectitemcache.ContainsKey(file)) return projectitemcache[file];
            Logger.Write("Getting project item for " + file);
            

            foreach (Project project in GetProjects())
            {
                var item = CheckProjectItemsRecursive(project.ProjectItems, file.ToLower());
                if (item != null)
                {
                    projectitemcache.Add(file, item);
                    return item;
                }
            }
            return null;
        }

        private static IEnumerable<Project> GetProjects()
        {
            foreach(Project project in _dte.Solution.Projects)
            {
                foreach(var entry in GetProjectsRecursive(project))
                {
                    yield return entry;
                }
                yield return project;
            }
        }

        private static IEnumerable<Project> GetProjectsRecursive(Project project)
        {
            if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}") //EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    var subProject = item.SubProject;
                    if (subProject == null)
                        continue;
                    foreach (var p in GetProjectsRecursive(subProject))
                        yield return p;
                }
            }
            else
            {
                yield return project;
            }
        }


        private static ProjectItem CheckProjectItemsRecursive(ProjectItems projectItems, string path)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.ProjectItems != null && projectItem.ProjectItems.Count > 0)
                {
                    var x = CheckProjectItemsRecursive(projectItem.ProjectItems, path);
                    if (x != null)
                    {
                        return x;
                    }
                }
                for (short i = 1; i <= projectItem.FileCount; i++)
                {
                    try
                    {
                        var filename = projectItem.FileNames[i];
                        if (filename != null)
                        {
                            if (filename.ToLower() == path)
                            {
                                return projectItem;
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        
                    }
                }
            }
            return null;
        }

        private static void RecurseElements(CodeElements codeElements, List<Signature> found)
        {
            foreach (var item in codeElements)
            {
                var element = item as CodeElement;
                if (element == null) continue;
                if(element.Kind == vsCMElement.vsCMElementProperty)
                {
                    var property = (CodeProperty) element;
                    
                    if(property.Getter != null) 
                        found.Add(new Signature(property.Getter.StartPoint.Line - 1, MethodNameReader.GetMethodStringFromElement((CodeElement) property.Getter), element));
                    if(property.Setter != null)
                        found.Add(new Signature(property.Setter.StartPoint.Line - 1, MethodNameReader.GetMethodStringFromElement((CodeElement) property.Setter), element));
                }
                else if (element.Kind == vsCMElement.vsCMElementFunction)
                {
                    found.Add(new Signature(element.StartPoint.Line - 1, MethodNameReader.GetMethodStringFromElement(element), element));
                }
                else if (element.Kind == vsCMElement.vsCMElementVariable)
                {
                    found.Add(new Signature(element.StartPoint.Line - 1, MethodNameReader.GetMethodStringFromElement(element), element));
                }
                else if (element.Children != null)
                {
                    RecurseElements(element.Children, found);
                }
            }
        }

        private static readonly Dictionary<string, List<Signature>> Cache = new Dictionary<string, List<Signature>>();
        private static DTE _dte;

        private static void UpdateCache(string key, Action<NormalizedSnapshotSpanCollection> callback, NormalizedSnapshotSpanCollection spans)
        {
            var runmore = true;
            while (runmore)
            {
                try
                {
                    Logger.Write("updating code cache for " + key);
                    var ret = BuildCodeCache(key);
                    Cache[key] = ret;
                    if (callback != null)
                    {
                        callback(spans);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
                lock (running)
                {
                    runmore = running[key];
		    running[key] = false;
                }
            }
            lock (running)
            {
                running.Remove(key);
            }
        }


        public static List<Signature> GetCodeInfoFor(ITextDocument document)
        {
            var filename = document.FilePath.ToLower();
            List<Signature> ret;
            if(Cache.TryGetValue(filename, out ret))
            {
                return ret;
            }
            return new List<Signature>();

        }

        private static readonly Dictionary<string, bool> running = new Dictionary<string, bool>();

        public static void CreateIfNeeded(ITextDocument document, ITextBuffer buffer, DTE dte)
        {
            var filename = document.FilePath.ToLower();
            if (_dte == null) _dte = dte;
            if(!Cache.ContainsKey(filename))
            {
                lock (running)
                {
                    if (running.ContainsKey(filename)) return;
                    running.Add(filename, true);
                }
                Logger.Write("Creating code cache item for " + filename);
                Cache.Add(filename, new List<Signature>());
                UpdateCache(filename, null, null);
            }
        }

        private static Timer t = new Timer(Timer_Callback, null, 100, 100);
        private static DateTime last = DateTime.MinValue;
        private static DateTime lastrun = DateTime.MinValue;
        private static Action<NormalizedSnapshotSpanCollection> toCallback = null;

        private static void Timer_Callback(object state)
        {
            var diff = DateTime.Now - last;
            if (diff.Milliseconds > 250)
            {
                if (lastrun == last) return;
                lock (running)
                {
                    if (running.Keys.Count == 0) return;
                    lastrun = last;
                    foreach (var item in running.Keys)
                    {
                        var s = item;
                        ThreadPool.QueueUserWorkItem(j => UpdateCache(s, toCallback, null));
                    }
                }
            }
        }

        public static void TryUpdateCache(string filename, Action<NormalizedSnapshotSpanCollection> callback, NormalizedSnapshotSpanCollection spans)
        {
            lock (running)
            {
                if (running.ContainsKey(filename))
                {
                    running[filename] = true;
                    return;
                }
                running.Add(filename, false);
                ThreadPool.QueueUserWorkItem(j => UpdateCache(filename, callback, spans));
            }
        }
    }
}
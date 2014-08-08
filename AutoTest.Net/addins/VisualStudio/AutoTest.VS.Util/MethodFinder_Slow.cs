using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
namespace AutoTest.VS.Util
{
    public class MethodFinder_Slow
    {
        private static IEnumerable<Project> GetProjects(DTE2 dte)
        {
            foreach (Project project in dte.Solution.Projects)
            {
                foreach (var entry in GetProjectsRecursive(project))
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


        private static IEnumerable<ProjectItem> GetProjectItemsRecursive(ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                if (projectItem.ProjectItems != null && projectItem.ProjectItems.Count > 0)
                {
                    foreach(var x in GetProjectItemsRecursive(projectItem.ProjectItems)) {
                        yield return x;
                    }
                }
                yield return projectItem;
            }
            yield break;
        }

        public static void GotoMethod(string signature, DTE2 dte) {
            try
            {
                GotoSignature(signature, dte);
            }
            catch (Exception ex)
            {
                Core.DebugLog.Debug.WriteDebug("Error on goto method " + signature);
                Core.DebugLog.Debug.WriteException(ex);
            }
        }

        public static void GotoMethodByFullname(string fullname, DTE2 dte)
        {
            try
            {
                GotoSignature(fullname, dte);
            }
            catch (Exception ex)
            {
                Core.DebugLog.Debug.WriteDebug("error going to " + fullname);
                Core.DebugLog.Debug.WriteException(ex);
            }
        }

        private static string GetTypeName(string fullname)
        {
            var space = fullname.IndexOf(" ");
            var endoftype = fullname.IndexOf("::");
            if (space == -1 || endoftype == -1 || endoftype - space < 0) return fullname;
            return fullname.Substring(space + 1, endoftype - space - 1).Replace("/", ".");
        }

        private static void GotoSignature(string signature, DTE2 dte)
        {
            Core.DebugLog.Debug.WriteDebug("going to signature: " + signature);
            var name = GetTypeName(signature);
            var item = GetProjectItemFromFullName(dte, name);
            if (item == null) return;
            var found = new List<Position>();
            RecurseElements(item.FileCodeModel.CodeElements, found, item.FileNames[0]);
            foreach (var position in found)
            {
                if (position.Signature == signature)
                {
                    goTo(dte, position);
                    break;
                }
            }
        }

        private const string vsViewKindCode = "{7651A701-06E5-11D1-8EBD-00A0C90F26EA}";
        //hack for embedded constants.

        private static void goTo(DTE2 dte, Position item)
        {
            var window = dte.OpenFile(vsViewKindCode, item.Filename);
            window.Activate();
            var selection = (TextSelection)dte.ActiveDocument.Selection;
            selection.MoveToDisplayColumn(item.LineNumber, 0);
            return;
        }

        public static void GotoTypeByFullName(DTE2 dte, string name)
        {
            try
            {
                var ProjectItem = GetProjectItemFromFullName(dte, name);
                var window = dte.OpenFile(vsViewKindCode, ProjectItem.FileNames[0]);
                window.Activate();
            }
            catch(Exception ex)
            {
                Core.DebugLog.Debug.WriteDebug("error going to type " + name);
                Core.DebugLog.Debug.WriteException(ex);
            }
        }

        private static ProjectItem GetProjectItemFromFullName(DTE2 dte, string name)
        {
            Core.DebugLog.Debug.WriteDebug("Finding " + name);
            foreach (var proj in GetProjects(dte))
            {
                var cm = proj.CodeModel;
                if (cm != null)
                {
                    CodeType typ = null;
                    try
                    {
                        typ = cm.CodeTypeFromFullName(name);
                    }
                    catch
                    {
                            
                    }
                        
                    Core.DebugLog.Debug.WriteDebug("Found.");
                    try
                    {
                        if (typ != null)
                        {
                            var item = typ.ProjectItem;
                            return item;
                        }
                    }
                    catch
                    {
                                
                    }
                    Core.DebugLog.Debug.WriteDebug("In hackity hackity hack hack.");
                    //OK so we know the project that its in ...
                    var items = GetProjectItemsRecursive(proj.ProjectItems);
                    if (items == null) return null;
                    foreach(var item in items)
                    {
                        if(item.FileCodeModel == null) continue;
                        if (FindType(item.FileCodeModel.CodeElements, name))
                        {
                            return item;
                        }
                    }
                }
            }
            throw new Exception("name not found in any projects");
        }

        private static bool FindType(CodeElements codeElements, string name)
        {
            foreach(var element in codeElements)
            {
                var elem = element as CodeElement;
                if(elem == null) continue;
                if(elem.Kind == vsCMElement.vsCMElementTypeDef)
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("found type : " + ((CodeTypeRef)elem).AsFullName);
                    if (((CodeTypeRef)elem).AsFullName == name) return true;
                }
                else if (elem.Kind == vsCMElement.vsCMElementStruct)
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("found struct : " + ((CodeStruct)elem).FullName);
                    if (((CodeStruct)elem).FullName == name) return true;
                }
                else if (elem.Kind == vsCMElement.vsCMElementInterface)
                {
                    AutoTest.Core.DebugLog.Debug.WriteDebug("found interface : " + ((CodeInterface)elem).FullName);
                    if (((CodeInterface)elem).FullName == name) return true;
                }
                else if(elem.Children.Count > 0)
                {
                    if(FindType(elem.Children, name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void RecurseElements(CodeElements codeElements, List<Position> found, string name)
        {
            if (codeElements == null) return;
            foreach (var item in codeElements)
            {
                var element = item as CodeElement;
                if (element == null) continue;
                if (element.Kind == vsCMElement.vsCMElementFunction || element.Kind == vsCMElement.vsCMElementProperty)
                {
                    var mname = MethodNameReader.GetMethodStringFromElement(element);
                    var fullname = element.FullName;
                    found.Add(new Position(mname, fullname, name, element.StartPoint.Line));
                }
                if (element.Children != null)
                {
                    RecurseElements(element.Children, found, name);
                }
            }
        }
    }

    internal class Position
    {
        public readonly string Signature;
        public readonly string Fullname;
        public readonly string Filename;
        public readonly int LineNumber;

        public Position(string signature, string fullname, string filename, int lineNumber)
        {
            Signature = signature;
            Fullname = fullname;
            LineNumber = lineNumber;
            Filename = filename;
        }
    }
}

using System;
using EnvDTE;
using EnvDTE80;
using AutoTest.Messages;
using System.IO;
using AutoTest.VS.Util.SourceParsers;

namespace AutoTest.VS.Util
{
    class Foo<T>
    {
        int x(T par)
        {
            return 0;
        }
    }

    public class OnDemandRunFromCursorPosition
    {
        private readonly DTE2 _application;

        private TextPoint _point;
        private FileCodeModel _fcm;
        private const vsCMElement _scopes = 0;

        private string _project;
        private string[] _namespace = new string[] {};
        private string[] _member = new string[] { };
        private string[] _test = new string[] { };

        public OnDemandRunFromCursorPosition(DTE2 application)
        {
            _application = application;
        }

        public OnDemandRun FromCurrentPosition()
        {
            if (isSpecFlowFeature())
                return getFromSpecFlowFeature();
            else
                return getFromCodeModel();
        }

        private bool isSpecFlowFeature()
        {
            return Path.GetExtension(_application.ActiveDocument.Name).ToLower().Equals(".feature");
        }

        private OnDemandRun getFromSpecFlowFeature()
        {
            try
            {
                _project = _application.ActiveDocument.ProjectItem.ContainingProject.FullName;
                var feature = _application.ActiveDocument.FullName;
                var items = _application.ActiveDocument.ProjectItem.ProjectItems;
                ProjectItem item = null;
                foreach (object itm in items)
                {
                    item = (ProjectItem)itm;
                    break;
                }
                var codeBehind = item.Document.FullName;
                var sel = (TextSelection)_application.ActiveDocument.Selection;
                var point = (TextPoint)sel.ActivePoint;
                var signature = new SpecFlowFeatureParser(File.ReadAllText(feature), File.ReadAllText(codeBehind)).GetTest(point.Line - 1);
                if (signature == null)
                    return new OnDemandRun(_project);
                if (signature.Type == SignatureType.Class)
                    return new OnDemandRun(_project, new string[] { }, new string[] { signature.Name }, new string[] { });
                return new OnDemandRun(_project, new string[] { signature.Name }, new string[] { }, new string[] { });
            }
            catch
            {
            }
            return new OnDemandRun(_project);
        }

        private OnDemandRun getFromCodeModel()
        {
            try
            {
                getCodeFromPosition();

                foreach (vsCMElement scope in Enum.GetValues(_scopes.GetType()))
                {
                    if (!validScope(scope))
                        continue;

                    try
                    {
                        CodeElement elem = _fcm.CodeElementFromPoint(_point, scope);
                        if (elem == null)
                            continue;

                        analyzeElement(elem);
                    }
                    catch (Exception ex)
                    {
                        AutoTest.Core.DebugLog.Debug.WriteException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
            if (_test.Length > 0 || _member.Length > 0)
                return new OnDemandRun(_project, _test, _member, new string[] { });
            else
                return new OnDemandRun(_project, _test, _member, _namespace);
        }

        private void analyzeElement(CodeElement elem)
        {
            _project = elem.ProjectItem.ContainingProject.FullName;
            if (elem.Kind == vsCMElement.vsCMElementNamespace)
                _namespace = new[] { elem.FullName };

            if (elem.Kind == vsCMElement.vsCMElementClass)
            {
                var cls = MethodNameReader.GetMethodStringFromElement(elem);
                _member = new[] { cls.Replace("/", "+") };
            }

            if (elem.Kind == vsCMElement.vsCMElementFunction)
            {
                var test = MethodNameReader.GetMethodStringFromElement(elem);
                test = test
                    .Replace("::", ".")
                    .Replace("/", "+");
                var parenthesis = test.IndexOf("(");
                if (parenthesis != -1)
                    test = test.Substring(0, parenthesis);
                var space = test.IndexOf(" ");
                if (space != -1)
                    test = test.Substring(space + 1, test.Length - (space + 1));
                _test = new[] { test };
            }
        }

        private void getCodeFromPosition()
        {
            var sel = (TextSelection)_application.ActiveDocument.Selection;
            _point = sel.ActivePoint;
            _fcm = _application.ActiveDocument.ProjectItem.FileCodeModel;
        }

        private static bool validScope(vsCMElement scope)
        {
            return scope == vsCMElement.vsCMElementFunction || scope == vsCMElement.vsCMElementClass || scope == vsCMElement.vsCMElementNamespace;
        }
    }
}

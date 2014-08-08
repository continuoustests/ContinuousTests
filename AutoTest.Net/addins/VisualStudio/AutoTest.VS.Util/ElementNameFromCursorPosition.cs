using System;
using EnvDTE80;
using EnvDTE;
using System.IO;
using AutoTest.VS.Util.SourceParsers;

namespace AutoTest.VS.Util
{
    public class ElementNameFromCursorPosition
    {
        private readonly DTE2 _application;

        public ElementNameFromCursorPosition(DTE2 application)
        {
            _application = application;
        }

        public string Get()
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

        private string getFromSpecFlowFeature()
        {
            try
            {
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
                var match = new SpecFlowFeatureParser(File.ReadAllText(feature), File.ReadAllText(codeBehind)).GetTest(point.Line - 1);
                if (match == null)
                    return null;
                return match.Name;
            }
            catch
            {
            }
            return null;
        }

        private string getFromCodeModel()
        {
            try
            {
                var sel = (TextSelection)_application.ActiveDocument.Selection;
                var point = (TextPoint)sel.ActivePoint;
                var fcm = _application.ActiveDocument.ProjectItem.FileCodeModel;
                var elem = CodeElementFromPoint(fcm, point, vsCMElement.vsCMElementFunction);
                if (elem != null)
                {
                    return MethodNameReader.GetMethodStringFromElement(elem);
                }
                var elem1 = CodeElementFromPoint(fcm, point, vsCMElement.vsCMElementProperty);
                if (elem1 != null)
                {
                    var property = (CodeProperty)elem1;
                    var setterContains = property.Setter != null && property.Setter.StartPoint != null &&
                                         property.Setter.StartPoint.LessThan(point) && property.Setter.EndPoint.GreaterThan(point);
                    if (setterContains)
                        return MethodNameReader.GetMethodStringFromElement((CodeElement)property.Setter);
                    return MethodNameReader.GetMethodStringFromElement((CodeElement)property.Getter);
                }
                var elem2 = CodeElementFromPoint(fcm, point, vsCMElement.vsCMElementVariable);
                if (elem2 != null)
                    return MethodNameReader.GetMethodStringFromElement(elem2);
                return null;
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
                return null;
            }
        }

        private static CodeElement CodeElementFromPoint(FileCodeModel fcm, TextPoint point, vsCMElement type)
        {
            try
            {
                return fcm.CodeElementFromPoint(point, type);
            }
            catch
            {
                return null;
            }
        }
    }
}

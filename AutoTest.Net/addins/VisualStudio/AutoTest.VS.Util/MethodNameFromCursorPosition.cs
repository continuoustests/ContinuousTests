using System;
using EnvDTE80;
using EnvDTE;
using AutoTest.Client.Logging;

namespace AutoTest.VS.Util
{
    public class MethodNameFromCursorPosition
    {
        private readonly DTE2 _application;

        public MethodNameFromCursorPosition(DTE2 application)
        {
            _application = application;
        }

        public string Get()
        {
            try
            {
                var sel = (TextSelection)_application.ActiveDocument.Selection;
                var point = (TextPoint)sel.ActivePoint;
                var fcm = _application.ActiveDocument.ProjectItem.FileCodeModel;
                var elem = fcm.CodeElementFromPoint(point, vsCMElement.vsCMElementFunction);
                return MethodNameReader.GetMethodStringFromElement(elem);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }
    }
}

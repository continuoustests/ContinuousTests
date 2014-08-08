using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.IO;

namespace AutoTest.VS.Util
{
    public class SolutionStateHandler
    {
        private static DTE2 _application;
        private static DocumentEvents _events;
        private static _dispDocumentEvents_DocumentSavedEventHandler _documentSaved;

        private static bool _isDirty = true;

        public static bool IsDirty { get { return _isDirty; } }

        public static void BindEvents(DTE2 applicationObject)
        {
            if (_events != null)
                return;

            _application = applicationObject;
            _events = _application.Events.DocumentEvents;
            _documentSaved = new _dispDocumentEvents_DocumentSavedEventHandler(documentSaved);
            _events.DocumentSaved += _documentSaved;
        }

        public static void Reset()
        {
            _isDirty = false;
        }

        static void documentSaved(Document document)
        {
            _isDirty = true;
        }
    }
}

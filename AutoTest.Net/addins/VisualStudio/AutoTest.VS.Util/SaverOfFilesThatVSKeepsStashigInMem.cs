using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE;

namespace AutoTest.VS.Util
{
    public static class SaverOfFilesThatVSKeepsStashigInMem
    {
        private static ProjectItemsEvents _events = null;
        private static ProjectItemsEvents _solutionEvents = null;
        private static _dispProjectItemsEvents_ItemAddedEventHandler _added;
        private static _dispProjectItemsEvents_ItemRemovedEventHandler _deleted;
        private static _dispProjectItemsEvents_ItemRenamedEventHandler _renamed;
        private static DTE2 _application;

        public static void BindEvents(DTE2 application)
        {
            if (_events != null)
                return;

            _application = application;
            var letsDoTheMSCastBoogie = (Events2)_application.Events;
            _events = letsDoTheMSCastBoogie.ProjectItemsEvents;
            _solutionEvents = _application.Events.SolutionItemsEvents;
            _added = new _dispProjectItemsEvents_ItemAddedEventHandler(_events_ItemAdded);
            _deleted = new _dispProjectItemsEvents_ItemRemovedEventHandler(_events_ItemRemoved);
            _renamed = new _dispProjectItemsEvents_ItemRenamedEventHandler(_events_ItemRenamed);
            _events.ItemAdded += _added;
            _events.ItemRemoved += _deleted;
            _events.ItemRenamed += _renamed;
            //_solutionEvents.ItemAdded += _added;
            //_solutionEvents.ItemRemoved += _deleted;
            //_solutionEvents.ItemRenamed += _renamed;
        }

        static void _events_ItemRenamed(ProjectItem ProjectItem, string OldName)
        {
            persist(ProjectItem);
        }

        static void _events_ItemRemoved(ProjectItem ProjectItem)
        {
            persist(ProjectItem);
        }

        static void _events_ItemAdded(ProjectItem ProjectItem)
        {
            persist(ProjectItem);
        }

        private static void persist(ProjectItem ProjectItem)
        {
            _application.ExecuteCommand("File.SaveAll");
        }
    }
}

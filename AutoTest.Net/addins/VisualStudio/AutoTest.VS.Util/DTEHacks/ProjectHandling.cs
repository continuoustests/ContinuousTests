using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;

namespace AutoTest.VS.Util.DTEHacks
{
    public static class ProjectHandling
    {
        public static List<Project> GetAll(DTE2 application)
        {
            var projects = new List<Project>();
            foreach (Project project in application.Solution.Projects)
                addProjects(projects, project);
            return projects;
        }

        private static void addProjects(List<Project> projects, Project project)
        {
            projects.Add(project);
            if (project.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in project.ProjectItems)
                {
                    Project subProject = item.SubProject;
                    if (subProject == null)
                        continue;
                    addProjects(projects, subProject);
                }
                return;
            }
        }
    }
}

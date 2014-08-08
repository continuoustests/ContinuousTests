using System;
using AutoTest.Core.Caching;
using Rhino.Mocks;
using AutoTest.Core.Caching.Projects;
using AutoTest.Core.Messaging.MessageConsumers;
using NUnit.Framework;
using System.IO;
using AutoTest.Core.Configuration;
namespace AutoTest.Test.Core.Messaging.MessageConsumers
{
	[TestFixture]
	public class BuildOptimizerTest
	{
		private BuildOptimizer _optimizer;
		private ICache _cache;
		private RunInfo[] _runInfos;
        private string[] _projectList;
		
		[SetUp]
		public void SetUp()
		{
			// Project dependency graph
			//
			//		    Project2
			//		   /
			//Project0			 Project5
			//		   \ 		/
			//			Project6
			//					\
			//Project1			Project4
			//	      \        /
			//		   Project3
			//
			_projectList = new string[]
									{ 
										string.Format("Proj0{0}Project0.csproj", Path.DirectorySeparatorChar), 
										string.Format("Proj1{0}Project1.csproj", Path.DirectorySeparatorChar), 
										string.Format("Proj2{0}Project2.csproj", Path.DirectorySeparatorChar), 
										string.Format("Proj3{0}Project3.csproj", Path.DirectorySeparatorChar),
										string.Format("Proj4{0}Project4.csproj", Path.DirectorySeparatorChar), 
										string.Format("Proj5{0}Project5.csproj", Path.DirectorySeparatorChar), 
										string.Format("Proj6{0}Project6.csproj", Path.DirectorySeparatorChar)
									};
			_cache = MockRepository.GenerateMock<ICache>();
			var document = new ProjectDocument(ProjectType.CSharp);
			document.AddReferencedBy(new string[] { _projectList[2], _projectList[6] });
			document.SetAssemblyName("Project0.dll");
            document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			_cache.Stub(c => c.Get<Project>(_projectList[0])).Return(new Project(_projectList[0], document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReferencedBy(_projectList[3]);
			document.SetAssemblyName("Project1.dll");
            document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			document.RebuildOnNextRun();
			_cache.Stub(c => c.Get<Project>(_projectList[1])).Return(new Project(_projectList[1], document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(_projectList[0]);
			document.SetAssemblyName("Project2.dll");
			document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			_cache.Stub(c => c.Get<Project>(_projectList[2])).Return(new Project(_projectList[2], document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(_projectList[1]);
			document.AddReferencedBy(_projectList[4]);
			document.SetAssemblyName("Project3.dll");
            document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			_cache.Stub(c => c.Get<Project>(_projectList[3])).Return(new Project(_projectList[3], document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(new string[] { _projectList[6], _projectList[3] });
			document.SetAssemblyName("Project4.dll");
			document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			_cache.Stub(c => c.Get<Project>(_projectList[4])).Return(new Project(_projectList[4], document));                                                                      
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(_projectList[6]);
			document.SetAssemblyName("Project5.dll");
			document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			document.RebuildOnNextRun();
			_cache.Stub(c => c.Get<Project>(_projectList[5])).Return(new Project(_projectList[5], document));
			document = new ProjectDocument(ProjectType.CSharp);
			document.AddReference(_projectList[0]);
			document.AddReferencedBy(new string[] { _projectList[4], _projectList[5] });
			document.SetAssemblyName("Project6.dll");
            document.SetOutputPath(string.Format("bin{0}Debug", Path.DirectorySeparatorChar));
			_cache.Stub(c => c.Get<Project>(_projectList[6])).Return(new Project(_projectList[6], document));
			                                                                                                                                                                                                                                                                                                                        
			_optimizer = new BuildOptimizer(_cache, MockRepository.GenerateMock<IConfiguration>());
		}
		
		[Test]
		public void Should_only_build_projects_without_referencedbys()
		{
            _runInfos = _optimizer.AssembleBuildConfiguration(_projectList);
			_runInfos[0].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[1].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[2].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[3].ShouldBeBuilt.ShouldBeFalse();
			_runInfos[4].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[5].ShouldBeBuilt.ShouldBeTrue();
			_runInfos[6].ShouldBeBuilt.ShouldBeFalse();
		}

        //[Test]
        //public void Should_set_assembly_path_to_build_source()
        //{
        //    _runInfos = _optimizer.AssembleBuildConfiguration(_projectList);
        //    _runInfos[0].Assembly.ShouldEqual(string.Format("Proj5{0}bin{0}Debug{0}Project0.dll", Path.DirectorySeparatorChar));
        //    _runInfos[1].Assembly.ShouldEqual(string.Format("Proj4{0}bin{0}Debug{0}Project1.dll", Path.DirectorySeparatorChar));
        //    _runInfos[2].Assembly.ShouldEqual(string.Format("Proj2{0}bin{0}Debug{0}Project2.dll", Path.DirectorySeparatorChar));
        //    _runInfos[3].Assembly.ShouldEqual(string.Format("Proj4{0}bin{0}Debug{0}Project3.dll", Path.DirectorySeparatorChar));
        //    _runInfos[4].Assembly.ShouldEqual(string.Format("Proj4{0}bin{0}Debug{0}Project4.dll", Path.DirectorySeparatorChar));
        //    _runInfos[5].Assembly.ShouldEqual(string.Format("Proj5{0}bin{0}Debug{0}Project5.dll", Path.DirectorySeparatorChar));
        //    _runInfos[6].Assembly.ShouldEqual(string.Format("Proj5{0}bin{0}Debug{0}Project6.dll", Path.DirectorySeparatorChar));
        //}

        [Test]
        public void Should_set_assembly_path_to_individual_dir()
        {
            _runInfos = _optimizer.AssembleBuildConfiguration(_projectList);
            _runInfos[0].Assembly.ShouldEqual(string.Format("Proj0{0}bin{0}Debug{0}Project0.dll", Path.DirectorySeparatorChar));
            _runInfos[1].Assembly.ShouldEqual(string.Format("Proj1{0}bin{0}Debug{0}Project1.dll", Path.DirectorySeparatorChar));
            _runInfos[2].Assembly.ShouldEqual(string.Format("Proj2{0}bin{0}Debug{0}Project2.dll", Path.DirectorySeparatorChar));
            _runInfos[3].Assembly.ShouldEqual(string.Format("Proj3{0}bin{0}Debug{0}Project3.dll", Path.DirectorySeparatorChar));
            _runInfos[4].Assembly.ShouldEqual(string.Format("Proj4{0}bin{0}Debug{0}Project4.dll", Path.DirectorySeparatorChar));
            _runInfos[5].Assembly.ShouldEqual(string.Format("Proj5{0}bin{0}Debug{0}Project5.dll", Path.DirectorySeparatorChar));
            _runInfos[6].Assembly.ShouldEqual(string.Format("Proj6{0}bin{0}Debug{0}Project6.dll", Path.DirectorySeparatorChar));
        }
		
		[Test]
		public void Should_rebuild_project_()
		{
            _runInfos = _optimizer.AssembleBuildConfiguration(_projectList);
			_runInfos[0].Project.Value.RequiresRebuild.ShouldBeFalse();
			_runInfos[1].Project.Value.RequiresRebuild.ShouldBeTrue();
			_runInfos[2].Project.Value.RequiresRebuild.ShouldBeFalse();
			_runInfos[3].Project.Value.RequiresRebuild.ShouldBeFalse();
			_runInfos[4].Project.Value.RequiresRebuild.ShouldBeTrue();
			_runInfos[5].Project.Value.RequiresRebuild.ShouldBeTrue();
			_runInfos[6].Project.Value.RequiresRebuild.ShouldBeFalse();
		}

        [Test]
        public void Should_support_list_of_projects()
        {
            var projectList = new Project[] { new Project("someProject", new ProjectDocument(ProjectType.CSharp)) };
            projectList[0].Value.SetOutputPath("something");
            projectList[0].Value.SetAssemblyName("Project5.dll");
            var optimizer = new BuildOptimizer(_cache, MockRepository.GenerateMock<IConfiguration>());
            var runInfos = optimizer.AssembleBuildConfiguration(projectList);
            runInfos.Length.ShouldEqual(1);
            runInfos[0].Project.Key.ShouldEqual("someProject");
        }
	}
}
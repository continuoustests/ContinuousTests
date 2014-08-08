using System;

using Microsoft.Build.BuildEngine;

namespace AutoTest.Test.Core.BuildRunners
{
    public interface IBuildEngine
    {
        IBuildResult Build(IBuildRequest request);
    }

    public interface IBuildResult
    {
        bool Success { get; }
    }

    public interface IBuildRequest
    {
        string Name { get; }
        string FullPath { get; }
    }

    public class InProcBuilder : IBuildEngine
    {
        public IBuildResult Build(IBuildRequest request)
        {
            //try
            //{
            //    var project = new Project(Engine.GlobalEngine);
            //    project.Configure(project.FullFileName);
            //}
            //catch (Exception e)
            //{
            //}
            throw new NotImplementedException();
        }
    }
}
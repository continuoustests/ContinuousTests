using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AutoTest.TestRunners.Shared.Options;
using AutoTest.TestRunners.Shared.Targeting;
using Rhino.Mocks;
using AutoTest.TestRunners.Shared.AssemblyAnalysis;

namespace AutoTest.TestRunners.Tests.Targeting
{
    [TestFixture]
    public class TargetedRunAssemblerTests
    {
        [Test]
        public void Should_group_by_assembly()
        {
            var locator = MockRepository.GenerateMock<IAssemblyPropertyReader>();
            locator.Stub(x => x.GetTargetFramework("Assembly1")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly2")).Return(new Version(4, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly3")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly4")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly5")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly6")).Return(new Version(4, 0));

            locator.Stub(x => x.GetPlatform("Assembly1")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly2")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly3")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly4")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly5")).Return(Platform.x86);
            locator.Stub(x => x.GetPlatform("Assembly6")).Return(Platform.x86);

            var options = new RunOptions();
            var runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly1"), new AssemblyOptions("Assembly2") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly3"), new AssemblyOptions("Assembly5") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("XUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly4"), new AssemblyOptions("Assembly6") });
            options.AddTestRun(runner);

            var assembler = new TargetedRunAssembler(options, locator, false);
            var targeted = assembler.Assemble();

            Assert.That(targeted.Count(), Is.EqualTo(4));
            Assert.That(targeted.ElementAt(0).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(0).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(0).Runners.Count(), Is.EqualTo(2));
            Assert.That(targeted.ElementAt(0).Runners.Count(), Is.EqualTo(2));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly1"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).Assemblies.ElementAt(1).Assembly, Is.EqualTo("Assembly3"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(1).ID, Is.EqualTo("XUnit"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(1).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly4"));
            Assert.That(targeted.ElementAt(1).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(1).TargetFramework, Is.EqualTo(new Version(4, 0)));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly2"));
            Assert.That(targeted.ElementAt(2).Platform, Is.EqualTo(Platform.x86));
            Assert.That(targeted.ElementAt(2).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(2).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(2).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly5"));
            Assert.That(targeted.ElementAt(3).Platform, Is.EqualTo(Platform.x86));
            Assert.That(targeted.ElementAt(3).TargetFramework, Is.EqualTo(new Version(4, 0)));
            Assert.That(targeted.ElementAt(3).Runners.ElementAt(0).ID, Is.EqualTo("XUnit"));
            Assert.That(targeted.ElementAt(3).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly6"));
        }

        [Test]
        public void Should_not_group_by_assembly_when_when_run_parallel()
        {
            var locator = MockRepository.GenerateMock<IAssemblyPropertyReader>();
            locator.Stub(x => x.GetTargetFramework("Assembly1")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly2")).Return(new Version(4, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly3")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly4")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly5")).Return(new Version(2, 0));
            locator.Stub(x => x.GetTargetFramework("Assembly6")).Return(new Version(4, 0));

            locator.Stub(x => x.GetPlatform("Assembly1")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly2")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly3")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly4")).Return(Platform.AnyCPU);
            locator.Stub(x => x.GetPlatform("Assembly5")).Return(Platform.x86);
            locator.Stub(x => x.GetPlatform("Assembly6")).Return(Platform.x86);

            var options = new RunOptions();
            var runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly1"), new AssemblyOptions("Assembly2") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("NUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly3"), new AssemblyOptions("Assembly5") });
            options.AddTestRun(runner);
            runner = new RunnerOptions("XUnit");
            runner.AddAssemblies(new AssemblyOptions[] { new AssemblyOptions("Assembly4"), new AssemblyOptions("Assembly6") });
            options.AddTestRun(runner);

            var assembler = new TargetedRunAssembler(options, locator, true);
            var targeted = assembler.Assemble();

            Assert.That(targeted.Count(), Is.EqualTo(6));
            Assert.That(targeted.ElementAt(0).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(0).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(0).Runners.Count(), Is.EqualTo(1));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(0).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly1"));

            Assert.That(targeted.ElementAt(2).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(2).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(2).Runners.Count(), Is.EqualTo(1));
            Assert.That(targeted.ElementAt(2).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(2).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly3"));

            Assert.That(targeted.ElementAt(4).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(4).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(4).Runners.Count(), Is.EqualTo(1));
            Assert.That(targeted.ElementAt(4).Runners.ElementAt(0).ID, Is.EqualTo("XUnit"));
            Assert.That(targeted.ElementAt(4).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly4"));

            Assert.That(targeted.ElementAt(1).Platform, Is.EqualTo(Platform.AnyCPU));
            Assert.That(targeted.ElementAt(1).TargetFramework, Is.EqualTo(new Version(4, 0)));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(1).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly2"));

            Assert.That(targeted.ElementAt(3).Platform, Is.EqualTo(Platform.x86));
            Assert.That(targeted.ElementAt(3).TargetFramework, Is.EqualTo(new Version(2, 0)));
            Assert.That(targeted.ElementAt(3).Runners.ElementAt(0).ID, Is.EqualTo("NUnit"));
            Assert.That(targeted.ElementAt(3).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly5"));

            Assert.That(targeted.ElementAt(5).Platform, Is.EqualTo(Platform.x86));
            Assert.That(targeted.ElementAt(5).TargetFramework, Is.EqualTo(new Version(4, 0)));
            Assert.That(targeted.ElementAt(5).Runners.ElementAt(0).ID, Is.EqualTo("XUnit"));
            Assert.That(targeted.ElementAt(5).Runners.ElementAt(0).Assemblies.ElementAt(0).Assembly, Is.EqualTo("Assembly6"));
        }
    }
}

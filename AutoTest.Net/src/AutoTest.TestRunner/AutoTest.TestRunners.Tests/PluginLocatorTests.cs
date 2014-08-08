using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using AutoTest.TestRunners.Shared.Plugins;
using AutoTest.TestRunners.Shared;

namespace AutoTest.TestRunners.Tests
{
    [TestFixture]
    public class PluginLocatorTests
    {
        [Test]
        public void Should_locate_plugins()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var locator = new PluginLocator(path);
            var plugins = locator.Locate();
            
            Assert.IsTrue(plugins.Count() > 0);
            Assert.That(plugins.Where(x => x.Assembly.Equals(Path.Combine(path, "AutoTest.TestRunners.Tests.dll")) && x.Type.Equals("AutoTest.TestRunners.Tests.Plugins.Plugin1")).Count(), Is.EqualTo(1));
            Assert.That(plugins.Where(x => x.Assembly.Equals(Path.Combine(path, "AutoTest.TestRunners.Tests.dll")) && x.Type.Equals("AutoTest.TestRunners.Tests.Plugins.Plugin2")).Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_create_instance()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var locator = new PluginLocator(path);
            var plugins = locator.Locate();
            var plugin = plugins.Where(x => x.Assembly.Equals(Path.Combine(path, "AutoTest.TestRunners.Tests.dll")) && x.Type.Equals("AutoTest.TestRunners.Tests.Plugins.Plugin1")).First();
            Assert.That(plugin.New(), Is.InstanceOf<IAutoTestNetTestRunner>());
        }
    }
}

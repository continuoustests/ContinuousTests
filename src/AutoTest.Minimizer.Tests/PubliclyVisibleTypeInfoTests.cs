using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
namespace AutoTest.Minimizer.Tests
{
    [TestFixture]
    public class PubliclyVisibleTypeInfoTests : AssemblyTestFixture
    {
        
        [Test]
        public void can_detect_a_public_static_field()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(list.Count(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::staticpublic") == 1);
        } 
         
        [Test]
        public void does_not_detect_a_private_static_field()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(!list.Any(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::staticprivate"));
        }


        [Test]
        public void can_detect_a_public_static_method()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(list.Count(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::staticpublicmethod()") == 1);
        }

        [Test]
        public void does_not_detect_a_private_static_method()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(!list.Any(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::staticprivatemethod()"));
        }


        [Test]
        public void can_detect_a_public_instance_method()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(list.Count(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::publicmethod()") == 1);
        }

        [Test]
        public void does_not_detect_a_private_instance_method()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(!list.Any(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::privatemethod()"));
        }


        [Test]
        public void can_detect_a_public_instance_constructor()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(list.Count(x => x.FullName == "System.Void AutoTest.Minimizer.Tests.TestType::.ctor(System.Int32)") == 1);
        }

        [Test]
        public void does_not_detect_a_private_instance_constructor()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(!list.Any(x => x.FullName == "System.Void AutoTest.Minimizer.Tests.TestType::.ctor()"));
        }

        [Test]
        public void can_detect_a_public_instance_field()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(list.Count(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::publicinstance") == 1);
        }

        [Test]
        public void does_not_detect_a_private_instance_field()
        {
            var list = new List<MemberReference>();
            var def = assembly.GetTypeDefinition<TestType>();
            def.GetPublicsAndProtectedsFromType(list);
            Assert.That(!list.Any(x => x.FullName == "System.Int32 AutoTest.Minimizer.Tests.TestType::privateinstance"));
        }
    }

    public class TestType
    {
        private static int staticprivate;
        public static int staticpublic;
        private static int privateinstance;
        public static int publicinstance;

        public static Int32 staticpublicmethod() { return 0; }
        private static Int32 staticprivatemethod() { return 0; }

        public static Int32 publicmethod() { return 0; }
        private static Int32 privatemethod() { return 0; }

        public TestType(Int32 a) { }
        private TestType() { }
    }
}

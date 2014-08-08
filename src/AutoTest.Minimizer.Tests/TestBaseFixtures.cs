using System.Collections.Generic;
using Mono.Cecil;
using NUnit.Framework;

namespace AutoTest.Minimizer.Tests
{

	public class SingleTypeTestFixture<T> : AssemblyTestFixture {
	    protected IList<TypeReference> coupledTypes;
		
		public override void SetUp ()
		{
			base.SetUp ();
			var type = assembly.GetTypeDefinition<T>();
			coupledTypes = TypeScanner.GetDirectDependencies(type);
		}
	}
	
	public class AssemblyTestFixture {
	    protected AssemblyDefinition assembly;

		[SetUp]
		public virtual void SetUp()
		{
            var mdr = new DefaultAssemblyResolver();
		    assembly = mdr.Resolve("AutoTest.Minimizer.Tests");
			Assert.IsNotNull(assembly, "assembly was null");
		}
	}
}


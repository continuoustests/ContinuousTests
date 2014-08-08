using System.Collections.Generic;

namespace Simple.Testing.Framework
{
    internal interface ISpecificationGenerator
    {
        IEnumerable<SpecificationToRun> GetSpecifications();
    }
}
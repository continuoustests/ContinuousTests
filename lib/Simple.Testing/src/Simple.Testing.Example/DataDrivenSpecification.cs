using System.Collections.Generic;
using System.Linq;
using Simple.Testing.Framework;

namespace Simple.Testing.Example
{
    /*
     * This sample shows the building of a data driven specification. This is the simplest
     * version of a generator. Basically the runner will call anything that returns an IEnumerable<Specification>.
     * You can customize how the specifications are returned. In this case multiple specifications are
     * generated, based on the data involved. In this case, four specifications are returned.
     * 
     * With a little bit of work this could also be made much more generic (eg to support all data driven specs)
     */
    public class DataDrivenSpecifications
    {
        public IEnumerable<Specification> when_adding_numbers()
        {
            return AddingTestData().Select(x =>
                       new QuerySpecification<Calculator, int>
                        {
                            Name = "when adding numbers '" + x.Name + "'",
                            On = () => new Calculator(),
                            When = calc => calc.Add(x.OperandOne, x.OperantTwo),
                            Expect =
                                    {
                                        result => result == x.ExpectedResult
                                    },
                        });
        }

        public IEnumerable<AddArgs> AddingTestData()
        {
            yield return new AddArgs { OperandOne = 1, OperantTwo = 1, ExpectedResult = 3 };
            yield return new AddArgs { OperandOne = -1, OperantTwo = 1, ExpectedResult = 0 };
            yield return new AddArgs { OperandOne = 50, OperantTwo = -55, ExpectedResult = -5 };
            yield return new AddArgs { OperandOne = 0, OperantTwo = 0, ExpectedResult = 0 };
        }
    }

    public class Calculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }
    }

    public class AddArgs
    {
        public string Name
        {
            get { return OperandOne + "+" + OperantTwo + "=" + ExpectedResult; }
        }
        public int OperandOne;
        public int OperantTwo;
        public int ExpectedResult;
    }
}

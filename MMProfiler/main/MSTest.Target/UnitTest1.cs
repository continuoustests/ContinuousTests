using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTest.Target
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod2()
        {
            //(new GenericBase<string>()).Y("");
            //(new SimpleImpl()).X();
            //(new SimpleImpl()).Z();
            //(new GenericImpl<int>()).Y(0);
            //(new GenericImpl<Base>()).Z("");
            //(new StringImpl()).Y("");
            //(new StringImpl()).Z();
            //(new Base()).X();

            (new AbstractImpl()).X<string>();
        }

    }

    public abstract class AbstractBase<T>
    {
        public T X<Y>()
        {
            return (default(T));
        }
    }

    public class AbstractImpl : AbstractBase<string>
    {
        
    }

    public class Base
    {
        public void X()
        {    
        }
    }

    public class GenericBase<T>
    {
        public IList<T[]> Y(T thing)
        {
            return default(IList<T[]>);
        }
    }

    public class SimpleImpl : Base
    {
        public void Z()
        {
        }
    }

    public class StringImpl : GenericBase<string>
    {
        public void Z()
        {         
        }
    }

    public class GenericImpl<T> : GenericBase<T>
    {
        public T Z<T1>(T1 val)
        {
            return default(T);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoTest.Core.Messaging;

using Ninject.Core;

namespace AutoTest.Core.Configuration
{
    public abstract class TypeScanningModule : StandardModule
    {
        private readonly Assembly _assembly;

        protected TypeScanningModule(Assembly assembly)
        {
            _assembly = assembly;
        }

        public override void Load()
        {
            foreach (var concreteType in GetConcereteTypes())
            {
                foreach (var interfaceType in concreteType.GetInterfaces())
                {
                    if (interfaceType.GetInterfaces().Contains(typeof (IMessageConsumer)))
                    {
                        //bind with not default binding
                        Bind(interfaceType).To(concreteType).OnlyIf(c => c.Service.Name == "asdf");
                    }
                }
            }
        }

        private IEnumerable<Type> GetConcereteTypes()
        {
            foreach (var type in _assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                {
                    continue;
                }
                yield return type;
            }
        }
    }
}
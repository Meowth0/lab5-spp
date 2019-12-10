using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public class ImplementationInfo
    {
        private object instance;       
        private static object syncRoot;
        public Type implementationType { get; }
        public bool isSingleton { get; }

        public ImplementationInfo(Type dependencyType, bool isSingleton)
        {
            implementationType = dependencyType;
            this.isSingleton = isSingleton;
            syncRoot = new object();
            instance = null;
        }
    }
}

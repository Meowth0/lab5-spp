using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjection;
using System.Collections.Generic;
using System.Collections;

namespace DependencyTests
{
    [TestClass]
    public class ContainerTests
    {
        [TestMethod]
        public void CreateDependencyTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IExample, ExampleClass1>(false);
            config.Register<IExample, ExampleClass2>(true);
            provider = new DependencyProvider(config);
            IExample actual = provider.Resolve<IExample>();

            Assert.IsNotNull(actual);   
        }

        [TestMethod]
        public void SingletonTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IExample, ExampleClass1>(true);
            provider = new DependencyProvider(config);
            IExample expected = provider.Resolve<IExample>();
            IExample actual = provider.Resolve<IExample>();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InstancePerDependencyTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IExample, ExampleClass1>(false);
            provider = new DependencyProvider(config);
            IExample expected = provider.Resolve<IExample>();
            IExample actual = provider.Resolve<IExample>();

            Assert.AreNotEqual(expected, actual);
        }

        [TestMethod]
        public void SelfRegistrationTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<ExampleClass1, ExampleClass1>(true);
            provider = new DependencyProvider(config);
            ExampleClass1 actual = provider.Resolve<ExampleClass1>();

            Assert.IsNotNull(actual);
        }
        [TestMethod]
        public void GetSomeImplementationsTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IExample, ExampleClass1>(true);
            config.Register<IExample, ExampleClass2>(false);
            provider = new DependencyProvider(config);
            IEnumerable<IExample> actual = provider.Resolve<IEnumerable<IExample>>();

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, ((IList)actual).Count);
        }

        [TestMethod]
        public void GenericTypeTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IRepository, MyRepository>(true);
            config.Register<ServiceImpl<IRepository>, ServiceImpl<IRepository>>(false);
            provider = new DependencyProvider(config);
            ServiceImpl<IRepository> actual = provider.Resolve<ServiceImpl<IRepository>>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(111, actual.GetNum());
        }

        [TestMethod]
        public void OpenGenericTypeTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<IRepository, MyRepository>(false);
            config.Register(typeof(ServiceImpl<>), typeof(ServiceImpl<>), false);
            provider = new DependencyProvider(config);
            ServiceImpl<IRepository> actual = provider.Resolve<ServiceImpl<IRepository>>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(111, actual.GetNum());
        }

        [TestMethod]
        public void CycleDependencyTest()
        {
            DependencyProvider provider;
            DependenciesConfig config = new DependenciesConfig();

            config.Register<ExampleClass1, ExampleClass1>(false);
            config.Register<ExampleClass2, ExampleClass2>(true);
            config.Register<ExampleClass3, ExampleClass3>(true);
                
            provider = new DependencyProvider(config);
            ExampleClass1 actual = provider.Resolve<ExampleClass1>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(null, actual.example.example.example);
        }
    }
}

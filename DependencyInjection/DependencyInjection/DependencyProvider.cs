using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public class DependencyProvider
    {
        private DependenciesConfig config;
        private readonly ConcurrentStack<Type> stack;
        private Type currentGenericType;
        public DependencyProvider(DependenciesConfig configuration)
        {
            if (ValidateConfiguration(configuration))
            {
                config = configuration;
                stack = new ConcurrentStack<Type>();
            }
            else
            {
                throw new ConfigurationValidationException("Configuration is not valid!");
            }
        }

        private bool ValidateConfiguration(DependenciesConfig configuration)
        {
            foreach (Type tDependency in configuration.dependencies.Keys)
            {
                if (!tDependency.IsValueType)
                {
                    foreach (ImplementationInfo dependency in configuration.dependencies[tDependency])
                    {
                        Type tImplementation = dependency.implementationType;

                        if (tImplementation.IsAbstract || tImplementation.IsInterface || !tDependency.IsAssignableFrom(tImplementation))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public T Resolve<T>() where T : class
        {
            Type t = typeof(T);

            return (T)Resolve(t);
        }

        private object Resolve(Type t)
        {
            List<ImplementationInfo> implementations;
            //IEnumerable of interfaces
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return CreateGeneric(t);
            }
            //single implemenation
            config.dependencies.TryGetValue(t, out implementations);
            if (implementations != null)
                return GetInstance(implementations.First());
            else
            {
                //open generic
                Type genericDefinition;
                if (t.IsGenericType)
                {
                    currentGenericType = t;
                    genericDefinition = t.GetGenericTypeDefinition();
                    config.dependencies.TryGetValue(genericDefinition, out implementations);
                    if (implementations != null)
                        return GetInstance(implementations.First());
                }
                throw new TypeNotRegisterException("Unknown type " + t.Name);
            }
        }

        //crete list of interfaces
        private object CreateGeneric(Type t)
        {
            object result = null;
            List<ImplementationInfo> implementations;
            Type tResolve = t.GetGenericArguments()[0];

            config.dependencies.TryGetValue(tResolve, out implementations);
            if (implementations != null)
            {
                result = Activator.CreateInstance(typeof(List<>).MakeGenericType(tResolve));
                foreach (ImplementationInfo tImplementation in implementations)
                {
                    ((IList)result).Add(GetInstance(tImplementation));
                }
            }
            else
                throw new TypeNotRegisterException("Unknown type " + t.Name);
            return result;
        }

        //here validation for singleton
        private object GetInstance(ImplementationInfo tImplementation)
        {
            if (tImplementation.isSingleton)
            {
                return tImplementation.GetInstance(this);
            }
            else
            {
                return Create(tImplementation.implementationType);
            }
        }

        //here t is implementation  
        public object Create(Type t)
        {
            object result;
            if (!stack.Contains(t))
            {
                stack.Push(t);

                if (t.IsGenericTypeDefinition)
                {
                    t = t.MakeGenericType(currentGenericType.GenericTypeArguments);
                }

                ConstructorInfo constructor = GetRightConstructor(t);

                if (constructor != null)
                {
                    result = constructor.Invoke(GetConstructorParametersValues(constructor.GetParameters()));
                }
                else
                {
                    throw new ConstructorNotFoundException("Cannot find right constructor!");
                }
                stack.TryPop(out t);
            }
            else
            {
                result = null;
            }

            return result;
        }

        private ConstructorInfo GetRightConstructor(Type t)
        {
            ConstructorInfo result = null;
            ConstructorInfo[] constructors = t.GetConstructors();
            bool isRight;

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                isRight = true;
                foreach (ParameterInfo parameter in parameters)
                {
                    if (!config.dependencies.ContainsKey(parameter.ParameterType))
                    {
                        isRight = false;
                        break;
                    }
                }

                if (isRight)
                {
                    result = constructor;
                    break;
                }
            }
            return result;
        }

        private object[] GetConstructorParametersValues(ParameterInfo[] parameters)
        {
            object[] result = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                result[i] = Resolve(parameters[i].ParameterType);
            }
            return result;
        }
    }
}

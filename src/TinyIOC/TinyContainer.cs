using System;
using System.Collections.Generic;
using System.Reflection;

namespace TinyIOC
{
    public class TinyContainer
    {

        Dictionary<Type, Type> RegisteredServices { get; set; } = new Dictionary<Type, Type>();
        Dictionary<Type, object> instances { get; set; } = new Dictionary<Type, object>();

        public void RegisterService<TContract, TImplementation>() where TContract : class
        where TImplementation : class
        {
            this.RegisteredServices.Add(typeof(TContract), typeof(TImplementation));
        }

        public void RegisterService<TImplementation>()
        {
            this.RegisteredServices.Add(typeof(TImplementation), typeof(TImplementation));
        }

        public void RegisterSingleton<TImplementation>()
        {
            this.instances.Add(
                typeof(TImplementation),
                Activator.CreateInstance(typeof(TImplementation))
                );
        }

        public void RegisterSingleton<TImplementation>(TImplementation instance)
        where TImplementation : class
        {
            this.instances.Add(typeof(TImplementation), instance);
        }

        public T ResolveService<T>() where T : class
        {
            return (T)this.ResolveService(typeof(T));
        }

        public object ResolveService(Type type)
        {

            if (instances.TryGetValue(type, out object value))
            {
                return value;
            }

            if (RegisteredServices.TryGetValue(type, out Type implementation))
            {
                ConstructorInfo[] infoCollection = GatherConstructors(implementation);
                return ConstructService(implementation, infoCollection);
            }


            throw new ArgumentException(
                $"No services are registered that implement the given type: {type.ToString()}");


        }

        private object ConstructService(Type implementation, ConstructorInfo[] infoCollection)
        {

            for (int i = 0; i < infoCollection.Length; i++)
            {
                ConstructorInfo constructorInfo = infoCollection[i];
                ParameterInfo[] parameters = constructorInfo.GetParameters();

                if (parameters.Length == 0)
                {
                    return Activator.CreateInstance(implementation);
                }

                try
                {
                    List<object> paramInstances = ResolveParameters(parameters);
                    return Activator.CreateInstance(implementation, paramInstances.ToArray());
                }
                catch
                {
                    if (i < infoCollection.Length - 1)
                    {
                        continue;
                    }

                    // Throw on the last constructor.
                    throw;
                }
            }

            throw new ArgumentException($"Could not resolve service {implementation.FullName}");
        }

        private static ConstructorInfo[] GatherConstructors(Type implementation)
        {
            ConstructorInfo[] infoCollection = implementation.GetConstructors();

            if (infoCollection.Length <= 0)
            {
                throw new ArgumentException($"No public constructors found on type: {implementation.FullName}");
            }

            return infoCollection;
        }

        private List<object> ResolveParameters(ParameterInfo[] parameters)
        {
            var paramInstances = new List<object>();
            foreach (var par in parameters)
            {
                var paramType = par.ParameterType;
                var instance = this.ResolveService(paramType);
                paramInstances.Add(instance);
            }

            return paramInstances;
        }
    }
}
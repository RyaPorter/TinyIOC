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
            else
            {
                if (RegisteredServices.TryGetValue(type, out Type implementation))
                {
                    return this.ConstructService(implementation);
                }
                else
                {
                    throw new ArgumentException(
                        $"No services are registered that implement the given type: {type.ToString()}");
                }
            }
        }

        private object ConstructService(Type implementation)
        {
            var constructorInfo = implementation.GetConstructors();

            foreach (var con in constructorInfo)
            {
                var parameters = con.GetParameters();

                if (parameters.Length == 0)
                {
                    return Activator.CreateInstance(implementation);
                }
                else
                {
                    var paramInstances = new List<object>();
                    foreach (var par in parameters)
                    {
                        var paramType = par.ParameterType;

                        var instance = this.ResolveService(paramType);
                        paramInstances.Add(instance);
                    }

                    return Activator.CreateInstance(implementation, paramInstances.ToArray());
                }
            }

            return default;
        }

    }
}
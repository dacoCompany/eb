using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Unity;
using Unity.Exceptions;
using Unity.Resolution;

namespace Web.eBado
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (typeof(IController).IsAssignableFrom(serviceType))
            {
                return container.Resolve(serviceType, new ResolverOverride[0]);
            }
            try
            {
                return container.Resolve(serviceType, new ResolverOverride[0]);
            }

            //TODO: log error
            catch (ResolutionFailedException ex)
            {
                return (object)null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.ResolveAll(serviceType, new ResolverOverride[0]);
        }
    }
}
﻿using Infrastructure.Common.DB;
using Messaging.Email;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Web.eBado.IoC
{
    public class NinjectResolver : IDependencyResolver
    {
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectResolver"/> class.
        /// </summary>
        public NinjectResolver()
        {
            kernel = new StandardKernel();
            AddBindings();
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>
        /// The requested service or object.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>
        /// The requested services.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// Sepecific instance based on requested interface
        /// </returns>
        public static T GetInstance<T>() where T : IDisposable
        {
            return DependencyResolver.Current.GetService<T>();
        }

        /// <summary>
        /// Adds the bindings.
        /// </summary>
        private void AddBindings()
        {
            kernel.Bind<IEmailSender>().To<SmtpEmailSender>().InSingletonScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
        }
    }
}
﻿using System;
using CSCZJ.Core.Configuration;
using CSCZJ.Core.Infrastructure.DependencyManagement;
using System.Web.Http;

namespace CSCZJ.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the QMTech engine. Edit functionality, modules
    /// and implementations access most QMTech functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Container manager
        /// </summary>
        ContainerManager ContainerManager { get; }
        
        /// <summary>
        /// Initialize components and plugins in the QMTech environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(CSCZJConfig config, HttpConfiguration httpConfig);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        T[] ResolveAll<T>();
    }
}

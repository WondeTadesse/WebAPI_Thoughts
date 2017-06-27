//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2014 - Present         |
//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// Response sample builder interface
    /// </summary>
    internal interface IFluentResponseBuilder : IFluentBuilder
    {
        /// <summary>
        /// Build response sample API Documentation
        /// </summary>
        /// <param name="type">Type value</param>
        /// <returns>IFluentResponseBuilder object</returns>
        IFluentResponseBuilder BuildSample(Type type);

        /// <summary>
        /// Build response sample API Documentation
        /// </summary>
        /// <param name="successResponseType">Success response type</param>
        /// <param name="errorResponseType">Error response type</param>
        /// <returns>IFluentResponseBuilder object</returns>
        IFluentResponseBuilder BuildSample(Type successResponseType, Type errorResponseType);
    }
}

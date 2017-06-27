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
    /// Request sample builder interface
    /// </summary>
    internal interface IFluentRequestBuilder : IFluentBuilder
    {
        /// <summary>
        /// Build request sample API Documentation
        /// </summary>
        /// <param name="type">Type value</param>
        /// <param name="parameterName">ParameterName value</param>
        /// <returns>IFluentRequestBuilder object</returns>
        IFluentRequestBuilder BuildSample(Type type, string parameterName);
    }
}

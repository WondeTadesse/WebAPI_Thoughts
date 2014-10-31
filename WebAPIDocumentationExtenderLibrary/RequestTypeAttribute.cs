//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// RequestType attribute class used to decorate Web API request objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestTypeAttribute : Attribute
    {
        /// <summary>
        /// Type used to represent request object
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// ParameterName to represent request parameter name
        /// </summary>
        private string _parameterName = string.Empty;

        public override object TypeId
        {
            get
            {
                return _parameterName ?? base.TypeId;
            }
        }

        /// <summary>
        /// RequestType attribute class used to decorate Web API request objects
        /// </summary>
        /// <param name="type">Type value that represents the request value</param>
        /// <param name="parameterName">ParameterName value that represents the request value</param>
        public RequestTypeAttribute(Type type, string parameterName)
        {
            if (type == null)
                throw new ArgumentNullException("Request type value is null !");

            Type = type;
            _parameterName = parameterName;
        }

    }
}
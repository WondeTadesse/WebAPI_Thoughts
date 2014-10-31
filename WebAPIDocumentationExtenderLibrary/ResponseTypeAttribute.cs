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
    /// Response attribute class used to decorate Web API response objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ResponseTypeAttribute : Attribute
    {
        /// <summary>
        /// Type used to represent response object
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Type used to represent error response object
        /// </summary>
        public Type ErrorResponseType { get; private set; }

        /// <summary>
        /// Type used to represent error response object
        /// </summary>
        public Type SuccessResponseType { get; private set; }

        /// <summary>
        /// ResponseType attribute class used to decorate Web API response objects
        /// </summary>
        /// <param name="type">Type value that represents the response value</param>
        public ResponseTypeAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Response type value is null !");

            Type = type;
        }

        /// <summary>
        /// ResponseType attribute class used to decorate Web API response objects
        /// </summary>
        /// <param name="successResponseType">Type value that represents success response value</param>
        /// <param name="errorResponseType">Type value that represents error response value</param>
        public ResponseTypeAttribute(Type successResponseType, Type errorResponseType)
        {
            if (successResponseType == null)
                throw new ArgumentNullException("Success Response type value is null !");
            if (errorResponseType == null)
                throw new ArgumentNullException("Error Response type value is null !");

            SuccessResponseType = successResponseType;

            ErrorResponseType = errorResponseType;
        }
    }
}
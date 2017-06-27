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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// Register Response Type class
    /// </summary>
    internal sealed class RegisterResponseTypes
    {
        /// <summary>
        /// Register Response types 
        /// </summary>
        /// <param name="httpConfiguration">HttpConfiguration value</param>
        /// <param name="setSampleResponse">SampleRequest MethodInfo value</param>
        /// <param name="controllerName">ControllerName value</param>
        /// <param name="responseActions">ResponseActions value</param>
        internal static void Register(HttpConfiguration httpConfiguration, MethodInfo setSampleResponse, string controllerName, IEnumerable<MethodInfo> responseActions, MethodInfo generateObject)
        {
            try
            {
                JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

                if (httpConfiguration.Formatters != null && httpConfiguration.Formatters.Count > 0)
                {
                    jsonFormatter = httpConfiguration.Formatters.JsonFormatter;
                }

                foreach (var action in responseActions)
                {
                    var actionName = action.Name;

                    string jsonSample = string.Empty;
                    string xmlSample = string.Empty;

                    IFluentResponseBuilder jsonSampleBuilder = new JSONSampleBuilder(generateObject, jsonFormatter);
                    IFluentResponseBuilder xmlSampleBuilder = new XMLSampleBuilder(generateObject);

                    var responseCustomAttribs = Attribute.GetCustomAttributes(action);
                    if (responseCustomAttribs != null && responseCustomAttribs.Count() > 0)
                    {
                        // Check if the action is decorated with [ResponseTypeAttribute] attribute class and if so grab the type from the attribute
                        var typeQuery = responseCustomAttribs.
                            Where(rt => rt is ResponseTypeAttribute);

                        if (typeQuery != null && typeQuery.Count() > 0)
                        {
                            jsonSample = BuildSample(jsonSampleBuilder, typeQuery.FirstOrDefault(), action.ReturnType);
                            xmlSample = BuildSample(xmlSampleBuilder, typeQuery.FirstOrDefault(), action.ReturnType);
                        }
                        else
                        {
                            Type type = action.ReturnType;
                            jsonSample = jsonSampleBuilder.BuildSample(type).Sample;
                            xmlSample = xmlSampleBuilder.BuildSample(type).Sample;
                        }
                    }
                    else
                    {
                        Type type = action.ReturnType;
                        jsonSample = jsonSampleBuilder.BuildSample(type).Sample;
                        xmlSample = xmlSampleBuilder.BuildSample(type).Sample;
                    }

                    var parameters = action.GetParameters().Select(a => a.Name).ToArray();

                    setSampleResponse.Invoke(null, new object[] { httpConfiguration, jsonSample, new MediaTypeHeaderValue("text/json"), controllerName, actionName, parameters });

                    setSampleResponse.Invoke(null, new object[] { httpConfiguration, jsonSample, new MediaTypeHeaderValue("application/json"), controllerName, actionName, parameters });

                    setSampleResponse.Invoke(null, new object[] { httpConfiguration, xmlSample, new MediaTypeHeaderValue("text/xml"), controllerName, actionName, parameters });

                    setSampleResponse.Invoke(null, new object[] { httpConfiguration, xmlSample, new MediaTypeHeaderValue("application/xml"), controllerName, actionName, parameters });
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Build sample
        /// </summary>
        /// <param name="jsonFormatter">IFluentResponseBuilder value</param>
        /// <param name="attribute">Attribute value</param>
        /// <param name="defaultType">DefaultType value</param>
        /// <param name="xmlSample">Sample string value</param>
        private static string BuildSample(IFluentResponseBuilder fluentResponseBuilder, Attribute attribute, Type defaultType)
        {
            if (((ResponseTypeAttribute)attribute).Type != null)
            {
                Type type = ((ResponseTypeAttribute)attribute).Type;
                fluentResponseBuilder.BuildSample(type);
            }
            else
            {
                Type successResponseType = ((ResponseTypeAttribute)attribute).SuccessResponseType;
                Type errorResponseType = ((ResponseTypeAttribute)attribute).ErrorResponseType;

                if (successResponseType != null && errorResponseType != null)
                {
                    fluentResponseBuilder.BuildSample(successResponseType, errorResponseType);
                }
                else
                {
                    fluentResponseBuilder.BuildSample(defaultType);
                }
            }
            return fluentResponseBuilder.Sample;
        }

    }
}
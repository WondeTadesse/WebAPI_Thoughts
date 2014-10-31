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
using System.Drawing;
using System.IO;
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
    /// Register Request Type class
    /// </summary>
    internal sealed class RegisterRequestTypes
    {
        /// <summary>
        /// Register Request types 
        /// </summary>
        /// <param name="httpConfiguration">HttpConfiguration value</param>
        /// <param name="setSampleRequest">SampleRequest MethodInfo value</param>
        /// <param name="controllerName">ControllerName value</param>
        /// <param name="requestActions">RequestActions value</param>
        internal static void Register(HttpConfiguration httpConfiguration, MethodInfo setSampleRequest, string controllerName, IEnumerable<MethodInfo> requestActions, MethodInfo generateObject)
        {
            try
            {
                JsonMediaTypeFormatter jsonFormatter = null;

                if (httpConfiguration.Formatters != null && httpConfiguration.Formatters.Count > 0)
                {
                    jsonFormatter = httpConfiguration.Formatters.JsonFormatter;
                }

                foreach (var action in requestActions)
                {

                    if (action.GetParameters().Count() > 0) // Make sure request action is a parameterized action
                    {
                        // Documentation output builders
                        IFluentRequestBuilder jsonSampleBuilder = new JSONSampleBuilder(generateObject, jsonFormatter);
                        IFluentRequestBuilder xmlSampleBuilder = new XMLSampleBuilder(generateObject);

                        var actionName = action.Name;
                        var requestCustomAttribs = Attribute.GetCustomAttributes(action);

                        foreach (var parameter in action.GetParameters())
                        {
                            Type type = parameter.ParameterType;
                            string parameterName = parameter.Name;

                            if (requestCustomAttribs != null && requestCustomAttribs.Count() > 0)
                            {
                                // Check if the action is decorated with [RequestTypeAttribute] attribute class and if so grab the type from the attribute
                                var typeQuery = requestCustomAttribs.
                                    Where(rt => rt is RequestTypeAttribute).
                                    Where(p => p.TypeId.ToString().Equals(parameterName));
                                type = (typeQuery != null && typeQuery.Count() > 0) ?
                                    type = ((RequestTypeAttribute)typeQuery.FirstOrDefault()).Type : type;
                            }

                            jsonSampleBuilder = jsonSampleBuilder.BuildSample(type, parameterName);
                            xmlSampleBuilder = xmlSampleBuilder.BuildSample(type, parameterName);
                        }

                        var parameters = action.GetParameters().Select(a => a.Name).ToArray();

                        setSampleRequest.Invoke(null, new object[] { httpConfiguration, jsonSampleBuilder.Sample, new MediaTypeHeaderValue("text/json"), controllerName, actionName, parameters });

                        setSampleRequest.Invoke(null, new object[] { httpConfiguration, jsonSampleBuilder.Sample, new MediaTypeHeaderValue("application/json"), controllerName, actionName, parameters });

                        setSampleRequest.Invoke(null, new object[] { httpConfiguration, xmlSampleBuilder.Sample, new MediaTypeHeaderValue("text/xml"), controllerName, actionName, parameters });

                        setSampleRequest.Invoke(null, new object[] { httpConfiguration, xmlSampleBuilder.Sample, new MediaTypeHeaderValue("application/xml"), controllerName, actionName, parameters });
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
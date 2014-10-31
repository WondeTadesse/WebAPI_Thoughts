//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;
using System.Web.Http.Controllers;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// Register API Help class
    /// </summary>
    public static class RegisterAPIHelp
    {
        /// <summary>
        /// Register Response/Request sample API Documentation
        /// </summary>
        /// <param name="httpConfiguration">HttpConfiguration object</param>
        /// <param name="assembly">Assembly object</param>
        public static void RegisterRequestResponseHelp(this HttpConfiguration httpConfiguration, Assembly assembly = null)
        {
            try
            {
                // If assembly not provided then Get calling assembly
                if (assembly == null)
                    assembly = Assembly.GetCallingAssembly();

                // Collect type of ApiController classes from the assembly
                var apiControllerTypes = assembly
                    .GetTypes().Where(typeof(ApiController).IsAssignableFrom);

                MethodInfo setSampleRequest;
                MethodInfo setSampleResponse;
                MethodInfo generateObject;

                // Validate loaded assembly 
                ValidateAssembly(assembly, apiControllerTypes, out setSampleRequest, out setSampleResponse, out generateObject);

                BindingFlags bindingFlags =
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Public |
                    BindingFlags.Instance;

                Dictionary<string, List<MethodInfo>> totalRequestMethods = new Dictionary<string, List<MethodInfo>>();
                Dictionary<string, List<MethodInfo>> totalResponseMethods = new Dictionary<string, List<MethodInfo>>();

                foreach (var apiControllerType in apiControllerTypes)
                {
                    var controllerName = apiControllerType.Name.Substring(0,
                            apiControllerType.Name.LastIndexOf("Controller",
                                                StringComparison.OrdinalIgnoreCase));

                    var allControllerMethods = apiControllerType.GetMethods(bindingFlags);

                    // Consider only parameterized Request methods only
                    IEnumerable<MethodInfo> requestActions = allControllerMethods.Where(method =>
                        method.GetParameters() != null && method.GetParameters().Count() > 0).ToArray();

                    requestActions = FilterActions(totalRequestMethods, requestActions, controllerName);

                    // Consider return type methods only
                    IEnumerable<MethodInfo> responseActions = allControllerMethods.Where(method =>
                        method.ReturnType != typeof(void)).ToArray();

                    responseActions = FilterActions(totalResponseMethods, responseActions, controllerName);

                    // Register Request API documents
                    RegisterRequestTypes.Register(httpConfiguration, setSampleRequest, controllerName, requestActions, generateObject);

                    // Register Response API documents
                    RegisterResponseTypes.Register(httpConfiguration, setSampleResponse, controllerName, responseActions, generateObject);

                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Filter any documented Request/Response actions from API controller
        /// </summary>
        /// <param name="controllerActions">ControllerActions value</param>
        /// <param name="actions">Actions value</param>
        /// <param name="apiControllerName">ApiControllerName value</param>
        /// <returns>Filtered Actions</returns>
        private static IEnumerable<MethodInfo> FilterActions(Dictionary<string, List<MethodInfo>> controllerActions, IEnumerable<MethodInfo> actions, string apiControllerName)
        {
            apiControllerName = apiControllerName.ToLower();
            if (controllerActions.Count() == 0)
                controllerActions.Add(apiControllerName, actions.ToList());
            else
            {
                if (controllerActions.ContainsKey(apiControllerName))
                {
                    IEnumerable<MethodInfo> filteredActions = (
                        from action in actions
                        where !(from a in
                                    controllerActions[apiControllerName].Select(
                                        m => string.Concat(m.Name,
                                        string.Join("",
                                        (from p in
                                             m.GetParameters()
                                         select p.Name).ToArray())).ToLower())
                                select a).Contains(
                                string.Concat(action.Name,
                                string.Join("",
                                (from p
                                     in action.GetParameters()
                                 select p.Name).ToArray())).ToLower())
                        select action).ToArray();

                    controllerActions[apiControllerName].AddRange(filteredActions);
                    return filteredActions;
                }
                else
                    controllerActions.Add(apiControllerName, actions.ToList());
            }
            return actions;
        }

        /// <summary>
        /// Validate loaded assembly 
        /// </summary>
        /// <param name="assembly">Assembly value</param>
        /// <param name="apiControllerTypes">ApiContollerTypes value</param>
        /// <param name="setSampleRequest">SetSampleRequest value</param>
        /// <param name="setSampleResponse">SetSampleResponse value</param>
        /// <param name="generateObject">GenerateObject value</param>
        private static void ValidateAssembly(Assembly assembly, IEnumerable<Type> apiControllerTypes, out MethodInfo setSampleRequest, out MethodInfo setSampleResponse, out MethodInfo generateObject)
        {
            if (apiControllerTypes == null) // Check if loaded Assembly contains ApiContoller
                throw new ArgumentException("APIController class could not be found from the assembly !");

            Type helpPageConfigurationExtensions = assembly.GetTypes().Where(t => t.FullName.Contains("HelpPageConfigurationExtensions")).FirstOrDefault();
            Type objectGeneratorClass = assembly.GetTypes().Where(t => t.FullName.Contains("ObjectGenerator")).FirstOrDefault();

            if (helpPageConfigurationExtensions == null) // Check if loaded Assembly contains HelpPageConfigurationExtensions class
                throw new Exception("HelpPageConfigurationExtensions class could not be found from the assembly !");

            if (objectGeneratorClass == null) // Check if loaded Assembly contains ObjectGenerator class
                throw new Exception("ObjectGenerator class could not be found from the assembly !");

            setSampleRequest = helpPageConfigurationExtensions.GetMethod("SetSampleRequest", Types());

            if (setSampleRequest == null) // Check if HelpPageConfigurationExtensions class contains SetSampleRequest method
                throw new Exception("SetSampleRequest method could not be found from HelpPageConfigurationExtensions class !");

            setSampleResponse = helpPageConfigurationExtensions.GetMethod("SetSampleResponse", Types());

            if (setSampleResponse == null) // Check if HelpPageConfigurationExtensions class contains SetSampleResponse method
                throw new Exception("SetSampleResponse method could not be found from HelpPageConfigurationExtensions class !");

            generateObject = objectGeneratorClass.GetMethod("GenerateObject", new Type[] { typeof(Type) });

            if (generateObject == null) // Check if ObjectGenerator class contains GenerateObject method
                throw new Exception("GenerateObject method could not be found from ObjectGenerator class !");

        }

        /// <summary>
        /// Array of Types
        /// </summary>
        /// <returns>array of Types</returns>
        private static Type[] Types()
        {
            return new Type[] { typeof(HttpConfiguration), typeof(object), typeof(MediaTypeHeaderValue), typeof(string), typeof(string), typeof(string[]) };
        }
    }
}
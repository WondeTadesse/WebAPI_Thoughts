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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// JSON sample builder class
    /// </summary>
    sealed class JSONSampleBuilder : APISampleBuilder
    {
        private JsonMediaTypeFormatter _jsonFormatter;

        /// <summary>
        /// Json formatter class
        /// </summary>
        public JsonMediaTypeFormatter JsonFormatter
        {
            get { return _jsonFormatter; }
        }


        /// <summary>
        /// JSON sample builder class
        /// </summary>
        /// <param name="generateObject">GenerateObject methodinfo object</param>
        /// <param name="jsonFormatter">JsonMediaTypeFormatter object</param>
        public JSONSampleBuilder(MethodInfo generateObject, JsonMediaTypeFormatter jsonFormatter)
            : base(generateObject)
        {
            _jsonFormatter = jsonFormatter;
        }

        /// <summary>
        /// BuildSample API Documentation sample
        /// </summary>
        /// <param name="instance">Instance value</param>
        /// <returns>IFluentBuilder object</returns>
        public override IFluentBuilder BuildSample(object instance)
        {
            string json = string.Empty;
            try
            {
                // Helps to serialzied the exact type of the object. i.e Base vs Drived classes 
                
                JsonSerializerSettings jss = new JsonSerializerSettings();

                if (_jsonFormatter != null && _jsonFormatter.SerializerSettings != null &&
                    _jsonFormatter.SerializerSettings.TypeNameHandling != TypeNameHandling.None)
                {
                    jss = _jsonFormatter.SerializerSettings;
                }
                else
                {
                    jss.TypeNameHandling = TypeNameHandling.Auto;
                };
                json = JsonConvert.SerializeObject(instance, Formatting.Indented, jss);
            }
            catch (Exception)
            {
            }
            return base.BuildSample(json);
        }
    }
}

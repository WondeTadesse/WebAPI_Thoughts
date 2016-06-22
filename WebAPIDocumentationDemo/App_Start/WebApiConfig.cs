using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using POCOLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIDocumentationExtenderLibrary;
namespace WebAPIDocumentationDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new ExternalPhysicianConvertor());
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new InternalPhysicianConvertor());

            // ExternalPhysicianBase and InternalPhysicianBase XmlSerializer 
            config.Formatters.XmlFormatter.SetSerializer(typeof(ExternalPhysicianBase), new DataContractSerializer(typeof(ExternalPhysician)));
            config.Formatters.XmlFormatter.SetSerializer(typeof(InternalPhysicianBase), new DataContractSerializer(typeof(InternalPhysician)));
            config.RegisterRequestResponseHelp();
        }
    }

    #region JSON Convertors 
    
    /*
     For more Convertors create a generic class, locate the instance from service locator inside Create method. i.e
     * 
     * 
     public class GenericConvertor<T> : CustomCreationConverter<T>
     {
        public InternalPhysicianConvertor() { }

        /// <summary>
        /// Internal Physician Converter 
        /// </summary>
        public override T Create(Type objectType)
        {
            return (T)ServiceLocator.Current.GetInstance(objectType);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var obj = Create(objectType);
            serializer.Populate(reader, obj);
            return obj;
        }
    }
     */


    /// <summary>
    /// Internal Physician Converter 
    /// </summary>
    public class InternalPhysicianConvertor : CustomCreationConverter<InternalPhysicianBase>
    {
        public InternalPhysicianConvertor() { }

        /// <summary>
        /// Internal Physician Converter 
        /// </summary>
        public override InternalPhysicianBase Create(Type objectType)
        {
            return new InternalPhysician();
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var obj = Create(objectType);
            serializer.Populate(reader, obj);
            return obj;
        }
    }

    /// <summary>
    /// External Physician Converter 
    /// </summary>
    public class ExternalPhysicianConvertor : CustomCreationConverter<ExternalPhysicianBase>
    {
        /// <summary>
        /// External Physician Converter 
        /// </summary>
        public ExternalPhysicianConvertor() { }

        public override ExternalPhysicianBase Create(Type objectType)
        {
            return new ExternalPhysician();
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var obj = Create(objectType);
            serializer.Populate(reader, obj);
            return obj;
        }
    }

    #endregion
}

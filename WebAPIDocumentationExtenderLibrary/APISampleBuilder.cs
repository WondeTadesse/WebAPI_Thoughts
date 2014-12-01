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
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// API Documentation Builder abstract class
    /// </summary>
    internal abstract class APISampleBuilder : IFluentRequestBuilder, IFluentResponseBuilder
    {
        #region Private Variables

        private static string messageLiner = "\n--------------------------------------------------------------------------------------------------------------------------------\n";
        private StringBuilder sampleStringBuilder;
        private MethodInfo generateObject;

        #endregion

        #region Constructor

        /// <summary>
        /// API Documentation Builder abstract class
        /// </summary>
        /// <param name="generateObject">GenerateObject methodinfo object</param>
        public APISampleBuilder(MethodInfo generateObject)
        {
            sampleStringBuilder = new StringBuilder();
            this.generateObject = generateObject;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get Sample
        /// </summary>
        public string Sample { get { return (sampleStringBuilder ?? new StringBuilder()).ToString(); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Build sample API Documentation
        /// </summary>
        /// <param name="instance">Instance value</param>
        /// <returns>IFluentBuilder object</returns>
        public abstract IFluentBuilder BuildSample(object instance);

        /// <summary>
        /// Build sample API Documentation
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>IFluentBuilder object</returns>
        public IFluentBuilder BuildSample(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                sampleStringBuilder.AppendLine(input);
            return this;
        }

        /// <summary>
        /// Build request sample
        /// </summary>
        /// <param name="type">Type value</param>
        /// <param name="parameterName">ParameterName value</param>
        /// <returns>IFluentRequestBuilder object</returns>
        public IFluentRequestBuilder BuildSample(Type type, string parameterName)
        {
            string header = "Request";
            string messageHeader = string.Empty;
            if (!string.IsNullOrWhiteSpace(parameterName))
            {
                messageHeader = string.Format("{0} sample for {1} ", header, parameterName);
                BuildSample(messageHeader)
                    .BuildSample(messageLiner);
            }
            else
                BuildSample(messageLiner);

            return BuilderSample(type, header) as IFluentRequestBuilder;
        }

        /// <summary>
        /// Build response sample
        /// </summary>
        /// <param name="type">Type value</param>
        /// <returns>IFluentResponseBuilder object</returns>
        public IFluentResponseBuilder BuildSample(Type type)
        {
            string header = "Response";
            BuildSample(messageLiner);
            return BuilderSample(type, header) as IFluentResponseBuilder;
        }

        /// <summary>
        /// Build response sample API Documentation
        /// </summary>
        /// <param name="successResponseType">Success response type</param>
        /// <param name="errorResponseType">Error response type</param>
        /// <returns>IFluentResponseBuilder object</returns>
        public IFluentResponseBuilder BuildSample(Type successResponseType, Type errorResponseType)
        {
          return ((BuildSample("Success response message sample") as IFluentResponseBuilder)
                         .BuildSample(successResponseType)
                         .BuildSample("Error response message sample") as IFluentResponseBuilder)
                         .BuildSample(errorResponseType);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Build sample
        /// </summary>
        /// <param name="type">Type value</param>
        /// <param name="header">Header value</param>
        /// <returns>IFluentBuilder object</returns>
        private IFluentBuilder BuilderSample(Type type, string header)
        {
            string messageHeader = string.Empty;
            if (type == typeof(HttpRequestMessage) ||
                type == typeof(HttpResponseMessage) ||
                type == typeof(object))
            {
                messageHeader = string.Format("{0} type is {1}.{2}", header, type.Name, "Sample is not available.");
                BuildSample(messageHeader)
                    .BuildSample(messageLiner);
            }
            else
            {
                object instance = GenerateSampleObject(type);
                if (instance != null)
                {
                    BuildSample(instance)
                        .BuildSample(messageLiner);
                }
                else
                {
                    Type topType = GetPossibleSerializableType(type);
                    instance = GenerateSampleObject(topType);
                    if (instance != null)
                    {
                        string message = string.Format("Actual {0} type is", header);

                        message = string.Concat(message, BuildSampleHeader(type));

                        BuildSample(string.Concat(message,
                            ".But the following ", header.ToLower(), string.Concat(" value can also be ", header.ToLower().Equals("response") ? "expected." : "used.")))
                            .BuildSample(messageLiner);

                        BuildSample(instance)
                            .BuildSample(messageLiner);
                    }
                    else
                    {
                        List<string> typeProperties = (from p in type.GetProperties()
                                                       select string.Concat(p.PropertyType, " ", p.Name)).ToList();
                        messageHeader = BuildSampleHeader(type, header, typeProperties);
                        BuildSample(messageHeader)
                            .BuildSample(messageLiner);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Build sample header for Abstract/Interface
        /// </summary>
        /// <param name="type">Type value</param>
        /// <returns>string value</returns>
        private static string BuildSampleHeader(Type type)
        {
            if (type.IsAbstract)
                if (type.IsInterface)
                    return string.Concat(" an interface ", type.Name);
                else
                    return string.Concat(" an abstract class ", type.Name);
            return string.Concat(" ", type.Name);
        }

        /// <summary>
        /// Build sample header for Abstract/Interface
        /// </summary>
        /// <param name="type">Type value</param>
        /// <param name="header">Header value</param>
        /// <param name="input">Input value</param>
        /// <returns>string value</returns>
        private static string BuildSampleHeader(Type type, string header, string input)
        {
            if (type.IsInterface)
                return string.Concat(input, ".Sample is not available.\nAny type that implements this interface can be ", header.ToLower().Equals("response") ? "expected." : "used.");
            return string.Concat(input, ".Sample is not available.\nAny type that drives from this class can be ", header.ToLower().Equals("response") ? "expected." : "used.");
        }

        /// <summary>
        /// Build sample header for types that can't produce sample
        /// </summary>
        /// <param name="type">Type value</param>
        /// <param name="header">Header value</param>
        /// <param name="typeProperties">Type properties value</param>
        /// <returns>string value</returns>
        private static string BuildSampleHeader(Type type, string header, List<string> typeProperties)
        {
            string messageHeader = string.Format("Actual {0} type is", header);
            string properties = typeProperties != null && typeProperties.Count > 0 ?
                string.Join(",", typeProperties) : string.Empty;

            if (!string.IsNullOrWhiteSpace(properties))
            {
                string pluralSingular = typeProperties.Count > 1 ? "ies are" : "y is";
                if (type.IsAbstract)
                {
                    messageHeader = string.Concat(messageHeader, string.Format("{0}.It's propert{1} {2}", BuildSampleHeader(type), pluralSingular, properties));
                    messageHeader = BuildSampleHeader(type, header, messageHeader);
                }
                else
                    messageHeader = string.Format("{0} type is {1}.It's propert{2} {3}.{4}", header, type.Name, pluralSingular, properties, "Sample is not available.");
            }
            else
            {
                if (type.IsAbstract)
                    messageHeader = string.Concat(messageHeader, BuildSampleHeader(type, header, BuildSampleHeader(type)));
                else
                    messageHeader = string.Format("{0} type is {1}.{2}", header, type.Name, "Sample is not available.");
            }
            return messageHeader;
        }

        /// <summary>
        ///  Get a possible serializable type
        /// </summary>
        /// <param name="type">Type value</param>
        /// <returns>Type object</returns>
        private Type GetPossibleSerializableType(Type type)
        {
            Type topBase = type.BaseType;
            while (topBase != null)
            {
                if (topBase != null && ((topBase.IsClass && !topBase.IsAbstract && topBase != typeof(System.Object)) ||
                    topBase.IsGenericType || topBase.IsGenericTypeDefinition))
                    return topBase;
                topBase = topBase.BaseType;
            }
            return type;// Is not serializable
        }

        /// <summary>
        /// Generate object by using API Documentation ObjectGenerator class through Reflection.
        /// </summary>
        /// <param name="type">Type value</param>
        /// <returns>object value</returns>
        private object GenerateSampleObject(Type type)
        {
            return generateObject.Invoke(Activator.CreateInstance(generateObject.DeclaringType), new object[] { type });
        }

        #endregion




    }
}

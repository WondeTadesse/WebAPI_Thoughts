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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// XML Sample builder class
    /// </summary>
    sealed class XMLSampleBuilder : APISampleBuilder
    {
        /// <summary>
        /// XML sample builder class
        /// </summary>
        /// <param name="generateObject">GenerateObject methodinfo object</param>
        public XMLSampleBuilder(MethodInfo generateObject)
            : base( generateObject)
        {
        }

        /// <summary>
        /// BuildSample API Documentation sample
        /// </summary>
        /// <param name="instance">Instance value</param>
        /// <returns>IFluentBuilder object</returns>
        public override IFluentBuilder BuildSample(object instance)
        {
            string xml = string.Empty;
            try
            {
                using (Stream streamWriter = new MemoryStream())
                using (StreamReader streamReader = new StreamReader(streamWriter))
                {
                    DataContractSerializer xmlSerializer = new DataContractSerializer(instance.GetType());
                    xmlSerializer.WriteObject(streamWriter, instance);
                    streamWriter.Position = 0;
                    xml = streamReader.ReadToEnd();
                    xml = XElement.Parse(xml).ToString(); // Helps for proper indentation
                }
            }
            catch (Exception)
            {
            }
            return base.BuildSample(xml);
        }
    }
}

//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2014 - Present         |
//|---------------------------------------------------------------|
//|           WEB API DOUCMENTATION EXTENDER LIBRARY              |
//|---------------------------------------------------------------|
using System;
namespace WebAPIDocumentationExtenderLibrary
{
    /// <summary>
    /// API Documentation Builder interface
    /// </summary>
    internal interface IFluentBuilder
    {
        /// <summary>
        /// Get Sample
        /// </summary>
        string Sample { get; }

        /// <summary>
        /// Build sample API Documentation
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>IFluentBuilder object</returns>
        IFluentBuilder BuildSample(string input);

    }
}

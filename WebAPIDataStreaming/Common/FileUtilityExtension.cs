//|---------------------------------------------------------------|
//|                     WEB API DATA STREAMING                    |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                     WEB API DATA STREAMING                    |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.WebHost;

namespace WebAPIDataStreaming.Common
{
    /// <summary>
    /// File utility extension class
    /// </summary>
    public static class FileUtilityExtension
    {
        /// <summary>
        /// Get Context object
        /// </summary>
        /// <param name="controller">ApiController value</param>
        /// <returns>HttpContextWrapper object</returns>
        public static HttpContextWrapper GetContext(this ApiController controller)
        {
            return ((HttpContextWrapper)controller.Request.Properties["MS_HttpContext"]);
        }

        /// <summary>
        /// Get upload path
        /// </summary>
        /// <param name="controller">ApiController value</param>
        /// <returns>string value</returns>
        public static string GetUploadPath(this ApiController controller)
        {
            return controller.GetContext().Server.MapPath("~/Uploads");
        }

        /// <summary>
        /// Get Request object
        /// </summary>
        /// <param name="controller">ApiController value</param>
        /// <returns>HttpRequestBase object</returns>
        public static HttpRequestBase Request(this ApiController controller)
        {
            return controller.GetContext().Request;
        }

        /// <summary>
        /// Get request content length
        /// </summary>
        /// <param name="controller">ApiController value</param>
        /// <returns>long value</returns>
        public static long GetRequestContentLength(this ApiController controller)
        {
            return controller.GetContext().Request.ContentLength;
        }

        /// <summary>
        /// Get Download Path
        /// </summary>
        /// <param name="controller">ApiController value</param>
        /// <returns>String value</returns>
        public static string GetDownloadPath(this ApiController controller)
        {
            return controller.GetContext().Server.MapPath("~/Downloads");
        }

        /// <summary>
        /// Write stream to a file
        /// </summary>
        /// <param name="inputStream">InputStream value</param>
        /// <param name="filePath">FilePath valye</param>
        /// <returns>true/false</returns>
        public static bool WriteStream(this Stream inputStream, string filePath)
        {
            try
            {
                using (FileStream outPutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[1048575]; // 1MB buffer
                    while (true)
                    {
                        int read = inputStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                            outPutStream.Write(buffer, 0, read);
                        else
                            return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Write stream to a file
        /// </summary>
        /// <param name="httpPostedFile">HttpPostedFile value</param>
        /// <param name="filePath">FilePath value</param>
        /// <returns>true/false</returns>
        public static bool WriteStream(this HttpPostedFileBase httpPostedFile, string filePath)
        {
            try
            {
                return httpPostedFile.InputStream.WriteStream(filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Read stream value
        /// </summary>
        /// <param name="fileStream">FileStream to be read</param>
        /// <param name="filePath">FilePath value</param>
        /// <returns>Stream object</returns>
        public static Stream ReadStream(this FileInfo fileInfo)
        {
            int bufferSize = 1048575; // 1MB
            return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);;
        }

    }
}
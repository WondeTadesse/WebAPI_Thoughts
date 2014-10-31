//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIClient
{
    /// <summary>
    /// Data streaming extension class
    /// </summary>
    public static class DataStreamingExtensions
    {
        #region Shared Variable 

        private static string _uploadErrorMesage = string.Empty;

        #endregion

        #region Extension Methods 
  
        /// <summary>
        /// Check MultipartFormDataContent has value
        /// </summary>
        /// <param name="multipartFormDataContent">MultipartFormDataContent value</param>
        /// <returns>true/false</returns>
        public static bool HasContent(this MultipartFormDataContent multipartFormDataContent)
        {
            return multipartFormDataContent != null && multipartFormDataContent.Count() > 0;
        }

        /// <summary>
        /// Get upload file error message. Works with AddUploadFile method 
        /// </summary>
        /// <param name="multipartFormDataContent">MultipartFormDataContent value</param>
        /// <returns>string value</returns>
        public static string GetUploadFileErrorMesage(this MultipartFormDataContent multipartFormDataContent)
        {
            string tempErrorMessage = _uploadErrorMesage;
            _uploadErrorMesage = string.Empty; // reset 
            return tempErrorMessage;
        }

        /// <summary>
        /// Read content of file
        /// </summary>
        /// <param name="content">Content value</param>
        /// <param name="fileFullPath">FileFullPath value</param>
        /// <param name="overWrite">Overwrite value</param>
        /// <returns>Awaitable task value</returns>
        public static async Task<bool> ReadContent(this HttpContent content, string fileFullPath, bool overWrite = true)
        {
            FileStream fileStream = null;
            bool result = false;
            try
            {
                content.ValidateDownload(fileFullPath, overWrite);

                fileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None);

                await content.CopyToAsync(fileStream).
                   ContinueWith(
                   (copyTask)
                       =>
                   {
                       fileStream.Close();
                       result = !result;
                   });
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                if (fileStream != null)
                    fileStream.Close();

                throw directoryNotFoundException;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                if (fileStream != null)
                    fileStream.Close();

                throw invalidOperationException;
            }
            catch (Exception exception)
            {
                if (fileStream != null)
                    fileStream.Close();

                throw new Exception("Unable to read content ", exception);
            }
            return result;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="content">Content value</param>
        /// <param name="fileFullPath">FileFullPath value</param>
        /// <param name="overWrite">Overwrite value</param>
        /// <returns>Awaitable task value</returns>
        public static async Task<string> DownloadFile(this HttpContent content, string fileFullPath, bool overWrite = true)
        {
            FileStream fileStream = null;
            string result = string.Empty;
            try
            {
                return await
                    content.ReadContent(fileFullPath, overWrite).
                    ContinueWith((response)
                        =>
                    {
                        try
                        {
                            if (!response.IsFaulted && response.Result)
                            {
                                result = string.Format("Download completed @ {0}, {1} time ",
                                    DateTime.Now.ToLongDateString(),
                                    DateTime.Now.ToLongTimeString());
                            }
                            else
                            {
                                result = string.Format("Download not succeeded @ {0}, {1} time \nException : {2}",
                                    DateTime.Now.ToLongDateString(),
                                    DateTime.Now.ToLongTimeString(),
                                    JsonConvert.SerializeObject(response.Exception, Formatting.Indented));
                            }
                        }
                        catch (AggregateException aggregateException)
                        {
                            throw aggregateException;
                        }
                        return result;
                    });
            }
            catch (Exception exception)
            {
                if (fileStream != null)
                    fileStream.Close();

                throw new Exception("Unable to download file", exception);
            }
        }

        /// <summary>
        /// Try to get result from Task
        /// </summary>
        /// <typeparam name="T">Type value</typeparam>
        /// <param name="task">Task value</param>
        /// <returns>Object value</returns>
        public static object TryResult<T>(this Task<T> task)
        {
            if (task != null)
            {
                try
                {
                    return task.Result;
                }
                catch (Exception)
                {
                }
            }
            return task;
        }

        /// <summary>
        /// Add upload file value
        /// </summary>
        /// <param name="multipartFormDataContent">MultipartFormDataContent value</param>
        /// <param name="fileFullName">FileFullName value</param>
        public static void AddUploadFile(this MultipartFormDataContent multipartFormDataContent, string fileFullName)
        {
            try
            {
                if (!File.Exists(fileFullName))
                {
                    _uploadErrorMesage += string.Format("File {0} is not found !", fileFullName) + "\n";
                }
                else
                {
                    StreamContent file = new StreamContent(new FileStream(fileFullName, FileMode.Open, FileAccess.Read));
                    file.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                    file.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    file.Headers.ContentDisposition.FileName = Path.GetFileName(fileFullName);
                    multipartFormDataContent.Add(file);
                    multipartFormDataContent.ValidateUpload();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Validate download
        /// </summary>
        /// <param name="content">Content value</param>
        /// <param name="fileFullPath">FileFullPath value</param>
        /// <param name="overWrite">Overwrite value</param>
        private static void ValidateDownload(this HttpContent content, string fileFullPath, bool overWrite)
        {
            string directory = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(string.Format("{0} directory is not found !", directory));

            if (!overWrite && File.Exists(fileFullPath))
                throw new InvalidOperationException(string.Format("{0} file is already exists !", fileFullPath));

            if (!content.Headers.ContentLength.HasValue && content.Headers.ContentLength.Value > 0)
                throw new InvalidOperationException(string.Format("{0} file stream content is empty !", fileFullPath));
        }

        /// <summary>
        /// Validate upload value
        /// </summary>
        /// <param name="multipartFormDataContent">MultipartFormDataContent value</param>
        private static void ValidateUpload(this MultipartFormDataContent multipartFormDataContent)
        {
            double gigaSize = 0.0;

            if (multipartFormDataContent != null && multipartFormDataContent.Count() > 0)
            {
                long totalUploadFileSize = 0;
                foreach (var item in multipartFormDataContent)
                    totalUploadFileSize += item.Headers.ContentLength ?? item.Headers.ContentLength.Value;

                gigaSize = Math.Round(totalUploadFileSize / (1024.0 * 1024.0 * 1024.0), 5);

                if (totalUploadFileSize > int.MaxValue) // Max uploadable size is 2GB 
                    throw new InvalidOperationException(string.Format("Upload data content size is ({0} GB) which is beyond maximum allowed size(2.0 GB) !", gigaSize));
            }
        }
       
        #endregion
    }
}

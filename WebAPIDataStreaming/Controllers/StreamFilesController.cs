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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using WebAPIDataStreaming.Common;
using POCOLibrary;

namespace WebAPIDataStreaming.Controllers
{
    /// <summary>
    /// File streaming API
    /// </summary>
    [RoutePrefix("filestreaming")]
    [RequestModelValidator]
    public class StreamFilesController : ApiController
    {
        /// <summary>
        /// Get File meta data
        /// </summary>
        /// <param name="fileName">FileName value</param>
        /// <returns>FileMeta data response.</returns>
        [Route("getfilemetadata")]
        public HttpResponseMessage GetFileMetaData(string fileName)
        {
            try
            {
                string filePath = string.Concat(this.GetDownloadPath(), "\\", fileName);
                FileInfo fileInfo = new FileInfo(filePath);
                FileMetaData metaData = new FileMetaData();

                if (!fileInfo.Exists)
                {
                    metaData.FileResponseMessage.IsExists = false;
                    metaData.FileResponseMessage.Content = string.Format("{0} file is not found !", fileName);
                    return Request.CreateResponse(HttpStatusCode.NotFound, metaData, new MediaTypeHeaderValue("text/json"));
                }

                metaData.FileResponseMessage.IsExists = true;
                metaData.FileName = fileName;
                metaData.FileExtension = fileInfo.Extension;
                metaData.FileSize = fileInfo.Length;
                metaData.FilePath = filePath;
                metaData.FileResponseMessage.Content = string.Format("{0} file is found !", fileName);
                return Request.CreateResponse(HttpStatusCode.OK, metaData, new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }

        }

        /// <summary>
        /// Search file and return it's meta data in all download directories
        /// </summary>
        /// <param name="fileName">FileName value</param>
        /// <returns>List of file meta datas response</returns>
        [HttpGet]
        [Route("searchfileindownloaddirectory")]
        public HttpResponseMessage SearchFileInDownloadDirectory(string fileName)
        {
            try
            {
                string[] files = Directory.GetFiles(this.GetDownloadPath(), fileName, SearchOption.AllDirectories);

                if (files != null && files.Count() > 0)
                {
                    List<FileMetaData> filesMetaData = new List<FileMetaData>();
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        FileMetaData metaData = new FileMetaData();
                        metaData.FileResponseMessage.IsExists = true;
                        metaData.FileName = fileName;
                        metaData.FileExtension = fileInfo.Extension;
                        metaData.FileSize = fileInfo.Length;
                        metaData.FilePath = file.Replace(fileInfo.Name, "");
                        filesMetaData.Add(metaData);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, filesMetaData, new MediaTypeHeaderValue("text/json"));
                }

                return Request.CreateResponse(HttpStatusCode.NotFound,
                    new FileMetaData()
                    {
                        FileResponseMessage = new FileResponseMessage
                        {
                            IsExists = false,
                            Content = string.Format("{0} file is not found !", fileName)
                        }
                    }, new MediaTypeHeaderValue("text/json"));
            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
        }

        /// <summary>
        /// Asynchronous Download file
        /// </summary>
        /// <param name="fileName">FileName value</param>
        /// <returns>Tasked File stream response</returns>
        [Route("downloadasync")]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadFileAsync(string fileName)
        {
            return await new TaskFactory().StartNew(
                () =>
                {
                    return DownloadFile(fileName);
                });
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="fileName">FileName value</param>
        /// <returns>File stream response</returns>
        [Route("download")]
        [HttpGet]
        public HttpResponseMessage DownloadFile(string fileName)
        {
            HttpResponseMessage response = Request.CreateResponse();
            try
            {
                string filePath = string.Concat(this.GetDownloadPath(), "\\", fileName);
                FileInfo fileInfo = new FileInfo(filePath);
                FileMetaData metaData = new FileMetaData();

                if (!fileInfo.Exists)
                {
                    metaData.FileResponseMessage.IsExists = false;
                    metaData.FileResponseMessage.Content = string.Format("{0} file is not found !", fileName);
                    response = Request.CreateResponse(HttpStatusCode.NotFound, metaData, new MediaTypeHeaderValue("text/json"));
                }
                else
                {
                    response.Headers.AcceptRanges.Add("bytes");
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StreamContent(fileInfo.ReadStream());
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response.Content.Headers.ContentLength = fileInfo.Length;
                }
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
            return response;
        }


        /// <summary>
        /// Upload file(s)
        /// </summary>
        /// <param name="overWrite">An indicator to overwrite a file if it exist in the server.</param>
        /// <returns>Message response</returns>
        [Route("upload")]
        [HttpPost]
        public HttpResponseMessage UploadFile(bool overWrite)
        {
            HttpResponseMessage response = Request.CreateResponse();
            List<FileResponseMessage> fileResponseMessages = new List<FileResponseMessage>();
            FileResponseMessage fileResponseMessage = new FileResponseMessage { IsExists = false, };

            try
            {

                if (!Request.Content.IsMimeMultipartContent())
                {
                    fileResponseMessage.Content = "Upload data request is not valid !";
                    fileResponseMessages.Add(fileResponseMessage);
                    response = Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, fileResponseMessages, new MediaTypeHeaderValue("text/json"));
                }

                else
                {
                    response = ProcessUploadRequest(overWrite);
                }
            }
            catch (Exception exception)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
            }
            return response;
        }

        /// <summary>
        /// Asynchronous Upload file
        /// </summary>
        /// <param name="overWrite">An indicator to overwrite a file if it exist in the server.</param>
        /// <returns>Tasked Message response</returns>
        [Route("uploadasync")]
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFileAsync(bool overWrite)
        {
            return await new TaskFactory().StartNew(
               () =>
               {
                   return UploadFile(overWrite);
               });
        }

        /// <summary>
        /// Process upload request in the server
        /// </summary>
        /// <param name="overWrite">An indicator to overwrite a file if it exist in the server.</param>
        /// <returns>List of message object</returns>
        private HttpResponseMessage ProcessUploadRequest(bool overWrite)
        {
            List<FileResponseMessage> fileResponseMessages = new List<FileResponseMessage>();

            HttpResponseMessage response = null;
            FileResponseMessage fileResponseMessage = new FileResponseMessage();

            if (this.GetRequestContentLength() > WebApiApplication.MaxRequestLength)
            {
                int maxSize = Convert.ToInt32(Math.Round(WebApiApplication.MaxRequestLength / 1024.0, 1));
                fileResponseMessage.Content = string.Format("Upload data content size is beyond maximum size({0}MB) allowed by the server !",
                    maxSize < 1 ? 1 : maxSize);
                fileResponseMessages.Add(fileResponseMessage);
                response = Request.CreateResponse(HttpStatusCode.BadRequest, fileResponseMessages, new MediaTypeHeaderValue("text/json"));
            }
            else
            {
                try
                {
                    HttpRequestBase request = this.Request();
                    string uploadPath = this.GetUploadPath();
                    HttpFileCollectionBase files = request.Files;

                    foreach (string file in files)
                    {
                        string filePath = string.Concat(uploadPath, "\\", file);
                        fileResponseMessage = new FileResponseMessage();

                        if (!overWrite && File.Exists(filePath))
                        {
                            fileResponseMessage.Content = string.Format("{0} file already exist !", file);
                            fileResponseMessage.IsExists = false;
                        }
                        else
                        {
                            if (files[file].WriteStream(filePath))
                            {
                                fileResponseMessage.Content = string.Format("{0} file uploaded successfully !", file);
                                fileResponseMessage.IsExists = true;
                            }
                            else
                            {
                                fileResponseMessage.Content = string.Format("Can't upload {0} file !", file);
                                fileResponseMessage.IsExists = false;
                            }
                        }
                        fileResponseMessages.Add(fileResponseMessage);
                    }
                    if (fileResponseMessages.Count == 0)
                    {
                        fileResponseMessage = new FileResponseMessage();
                        fileResponseMessage.Content = "No upload file(s) found !";
                        fileResponseMessages.Add(fileResponseMessage);
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, fileResponseMessages, new MediaTypeHeaderValue("text/json"));
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, fileResponseMessages, new MediaTypeHeaderValue("text/json"));
                    }
                }
                catch (Exception exception)
                {
                    response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exception);
                }
            }
            return response;
        }


    }
}

//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                         WEB API CLIENT                        |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;
using System.Net.NetworkInformation;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;

using POCOLibrary;
using WebAPICommonLibrary;


namespace WebAPIClient
{
    public class Program
    {
        #region Private Static Variables

        static string serverFullName;
        static Uri baseStreamingURL;
        static Uri serverDefaultSecureURL;
        static Uri secureURL;

        #endregion

        static void Main(string[] args)
        {
            try
            {
                serverFullName = string.Format("{0}.{1}", Environment.MachineName, IPGlobalProperties.GetIPGlobalProperties().DomainName);
                baseStreamingURL = new Uri(string.Format("http://{0}/WebAPIDataStreaming/filestreaming/", serverFullName)); //"http://localhost:2461/filestreaming/");//
                serverDefaultSecureURL = new Uri(string.Format("https://{0}/", serverFullName)); //"https://localhost/");//
                secureURL = new Uri(string.Format("https://{0}/WebAPISecureSocketLayering/", serverFullName));//"https://localhost:44302");//
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Concat("Exception : ", exception));
                Console.ReadLine();
                return;
            }

            MainMenu();

            ExecuteOperation(() => Console.WriteLine("You exited the system successfully !"), 2);
        }

        #region Main Operations

        /// <summary>
        /// Main menu
        /// </summary>
        private static void MainMenu()
        {
            ConsoleKeyInfo keyInfo;
            bool isNewLineRequired = false;
            Console.WindowWidth = 90;

            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                if (isNewLineRequired)
                    Console.WriteLine();
                else
                    isNewLineRequired = true;

                Console.WriteLine("Select the operation you want to perform !\nNote: Each operation result will be displayed for only 5 seconds.\n");
                Console.WriteLine("Select 1. To perform GetFileMetaData.");
                Console.WriteLine("Select 2. To perform Download. Default Download files path is C:\\");
                Console.WriteLine("Select 3. To perform Upload. Default path for files to be uploaded is C:\\");
                Console.WriteLine("Select 4. To perform IIS Https Call.");
                Console.WriteLine("Select 5. To perform Self-Host Https Call.");
                Console.WriteLine("Select 6. To perform OWIN Https Call.");
                Console.WriteLine("Select 7. To Search file in download directory.");
                Console.WriteLine("Select End. To Exit.\n");

                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        ExecuteOperation(() => GetFileMetaData().Wait());
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        ExecuteOperation(() => DownloadFile().Wait());
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        ExecuteOperation(() => UploadFile().Wait());
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        ExecuteOperation(() => CallIISHttpsService().Wait());
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        ExecuteOperation(() => CallSelfHostHttpsService().Wait());
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        ExecuteOperation(() => CallOWINHttpsService().Wait());
                        break;
                    case ConsoleKey.D7:
                    case ConsoleKey.NumPad7:
                        ExecuteOperation(() => SearchFile().Wait());
                        break;
                    default:
                        continue;
                }
            } while (keyInfo.Key != ConsoleKey.End);
        }

        /// <summary>
        /// Get File MetaData
        /// </summary>
        /// <returns>Awaitable Task object</returns>
        private static async Task GetFileMetaData()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please specify file name  with extension and Press Enter :- ");
            string fileName = Console.ReadLine();
            string actionURL = string.Concat("getfilemetadata?fileName=", fileName);

            try
            {
                Console.WriteLine(string.Format("Start calling GetFileMetaData operation @ {0}, {1} time ",
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString()));

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = baseStreamingURL;
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, actionURL);

                    await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).
                        ContinueWith((response)
                            =>
                            {
                                try
                                {
                                    ProcessGetFileMetaData(response);
                                }
                                catch (AggregateException aggregateException)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(string.Concat("Exception : ", aggregateException));
                                }
                            });
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Process get file meta data response object
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessGetFileMetaData(Task<HttpResponseMessage> response)
        {
            if (response.Result.IsSuccessStatusCode)
            {
                string getFileMetaDataMessage = string.Format("\nGetFileMatadata operation completed @ {0}, {1} time ",
                            DateTime.Now.ToLongDateString(),
                            DateTime.Now.ToLongTimeString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("{0} Message : \n{1}", getFileMetaDataMessage,
                    JsonConvert.SerializeObject(response.Result.Content.ReadAsAsync<object>().TryResult(), Formatting.Indented)));
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        /// <summary>
        /// Search file in downloa ddirectory
        /// </summary>
        /// <returns>Awaitable Task object</returns>
        private static async Task SearchFile()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please specify file name  with extension and Press Enter :- ");
            string fileName = Console.ReadLine();
            string actionURL = string.Concat("searchfileindownloaddirectory?fileName=", fileName);

            try
            {

                Console.WriteLine(string.Format("Start calling SearchFileInDownloadDirectory operation @ {0}, {1} time ",
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString()));

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = baseStreamingURL;
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, actionURL);

                    await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).
                        ContinueWith((response)
                            =>
                        {
                            try
                            {
                                ProcessSearchFileInDownloadDirectory(response);
                            }
                            catch (AggregateException aggregateException)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(string.Concat("Exception : ", aggregateException));
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Process searchfileindownloaddirectory response object
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessSearchFileInDownloadDirectory(Task<HttpResponseMessage> response)
        {
            if (response.Result.IsSuccessStatusCode)
            {
                string searchfileindownloaddirectoryMessage = string.Format("\nSearchFileInDownloadDirectory operation completed @ {0}, {1} time ",
                            DateTime.Now.ToLongDateString(),
                            DateTime.Now.ToLongTimeString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("{0} Message : \n{1}", searchfileindownloaddirectoryMessage,
                    JsonConvert.SerializeObject(response.Result.Content.ReadAsAsync<object>().TryResult(), Formatting.Indented)));
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <returns>Awaitable Task object</returns>
        private static async Task DownloadFile()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Please specify file name  with extension and Press Enter :- ");
            string fileName = Console.ReadLine();
            string localDownloadPath = Path.Combine(@"c:\", fileName); // the path can be configurable
            bool overWrite = true;
            string actionURL = string.Concat("downloadasync?fileName=", fileName);

            try
            {
                Console.WriteLine(string.Format("Start downloading @ {0}, {1} time ",
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString()));


                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = baseStreamingURL;
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, actionURL);

                    await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).
                        ContinueWith((response)
                            =>
                        {
                            Console.WriteLine();
                            try
                            {
                                ProcessDownloadResponse(localDownloadPath, overWrite, response);
                            }
                            catch (AggregateException aggregateException)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(string.Format("Exception : ", aggregateException));
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Process download response object
        /// </summary>
        /// <param name="localDownloadFilePath">Local download file path</param>
        /// <param name="overWrite">An indicator to overwrite a file if it exist in the client.</param>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessDownloadResponse(string localDownloadFilePath, bool overWrite, Task<HttpResponseMessage> response)
        {
            if (response.Result.IsSuccessStatusCode)
            {
                Console.WriteLine(string.Format("\nDownload path {0}\n", localDownloadFilePath));
                response.Result.Content.DownloadFile(localDownloadFilePath, overWrite).
                    ContinueWith((downloadmessage)
                        =>
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(downloadmessage.TryResult());
                    });
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <returns>Awaitable task object</returns>
        private static async Task UploadFile()
        {
            try
            {
                string uploadRequestURI = "uploadasync?overWrite=true";

                MultipartFormDataContent formDataContent = new MultipartFormDataContent();

                // Validate the file and add to MultipartFormDataContent object
                formDataContent.AddUploadFile(@"c:\nophoto.png");
                formDataContent.AddUploadFile(@"c:\ReadMe.txt");
                // Add more file
                if (!formDataContent.HasContent()) // No files found to be uploaded
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(formDataContent.GetUploadFileErrorMesage());
                    return;
                }
                else
                {
                    string uploadErrorMessage = formDataContent.GetUploadFileErrorMesage();
                    if (!string.IsNullOrWhiteSpace(uploadErrorMessage)) // Some files couldn't be found
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(uploadErrorMessage);
                    }

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uploadRequestURI);
                    request.Content = formDataContent;

                    using (HttpClient httpClient = new HttpClient())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("Start uploading @ {0}, {1} time ",
                        DateTime.Now.ToLongDateString(),
                        DateTime.Now.ToLongTimeString()));

                        httpClient.BaseAddress = baseStreamingURL;
                        await httpClient.SendAsync(request).
                              ContinueWith((response)
                                  =>
                                  {
                                      try
                                      {
                                          ProcessUploadResponse(response);
                                      }
                                      catch (AggregateException aggregateException)
                                      {
                                          Console.ForegroundColor = ConsoleColor.Red;
                                          Console.WriteLine(string.Format("Exception : ", aggregateException));
                                      }
                                  });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Process download response object
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessUploadResponse(Task<HttpResponseMessage> response)
        {
            if (response.Result.IsSuccessStatusCode)
            {
                string uploadMessage = string.Format("\nUpload completed @ {0}, {1} time ",
                            DateTime.Now.ToLongDateString(),
                            DateTime.Now.ToLongTimeString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Format("{0}\nUpload Message : \n{1}", uploadMessage,
                    JsonConvert.SerializeObject(response.Result.Content.ReadAsAsync<List<FileResponseMessage>>().TryResult(), Formatting.Indented)));
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        /// <summary>
        /// Call IIS Https Service
        /// </summary>
        /// <returns>Awaitable task object</returns>
        private async static Task CallIISHttpsService()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start processing IIS Https service operation call");

            string actionURI = "physician/activephysicians";

            try
            {
                string uri = string.Concat(secureURL.AbsoluteUri, actionURI);

                Uri address = new Uri(uri);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, address); // HttpMethod.Post is also works

                var cert = new X509Certificate2(File.ReadAllBytes(@"c:\MyCert.cer"));
                //var cert2 = new X509Certificate2(File.ReadAllBytes(@"c:\MyCert2.cer"));

                List<string> certs = new List<string>();
                certs.Add(cert.Thumbprint);
                //certs.Add(cert2.Thumbprint);
                request.Headers.Add("Thumbprint", certs);

                CancellationToken token = new CancellationToken();

                using (HttpClient httpClient = new HttpClient())
                {
                    await httpClient.SendAsync(request, token).
                        ContinueWith((response)
                            =>
                            {
                                try
                                {
                                    ProcessIISHttpsResponse(response);
                                }
                                catch (AggregateException aggregateException)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(string.Format("Exception {0}", aggregateException));
                                }
                            });
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Process IIS Https response object
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessIISHttpsResponse(Task<HttpResponseMessage> response)
        {
            if (response.Result.StatusCode == HttpStatusCode.OK ||
                response.Result.StatusCode == HttpStatusCode.Created)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Concat("Response message: \n",
                    JsonConvert.SerializeObject(response.Result.Content.ReadAsAsync<List<PhysicianBase>>().TryResult(), Formatting.Indented)));
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        /// <summary>
        /// Call SelfHost Https Service
        /// </summary>
        /// <returns>Awaitable task object</returns>
        private async static Task CallSelfHostHttpsService()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start processing Self-Host Https service operation call");

            try
            {
                // Set up SelfHost configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(serverDefaultSecureURL);

                config.MapHttpAttributeRoutes();

                config.Routes.MapHttpRoute(
                    name: "SelfHostActionApi",
                    routeTemplate: "{controller}/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

                // Create server
                using (HttpSelfHostServer server = new HttpSelfHostServer(config))
                {
                    // Open sever
                    server.OpenAsync().Wait();
                    Console.WriteLine(string.Concat("Opening server ", serverDefaultSecureURL));
                    Console.WriteLine();

                    // In order to communicate with server,need to validate remote server certificate 
                    ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                    Uri address = new Uri(serverDefaultSecureURL, "physicianselfhost/calculatesalaryraise");
                    await ProcessRaiseSalaryRequest(address);

                    server.CloseAsync().Wait();
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Call OWIN Https Service
        /// </summary>
        /// <returns>Awaitable task object</returns>
        private async static Task CallOWINHttpsService()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Start processing OWIN Https service operation call");

            try
            {
                // Create server
                using (WebApp.Start<ServerStartUp>(serverDefaultSecureURL.AbsoluteUri))
                {
                    Console.WriteLine(string.Concat("Opening server ", serverDefaultSecureURL));

                    //In order to communicate with server,need to validate remote server certificate 
                    ServicePointManager.ServerCertificateValidationCallback +=
                        new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                    Uri address = new Uri(serverDefaultSecureURL, "physicianselfhost/calculatesalaryraise");
                    await ProcessRaiseSalaryRequest(address);
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
        }

        /// <summary>
        /// Process Raise Salary request object
        /// <param name="address">Request Uri value</param>
        /// </summary>
        /// <param name="response">Awaitable task object</param>
        private static async Task ProcessRaiseSalaryRequest(Uri address)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.Method = HttpMethod.Post; // HttpMethod.Put is also works
                request.RequestUri = address;

                PhysicianBase physician = new InternalPhysician()
                {
                    FirstName = "Joe",
                    LastName = "Doe",
                    Salary = 120000
                };

                string requestobject = JsonConvert.SerializeObject(physician, Formatting.Indented);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Before Salary raised \n{0}", requestobject);

                request.Content = new ObjectContent(typeof(PhysicianBase), physician, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("text/json"));

                await httpClient.SendAsync(request).
                    ContinueWith((response)
                    =>
                    {
                        try
                        {
                            ProcessRaiseSalaryResponse(response);
                        }
                        catch (AggregateException aggregateException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(string.Format("Exception {0}", aggregateException));
                        }
                    });
            }
        }

        /// <summary>
        /// Process Raise Salary response object
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessRaiseSalaryResponse(Task<HttpResponseMessage> response)
        {
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Concat("Response message : \nAfter Salary raised \n",
                    JsonConvert.SerializeObject(response.Result.Content.ReadAsAsync<object>().TryResult(), Formatting.Indented)));
            }
            else
            {
                ProcessFailResponse(response);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Execute operation action callback method
        /// </summary>
        /// <param name="callBack">Action callback value</param>
        /// <param name="sleepTimeInSeconds">Operations sleep time value</param>
        static void ExecuteOperation(Action callBack, int sleepTimeInSeconds = 5)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAbout to perform operation!\n");
            callBack.Invoke();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Operation completed!");
            Thread.Sleep(sleepTimeInSeconds * 1000);
            Console.Clear();
        }

        /// <summary>
        /// Process fail response 
        /// </summary>
        /// <param name="response">Awaitable HttpResponseMessage task value</param>
        private static void ProcessFailResponse(Task<HttpResponseMessage> response)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Concat("Unsuccessful Response message: \n", string.Format("HttpStatus {0}, ReasonPhrase {1} \nDescription {2}",
                response.Result.StatusCode,
                response.Result.ReasonPhrase,
                response.Result.Content.ReadAsStringAsync().TryResult())));
        }

        /// <summary>
        /// Validate remote certificate delegate
        /// </summary>
        /// <param name="sender">Sender value</param>
        /// <param name="certificate">Certificate value</param>
        /// <param name="chain">Chain value</param>
        /// <param name="policyErrors">policyErrors value</param>
        /// <returns>True/False</returns>
        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = false;
            string message = string.Format("SUBJECT: {0} \nISSUER: {1} \nEXPIRES: {2} \nCERTHASH: {3} \n",
                certificate.Subject,
                certificate.Issuer,
                certificate.GetExpirationDateString(), certificate.GetCertHashString());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);

            // Process request if server name contains "localhost" or machine name
            if (!string.IsNullOrWhiteSpace(certificate.Subject) && (
                certificate.Subject.ToLower().Contains("localhost") ||
                certificate.Subject.Contains(serverFullName)))
                result = true;
            return result;
        }

        #endregion
    }
}

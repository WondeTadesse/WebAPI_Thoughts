//|---------------------------------------------------------------|
//|                WEB API SECURE SOCKET LAYERING                 |
//|---------------------------------------------------------------|
//|                       Developed by Wonde Tadesse              |
//|                             Copyright ©2014 - Present         |
//|---------------------------------------------------------------|
//|                WEB API SECURE SOCKET LAYERING                 |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using POCOLibrary;

namespace WebAPISecureSocketLayering.Common
{
    /// <summary>
    /// Certificate validator handler class 
    /// </summary>
    class CertificateValidationHandler : DelegatingHandler
    {
        /// <summary>
        /// Send request asynchronously
        /// </summary>
        /// <param name="request">HttpRequestMessage value</param>
        /// <param name="cancellationToken">CancellationToken value</param>
        /// <returns>HttpResponseMessage object</returns>
        protected override System.Threading.Tasks.Task<HttpResponseMessage>
                SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = ValidateCertificate(request);
            if (response.StatusCode == HttpStatusCode.OK)
                return base.SendAsync(request, cancellationToken);
            else
                return Task<HttpResponseMessage>.Factory.StartNew(() => response);
        }

        /// <summary>
        /// Validate Certificate
        /// </summary>
        /// <param name="request">HttpRequestMessage value</param>
        /// <returns>HttpResponseMessage object</returns>
        private HttpResponseMessage ValidateCertificate(HttpRequestMessage request)
        {
            IEnumerable<string> thumbPrints;

            if (!request.Headers.TryGetValues("Thumbprint", out thumbPrints) ||
                thumbPrints == null ||
                (thumbPrints != null && thumbPrints.Count() == 0))
            {
                return request.CreateResponse(HttpStatusCode.NotAcceptable, new Message() { Content = "Thumbprint request header is not available !" });
            }
            try
            {
                List<StoreLocation> locations = new List<StoreLocation> // Certificate location indicators
                {    
                    StoreLocation.CurrentUser, 
                    StoreLocation.LocalMachine
                };

                bool? verified = null; // A flag used to check Certificate validation
                List<string> thumbPrintCollection = new List<string>();

                if (thumbPrints.FirstOrDefault().Contains(',')) // Has many thumbprints
                {
                    thumbPrintCollection.AddRange(thumbPrints.FirstOrDefault().Split(','));
                }
                else
                {
                    thumbPrintCollection.Add(thumbPrints.FirstOrDefault());
                }

                OpenFlags openFlags = OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly;

                foreach (var thumbPrint in thumbPrintCollection)
                {
                    foreach (var location in locations)
                    {
                        X509Store store = new X509Store(StoreName.Root, location); // Look the certificates under Trusted Root Certification Authorities(CA)
                        try
                        {
                            store.Open(openFlags);

                            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, validOnly: true);// Make sure it's valid only

                            if (certificates != null && certificates.Count > 0)
                            {
                                // Check if the CN(Host + Domain Name) contains the request host  
                                foreach (var certificate in certificates)
                                {
                                    if (!string.IsNullOrWhiteSpace(certificate.Subject) &&
                                        !string.IsNullOrWhiteSpace(request.RequestUri.Host) &&
                                        certificate.Subject.ToLower().Contains(request.RequestUri.Host.ToLower()))

                                        return request.CreateResponse(HttpStatusCode.OK);
                                }
                            }
                            else
                            {
                                certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, validOnly: false);
                                if (certificates != null && certificates.Count > 0)
                                {
                                    verified = false;
                                }
                            }
                        }
                        finally
                        {
                            store.Close();
                        }
                    }
                }
                if (verified.HasValue && !verified.Value)
                {
                    return request.CreateResponse(HttpStatusCode.Unauthorized, new Message() { Content = "Certificate is available but not valid !" });
                }
                else
                {
                    return request.CreateResponse(HttpStatusCode.NotFound, new Message() { Content = "Certificate is not available !" });
                }
            }
            catch (Exception exception)
            {
                // Log error
                return request.CreateResponse(HttpStatusCode.BadRequest, new Message() { Content = string.Concat("Exception happens while processing certificate !\n", exception) });
            }
        }
    }
}
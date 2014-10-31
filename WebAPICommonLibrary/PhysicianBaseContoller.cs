//|---------------------------------------------------------------|
//|                   WEB API COMMON LIBRARY                      |
//|---------------------------------------------------------------|
//|                    Developed by Wonde Tadesse                 |
//|                               Copyright ©2014                 |
//|---------------------------------------------------------------|
//|                   WEB API COMMON LIBRARY                      |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

﻿using POCOLibrary;

namespace WebAPICommonLibrary
{
    public class PhysicianBaseController : ApiController
    {
        [NonAction]
        public virtual Physicians GetPhysicians()
        {
            try
            {
                return Physicians.Instance();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [NonAction]
        public virtual PhysicianBase GetPhysician(int id)
        {
            try
            {
                return Physicians.Instance().Where(p => p.ID == id).FirstOrDefault();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        [HttpGet]
        public virtual HttpResponseMessage ActivePhysicians()
        {
            try
            {
                var result = Physicians.Instance().Where(p => p.IsActive == true).ToList();
                return result != null ? Request.CreateResponse(HttpStatusCode.OK, result) : Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                     new Message() { Content = exception.Message });
            }
        }

        [HttpPost]
        public virtual HttpResponseMessage RemovePhysician(int id)
        {
            try
            {
                PhysicianBase physician = Physicians.Instance().Where(p => p.ID == id).FirstOrDefault();
                bool isRemoved = Physicians.Instance().Remove(physician);
                return isRemoved ? Request.CreateResponse(HttpStatusCode.OK, new Message() { Content = string.Format("Physician with an ID {0} is removed!", id) }) :
                    Request.CreateResponse(HttpStatusCode.BadRequest, new Message() { Content = string.Format("Unable to remove Physician using {0} ID", id) });
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new Message() { Content = exception.Message });
            }
        }

        [HttpPost]
        public virtual HttpResponseMessage AddPhysician(PhysicianBase physician)
        {
            try
            {
                Physicians.Instance().Add(physician);
                return Request.CreateResponse(HttpStatusCode.Created, new Message() { Content = string.Format("New Physician is added successfully")});
            }
            catch (Exception exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                     new Message() { Content = exception.Message });
            }
        }
    }
}

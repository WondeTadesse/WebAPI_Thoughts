﻿//|---------------------------------------------------------------|
//|                        POCO LIBRARY                           |
//|---------------------------------------------------------------|
//|                        Developed by Wonde Tadesse             |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                        POCO LIBRARY                           |
//|---------------------------------------------------------------|
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POCOLibrary
{
    public class PhysicianBase
    {
        #region Public Properties 

        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; }

        public double Salary { get; set; }

        #endregion
        
    }
}
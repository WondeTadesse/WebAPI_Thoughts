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
using System.Text;
using System.Threading.Tasks;

namespace POCOLibrary
{
    public class InternalPhysician : InternalPhysicianBase
    {
        #region Public Methods 

        public override double CalculateSalaryRaise() // Fixed salary raise
        {
            return base.Salary + base.Salary * 0.015;
        }
        
        #endregion
    }
}

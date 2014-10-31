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
    public class ExternalPhysician : ExternalPhysicianBase, ISpeciality
    {
        #region Public Properties

        public string Speciality { get; set; }

        #endregion

        #region Public Methods

        public override double CalculateBonus() // Fixed bonus
        {
            return base.Salary * 0.02;
        }

        #endregion
    }
}

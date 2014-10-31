//|---------------------------------------------------------------|
//|                        POCO LIBRARY                           |
//|---------------------------------------------------------------|
//|                        Developed by Wonde Tadesse             |
//|                                  Copyright ©2014              |
//|---------------------------------------------------------------|
//|                        POCO LIBRARY                           |
//|---------------------------------------------------------------|

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace POCOLibrary
{
    public class Physicians : List<PhysicianBase>
    {
        #region Private Members  

        private static Physicians _instance = new Physicians();
        
        #endregion

        #region Constructor 

        private Physicians()
        {
            // Default Collections
            this.Add(new InternalPhysician() { ID = 1, LastName = "T", FirstName = "Wonde", IsActive = true });
            this.Add(new ExternalPhysician() { ID = 2, LastName = "M", FirstName = "Mati", Speciality = "Plastic Surgent", IsActive = true });
            this.Add(new InternalPhysician() { ID = 3, LastName = "Doe", FirstName = "Jane", IsActive = false });
            this.Add(new ExternalPhysician() { ID = 4, LastName = "Doe", FirstName = "Joe", Speciality = "Cardiologist", IsActive = false });
        }

        #endregion

        #region Public Methods 
        
        public static Physicians Instance()
        {
            if (_instance == null)
            {
                lock (_instance)
                {
                    _instance = new Physicians();
                }
            }
            return _instance;
        }

        #endregion
    }
}
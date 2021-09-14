using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LAB2_ED2_DiegoRamirez_DanielElias.Models;

namespace LAB2_ED2_DiegoRamirez_DanielElias.Data
{   
    public sealed class Singleton
    {
        private readonly static Singleton _instance = new Singleton();
        public List<CompModel> CompList; 
        private Singleton()
        {
            CompList = new List<CompModel>();
        }

        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

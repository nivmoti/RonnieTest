using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonnieTest.APIHandler
{
    public class APIHandler
    {
        public Dictionary<string,int> APIS { get; set; } = new Dictionary<string,int>();
        public int length { get; set; } = 0;
        public APIHandler() { }

        public void AddAPI(string API)
        {
            APIS.Add(API, ++length);
        }
        public void RemoveAPI(string API)
        {
            if (APIS.ContainsKey(API)) { 
                APIS.Remove(API);
                --length;
            }
            else { return; }  
            
        }
    }
}

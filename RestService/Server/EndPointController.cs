using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Restservice.Http_Service;

namespace Restservice.Server
{
    public class EndPointApi<TCallFunctionBy,TReturnValue>
    {
        public Dictionary<string, Dictionary<string, Func<TCallFunctionBy, TReturnValue>>> Endpoints { get; protected set; }

        public EndPointApi()
        {
            Endpoints = new Dictionary<string, Dictionary<string, Func<TCallFunctionBy, TReturnValue>>>();
        }
        public void RegisterEndPoint(string verb, string endpoint, Func<TCallFunctionBy, TReturnValue> callbackfunction)
        {
            if (!Endpoints.ContainsKey(endpoint))
            {
                Endpoints[endpoint] = new Dictionary<string, Func<TCallFunctionBy, TReturnValue>>();
            }
            Endpoints[endpoint][verb] = callbackfunction;
        }
        public int InvokeEndPoint(string verb,string EndPointInRegexForm,TCallFunctionBy CallfunctionByThisType)
        {
            string location = null;
            foreach(var item in Endpoints)
            {
                if (Regex.Match(EndPointInRegexForm,"^" +item.Key+"$").Success)
                {
                    location = item.Key;
                    break;
                }
            }
            if (location == null)
            {
                return -2;
            }
            if (Endpoints[location].ContainsKey(verb))
            {
                return ToInt(Endpoints[location][verb].Invoke(CallfunctionByThisType));
            }
            return -3;
        }
        private int ToInt(TReturnValue x)
        {
            try
            {
                return Convert.ToInt32(x);
            }
            catch
            {
                return -1;
            }
        }

    }
}

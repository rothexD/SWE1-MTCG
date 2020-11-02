using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Restservice.Http_Service;

namespace Restservice.Server
{
    public class ResponseContext
    {
        public string HttpStatusCode;
        public string payload;

        public ResponseContext(string status, string payload)
        {
            this.HttpStatusCode = status;
            this.payload = payload;
        }
    }
    class EndPointController<TCallFunctionBy,TReturnValue>
    {
        public Dictionary<string,Dictionary<string,Func<TCallFunctionBy, TReturnValue>>> _Endpoints;

        EndPointController()
        {
            _Endpoints = new Dictionary<string, Dictionary<string, Func<TCallFunctionBy, TReturnValue>>>();
        }
        public void RegisterEndPoint(string verb, string endpoint, Func<TCallFunctionBy, TReturnValue> callbackfunction)
        {
            if (!_Endpoints.ContainsKey(endpoint))
            {
                _Endpoints[endpoint] = new Dictionary<string, Func<TCallFunctionBy, TReturnValue>>();
            }
            _Endpoints[endpoint][verb] = callbackfunction;
        }
        public bool InvokeEndPoint(string verb,string EndPointInRegexForm,TCallFunctionBy CallfunctionByThisType)
        {
            string location = null;
            foreach(var item in _Endpoints)
            {
                if (Regex.Match(item.Key, EndPointInRegexForm).Success)
                {
                    location = item.Key;
                    break;
                }
            }
            if (location == null)
            {
                return false;
            }
            if (_Endpoints[location].ContainsKey(verb))
            {
                _Endpoints[location][verb].Invoke(CallfunctionByThisType);
                return true;
            }
            return false;
        }

    }
}

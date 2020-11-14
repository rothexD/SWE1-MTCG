using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Restservice.Http_Service;

namespace Restservice.Server
{
    public class EndPointApi<TCallFunctionBy,TReturnValue> where TCallFunctionBy : notnull where TReturnValue : notnull
    {
        protected Dictionary<string, Dictionary<string, Func<TCallFunctionBy, TReturnValue>>> EndPoints { get; set; }

        public EndPointApi()
        {
            EndPoints = new Dictionary<string, Dictionary<string, Func<TCallFunctionBy, TReturnValue>>>();
        }
        public void RegisterEndPoint(string verb, string endPointsInRegexForm, Func<TCallFunctionBy, TReturnValue> callbackFunction)
        {
            if (!EndPoints.ContainsKey(endPointsInRegexForm))
            {
                EndPoints[endPointsInRegexForm] = new Dictionary<string, Func<TCallFunctionBy, TReturnValue>>();
            }
            EndPoints[endPointsInRegexForm][verb] = callbackFunction;
        }
        public TReturnValue InvokeEndPoint(string verb,string endPointInRegexForm,TCallFunctionBy callFunctionByThisType)
        {
            string location = null;
            foreach(var item in EndPoints)
            {
                if (Regex.Match(endPointInRegexForm,item.Key).Success)
                {
                    location = item.Key;
                    break;
                }
            }
            if (location == null)
            {
                throw new Exception("NotAValidEndpoint");
            }
            if (EndPoints[location].ContainsKey(verb))
            {
                return EndPoints[location][verb].Invoke(callFunctionByThisType);
            }
            throw new Exception("NotAValidVerbForEndpoint");
        }
    }
}

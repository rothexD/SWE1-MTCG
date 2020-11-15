using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Restservice.Http_Service;

namespace Restservice.Server
{
    public class EndPointApi<TCallFunctionWithThisParameter,TReturnValue> where TCallFunctionWithThisParameter : notnull where TReturnValue : notnull
    {
        //generic class that can register functions and returnvalues
        protected Dictionary<string, Dictionary<string, Func<TCallFunctionWithThisParameter, TReturnValue>>> EndPoints { get; set; }

        public EndPointApi()
        {
            EndPoints = new Dictionary<string, Dictionary<string, Func<TCallFunctionWithThisParameter, TReturnValue>>>();
        }
        public void RegisterEndPoint(string verb, string endPointsInRegexForm, Func<TCallFunctionWithThisParameter, TReturnValue> callbackFunction)
        {
            //checks if respource endpoint exists
            if (!EndPoints.ContainsKey(endPointsInRegexForm))
            {
                // if does not exists register it initialize second dictioanry
                EndPoints[endPointsInRegexForm] = new Dictionary<string, Func<TCallFunctionWithThisParameter, TReturnValue>>();
            }
            //register verb for that specific endpoint and possibly overwrite existing one
            EndPoints[endPointsInRegexForm][verb] = callbackFunction;
        }

        public TReturnValue InvokeEndPoint(string verb,string endPointInRegexForm, TCallFunctionWithThisParameter callFunctionWithThisParameter)
        {
            string location = null;

            //checks if endpoint exists
            foreach(var item in EndPoints)
            {
                //if endPointInRegexForm matches Regex of a resource endpoint get the location
                if (Regex.Match(endPointInRegexForm,item.Key).Success)
                {
                    location = item.Key;
                    break;
                }
            }
            if (location == null)
            {
                //if resource does not exist in dictionary throw exception
                throw new Exception("NotAValidEndpoint");
            }
            if (EndPoints[location].ContainsKey(verb))
            {
                //invokes endpoint and exeuctes lambda function behind it
                return EndPoints[location][verb].Invoke(callFunctionWithThisParameter);
            }
            //if verb does not exists for resource throw exception
            throw new Exception("NotAValidVerbForEndpoint");
        }
    }
}

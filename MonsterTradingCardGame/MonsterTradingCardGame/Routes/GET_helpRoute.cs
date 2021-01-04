using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCTG.Routes
{
    public static class GET_helpRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/help$", (IRequestContext httpRequest) =>
            {
                try
                {
                    httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", File.ReadAllText("helpFileJson.txt"));
                    return 200;
                }
                catch
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "500");
                    return 500;
                }
            });
        }
    }
}

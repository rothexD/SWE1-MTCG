using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;

namespace MCTG.Routes
{
    public static class POST_SessionRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/sessions$", (IRequestContext httpRequest) =>
            {
                try
                {
                    JsonSessions JsonData = JsonConvert.DeserializeObject<JsonSessions>(httpRequest.PayLoad);
                    string querystring = @$"select count(*) as count from users where LoginName='{JsonData.Username}' and Passwort='{JsonData.Password}'";
                    NpgsqlConnection conn = DbHelper.ConnectObj();
                    conn.Open();
                    bool trylogin = false;

                    using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            int count = Int32.Parse(reader["count"].ToString());
                            if (count == 1)
                            {
                                trylogin = true;
                            }
                        }
                        if (trylogin)
                        {
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", $"{{\"Authorization\": \"Basic {JsonData.Username}-mtcgToken\"}}");
                            conn.Close();
                            return 200;
                        }
                        else
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                            conn.Close();
                            return 404;
                        }
                    }
                }
                catch
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
            });
        }
    }
}

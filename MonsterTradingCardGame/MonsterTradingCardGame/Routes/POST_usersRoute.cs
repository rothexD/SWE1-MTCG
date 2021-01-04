using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;

namespace MCTG.Routes
{
    public static class POST_usersRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/users$", (IRequestContext httpRequest) =>
            {
                try
                {
                    JsonRegisterUser Data = JsonConvert.DeserializeObject<JsonRegisterUser>(httpRequest.PayLoad);

                    NpgsqlConnection conn = DbHelper.ConnectObj();
                    conn.Open();
                    string querystring = $"Insert into users (LoginName,Passwort) values('{Data.Username}','{Data.Password}') ";
                    using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                    {
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "500");
                            conn.Close();
                            return 500;
                        }

                        querystring = $"Insert into Scoreboard (LoginName_fk) values('{Data.Username}')";
                        using (NpgsqlCommand command2 = new NpgsqlCommand(querystring, conn))
                        {
                            command2.ExecuteNonQuery();
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "201");
                            conn.Close();
                            return 201;
                        }
                    }
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


using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class PUT_users_anyRoute
    {
        public static void registerRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("PUT", "^/users/.+$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
                string userToken = Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value;
                if (!Regex.IsMatch(httpRequest.MessageEndPoint, $"^/users/{userToken}$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "401");
                    return 401;
                }


                string querystring = @$"select LoginName from users where LoginName='{userToken}'";
                NpgsqlConnection conn = DbHelper.ConnectObj();
                conn.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                    reader.Read();
                    string UserID = reader[0].ToString();
                    reader.Close();

                    var UpdateData = JsonConvert.DeserializeObject<JsonProfileData>(httpRequest.PayLoad);
                    string query;
                    try
                    {
                        query = DbHelper.querybuilder(UpdateData, UserID);
                    }
                    catch
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }

                    using (NpgsqlCommand UpdateProfileData = new NpgsqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        UpdateProfileData.ExecuteNonQuery();
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        conn.Close();
                        return 200;
                    }
                }
            });
        }
    }
}
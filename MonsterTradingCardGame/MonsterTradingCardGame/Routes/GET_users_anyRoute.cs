using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class GET_users_anyRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/users/.+$", (IRequestContext httpRequest) =>
            {
                try
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
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "401");
                            conn.Close();
                            return 401;
                        }
                        reader.Read();
                        string UserID = reader[0].ToString();
                        reader.Close();

                        string query = $"Select loginname,username,bio,image,coins from users where LoginName='{UserID}'";
                        using (NpgsqlCommand GetUserdate = new NpgsqlCommand(query, conn))
                        {
                            NpgsqlDataReader readerGetUserdate = GetUserdate.ExecuteReader();
                            if (readerGetUserdate.HasRows == false)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                                conn.Close();
                                return 404;
                            }
                            readerGetUserdate.Read();
                            JsonProfileDataExtended data = new JsonProfileDataExtended();
                            data.LoginName = readerGetUserdate[0].ToString();
                            data.Name = readerGetUserdate[1].ToString();
                            data.Bio = readerGetUserdate[2].ToString();
                            data.Image = readerGetUserdate[3].ToString();
                            data.Coins = readerGetUserdate[4].ToString();

                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(data));
                            conn.Close();
                            return 200;
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

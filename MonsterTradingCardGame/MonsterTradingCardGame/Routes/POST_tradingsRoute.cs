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
    public static class POST_tradingsRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/tradings$", (IRequestContext httpRequest) =>
            {
                try
                {
                    httpRequest.Headers.TryGetValue("Authorization", out string token);
                    if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        return 400;
                    }
                    string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
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

                        JsonTradingOffer Data = JsonConvert.DeserializeObject<JsonTradingOffer>(httpRequest.PayLoad);

                        querystring = $"Select count(*) from UserhasCardsinStack where CardId_fk='{Data.CardToTrade}' and LoginName_fk='{UserID}' and CurrentlyTraded=false";
                        using (NpgsqlCommand verifyOwnedAndNotCurrentlyTraded = new NpgsqlCommand(querystring, conn))
                        {
                            NpgsqlDataReader readerverifyOwnedAndNotCurrentlyTraded = verifyOwnedAndNotCurrentlyTraded.ExecuteReader();
                            reader.Read();
                            if (Int32.Parse(reader[0].ToString()) != 1)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                return 400;
                            }
                            reader.Close();
                        }
                        querystring = $"Update UserhasCardsinStack set CurrentlyTraded=true where CardId_fk='{Data.CardToTrade}' and LoginName_fk='{UserID}' and CurrentlyTraded=false";
                        using (NpgsqlCommand UpdateCardTradeStatus = new NpgsqlCommand(querystring, conn))
                        {
                            UpdateCardTradeStatus.ExecuteNonQuery();
                        }
                        querystring = @$"Insert into Trading values ('{Data.Id}','{Data.CardToTrade}','{UserID}','{Data.Type}','{Data.MinimumDamage}', now())";
                        using (NpgsqlCommand ListOffer = new NpgsqlCommand(querystring, conn))
                        {
                            ListOffer.ExecuteNonQuery();
                        }
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "201");
                        conn.Close();
                        return 201;
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


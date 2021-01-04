using MCTG.DbHelpers;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class PUT_deckRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("PUT", "^/deck$", (IRequestContext httpRequest) =>
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

                        List<string> TryToAddToDeck = JsonConvert.DeserializeObject<List<string>>(httpRequest.PayLoad);
                        if (TryToAddToDeck.Count != 4)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }

                        //ownership test
                        try
                        {
                            foreach (var item in TryToAddToDeck)
                            {
                                string CountVerfiyOwned = $"select count(*) from UserhasCardsinStack where loginName_fk='{UserID}' and CardId_Fk='{item}' and CurrentlyTraded=false";
                                Console.WriteLine("trying: " + CountVerfiyOwned);
                                using (NpgsqlCommand VerifyOwned = new NpgsqlCommand(CountVerfiyOwned, conn))
                                {
                                    NpgsqlDataReader readerVerifyOwned = VerifyOwned.ExecuteReader();
                                    if (readerVerifyOwned.HasRows == false)
                                    {
                                        throw new ArgumentException("internal error");
                                    }
                                    readerVerifyOwned.Read();
                                    if (Int32.Parse(readerVerifyOwned[0].ToString()) != 1)
                                    {
                                        throw new ArgumentException("not owned by this person: " + item);
                                    }
                                    readerVerifyOwned.Close();
                                }
                            }
                        }
                        catch
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }

                        using (NpgsqlCommand DeleteOldDeck = new NpgsqlCommand($"delete from UserhasCardsinDeck where CardId_fk='{UserID}'", conn))
                        {
                            DeleteOldDeck.ExecuteNonQuery();
                        }

                        foreach (var item in TryToAddToDeck)
                        {
                            string CountVerfiyOwned = $"Insert into UserhasCardsinDeck values('{item}','{UserID}')";
                            using (NpgsqlCommand InsertNewDeck = new NpgsqlCommand(CountVerfiyOwned, conn))
                            {
                                InsertNewDeck.ExecuteNonQuery();
                            }
                        }
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        conn.Close();
                        return 200;
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

using MCTG.DbHelpers;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class GET_deckRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/deck$", (IRequestContext httpRequest) =>
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

                        string querystringUnionAllCards = @$"select CardId,CardName,BaseDamage from Card join UserhasCardsinDeck on Card.CardID = UserhasCardsinDeck.CardId_fk where LoginName_fk = '{UserID}'";
                        using (NpgsqlCommand getdeck = new NpgsqlCommand(querystringUnionAllCards, conn))
                        {
                            NpgsqlDataReader readerGetDeck = getdeck.ExecuteReader();
                            List<(string, string)> cardList = new List<(string, string)>();
                            while (readerGetDeck.Read())
                            {
                                (string, string) t = (readerGetDeck[0].ToString(), readerGetDeck[1].ToString());
                                cardList.Add(t);
                            }
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(cardList, Formatting.Indented));
                            conn.Close();
                            return 200;
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

﻿using MCTG.DbHelpers;
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
        public static void registerRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/deck$", (IRequestContext httpRequest) =>
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
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                    reader.Read();
                    string UserID = reader[0].ToString();
                    reader.Close();

                    string querystringUnionAllCards = @$"select CardId,CardName,BaseDamage from Card join UserhasCardsinDeck on Card.CardID = UserhasCardsinDeck.CardId_fk where LoginName_fk = '{UserID}'";
                    using (NpgsqlCommand getdeck = new NpgsqlCommand(querystringUnionAllCards, conn))
                    {
                        NpgsqlDataReader readergetdeck = getdeck.ExecuteReader();
                        List<(string, string)> CardList = new List<(string, string)>();
                        while (readergetdeck.Read())
                        {
                            (string, string) t = (readergetdeck[0].ToString(), readergetdeck[1].ToString());
                            CardList.Add(t);
                        }
                        httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(CardList, Formatting.Indented));
                        conn.Close();
                        return 200;
                    }
                }
            });
        }
    }
}

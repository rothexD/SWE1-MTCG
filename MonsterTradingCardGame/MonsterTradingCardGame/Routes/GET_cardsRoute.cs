using MCTG.DbHelpers;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class GET_cardsRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/cards$", (IRequestContext httpRequest) =>
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
                        string userId = reader[0].ToString();
                        reader.Close();
                        string querystringUnionAllCards = @$"select 'deck' as location,cardId_fk from UserhasCardsinDeck where LoginName_fk='{userId}' union select 'stack' as location,cardId_fk from UserhasCardsinStack where LoginName_fk='{userId}' and (select count(*) from UserhasCardsinDeck where UserhasCardsinDeck.cardID_fk = UserhasCardsinStack.cardID_fk) = 0";
                        using (NpgsqlCommand commandUnionSelect = new NpgsqlCommand(querystringUnionAllCards, conn))
                        {
                            NpgsqlDataReader readercommandUnionSelect = commandUnionSelect.ExecuteReader();
                            List<(string, string)> CardList = new List<(string, string)>();
                            while (readercommandUnionSelect.Read())
                            {
                                (string, string) t = (readercommandUnionSelect[0].ToString(), readercommandUnionSelect[1].ToString());
                                CardList.Add(t);
                            }
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(CardList, Formatting.Indented));
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

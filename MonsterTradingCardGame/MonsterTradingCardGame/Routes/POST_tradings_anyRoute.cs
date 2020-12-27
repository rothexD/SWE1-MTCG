using MCTG.DbHelpers;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class POST_tradings_anyRoute
    {
        public static void registerRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/tradings/.+$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "405");
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
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "406");
                        conn.Close();
                        return 400;
                    }
                    reader.Read();
                    string UserID = reader[0].ToString();
                    reader.Close();

                    string CardIdEndPoint = Regex.Match(httpRequest.MessageEndPoint, "^/tradings/(.*)$").Groups[1].Value;
                    querystring = $"select TradeId,CardToTradeID_fk,OriginalOwner_fk,typeToTrade,minDamage from Trading  where TradeID='{CardIdEndPoint}'";
                    using (NpgsqlCommand GetTradeData = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader readerGetTradeData = GetTradeData.ExecuteReader();
                        if (readerGetTradeData.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "407");
                            conn.Close();
                            return 400;
                        }
                        readerGetTradeData.Read();
                        if (readerGetTradeData[2].ToString() == UserID)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "408");
                            conn.Close();
                            return 400;
                        }

                        string TradeId = readerGetTradeData[0].ToString();
                        string CardToTradeID_fk = readerGetTradeData[1].ToString();
                        string OriginalOwner_fk = readerGetTradeData[2].ToString();
                        string typeToTrade = readerGetTradeData[3].ToString();
                        string minDamage = readerGetTradeData[4].ToString();
                        readerGetTradeData.Close();




                        querystring = $"Select CardStyle,BaseDamage from Card join UserhasCardsinStack on Card.CardID = CardId_fk where CardID='{httpRequest.PayLoad.Trim('\"')}' and LoginName_fk ='{UserID}' and CurrentlyTraded  = false; ";
                        Console.WriteLine(querystring);
                        using (NpgsqlCommand SelectCardForTrade = new NpgsqlCommand(querystring, conn))
                        {
                            NpgsqlDataReader readerSelectCardForTrade = SelectCardForTrade.ExecuteReader();
                            if (readerSelectCardForTrade.HasRows == false)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "409");
                                conn.Close();
                                return 400;
                            }
                            readerSelectCardForTrade.Read();

                            if (readerSelectCardForTrade[0].ToString().ToUpper() != typeToTrade.ToUpper())
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "499");
                                conn.Close();
                                return 400;
                            }
                            if (Int32.Parse(readerSelectCardForTrade[1].ToString()) <= Int32.Parse(minDamage))
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "489");
                                conn.Close();
                                return 400;
                            }
                            readerSelectCardForTrade.Close();

                            querystring = $"Select count(*) from UserhasCardsinDeck where LoginName_fk='{UserID}' and cardid_fk = '{httpRequest.PayLoad.Trim('\"')}'; ";
                            using (NpgsqlCommand VerifynotInDeckCurrently = new NpgsqlCommand(querystring, conn))
                            {
                                NpgsqlDataReader readerVerifynotInDeckCurrently = VerifynotInDeckCurrently.ExecuteReader();
                                readerVerifynotInDeckCurrently.Read();
                                if (Int32.Parse(readerVerifynotInDeckCurrently[0].ToString()) > 0)
                                {
                                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "479");
                                    conn.Close();
                                    return 400;
                                }
                                readerVerifynotInDeckCurrently.Close();
                            }

                            querystring = @$"Update UserhasCardsinStack set LoginName_fk='{UserID}', CurrentlyTraded = false where CardId_fk='{CardToTradeID_fk}'";
                            using (NpgsqlCommand TradeCommand = new NpgsqlCommand(querystring, conn))
                            {
                                TradeCommand.ExecuteNonQuery();
                            }
                            querystring = @$"Update UserhasCardsinStack set LoginName_fk='{OriginalOwner_fk}', CurrentlyTraded = false where CardId_fk='{httpRequest.PayLoad.Trim('\"')}'";
                            using (NpgsqlCommand TradeCommand = new NpgsqlCommand(querystring, conn))
                            {
                                TradeCommand.ExecuteNonQuery();
                            }
                            querystring = @$"Delete from Trading where TradeID='{CardIdEndPoint}'";
                            using (NpgsqlCommand TradeCommand = new NpgsqlCommand(querystring, conn))
                            {
                                TradeCommand.ExecuteNonQuery();
                            }

                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                            conn.Close();
                            return 200;

                        }
                    }
                }
            });
        }
    }
}

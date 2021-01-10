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
    public static class GET_statsRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/stats$", (IRequestContext httpRequest) =>
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

                        string querystringUnionAllCards = @$"select win,tie,lose,elo from Scoreboard where LoginName_fk = '{UserID}'";
                        using (NpgsqlCommand getStats = new NpgsqlCommand(querystringUnionAllCards, conn))
                        {
                            NpgsqlDataReader readergetStats = getStats.ExecuteReader();
                            if (readergetStats.HasRows == false)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                                return 404;
                            }
                            readergetStats.Read();
                            var Score = new JsonScore();
                            Score.Win = readergetStats[0].ToString();                         
                            Score.Tie = readergetStats[1].ToString();
                            Score.Lose = readergetStats[2].ToString();
                            Score.Elo = readergetStats[3].ToString();
                            Score.LoginName = UserID;
                            if ((Int32.Parse(Score.Lose) == 0 && Int32.Parse(Score.Tie) == 0))
                            {
                                if (Int32.Parse(Score.Win) > 0)
                                {
                                    Score.WLTratio = 1;
                                }
                                else
                                {
                                    Score.WLTratio = 0;
                                }
                            }
                            else
                            {
                                Score.WLTratio = (Int32.Parse(Score.Win) / (Int32.Parse(Score.Lose) + Int32.Parse(Score.Tie)));
                            }



                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(Score, Formatting.Indented));
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
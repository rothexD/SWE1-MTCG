using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Collections.Generic;

namespace MCTG.Routes
{
    public static class GET_scoreRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/score$", (IRequestContext httpRequest) =>
            {
                try
                {

                    NpgsqlConnection conn = DbHelper.ConnectObj();
                    conn.Open();

                    string querystringUnionAllCards = @$"select win,tie,lose,elo,LoginName from Scoreboard join users on scoreboard.LoginName_fk = users.LoginName order by elo desc,win desc,tie desc,lose asc";
                    using (NpgsqlCommand getScorebaord = new NpgsqlCommand(querystringUnionAllCards, conn))
                    {
                        NpgsqlDataReader readergetScorebaord = getScorebaord.ExecuteReader();
                        List<JsonScore> Scoreboard = new List<JsonScore>();

                        while (readergetScorebaord.Read())
                        {
                            var Score = new JsonScore();
                            Score.Win = readergetScorebaord[0].ToString();                          
                            Score.Tie = readergetScorebaord[1].ToString();
                            Score.Lose = readergetScorebaord[2].ToString();
                            Score.Elo = readergetScorebaord[3].ToString();
                            Score.LoginName = readergetScorebaord[4].ToString();
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
                            Scoreboard.Add(Score);
                        }

                        httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(Scoreboard, Formatting.Indented));
                        conn.Close();
                        return 200;
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

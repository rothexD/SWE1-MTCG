using MCTG.Cards;
using MCTG.DbHelpers;
using MCTG.FightHandlers;
using MCTG.Players;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace MCTG.Routes
{
    public static class POST_battlesRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/battles$", (IRequestContext httpRequest) =>
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

                    string UserID;
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
                        UserID = reader[0].ToString();
                        reader.Close();
                    }
                    Player player = new Player();
                    querystring = @$"select loginname,Elo from users join Scoreboard on users.LoginName = Scoreboard.LoginName_fk where loginname='{UserID}'";
                    using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "500");
                            conn.Close();
                            return 500;
                        }
                        reader.Read();
                        player.Elo = reader[1].ToString();
                        player.UserName = reader[0].ToString();
                        reader.Close();
                    }
                    querystring = @$"select CardID,Cardtype,CardName,BaseDamage,CardElement,CardStyle from Card join UserhasCardsinDeck on Card.CardID = UserhasCardsinDeck.CardId_fk where LoginName_fk='{UserID}'";
                    List<Card> Deck = new List<Card>();
                    using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }
                        while (reader.Read())
                        {
                            Card.CardelEmentEnum Type;
                            if (reader[4].ToString() == "fire")
                            {
                                Type = Card.CardelEmentEnum.fire;
                            }
                            else if (reader[4].ToString() == "water")
                            {
                                Type = Card.CardelEmentEnum.water;
                            }
                            else
                            {
                                Type = Card.CardelEmentEnum.normal;
                            }
                            string whatcard;
                            if (reader[5].ToString() == "Spell")
                            {
                                whatcard = "Spell";
                            }
                            else
                            {
                                whatcard = reader[1].ToString();
                            }

                            Deck.Add(CardHelpers.Cardmaker(Int32.Parse(reader[3].ToString()), reader[2].ToString(), reader[0].ToString(), Type, whatcard));
                        }
                    }
                    if (Deck.Count != 4)
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                    player.Deck = Deck;
                    AutoResetEvent myEvent = new AutoResetEvent(false);
                    Programm.fightApi.QueUpForFight(player, myEvent);
                    myEvent.WaitOne();

                    switch (player.Status)
                    {
                        case FightHandler.BattleStatus.Win:
                            DbHelper.IncreaseWin(UserID);
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", player.AssignedBattleLog);
                            conn.Close();
                            return 200;
                        case FightHandler.BattleStatus.Lose:
                            DbHelper.IncreaseLose(UserID);
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", player.AssignedBattleLog);
                            conn.Close();
                            return 200;
                        case FightHandler.BattleStatus.Tie:
                            DbHelper.IncreaseTie(UserID);
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", player.AssignedBattleLog);
                            conn.Close();
                            return 200;
                        default:
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "500");
                            conn.Close();
                            return 500;
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


using MCTG.DbHelpers;
using MCTG.FightHandlers;
using MCTG.Routes;
using Restservice.Server;
using System;

namespace MCTG
{
    class Programm
    {
        public static readonly ServerTcpListener server = new ServerTcpListener("127.0.0.1", 10001);
        public static FightHandler fightApi = new FightHandler();
        private static double Difference(DateTime early,DateTime later)
        {
            return later.Subtract(early).TotalMilliseconds;
        }
        static void Main(string[] parameter)
        {
            DateTime early;
            DateTime later;

            Console.WriteLine("Registering Routes...");
            early = DateTime.Now;
            POST_SessionRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/sessions$", (IRequestContext httpRequest) =>
            {
                JsonSessions JsonData = JsonConvert.DeserializeObject<JsonSessions>(httpRequest.PayLoad);
                string querystring = @$"select count(*) as count from users where LoginName='{JsonData.Username}' and Passwort='{JsonData.Password}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();
                bool trylogin = false;

                using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int count = Int32.Parse(reader["count"].ToString());
                        if (count == 1)
                        {
                            trylogin = true;
                        }
                    }
                    if (trylogin)
                    {
                        httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", $"{{\"Authorization\": \"Basic {JsonData.Username}-mtcgToken\"}}");
                        conn.Close();
                        return 200;
                    }
                    else
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                }
            });*/
            POST_packagesRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/packages$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "Basic admin-mtcgToken"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "405");
                    return 405;
                }

                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();

                using (NpgsqlCommand PackageInsert = new NpgsqlCommand(@$"Insert into Packages(StringCards) values('{httpRequest.PayLoad}');", conn))
                {
                    try
                    {
                        PackageInsert.ExecuteNonQuery();
                    }
                    catch
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        return 400;
                    }

                }

                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "201");
                return 201;
            });*/
            POST_packages_transactionsRoutecs.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/transactions/packages$", (IRequestContext httpRequest) =>
            {
                string userID;
                string Coins;

                Console.WriteLine("crash0");
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
                Console.WriteLine("crash1");
                string querystring = @$"select LoginName,Coins from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                Console.WriteLine(querystring);
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();
                Console.WriteLine("crash2");
                using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        Console.WriteLine("illegal FieldCount " + reader.FieldCount);
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                    Console.WriteLine("crash3");

                    reader.Read();
                    userID = reader[0].ToString();
                    Coins = reader[1].ToString();
                    reader.Close();

                    if (Int32.Parse(Coins) < 4)
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }
                    Console.WriteLine("crash4");
                    using (NpgsqlCommand command2 = new NpgsqlCommand("Select PackageId,StringCards from Packages order by PackageID asc limit 1", conn))
                    {
                        NpgsqlDataReader readerPackageString = command2.ExecuteReader();
                        if (readerPackageString.HasRows == false)
                        {
                            Console.WriteLine("no rows");
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }
                        Console.WriteLine("crash5");
                        readerPackageString.Read();
                        string PackageID = readerPackageString[0].ToString();
                        string stringCards = readerPackageString[1].ToString();
                        readerPackageString.Close();
                        using (var commandDelete = new NpgsqlCommand($"Delete from Packages where PackageId='{PackageID}'", conn)) { commandDelete.ExecuteNonQuery(); };
                        using (var commandDelete = new NpgsqlCommand($"Update users set coins='{(Int32.Parse(Coins.ToString()) - 4).ToString()}' where LoginName='{userID}'", conn)) { commandDelete.ExecuteNonQuery(); };


                        var CardList = PackageArrayToListCards(stringCards);
                        foreach (var item in CardList)
                        {
                            string cardelement;
                            Console.WriteLine(item.CardId);
                            if (item.CardElement == Card.CardelEmentEnum.fire)
                            {
                                cardelement = "Fire";
                            }
                            else if (item.CardElement == Card.CardelEmentEnum.water)
                            {
                                cardelement = "Water";
                            }
                            else
                            {
                                cardelement = "Normal";
                            }
                            using (var commandInsertCard = new NpgsqlCommand($"insert into Card(CardID,Cardtype,CardName,BaseDamage,CardElement,CardStyle) values('{item.CardId}','{item.CardType}','{item.CardName}','{item.BaseDamage.ToString()}','{cardelement}','{item.CardStyle}')", conn)) { commandInsertCard.ExecuteNonQuery(); };
                            using (var commandInsertStack = new NpgsqlCommand($"insert into UserhasCardsinStack(CardId_fk,LoginName_Fk) values('{item.CardId}','{userID}')", conn)) { commandInsertStack.ExecuteNonQuery(); };
                        }
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        conn.Close();
                        return 200;
                    }
                }
            });*/

            GET_cardsRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/cards$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }

                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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
                    string userId = reader[0].ToString();
                    reader.Close();
                    string querystringUnionAllCards = @$"select 'deck' as location,cardId_fk from UserhasCardsinDeck where LoginName_fk='{userId}' union select 'stack' as location,cardId_fk from UserhasCardsinStack where LoginName_fk='{userId}' and (select count(*) from UserhasCardsinDeck where UserhasCardsinDeck.cardID_fk = UserhasCardsinStack.cardID_fk) = 0";
                    Console.WriteLine(querystringUnionAllCards);
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
            });*/
            GET_deckRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/deck$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }

                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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
            });*/
            GET_statsRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/stats$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }

                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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

                    string querystringUnionAllCards = @$"select win,tie,lose,elo from Scoreboard where LoginName_fk = '{UserID}'";
                    using (NpgsqlCommand getStats = new NpgsqlCommand(querystringUnionAllCards, conn))
                    {
                        NpgsqlDataReader readergetStats = getStats.ExecuteReader();
                        if (readergetStats.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            return 400;
                        }
                        readergetStats.Read();
                        var Score = new JsonScore();
                        Score.Win = readergetStats[0].ToString();
                        Score.Lose = readergetStats[1].ToString();
                        Score.Tie = readergetStats[2].ToString();
                        Score.Elo = readergetStats[3].ToString();
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
            });*/


            GET_scoreRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/score$", (IRequestContext httpRequest) =>
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();

                string querystringUnionAllCards = @$"select win,tie,lose,elo,LoginName from Scoreboard join users on scoreboard.LoginName_fk = users.LoginName order by elo desc,win desc";
                using (NpgsqlCommand getScorebaord = new NpgsqlCommand(querystringUnionAllCards, conn))
                {
                    NpgsqlDataReader readergetScorebaord = getScorebaord.ExecuteReader();
                    List<JsonScore> Scoreboard = new List<JsonScore>();

                    while (readergetScorebaord.Read())
                    {
                        var Score = new JsonScore();
                        Score.Win = readergetScorebaord[0].ToString();
                        Score.Lose = readergetScorebaord[1].ToString();
                        Score.Tie = readergetScorebaord[2].ToString();
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

            });*/

            GET_users_anyRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/users/.+$", (IRequestContext httpRequest) =>
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
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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
            });*/
            PUT_users_anyRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("PUT", "^/users/.+$", (IRequestContext httpRequest) =>
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
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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

                    var UpdateData = JsonConvert.DeserializeObject<JsonProfileData>(httpRequest.PayLoad);
                    string query;
                    try
                    {
                        query = querybuilder(UpdateData, UserID);
                    }
                    catch
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }

                    using (NpgsqlCommand UpdateProfileData = new NpgsqlCommand(query, conn))
                    {
                        Console.WriteLine(query);
                        UpdateProfileData.ExecuteNonQuery();
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        conn.Close();
                        return 200;
                    }
                }
            });*/
            PUT_deckRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("PUT", "^/deck$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "401");
                    return 400;
                }

                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();

                using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                {
                    NpgsqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "402");
                        conn.Close();
                        return 400;
                    }
                    reader.Read();
                    string UserID = reader[0].ToString();
                    reader.Close();

                    List<string> TryToAddToDeck = JsonConvert.DeserializeObject<List<string>>(httpRequest.PayLoad);
                    if (TryToAddToDeck.Count != 4)
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "403");
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
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "409");
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
            });*/


            GET_tradingsRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("GET", "^/tradings$", (IRequestContext httpRequest) =>
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();
                using (NpgsqlCommand GetAllTrades = new NpgsqlCommand($"Select TradeID,CardToTradeID_fk,OriginalOwner_fk,typeToTrade,minDamage,TimestampOffer from Trading order by TimestampOffer", conn))
                {
                    List<JsonTradingHelper> TradeList = new List<JsonTradingHelper>();
                    NpgsqlDataReader readerGetAllTrades = GetAllTrades.ExecuteReader();
                    while (readerGetAllTrades.Read())
                    {
                        JsonTradingHelper item = new JsonTradingHelper();
                        item.TradeID = readerGetAllTrades[0].ToString();
                        item.CardToTradeID_fk = readerGetAllTrades[1].ToString();
                        item.OriginalOwner_fk = readerGetAllTrades[2].ToString();
                        item.typeToTrade = readerGetAllTrades[3].ToString();
                        item.minDamage = readerGetAllTrades[4].ToString();
                        item.TimestampOffer = readerGetAllTrades[5].ToString();
                        TradeList.Add(item);
                    }
                    httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", JsonConvert.SerializeObject(TradeList));
                    conn.Close();
                    return 200;
                }
            });*/

            DELETE_tradings_anyRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("Delete", "^/tradings/.+$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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

                    string DeleteId = Regex.Match(httpRequest.MessageEndPoint, "^/tradings/(.*)$").Groups[1].Value;
                    querystring = @$"Delete from Trading where CardToTradeID_fk={DeleteId} and OriginalOwner_fk='{UserID}'";
                    using (NpgsqlCommand DeleteTrading = new NpgsqlCommand(querystring, conn))
                    {
                        DeleteTrading.ExecuteNonQuery();
                        querystring = $"update UserhasCardsinStack set CurrentlyTraded=true where CardId_fk='{DeleteId}' and LoginName_fk='{UserID}'";
                        using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(querystring, conn))
                        {
                            UpdateCardStatus.ExecuteNonQuery();
                        }
                    }
                }
                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                return 200;
            });*/

            POST_tradingsRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/tradings$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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
            });*/
            POST_tradings_anyRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/tradings/.+$", (IRequestContext httpRequest) =>
            {
                httpRequest.Headers.TryGetValue("Authorization", out string token);
                if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                {
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
                string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
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

                    string CardIdEndPoint = Regex.Match(httpRequest.MessageEndPoint, "^/tradings/(.*)$").Groups[1].Value;
                    querystring = $"select TradeId,CardToTradeID_fk,OriginalOwner_fk,typeToTrade,minDamage from Trading  where TradeID='{CardIdEndPoint}'";
                    using (NpgsqlCommand GetTradeData = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader readerGetTradeData = GetTradeData.ExecuteReader();
                        if (readerGetTradeData.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }
                        readerGetTradeData.Read();
                        if (readerGetTradeData[2].ToString() == UserID)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }

                        string TradeId = readerGetTradeData[0].ToString();
                        string CardToTradeID_fk = readerGetTradeData[1].ToString();
                        string OriginalOwner_fk = readerGetTradeData[2].ToString();
                        string typeToTrade = readerGetTradeData[3].ToString();
                        string minDamage = readerGetTradeData[4].ToString();
                        readerGetTradeData.Close();




                        querystring = $"Select CardStyle,BaseDamage from Card join UserhasCardsinStack on Card.CardID = CardId_fk where CardID='{httpRequest.PayLoad}' and LoginName_fk ='{UserID}' and CurrentlyTraded  = false; ";
                        using (NpgsqlCommand SelectCardForTrade = new NpgsqlCommand(querystring, conn))
                        {
                            NpgsqlDataReader readerSelectCardForTrade = SelectCardForTrade.ExecuteReader();
                            if (readerSelectCardForTrade.HasRows == false)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                conn.Close();
                                return 400;
                            }
                            readerSelectCardForTrade.Read();

                            if (readerSelectCardForTrade[0].ToString() != typeToTrade)
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                conn.Close();
                                return 400;
                            }
                            if (Int32.Parse(readerSelectCardForTrade[1].ToString()) <= Int32.Parse(minDamage))
                            {
                                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                conn.Close();
                                return 400;
                            }
                            readerSelectCardForTrade.Close();

                            querystring = $"Select count(*) from UserhasCardsinDeck where LoginName_fk='{UserID}' and CardID = '{httpRequest.PayLoad}'; ";
                            using (NpgsqlCommand VerifynotInDeckCurrently = new NpgsqlCommand(querystring, conn))
                            {
                                NpgsqlDataReader readerVerifynotInDeckCurrently = VerifynotInDeckCurrently.ExecuteReader();
                                readerVerifynotInDeckCurrently.Read();
                                if (Int32.Parse(readerVerifynotInDeckCurrently[0].ToString()) > 0)
                                {
                                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                    conn.Close();
                                    return 400;
                                }
                            }

                            querystring = @$"Update set UserhasCardsinStack LoginName_fk='{UserID}' and CurrentlyTraded = false where CardId_fk='{CardToTradeID_fk}'";
                            using (NpgsqlCommand TradeCommand = new NpgsqlCommand(querystring, conn))
                            {
                                TradeCommand.ExecuteNonQuery();
                            }
                            querystring = @$"Update set UserhasCardsinStack LoginName_fk='{OriginalOwner_fk}' and CurrentlyTraded = false where CardId_fk='{httpRequest.PayLoad}";
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
            });*/
            POST_usersRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/users$", (IRequestContext httpRequest) =>
            {
                JsonRegisterUser Data = JsonConvert.DeserializeObject<JsonRegisterUser>(httpRequest.PayLoad);

                NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                conn.Open();
                string querystring = $"Insert into users (LoginName,Passwort) values('{Data.Username}','{Data.Password}') ";
                using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                        conn.Close();
                        return 400;
                    }

                    querystring = $"Insert into Scoreboard (LoginName_fk) values('{Data.Username}')";
                    using (NpgsqlCommand command2 = new NpgsqlCommand(querystring, conn))
                    {
                        command2.ExecuteNonQuery();
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "201");
                        conn.Close();
                        return 201;
                    }
                }
            });*/
            POST_battlesRoute.RegisterRoute(server);
            /*server.EndPointApi.RegisterEndPoint("POST", "^/battles$", (IRequestContext httpRequest) =>
             {
                 httpRequest.Headers.TryGetValue("Authorization", out string token);
                 if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                 {
                     httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "600");
                     return 400;
                 }
                 string querystring = @$"select LoginName from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                 NpgsqlConnection conn = new NpgsqlConnection(connectstring);
                 conn.Open();

                 string UserID;
                 using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                 {
                     NpgsqlDataReader reader = command.ExecuteReader();
                     if (reader.HasRows == false)
                     {
                         httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "601");
                         conn.Close();
                         return 400;
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
                         httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "602");
                         conn.Close();
                         return 400;
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
                         httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "603");
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

                         Deck.Add(Cardmaker(Int32.Parse(reader[3].ToString()), reader[2].ToString(), reader[0].ToString(), Type, whatcard));
                     }
                 }
                 if(Deck.Count != 4)
                 {
                     httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "604");
                     conn.Close();
                     return 400;
                 }
                 player.Deck = Deck;
                 AutoResetEvent myEvent = new AutoResetEvent(false);
                 fightApi.addToList(player, myEvent);
                 myEvent.WaitOne();

                 switch (player.status)
                 {
                     case FightHandler.BattleStatus.Win:
                         IncreaseWin(UserID);
                         httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", player.assignedBattleLog);
                         conn.Close();
                         return 200;
                     case FightHandler.BattleStatus.Lose:
                         IncreaseLose(UserID);
                         httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", player.assignedBattleLog);
                         conn.Close();
                         return 200;
                     case FightHandler.BattleStatus.Tie:
                         IncreaseTie(UserID);
                         httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200",player.assignedBattleLog);
                         conn.Close();
                         return 200;
                     default:
                         httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "605");
                         conn.Close();
                         return 500;
                 }
             });*/
            GET_helpRoute.RegisterRoute(server);
            GET_helloRoute.RegisterRoute(server);

            later = DateTime.Now;
            Console.WriteLine($"Took: {Difference(early, later)} millSeconds");


            
            Console.WriteLine("Trying to connect to database and define table setup....");
            early = DateTime.Now;
            DbHelper.CreateTables();
            later = DateTime.Now;
            Console.WriteLine($"Took: {Difference(early, later)} millSeconds");



            Console.WriteLine("Starting Matching Thread....");
            early = DateTime.Now;
            fightApi.StartMatching();
            later = DateTime.Now;
            Console.WriteLine($"Took: {Difference(early, later)} millSeconds");



            Console.WriteLine("listening for connections....");
            server.ListenForConnections();
        }
    }
}

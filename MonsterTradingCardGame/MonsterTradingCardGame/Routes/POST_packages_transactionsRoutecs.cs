using MCTG.Cards;
using MCTG.DbHelpers;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class POST_packages_transactionsRoutecs
    {
        public static void RegisterRoute(ServerTcpListener server)
        {

            server.EndPointApi.RegisterEndPoint("POST", "^/transactions/packages$", (IRequestContext httpRequest) =>
                     {
                         try
                         {
                             string userID;
                             string Coins;
                             httpRequest.Headers.TryGetValue("Authorization", out string token);
                             if (!Regex.IsMatch(token, "^Basic (.*)-mtcgToken$"))
                             {
                                 httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                 return 400;
                             }
                             string querystring = @$"select LoginName,Coins from users where LoginName='{Regex.Match(token, "^Basic (.*)-mtcgToken$").Groups[1].Value}'";
                             NpgsqlConnection conn = DbHelper.ConnectObj();
                             conn.Open();
                             using (NpgsqlCommand command = new NpgsqlCommand(querystring, conn))
                             {
                                 NpgsqlDataReader reader = command.ExecuteReader();
                                 if (reader.HasRows == false)
                                 {
                                     Console.WriteLine("illegal FieldCount " + reader.FieldCount);
                                     httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "401");
                                     conn.Close();
                                     return 401;
                                 }

                                 reader.Read();
                                 userID = reader[0].ToString();
                                 Coins = reader[1].ToString();
                                 reader.Close();

                                 if (Int32.Parse(Coins) < 5)
                                 {
                                     httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                                     conn.Close();
                                     return 400;
                                 }
                                 using (NpgsqlCommand command2 = new NpgsqlCommand("Select PackageId,StringCards from Packages order by PackageID asc limit 1", conn))
                                 {
                                     NpgsqlDataReader readerPackageString = command2.ExecuteReader();
                                     if (readerPackageString.HasRows == false)
                                     {
                                         httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "409");
                                         conn.Close();
                                         return 409;
                                     }
                                     readerPackageString.Read();
                                     string PackageID = readerPackageString[0].ToString();
                                     string stringCards = readerPackageString[1].ToString();
                                     readerPackageString.Close();
                                     using (var commandDelete = new NpgsqlCommand($"Delete from Packages where PackageId='{PackageID}'", conn)) { commandDelete.ExecuteNonQuery(); };
                                     using (var commandDelete = new NpgsqlCommand($"Update users set coins='{(Int32.Parse(Coins.ToString()) - 5).ToString()}' where LoginName='{userID}'", conn)) { commandDelete.ExecuteNonQuery(); };


                                     var CardList = Cards.CardHelpers.PackageArrayToListCards(stringCards);
                                     foreach (var item in CardList)
                                     {
                                         string cardelement;
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

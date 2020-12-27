using MCTG.DbHelpers;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Text.RegularExpressions;
using System;

namespace MCTG.Routes
{
    public static class DELETE_tradings_anyRoute
    {
        public static void registerRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("DELETE", "^/tradings/.+$", (IRequestContext httpRequest) =>
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

                    string DeleteId = Regex.Match(httpRequest.MessageEndPoint, "^/tradings/(.*)$").Groups[1].Value;

                    string CardID;
                    querystring = $"Select cardtotradeid_fk from Trading where tradeid='{DeleteId}' and originalowner_fk='{UserID}'";
                    Console.WriteLine(querystring);
                    using (NpgsqlCommand DeleteTrading = new NpgsqlCommand(querystring, conn))
                    {
                        NpgsqlDataReader readerSelect = DeleteTrading.ExecuteReader();
                        if (readerSelect.HasRows == false)
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                            conn.Close();
                            return 400;
                        }
                        readerSelect.Read();
                        CardID = readerSelect[0].ToString();
                        readerSelect.Close();
                    }
                    Console.WriteLine(CardID);
                    querystring = @$"Delete from Trading where tradeID='{DeleteId}' and OriginalOwner_fk='{UserID}'";
                    Console.WriteLine(querystring);

                    using (NpgsqlCommand DeleteTrading = new NpgsqlCommand(querystring, conn))
                    {
                        DeleteTrading.ExecuteNonQuery();
                        querystring = $"update UserhasCardsinStack set CurrentlyTraded=false where CardId_fk='{CardID}' and LoginName_fk='{UserID}'";
                        Console.WriteLine(querystring);
                        using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(querystring, conn))
                        {
                            UpdateCardStatus.ExecuteNonQuery();
                        }
                    }
                }
                httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                return 200;
            });
        }
    }
}

using MCTG.DbHelpers;
using MCTG.JsonHelper;
using Newtonsoft.Json;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Collections.Generic;

namespace MCTG.Routes
{
    public static class GET_tradingsRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/tradings$", (IRequestContext httpRequest) =>
            {
                try
                {
                    NpgsqlConnection conn = DbHelper.ConnectObj();
                    conn.Open();
                    using (NpgsqlCommand GetAllTrades = new NpgsqlCommand($"Select TradeID,CardToTradeID_fk,OriginalOwner_fk,typeToTrade,minDamage,TimestampOffer from Trading order by TimestampOffer", conn))
                    {
                        List<JsonTradingHelper> TradeList = new List<JsonTradingHelper>();
                        NpgsqlDataReader readerGetAllTrades = GetAllTrades.ExecuteReader();
                        if (readerGetAllTrades.HasRows)
                        {
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
                        else
                        {
                            httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", " \"No trades currently\" ");
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

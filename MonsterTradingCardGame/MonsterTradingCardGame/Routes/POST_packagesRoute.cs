using MCTG.DbHelpers;
using Npgsql;
using Restservice.Http_Service;
using Restservice.Server;
using System.Text.RegularExpressions;

namespace MCTG.Routes
{
    public static class POST_packagesRoute
    {
        public static void RegisterRoute(ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("POST", "^/packages$", (IRequestContext httpRequest) =>
            {
                try
                {
                    httpRequest.Headers.TryGetValue("Authorization", out string token);
                    if (!Regex.IsMatch(token, "Basic admin-mtcgToken"))
                    {
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "405");
                        return 405;
                    }

                    NpgsqlConnection conn = DbHelper.ConnectObj();
                    conn.Open();

                    using (NpgsqlCommand PackageInsert = new NpgsqlCommand(@$"Insert into Packages(StringCards) values('{httpRequest.PayLoad}');", conn))
                    {
                        try
                        {
                            PackageInsert.ExecuteNonQuery();
                        }
                        catch
                        {
                            httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "500");
                            return 500;
                        }

                    }

                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "201");
                    return 201;
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


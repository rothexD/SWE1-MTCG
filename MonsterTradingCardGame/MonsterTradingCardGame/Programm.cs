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
            POST_packagesRoute.RegisterRoute(server);
            POST_packages_transactionsRoutecs.RegisterRoute(server);
            GET_cardsRoute.RegisterRoute(server);
            GET_deckRoute.RegisterRoute(server);
            GET_statsRoute.RegisterRoute(server);
            GET_scoreRoute.RegisterRoute(server);
            GET_users_anyRoute.RegisterRoute(server);
            PUT_users_anyRoute.RegisterRoute(server);
            PUT_deckRoute.RegisterRoute(server);
            GET_tradingsRoute.RegisterRoute(server);
            DELETE_tradings_anyRoute.RegisterRoute(server);
            POST_tradingsRoute.RegisterRoute(server);
            POST_tradings_anyRoute.RegisterRoute(server);
            POST_usersRoute.RegisterRoute(server);
            POST_battlesRoute.RegisterRoute(server);
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

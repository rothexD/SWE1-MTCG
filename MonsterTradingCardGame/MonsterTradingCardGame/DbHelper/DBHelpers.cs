using MCTG.JsonHelper;
using Npgsql;
using System;
using System.Data;

namespace MCTG.DbHelpers
{
    public static class DbHelper
    {
        private static readonly string _Connectstring = "Host=localhost;Username=postgres;Password=postgres;Database=swe1";
        public static NpgsqlConnection ConnectObj()
        {
            return new NpgsqlConnection(_Connectstring);
        }
        public static string querybuilder(JsonProfileData data, string userID)
        {
            string returnval = "Update users set ";
            if (data.Image == null && data.Name == null && data.Bio == null)
            {
                throw new ArgumentException("all null,update empty");
            }
            returnval += " Image='" + data.Image + "'" ?? "";
            returnval += ", username='" + data.Name + "'" ?? "";
            returnval += ", Bio='" + data.Bio + "'" ?? "";
            returnval += $" where LoginName='{userID}'";
            return returnval;
        }
        public static void CreateTables()
        {
            try
            {
                IDbConnection conn = DbHelper.ConnectObj();
                conn.Open();
                {
                    IDbCommand command = conn.CreateCommand();
                    Console.WriteLine("preparing database ....");
                    command.CommandText = @"Create table if not exists users
                                        (
                                        LoginName Varchar primary key,
                                        Passwort Varchar not null,
                                        UserName Varchar,
                                        bio Varchar,
                                        Image Varchar,
                                        Coins int default 20
                                        ); ";
                    command.ExecuteNonQuery();
                }

                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @" Create table if not exists Scoreboard(
                                        LoginName_fk Varchar primary key,
                                        win int default 0,
                                        tie int default 0,
                                        lose int default 0 ,
                                        elo int default 100,
                                        FOREIGN KEY(LoginName_fk) REFERENCES users(LoginName) ON DELETE CASCADE
                                    );";
                    command.ExecuteNonQuery();
                }
                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @"Create table if not exists Card
                                    (
                                        CardID Varchar primary key,
                                        Cardtype Varchar,
                                        CardName Varchar,
                                        BaseDamage varchar,
                                        CardElement Varchar,
                                        CardStyle Varchar       
                                    );";
                    command.ExecuteNonQuery();
                }
                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @"Create table if not exists UserhasCardsinDeck(
                                        CardId_fk varchar,
                                        LoginName_fk varchar,
                                        FOREIGN KEY(CardId_fk) REFERENCES Card(CardID) ON DELETE CASCADE,
                                        FOREIGN KEY(LoginName_fk) REFERENCES users(LoginName) ON DELETE CASCADE,
                                        PRIMARY KEY (CardId_fk,LoginName_fk)
                                    );";
                    command.ExecuteNonQuery();
                }
                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @"Create table if not exists UserhasCardsinStack(
                                        CardId_fk varchar,
                                        LoginName_fk varchar,
                                        CurrentlyTraded bool default false,
                                        FOREIGN KEY(CardId_fk) REFERENCES Card(CardID) ON DELETE CASCADE,
                                        FOREIGN KEY(LoginName_fk) REFERENCES users(LoginName) ON DELETE CASCADE,
                                        PRIMARY KEY (CardId_fk,LoginName_fk)        
                                    );";
                    command.ExecuteNonQuery();
                }
                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @" Create table if not exists Packages
                                        (
                                        PackageId serial primary Key,
                                        StringCards varchar
                                        );
                                        ";
                    command.ExecuteNonQuery();
                }

                {
                    IDbCommand command = conn.CreateCommand();
                    command.CommandText = @"Create table if not exists Trading(
                                        TradeID varchar primary key,
                                        CardToTradeID_fk varchar unique,
                                        OriginalOwner_fk varchar,
                                        typeToTrade varchar,
                                        minDamage varchar,
                                        TimestampOffer timestamp default now(),
                                        foreign key(CardToTradeID_fk) references Card(CardID),
                                        foreign key(OriginalOwner_fk) references users(loginname)
                                    );";
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch
            {
                Console.WriteLine("DB connection could not be established");
                Environment.Exit(1);
            }
        }
        public static void IncreaseWin(string loginName)
        {
            NpgsqlConnection conn = ConnectObj();
            conn.Open();
            string query = $"update Scoreboard set win=win+1,elo=elo+4 where loginName_fk = '{loginName}'";
            using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(query, conn))
            {
                UpdateCardStatus.ExecuteNonQuery();
            }
            query = $"update users set coins=coins+1 where loginName = '{loginName}'";
            using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(query, conn))
            {
                UpdateCardStatus.ExecuteNonQuery();
            }
            conn.Close();
        }
        public static void IncreaseTie(string loginName)
        {
            NpgsqlConnection conn = ConnectObj();
            conn.Open();
            string query = $"update Scoreboard set tie=tie+1,elo=elo-1 where loginName_fk = '{loginName}'";
            using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(query, conn))
            {
                UpdateCardStatus.ExecuteNonQuery();
            }
            conn.Close();
        }
        public static void IncreaseLose(string loginName)
        {
            NpgsqlConnection conn = ConnectObj();
            conn.Open();
            string query = $"update Scoreboard set lose=lose+1,elo=elo-4 where loginName_fk = '{loginName}'";
            using (NpgsqlCommand UpdateCardStatus = new NpgsqlCommand(query, conn))
            {
                UpdateCardStatus.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}

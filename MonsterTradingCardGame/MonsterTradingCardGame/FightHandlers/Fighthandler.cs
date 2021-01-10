using MCTG.Cards;
using MCTG.Players;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MCTG.FightHandlers
{
    public class FightHandler
    {
        public enum BattleStatus
        {
            Fighthasnothappened,
            Win,
            Lose,
            Tie,
            Error
        }

        public readonly int MaxRounds = 100;
        protected Random Dice;
        private BlockingCollection<(Player, AutoResetEvent)> WaitingQue { get; set; }

        public FightHandler()
        {
            Dice = new Random(Guid.NewGuid().GetHashCode());
            WaitingQue = new BlockingCollection<(Player, AutoResetEvent)>();
        }
        public FightHandler(int PredfinedDice)
        {
            Dice = new Random(PredfinedDice);
            WaitingQue = new BlockingCollection<(Player, AutoResetEvent)>();
        }
        public void StartMatching()
        {
            Thread startMatching = new Thread(delegate () { TryMatchmaking(); });
            startMatching.Start();
        }
        private void TryMatchmaking()
        {
            while (true)
            {
                (Player, AutoResetEvent) data1 = WaitingQue.Take();
                (Player, AutoResetEvent) data2 = WaitingQue.Take();

                Thread Fighthandler = new Thread(delegate () { Fighting(data1, data2); });
                Fighthandler.Start();
            }
        }
        private void Fighting((Player, AutoResetEvent) data1, (Player, AutoResetEvent) data2)
        {
            if (data1.Item1 == null || data2.Item1 == null)
            {
                Console.WriteLine("something null");
            }
            if (!Fight(data1.Item1, data2.Item1))
            {
                data1.Item1.Status = BattleStatus.Error;
                data2.Item1.Status = BattleStatus.Error;
                data1.Item2.Set();
                data2.Item2.Set();
            }
            else
            {
                data1.Item2.Set();
                data2.Item2.Set();
            }
        }
        public bool QueUpForFight(Player player, AutoResetEvent Iam)
        {
            if(player == null && Iam != null)
            {
                Iam.Set();
                return false;
            }else if( Iam == null)
            {
                return false;
            }
            WaitingQue.Add((player, Iam));
            return true;
        }
        private

        protected void BattlelogHeader(bool FirstPlayer, ref Player player1, ref Player player2, ref string Battlelog)
        {
            Player TempFirstPlayer;
            if (!FirstPlayer)
            {
                TempFirstPlayer = player2;
            }
            else
            {
                TempFirstPlayer = player1;
            }
            Battlelog += $"Fight between {player1.UserName} and {player2.UserName}{Environment.NewLine}";
            Battlelog += $"Max Rounds: {MaxRounds}, BattleTime: {DateTime.Now}, FirstAttacker: {TempFirstPlayer.UserName}{Environment.NewLine}";
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
            Battlelog += $"Player1: {player1.UserName}, ELo:{player1.Elo}{Environment.NewLine}";
            foreach (var item in player1.Deck)
            {
                Battlelog += $"\t Type: {item.CardStyle}({item.CardType}), CardName: {item.CardName}, CardBaseDamage:{item.BaseDamage}, Element:{item.CardElement}, CardId:{item.CardId}{Environment.NewLine}";
            }
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
            Battlelog += $"Player2: {player2.UserName}, ELo:{player2.Elo}{Environment.NewLine}";
            foreach (var item in player2.Deck)
            {
                Battlelog += $"\t Type: {item.CardStyle}({item.CardType}), CardName: {item.CardName}, CardBaseDamage:{item.BaseDamage}, Element:{item.CardElement}, CardId:{item.CardId}{Environment.NewLine}";
            }
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
        }
        protected bool FightOneRound(ref Player Attacker, ref Player Defender, ref string Battlelog)
        {
            if (Attacker.Deck.Count == 0 || Defender.Deck.Count == 0)
            {
                return false;
            }
            int AttackerIndex = Dice.Next(0, Attacker.Deck.Count - 1);
            int DefenderIndex = Dice.Next(0, Defender.Deck.Count - 1);
            Card CardOfAttacker = Attacker.Deck[AttackerIndex];
            Card CardOfDefender = Defender.Deck[DefenderIndex];
            int DamageAttacker = CardOfAttacker.CalculateDamage(CardOfDefender);
            int DamageDefender = CardOfDefender.CalculateDamage(CardOfAttacker);
            Battlelog += $"{CardOfAttacker.CardName}({CardOfAttacker.CardType}) uses {CardOfAttacker.AttackMoveName} to attack {CardOfDefender.CardName}({CardOfDefender.CardType}){Environment.NewLine}";
            Battlelog += $"AttackerDamage: {DamageAttacker}, DefenderDamage: {DamageDefender}{Environment.NewLine}";
            if (DamageAttacker > DamageDefender)
            {
                Battlelog += $"Winner is the Attacker, the defeated Defender joins our Ranks {Environment.NewLine}";
                Defender.Deck.RemoveAt(DefenderIndex);
                Attacker.Deck.Add(CardOfDefender);
                Battlelog += $"Deck Size: attacker: {Attacker.Deck.Count} defender: {Defender.Deck.Count}{Environment.NewLine}";
                return true;
            }
            {
                Battlelog += $"Winner is the Defender, the defeated Attacker joins our Army {Environment.NewLine}";
                Attacker.Deck.RemoveAt(AttackerIndex);
                Defender.Deck.Add(CardOfAttacker);
                Battlelog += $"Deck Size: attacker: {Attacker.Deck.Count} defender: {Defender.Deck.Count}{Environment.NewLine}";
                return true;
            }
        }
        protected void ShuffleTempLists(ref Player player1, ref Player player2)
        {
            //https://stackoverflow.com/questions/273313/randomize-a-listt
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (player1.Deck.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, player1.Deck.Count - 1);
                Card Tempcard = player1.Deck[ShuffleIndex];
                player1.Deck.RemoveAt(ShuffleIndex);
                player1.Deck.Add(Tempcard);
            }
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (player2.Deck.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, player2.Deck.Count - 1);
                Card Tempcard = player2.Deck[ShuffleIndex];
                player2.Deck.RemoveAt(ShuffleIndex);
                player2.Deck.Add(Tempcard);
            }
        }

        public bool Fight(Player player1, Player player2)
        {
            string Battlelog = "";
            bool PlayerTurn;
            if (Dice.Next(0, 100) % 2 == 1)
            {
                PlayerTurn = false;
            }
            else
            {
                PlayerTurn = true;
            }
            BattlelogHeader(PlayerTurn, ref player1, ref player2, ref Battlelog);
            for (int RoundCounter = 0; RoundCounter < MaxRounds; RoundCounter++)
            {
                if (player1.Deck.Count == 0 || player2.Deck.Count == 0)
                {
                    break;
                }
                ShuffleTempLists(ref player1, ref player2);
                if (PlayerTurn)
                {
                    Battlelog += $"{Environment.NewLine}Round{RoundCounter} Atacker: {player1.UserName}, Defender: {player2.UserName}{Environment.NewLine}";
                    FightOneRound(ref player1, ref player2, ref Battlelog);
                }
                else
                {
                    Battlelog += $"{Environment.NewLine}Round{RoundCounter} Atacker: {player2.UserName}, Defender: {player1.UserName}{Environment.NewLine}";
                    FightOneRound(ref player2, ref player1, ref Battlelog);
                }
                Battlelog += $"{Environment.NewLine}";
                PlayerTurn = !PlayerTurn;
            }
            if (player1.Deck.Count == player2.Deck.Count)
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                player1.AssignedBattleLog = Battlelog;
                player2.AssignedBattleLog = Battlelog;
                player2.Status = FightHandler.BattleStatus.Tie;
                player1.Status = FightHandler.BattleStatus.Tie;
                return true;
            }
            else if (player1.Deck.Count > player2.Deck.Count)
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                Battlelog += $"Winner is {player1.UserName}{Environment.NewLine}";
                player1.AssignedBattleLog = Battlelog;
                player2.AssignedBattleLog = Battlelog;
                player2.Status = FightHandler.BattleStatus.Lose;
                player1.Status = FightHandler.BattleStatus.Win;
                return true;
            }
            else
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                Battlelog += $"Winner is {player2.UserName}{Environment.NewLine}";
                player1.AssignedBattleLog = Battlelog;
                player2.AssignedBattleLog = Battlelog;
                player2.Status = FightHandler.BattleStatus.Win;
                player1.Status = FightHandler.BattleStatus.Lose;
                return true;
            }
        }
    }
}

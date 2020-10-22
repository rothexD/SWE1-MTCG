﻿using System;
using System.Collections.Generic;
using System.Text;
using MCTG.Cards;
using MCTG.Players;
namespace MCTG.FightHandlers
{
    public class FightHandler
    {
        public enum BattleStatus
        {
            FightHasntHappened,
            Tie,
            Player1Winner,
            Player2Winner
        }
        public string Battlelog { get; protected set;  }
        protected List<Card> Player1TempDeck;
        protected List<Card> Player2TempDeck;
        protected Player Player1;
        protected Player Player2;
        public const int MaxRounds = 100;
        protected Random Dice;
        public BattleStatus MostRecentStatus { get; protected set; }
        public BattleStatus Status { get; protected set; }
     
        
        public FightHandler(Player Player1,Player Player2)
        {
            if (Player1.Deck.Count != 5)
            {
                throw new ArgumentException("Player1 less than 5 cards in Deck");
            }
            if (Player2.Deck.Count != 5)
            {
                throw new ArgumentException("Player2 less than 5 cards in Deck");
            }
            Player1TempDeck = new List<Card>();
            Player2TempDeck = new List<Card>();
            this.Player1 = Player1;
            this.Player2 = Player2;
            this.Battlelog = "";
            Dice = new Random();
            Status = BattleStatus.FightHasntHappened;
            foreach(var item in Player1.Deck)
            {
                Player1TempDeck.Add(item.Value);
            }
            foreach (var item in Player2.Deck)
            {
                Player2TempDeck.Add(item.Value);
            }
            MostRecentStatus = BattleStatus.FightHasntHappened;
        }

        protected void BattlelogHeader(bool FirstPlayer)
        {
            Player TempFirstPlayer;
            if (!FirstPlayer)
            {
                TempFirstPlayer = Player2;
            }
            else
            {
                TempFirstPlayer = Player1;
            }
            Battlelog += $"Fight between {Player1.PlayerName} and {Player2.PlayerName}{Environment.NewLine}";
            Battlelog += $"Max Rounds: {MaxRounds}, BattleTime: {DateTime.Now}, FirstAttacker: {TempFirstPlayer.PlayerName}{Environment.NewLine}";
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
            Battlelog += $"Player1: {Player1.PlayerName}, ELo:{Player1.Elo}{Environment.NewLine}";
            foreach(var item in Player1TempDeck)
            {
                Battlelog += $"\t Type: {item.CardStyle}({item.CardType}), CardName: {item.CardName}, CardBaseDamage:{item.BaseDamage}, Element:{item.CardElement}, CardId:{item.CardId}{Environment.NewLine}";
            }
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
            Battlelog += $"Player2: {Player2.PlayerName}, ELo:{Player2.Elo}{Environment.NewLine}";
            foreach (var item in Player2TempDeck)
            {
                Battlelog += $"\t Type: {item.CardStyle}({item.CardType}), CardName: {item.CardName}, CardBaseDamage:{item.BaseDamage}, Element:{item.CardElement}, CardId:{item.CardId}{Environment.NewLine}";
            }
            Battlelog += $"--------------------------------------------------------------------------------------------{Environment.NewLine}";
        }
        protected bool FightOneRound(ref List<Card> Attacker, ref List<Card> Defender)
        {
            if (Attacker.Count == 0|| Defender.Count ==0)
            {
                return false;
            }
            int AttackerIndex = Dice.Next(0, Attacker.Count - 1);
            int DefenderIndex = Dice.Next(0, Defender.Count - 1);
            Card CardOfAttacker = Attacker[AttackerIndex];
            Card CardOfDefender = Defender[DefenderIndex];
            int DamageAttacker = CardOfAttacker.CalculateDamage(CardOfDefender);
            int DamageDefender = CardOfDefender.CalculateDamage(CardOfAttacker);
            Battlelog += $"{CardOfAttacker.CardName}({CardOfAttacker.CardType}) uses {CardOfAttacker.AttackMoveName} to attack {CardOfDefender.CardName}({CardOfDefender.CardType}){Environment.NewLine}";
            Battlelog += $"AttackerDamage: {DamageAttacker}, DefenderDamage: {DamageDefender}{Environment.NewLine}";
            if (DamageAttacker > DamageDefender)
            {
                Battlelog += $"Winner is the Attacker, the defeated Defender joins our Ranks {Environment.NewLine}";
                Defender.RemoveAt(DefenderIndex);
                Attacker.Add(CardOfDefender);
                Battlelog += $"Deck Size: attacker: {Attacker.Count} defender: {Defender.Count}{Environment.NewLine}";
                return true;
            }
            {
                Battlelog += $"Winner is the Defender, the defeated Attacker joins our Army {Environment.NewLine}";
                Attacker.RemoveAt(AttackerIndex);
                Defender.Add(CardOfAttacker);
                Battlelog += $"Deck Size: attacker: {Attacker.Count} defender: {Defender.Count}{Environment.NewLine}";
                return true;
            }
        }
        protected void ShuffleTempLists()
        {
        //https://stackoverflow.com/questions/273313/randomize-a-listt
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (Player1TempDeck.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, Player1TempDeck.Count - 1);
                Card Tempcard = Player1TempDeck[ShuffleIndex];
                Player1TempDeck.RemoveAt(ShuffleIndex);
                Player1TempDeck.Add(Tempcard);
            }
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (Player2TempDeck.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, Player2TempDeck.Count - 1);
                Card Tempcard = Player2TempDeck[ShuffleIndex];
                Player2TempDeck.RemoveAt(ShuffleIndex);
                Player2TempDeck.Add(Tempcard);
            }
        }

        public bool Fight()
        {
            bool PlayerTurn;          
            if (Dice.Next(0, 100)%2 == 1)
            {
                PlayerTurn = false;
            }
            else
            {
                PlayerTurn = true;
            }
            BattlelogHeader(PlayerTurn);
            for (int RoundCounter = 0; RoundCounter < MaxRounds; RoundCounter++)
            {
                if (Player1TempDeck.Count == 0 || Player2TempDeck.Count == 0)
                {
                    break;
                }
                ShuffleTempLists();
                if (PlayerTurn)
                {
                    Battlelog += $"{Environment.NewLine}Round{RoundCounter} Atacker: {Player1.PlayerName}, Defender: {Player2.PlayerName}{Environment.NewLine}";
                    FightOneRound(ref Player1TempDeck, ref Player2TempDeck);
                }
                else
                {
                    Battlelog += $"{Environment.NewLine}Round{RoundCounter} Atacker: {Player2.PlayerName}, Defender: {Player1.PlayerName}{Environment.NewLine}";
                    FightOneRound(ref Player2TempDeck, ref Player1TempDeck);
                }
                
                Battlelog += $"{Environment.NewLine}";
                PlayerTurn = !PlayerTurn;               
            }
            if (Player1TempDeck.Count == Player2TempDeck.Count)
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                Battlelog += "ITS A TIE";
                MostRecentStatus = BattleStatus.Tie;
                Player1.IncreaseTie();
                Player2.IncreaseTie();
                return true;
            }else if(Player1TempDeck.Count > Player2TempDeck.Count)
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                Battlelog += $"Winner is {Player1.PlayerName}{Environment.NewLine}";
                MostRecentStatus = BattleStatus.Player1Winner;
                Player1.IncreaseWin();
                Player2.IncreaseLose();
                return true;
            }
            else
            {
                Battlelog += $"-------------------------------------------------------------------{Environment.NewLine}";
                Battlelog += $"Winner is {Player2.PlayerName}{Environment.NewLine}";
                MostRecentStatus = BattleStatus.Player2Winner;
                Player2.IncreaseWin();
                Player1.IncreaseLose();
                return true;
            }
        }
    }
}

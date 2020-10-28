using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MCTG.Cards;

namespace MCTG.Players
{
    public enum DeckAndStackStatus
    {
        Success,
        CardAlreadyInStack,
        CardAlreadyInDeck,
        DeckIsFull,
        CardNotInStack,
        CardNotInDeck,
        UserAlreadyOwnsCard,
        NotEnoughCoins,
        Failed
    }
    public class Player
    {  
        public string PlayerName { get; private set; }
        public int Win { get; private set; }
        public int Lose { get; private set; }
        public int Tie { get; private set; }
        public int Coins { get; private set; }
        public int Elo { get; private set; }
        private Mutex PlayerMutex;
        public Dictionary<string, Card> Deck;
        public Dictionary<string, Card> Stack;
        public string MostRecentStatus { get; private set; }

        public Player(string PlayerName, int Win, int Lose, int Tie, int Coins, int Elo)
        {
            this.Coins = Coins;
            this.Elo = Elo;
            this.Win = Win;
            this.Lose = Lose;
            this.Tie = Tie;
            this.PlayerName = PlayerName;
            this.PlayerMutex = new Mutex();
            this.Deck = new Dictionary<string, Card>();
            this.Stack = new Dictionary<string, Card>();
            MostRecentStatus = "Nothing has Happened";
        }
        public Player(string PlayerName, int Win, int Lose, int Tie, int Coins, int Elo, Dictionary<string, Card> Stack, Dictionary<string, Card> Deck)
        {
            if (Deck.Count > 5)
            {
                throw new ArgumentException("To many cards in Deck Parameter (max5)");
            }
            this.Stack = new Dictionary<string, Card>(Stack);
            this.Deck = new Dictionary<string, Card>();
            foreach (KeyValuePair<string,Card> IndividualEntry in Deck)
            {
                if (!Stack.ContainsKey(IndividualEntry.Value.CardId))
                {
                    this.Deck.Add(IndividualEntry.Key, IndividualEntry.Value);
                }
                else
                {
                    throw new ArgumentException("Combination of Deck and Set not unique");
                }
            }                    
            this.Coins = Coins;
            this.Elo = Elo;
            this.Win = Win;
            this.Lose = Lose;
            this.Tie = Tie;
            this.PlayerName = PlayerName;
            this.PlayerMutex = new Mutex();
            MostRecentStatus = "Nothing has Happened";
        }
        public void IncreaseWin()
        {
            PlayerMutex.WaitOne();
            Win++;
            Elo += 5;
            Coins++;
            MostRecentStatus = "Increase Win,Increase Elo by 5 & Coins by 1";
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseLose()
        {
            PlayerMutex.WaitOne();
            Lose++;
            Elo -= 3;
            MostRecentStatus = "Increase Lose,Decrease Elo by 1";
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseTie()
        {
            PlayerMutex.WaitOne();
            Tie++;
            Elo -= 1;
            MostRecentStatus = "Increase Tie,Decrease Elo by 1";
            PlayerMutex.ReleaseMutex();
        }
        public bool AddToStack(Card CardInstance)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(CardInstance.CardId)||Deck.ContainsKey(CardInstance.CardId))
            {
                MostRecentStatus = $"User already Ownes Card {CardInstance.CardName}";
                PlayerMutex.ReleaseMutex();
                return false;              
            }
            else
            {
                Stack.Add(CardInstance.CardId, CardInstance);
                MostRecentStatus = "Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
        }
        public bool RemoveFromStack(string CardId)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(CardId))
            {
                Stack.Remove(CardId);
                MostRecentStatus = $"Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
            else
            {
                MostRecentStatus = $"Card not in Stack";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool MoveFromDeckToStack(string CardId)
        {
            PlayerMutex.WaitOne();
            if (Deck.ContainsKey(CardId))
            {
                if (Stack.ContainsKey(CardId))
                {
                    MostRecentStatus = $"Card Already in Stack";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                else
                {
                    Stack.Add(CardId, Deck[CardId]);
                    Deck.Remove(CardId);
                    MostRecentStatus = $"Succes";
                    PlayerMutex.ReleaseMutex();
                    return true;
                }
            }
            else
            {             
                MostRecentStatus = $"Card not in Deck";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool MoveFromStackToDeck(string CardId)
        {
            PlayerMutex.WaitOne();
            if (Deck.Count >= 5)
            {
                MostRecentStatus = $"Deck is Full";
                PlayerMutex.ReleaseMutex();
                return false;
            }
            if (Stack.ContainsKey(CardId))
            {
                if (Deck.ContainsKey(CardId))
                {
                    MostRecentStatus = $"Card Already in Deck";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                else
                {
                    Deck.Add(CardId, Stack[CardId]);
                    Stack.Remove(CardId);
                    MostRecentStatus = $"Success";
                    PlayerMutex.ReleaseMutex();
                    return true;
                }
            }
            else
            {
                MostRecentStatus = $"Card not in Stack";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool UserAttemptsCardPurchase(Card BuyThisCard, int Price)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(BuyThisCard.CardId) || Deck.ContainsKey(BuyThisCard.CardId))
            {
                MostRecentStatus = $"Card already owned";
                PlayerMutex.ReleaseMutex();
                return false;
            }
            else
            {
                if (this.Coins - Price < 0)
                {
                    MostRecentStatus = $"Not enough Coins";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                Stack.Add(BuyThisCard.CardId, BuyThisCard);
                this.Coins -= Price;
                MostRecentStatus = $"Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
        }
    }
}
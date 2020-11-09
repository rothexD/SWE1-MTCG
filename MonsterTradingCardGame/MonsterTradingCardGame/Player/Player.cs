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
        public Dictionary<int, Card> Deck;
        public Dictionary<int, Card> Stack;

        public Player(string PlayerName, int Win, int Lose, int Tie, int Coins, int Elo)
        {
            this.Coins = Coins;
            this.Elo = Elo;
            this.Win = Win;
            this.Lose = Lose;
            this.Tie = Tie;
            this.PlayerName = PlayerName;
            this.PlayerMutex = new Mutex();
            this.Deck = new Dictionary<int, Card>();
            this.Stack = new Dictionary<int, Card>();
        }
        public Player(string PlayerName, int Win, int Lose, int Tie, int Coins, int Elo, Dictionary<int, Card> Stack, Dictionary<int, Card> Deck)
        {
            if (Deck.Count > 5)
            {
                throw new ArgumentException("To many cards in Deck Parameter (max5)");
            }
            this.Stack = new Dictionary<int, Card>(Stack);
            this.Deck = new Dictionary<int, Card>();
            foreach (KeyValuePair<int,Card> IndividualEntry in Deck)
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
        }
        public void IncreaseWin(out string status)
        {
            PlayerMutex.WaitOne();
            Win++;
            Elo += 5;
            Coins++;
            status = "Increase Win,Increase Elo by 5 & Coins by 1";
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseLose(out string status)
        {
            PlayerMutex.WaitOne();
            Lose++;
            Elo -= 3;
            status = "Increase Lose,Decrease Elo by 1";
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseTie(out string status)
        {
            PlayerMutex.WaitOne();
            Tie++;
            Elo -= 1;
            status = "Increase Tie,Decrease Elo by 1";
            PlayerMutex.ReleaseMutex();
        }
        public bool AddToStack(Card CardInstance, out string status)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(CardInstance.CardId)||Deck.ContainsKey(CardInstance.CardId))
            {
                status = $"User already Ownes Card {CardInstance.CardName}";
                PlayerMutex.ReleaseMutex();
                return false;              
            }
            else
            {
                Stack.Add(CardInstance.CardId, CardInstance);
                status = "Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
        }
        public bool RemoveFromStack(int CardId, out string status)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(CardId))
            {
                Stack.Remove(CardId);
                status = $"Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
            else
            {
                status = $"Card not in Stack";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool MoveFromDeckToStack(int CardId, out string status)
        {
            PlayerMutex.WaitOne();
            if (Deck.ContainsKey(CardId))
            {
                if (Stack.ContainsKey(CardId))
                {
                    status = $"Card Already in Stack";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                else
                {
                    Stack.Add(CardId, Deck[CardId]);
                    Deck.Remove(CardId);
                    status = $"Succes";
                    PlayerMutex.ReleaseMutex();
                    return true;
                }
            }
            else
            {
                status = $"Card not in Deck";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool MoveFromStackToDeck(int CardId, out string status)
        {
            PlayerMutex.WaitOne();
            if (Deck.Count >= 5)
            {
                status = $"Deck is Full";
                PlayerMutex.ReleaseMutex();
                return false;
            }
            if (Stack.ContainsKey(CardId))
            {
                if (Deck.ContainsKey(CardId))
                {
                    status = $"Card Already in Deck";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                else
                {
                    Deck.Add(CardId, Stack[CardId]);
                    Stack.Remove(CardId);
                    status = $"Success";
                    PlayerMutex.ReleaseMutex();
                    return true;
                }
            }
            else
            {
                status = $"Card not in Stack";
                PlayerMutex.ReleaseMutex();
                return false;
            }
        }
        public bool UserAttemptsCardPurchase(Card BuyThisCard, int Price,out string status)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(BuyThisCard.CardId) || Deck.ContainsKey(BuyThisCard.CardId))
            {
                status = $"Card already owned";
                PlayerMutex.ReleaseMutex();
                return false;
            }
            else
            {
                if (this.Coins - Price < 0)
                {
                    status = $"Not enough Coins";
                    PlayerMutex.ReleaseMutex();
                    return false;
                }
                Stack.Add(BuyThisCard.CardId, BuyThisCard);
                this.Coins -= Price;
                status = $"Success";
                PlayerMutex.ReleaseMutex();
                return true;
            }
        }
    }
}
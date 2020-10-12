using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MCTG.Cards;

namespace MCTG.Player
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
            this.Deck = new Dictionary<int, Card>(Deck);
            this.Coins = Coins;
            this.Elo = Elo;
            this.Win = Win;
            this.Lose = Lose;
            this.Tie = Tie;
            this.PlayerName = PlayerName;
            this.PlayerMutex = new Mutex();
        }
        public void IncreaseWin()
        {
            PlayerMutex.WaitOne();
            Win++;
            Elo += 5;
            Coins++;
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseLose()
        {
            PlayerMutex.WaitOne();
            Lose++;
            Elo -= 3;
            PlayerMutex.ReleaseMutex();
        }
        public void IncreaseTie()
        {
            PlayerMutex.WaitOne();
            Tie++;
            Elo -= 1;
            PlayerMutex.ReleaseMutex();
        }
        public DeckAndStackStatus AddToStack(Card CardInstance)
        {
            PlayerMutex.WaitOne();
            if (!Stack.ContainsKey(CardInstance.CardId))
            {
                Stack.Add(CardInstance.CardId, CardInstance);
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.Success;
            }
            else
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.CardAlreadyInStack;
            }
        }
        public DeckAndStackStatus RemoveFromStack(int CardId)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(CardId))
            {
                Stack.Remove(CardId);
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.Success;
            }
            else
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.CardNotInStack;
            }
        }
        public DeckAndStackStatus MoveFromDeckToStack(int CardId)
        {
            PlayerMutex.WaitOne();
            if (Deck.ContainsKey(CardId))
            {
                if (Stack.ContainsKey(CardId))
                {
                    PlayerMutex.ReleaseMutex();
                    return DeckAndStackStatus.CardAlreadyInStack;
                }
                else
                {
                    Stack.Add(CardId, Deck[CardId]);
                    Deck.Remove(CardId);
                    PlayerMutex.ReleaseMutex();
                    return DeckAndStackStatus.Success;
                }
            }
            else
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.CardNotInDeck;
            }
        }
        public DeckAndStackStatus MoveFromStackToDeck(int CardId)
        {
            PlayerMutex.WaitOne();
            if (Deck.Count >= 5)
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.DeckIsFull;
            }
            if (Stack.ContainsKey(CardId))
            {
                if (Deck.ContainsKey(CardId))
                {
                    PlayerMutex.ReleaseMutex();
                    return DeckAndStackStatus.CardAlreadyInDeck;
                }
                else
                {
                    Deck.Add(CardId, Stack[CardId]);
                    Stack.Remove(CardId);
                    PlayerMutex.ReleaseMutex();
                    return DeckAndStackStatus.Success;
                }
            }
            else
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.CardNotInStack;
            }
        }
        public DeckAndStackStatus UserAttemptsCardPurchase(Card BuyThisCard, int Price)
        {
            PlayerMutex.WaitOne();
            if (Stack.ContainsKey(BuyThisCard.CardId) || Deck.ContainsKey(BuyThisCard.CardId))
            {
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.UserAlreadyOwnsCard;
            }
            else
            {
                if (this.Coins - Price < 0)
                {
                    PlayerMutex.ReleaseMutex();
                    return DeckAndStackStatus.NotEnoughCoins;
                }
                Stack.Add(BuyThisCard.CardId, BuyThisCard);
                this.Coins -= Price;
                PlayerMutex.ReleaseMutex();
                return DeckAndStackStatus.Success;
            }
        }
    }
}
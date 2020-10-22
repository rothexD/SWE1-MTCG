using NUnit.Framework;
using MCTG.Players;
using MCTG.Cards;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace NunitTests
{
    public class PlayerTests
    {
        public Card Ork;
        public Card Wizzard;
        public Card WaterSpell;
        public Card Goblin;
        public Card Dragon;
        public Card knight;
        public Card NormalSpell;
        [OneTimeSetUp]
        public void Construct()
        {
            Ork = new Ork(10, "hans", 1, Card.CardelEmentEnum.normal);
            Wizzard = new Wizzard(10, "Test", 2, Card.CardelEmentEnum.fire);
            WaterSpell = new WaterSpell(10, "spell", 3, Card.CardelEmentEnum.water);
            Goblin = new Goblin(10, "hans", 4, Card.CardelEmentEnum.normal);
            Dragon = new Dragon(10, "Test", 5, Card.CardelEmentEnum.fire);
            knight = new Knight(10, "Test", 6, Card.CardelEmentEnum.fire);
            NormalSpell = new WaterSpell(10, "spell", 7, Card.CardelEmentEnum.normal);
        }
        [Test]
        public void testConstructorToManyInDeck_exception()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();

            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);
            Deck.Add(knight.CardId, knight);
            
            Assert.Throws(typeof(ArgumentException) , delegate { new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck); });
            Assert.Throws(typeof(ArgumentException), delegate { new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck); });
        }
        public void testConstructorDuplicateDeckStack_exception()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();

            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);

            Stack.Add(Ork.CardId, Ork);

            Assert.Throws(typeof(ArgumentException), delegate { new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck); });
        }
        [Test]
        public void TestAddToStack_Success()
        {
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);

            int Stacksize = User.Stack.Count;

            Assert.IsTrue(User.AddToStack(Ork));
            Assert.AreEqual(Stacksize+1, User.Stack.Count);
        }
        [Test]
        public void TestAddToStack_UserHasCardInStackOrDeck()
        {
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);
            int Stacksize;

            User.AddToStack(Ork);
            Stacksize = User.Stack.Count;

            Assert.IsFalse(User.AddToStack(Ork));
            Assert.AreEqual(Stacksize, User.Stack.Count);
        }
        [Test]
        public void TestRemoveFromStack_Success()
        {
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);

            User.AddToStack(Ork);
            int Stacksize = User.Stack.Count;


            Assert.IsTrue(User.RemoveFromStack(Ork.CardId));
            Assert.AreEqual(Stacksize-1, User.Stack.Count);
        }
        [Test]
        public void TestRemoveFromStack_Failure()
        {
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);
            int Stacksize;

            Stacksize = User.Stack.Count;

            Assert.IsFalse(User.RemoveFromStack(Ork.CardId));
            Assert.AreEqual(Stacksize, User.Stack.Count);
        }
        [Test]
        public void TestMoveFromDeckToStack_Success()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            int DeckSize;
            int StackSize;
            //-------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            DeckSize = User.Deck.Count;
            StackSize = User.Stack.Count;
            //--------------------------------------------------------------------

            Assert.IsTrue(User.MoveFromDeckToStack(Ork.CardId));
            Assert.AreEqual(DeckSize-1, User.Deck.Count);
            Assert.AreEqual(StackSize+1, User.Stack.Count);
        }
        [Test]
        public void TestMoveFromDeckToStack_Failure()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            int DeckSize;
            int StackSize;
            //----------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            DeckSize = User.Deck.Count;
            StackSize = User.Stack.Count;
            //--------------------------------------------------------------------
            Assert.IsFalse(User.MoveFromDeckToStack(-1));
            Assert.AreEqual(DeckSize, User.Deck.Count);
            Assert.AreEqual(StackSize, User.Stack.Count);
        }
        //stop here
        [Test]
        public void TestMoveFromStackToDeck_Success()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            Player User2;
            int StackSize;
            int DeckSize;
            //----------------------------------------------------------------------------------
            Stack.Add(knight.CardId, knight);           
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            StackSize = User.Stack.Count;
            DeckSize = User.Deck.Count;
            //--------------------------------------------------------------------------------

            Assert.IsTrue(User.MoveFromStackToDeck(knight.CardId));
            Assert.AreEqual(StackSize-1, User.Stack.Count);
            Assert.AreEqual(DeckSize+1, User.Deck.Count);
        }
        [Test]
        public void TestMoveFromStackToDeck_DeckIsFull()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            Player User2;
            int StackSize;
            int DeckSize;
            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);
            Stack.Add(knight.CardId, knight);
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            StackSize = User.Stack.Count;
            DeckSize = User.Deck.Count;
            //--------------------------------------------------------------------------------

            Assert.IsFalse(User.MoveFromStackToDeck(Wizzard.CardId));
            Assert.AreEqual(StackSize, User.Stack.Count);
            Assert.AreEqual(DeckSize, User.Deck.Count);
            Assert.AreEqual(5, User.Deck.Count);
        }
        [Test]
        public void TestMoveFromStackToDeck_FailureCardNotInStack()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            Player User2;
            int StackSize;
            int DeckSize;
            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Stack.Add(NormalSpell.CardId, NormalSpell);
            Stack.Add(knight.CardId, knight);
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            StackSize = User.Stack.Count;
            DeckSize = User.Deck.Count;
            //--------------------------------------------------------------------------------
            Assert.IsFalse(User.MoveFromStackToDeck(-1));
            Assert.AreEqual(StackSize, User.Stack.Count);
            Assert.AreEqual(DeckSize, User.Deck.Count);
        }
        [Test]
        public void TestCardBuy_Success()
        {
            Player User;
            int StackSize;
            int DeckSize;
            int Coins;

            User = new Player("Lukas", 0, 0, 0, 20, 100);
            StackSize = User.Stack.Count;
            Coins = User.Coins;
            DeckSize = User.Deck.Count;

            Assert.IsTrue(User.UserAttemptsCardPurchase(Ork, 20));
            Assert.AreEqual(StackSize + 1, User.Stack.Count);
            Assert.AreEqual(Coins-20, User.Coins);
            Assert.AreEqual(DeckSize, User.Deck.Count);
        }
        [Test]
        public void TestCardBuy_CardAlreadyOwned()
        {
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            Player User;
            int DeckSize;
            int StackSize;
            int Coins;

            Deck.Add(Ork.CardId, Ork);
            User = new Player("Lukas", 0, 0, 0, 20, 100,Stack,Deck);
            StackSize = User.Stack.Count;
            Coins = User.Coins;
            DeckSize = User.Deck.Count;

            Assert.IsFalse(User.UserAttemptsCardPurchase(Ork, 20));
            Assert.AreEqual(StackSize, User.Stack.Count);
            Assert.AreEqual(Coins, User.Coins);
            Assert.AreEqual(DeckSize, User.Deck.Count);
        }
        [Test]
        public void TestCardBuy_NotEnoughCoins()
        {
            Player User;
            int StackSize;
            int DeckSize;
            int Coins;

            User = new Player("Lukas", 0, 0, 0, 10, 100);
            StackSize = User.Stack.Count;
            Coins = User.Coins;
            DeckSize = User.Deck.Count;

            Assert.IsFalse(User.UserAttemptsCardPurchase(WaterSpell, 20));
            Assert.AreEqual(StackSize, User.Stack.Count);
            Assert.AreEqual(Coins, User.Coins);
            Assert.AreEqual(DeckSize, User.Deck.Count);
        }

    }
}
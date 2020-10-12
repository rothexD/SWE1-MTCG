using NUnit.Framework;
using MCTG.Player;
using MCTG.Cards;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using System;
using System.Collections.Generic;
using System.Text;

namespace NunitTests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
        [Test]
        public void TestAddToStack()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);

            DeckAndStackStatus Success = User.AddToStack(Ork);
            DeckAndStackStatus FailureCardAlreadyInStack = User.AddToStack(Ork);

            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.CardAlreadyInStack, FailureCardAlreadyInStack);
        }
        [Test]
        public void TestRemoveFromStack()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);
            User.AddToStack(Ork);

            DeckAndStackStatus Success = User.RemoveFromStack(Ork.CardId);
            DeckAndStackStatus FailureCardNotInStack = User.RemoveFromStack(Ork.CardId);


            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.CardNotInStack, FailureCardNotInStack);
        }
        [Test]
        public void TestMoveFromDeckToStack()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card Wizzard = new Wizzard(10, "Test", 6, Card.CardelEmentEnum.fire);
            Card WaterSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.water);
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();
            
            //-------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);

            DeckAndStackStatus Success = User.MoveFromDeckToStack(Ork.CardId);
            DeckAndStackStatus FailureCardNotInDeck = User.MoveFromDeckToStack(-1);
            Console.WriteLine(Deck.Count + " " + User.Deck.Count);
            
            Player User2 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            DeckAndStackStatus FailureCardAlreadyInStack = User2.MoveFromDeckToStack(Ork.CardId);
            //--------------------------------------------------------------------

            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.CardNotInDeck, FailureCardNotInDeck);
            Assert.AreEqual(DeckAndStackStatus.CardAlreadyInStack, FailureCardAlreadyInStack);
        }
        [Test]
        public void TestMoveFromStackToDeck()
        {
            Card Ork = new Ork(10, "hans", 1, Card.CardelEmentEnum.normal);
            Card Wizzard = new Wizzard(10, "Test", 2, Card.CardelEmentEnum.fire);
            Card WaterSpell = new WaterSpell(10, "spell", 3, Card.CardelEmentEnum.water);
            Card Goblin = new Goblin(10, "hans", 4, Card.CardelEmentEnum.normal);
            Card Dragon = new Dragon(10, "Test", 5, Card.CardelEmentEnum.fire);
            Card knight = new Knight(10, "Test", 6, Card.CardelEmentEnum.fire);
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();


            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);

            Stack.Add(Wizzard.CardId,Wizzard);
            Stack.Add(knight.CardId, knight);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            Console.WriteLine(User.Stack.Count+" "+ User.Deck.Count);

            DeckAndStackStatus DeckFull = User.MoveFromStackToDeck(Wizzard.CardId);

            Deck.Remove(Goblin.CardId);

            Player User2 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            Console.WriteLine(User2.Deck.Count);
            DeckAndStackStatus FailureCardNotInStack = User2.MoveFromStackToDeck(-1);
            DeckAndStackStatus CardAlreadyInDeck = User2.MoveFromStackToDeck(Wizzard.CardId);
            DeckAndStackStatus Success = User2.MoveFromStackToDeck(knight.CardId);
            //--------------------------------------------------------------------------------

            Assert.AreEqual(DeckAndStackStatus.CardAlreadyInDeck, CardAlreadyInDeck);
            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.DeckIsFull, DeckFull);    
            Assert.AreEqual(DeckAndStackStatus.CardNotInStack, FailureCardNotInStack);
        }
        [Test]
        public void TestCardBuy()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card WaterSpell = new WaterSpell(10, "spell", 3, Card.CardelEmentEnum.water);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);

            DeckAndStackStatus Success = User.UserAttemptsCardPurchase(Ork,20);
            DeckAndStackStatus CardAlreadyOwned = User.UserAttemptsCardPurchase(Ork,20);
            DeckAndStackStatus NotEnoughCoins = User.UserAttemptsCardPurchase(WaterSpell, 20);


            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.UserAlreadyOwnsCard, CardAlreadyOwned);
            Assert.AreEqual(DeckAndStackStatus.NotEnoughCoins, NotEnoughCoins);
        }       

    }
}
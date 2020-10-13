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
        [Test]
        public void testConstructor()
        {
            bool FirstTryCatch = false;
            bool SecondTryCatch = false;
            Card Ork = new Ork(10, "hans", 1, Card.CardelEmentEnum.normal);
            Card Wizzard = new Wizzard(10, "Test", 2, Card.CardelEmentEnum.fire);
            Card WaterSpell = new WaterSpell(10, "spell", 3, Card.CardelEmentEnum.water);
            Card Goblin = new Goblin(10, "hans", 4, Card.CardelEmentEnum.normal);
            Card Dragon = new Dragon(10, "Test", 5, Card.CardelEmentEnum.fire);
            Card knight = new Knight(10, "Test", 6, Card.CardelEmentEnum.fire);
            Card NormalSpell = new WaterSpell(10, "spell", 7, Card.CardelEmentEnum.normal);
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();

            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);
            Deck.Add(knight.CardId, knight);

            try
            {
                Player User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            }
            catch (ArgumentException e)
            {
                FirstTryCatch = true;
                Assert.Pass(e.Message);
            }
            Stack.Add(Ork.CardId, Ork);
            try
            {
                Player User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            }
            catch (ArgumentException e)
            {
                SecondTryCatch = true;
                Assert.Pass(e.Message);
            }
            if (FirstTryCatch && SecondTryCatch)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test]
        public void TestAddToStack()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100);

            DeckAndStackStatus Success = User.AddToStack(Ork);
            DeckAndStackStatus UserAlreadyHasCardInStackOrDeck = User.AddToStack(Ork);

            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.UserAlreadyOwnsCard, UserAlreadyHasCardInStackOrDeck);
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
            //--------------------------------------------------------------------

            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.CardNotInDeck, FailureCardNotInDeck);
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
            Card NormalSpell = new WaterSpell(10, "spell", 7, Card.CardelEmentEnum.normal);
            Dictionary<int, Card> Deck = new Dictionary<int, Card>();
            Dictionary<int, Card> Stack = new Dictionary<int, Card>();


            //----------------------------------------------------------------------------------
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Goblin.CardId, Goblin);
            Deck.Add(Dragon.CardId, Dragon);

            Stack.Add(NormalSpell.CardId, NormalSpell);
            Stack.Add(knight.CardId, knight);
            Player User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            Console.WriteLine(User.Stack.Count + " " + User.Deck.Count);

            DeckAndStackStatus DeckFull = User.MoveFromStackToDeck(Wizzard.CardId);

            Deck.Remove(Wizzard.CardId);

            Player User2 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            Console.WriteLine(User2.Deck.Count);
            DeckAndStackStatus FailureCardNotInStack = User2.MoveFromStackToDeck(-1);
            DeckAndStackStatus Success = User2.MoveFromStackToDeck(knight.CardId);
            //--------------------------------------------------------------------------------

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

            DeckAndStackStatus Success = User.UserAttemptsCardPurchase(Ork, 20);
            DeckAndStackStatus CardAlreadyOwned = User.UserAttemptsCardPurchase(Ork, 20);
            DeckAndStackStatus NotEnoughCoins = User.UserAttemptsCardPurchase(WaterSpell, 20);


            Assert.AreEqual(DeckAndStackStatus.Success, Success);
            Assert.AreEqual(DeckAndStackStatus.UserAlreadyOwnsCard, CardAlreadyOwned);
            Assert.AreEqual(DeckAndStackStatus.NotEnoughCoins, NotEnoughCoins);
        }

    }
}
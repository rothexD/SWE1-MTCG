using NUnit.Framework;
using MCTG.Players;
using MCTG.Cards;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using System;
using System.Collections.Generic;
using System.Text;
using MCTG.FightHandle;

namespace NunitTests
{
    class TestFightHandler
    {
        [Test]
        public void testFight()
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
        
            Deck.Add(Ork.CardId, Ork);
            Deck.Add(Wizzard.CardId, Wizzard);
            Deck.Add(WaterSpell.CardId, WaterSpell);
            Deck.Add(Dragon.CardId, Dragon);
            Deck.Add(knight.CardId, knight);

            Player User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, Deck);
            Player User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, Deck);

            FightHandler Testfight = new FightHandler(User1, User2);
            Testfight.Fight();
            Console.Write(Testfight.Battlelog);
            Assert.Pass();        
        }
    }
}

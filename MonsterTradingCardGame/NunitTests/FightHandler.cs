using NUnit.Framework;
using MCTG.Players;
using MCTG.Cards;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using System;
using System.Collections.Generic;
using System.Text;
using MCTG.FightHandlers;
using Moq;

namespace NunitTests
{
    class TestFightManager
    {
        public Card Ork;
        public Card Wizzard;
        public Card WaterSpell;
        public Card Goblin;
        public Card Dragon;
        public Card knight;
        public Card NormalSpell;
        public Card FireElve;
        public Card FireSpell;
        public Card Kraken;
        public Dictionary<string, Card> DeckUser1Full;
        public Dictionary<string, Card> DeckUser2Full;
        public Dictionary<string, Card> Stack;
        public List<Card> ExampleCombatListPlayer1;
        public List<Card> ExampleCombatListPlayer2;

        [OneTimeSetUp]
        public void Construct()
        {
            Ork = new Ork(10, "hans", "1", Card.CardelEmentEnum.normal);
            Wizzard = new Wizzard(10, "Test", "2", Card.CardelEmentEnum.fire);
            WaterSpell = new WaterSpell(10, "spell", "3", Card.CardelEmentEnum.water);
            Goblin = new Goblin(10, "hans", "4", Card.CardelEmentEnum.normal);
            Dragon = new Dragon(10, "Test", "5", Card.CardelEmentEnum.fire);
            knight = new Knight(10, "Test", "6", Card.CardelEmentEnum.fire);
            NormalSpell = new WaterSpell(10, "spell", "7", Card.CardelEmentEnum.normal);
            FireElve = new FireElve(10, "hans", "5", Card.CardelEmentEnum.normal);
            FireSpell = new WaterSpell(10, "spell", "2", Card.CardelEmentEnum.fire);
            Kraken = new Kraken(10, "spell", "2", Card.CardelEmentEnum.fire);
        }
        [OneTimeSetUp]
        public void ConstructDecks()
        {
            DeckUser1Full = new Dictionary<string, Card>();
            DeckUser2Full = new Dictionary<string, Card>();
            Stack = new Dictionary<string, Card>();
            ExampleCombatListPlayer1 = new List<Card>();
            ExampleCombatListPlayer2 = new List<Card>();

            DeckUser1Full.Add(Ork.CardId, Ork);
            DeckUser1Full.Add(Wizzard.CardId, Wizzard);
            DeckUser1Full.Add(WaterSpell.CardId, WaterSpell);
            DeckUser1Full.Add(Dragon.CardId, Dragon);
            DeckUser1Full.Add(knight.CardId, knight);

            DeckUser2Full.Add(Ork.CardId, Ork);
            DeckUser2Full.Add(Wizzard.CardId, Wizzard);
            DeckUser2Full.Add(WaterSpell.CardId, WaterSpell);
            DeckUser2Full.Add(Dragon.CardId, Dragon);
            DeckUser2Full.Add(knight.CardId, knight);
        }
        private void FillListwith5Cards(ref List<Card>CardList,Card CardObject)
        {
            CardList.Clear();
            CardList.Add(CardObject);
            CardList.Add(CardObject);
            CardList.Add(CardObject);
            CardList.Add(CardObject);
            CardList.Add(CardObject);
        }
        public void RefreshExampleCombatLists()
        {
            ExampleCombatListPlayer1.Clear();
            ExampleCombatListPlayer2.Clear();
            foreach (var item in DeckUser1Full)
            {
                ExampleCombatListPlayer1.Add(item.Value);
            }
            foreach (var item in DeckUser2Full)
            {
                ExampleCombatListPlayer2.Add(item.Value);
            }
        }
        [Test]
        public void TestFightCombatLog()
        {
            Player User1;
            Player User2;

            User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);

            FightHandler Testfight = new FightHandler(User1, User2);
            Testfight.Fight();
            Console.Write(Testfight.Battlelog);
            Assert.Pass();        
        }
        [Test]
        public void FullFightWinnerPlayer1()
        {
            Player User1;
            Player User2;
            TestFightHandler TestFight;
            int EloUser1;
            int EloUser2;


            User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);

            TestFight = new TestFightHandler(User1, User2);
            FightHandler.BattleStatus StatusOfFight = TestFight.FullFightWinnerPlayer1_RiggedDice();
            EloUser1 = User1.Elo;
            EloUser2 = User2.Elo;

            Assert.AreEqual(FightHandler.BattleStatus.Player1Winner, StatusOfFight);
            Assert.AreEqual(105, EloUser1);
            Assert.AreEqual(97, EloUser2);
        }
        [Test]
        public void FullFightWinnerPlayer2()
        {
            Player User1;
            Player User2;
            TestFightHandler TestFight;
            int EloUser1;
            int EloUser2;


            User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);

            TestFight = new TestFightHandler(User1, User2);
            FightHandler.BattleStatus StatusOfFight = TestFight.FullFightWinnerPlayer2_RiggedDice();
            EloUser1 = User1.Elo;
            EloUser2 = User2.Elo;

            Assert.AreEqual(FightHandler.BattleStatus.Player2Winner, StatusOfFight);
            Assert.AreEqual(105, EloUser2);
            Assert.AreEqual(97, EloUser1);
        }
        /*
        [Test]
        public void FindTie()
        {
            Player User1;
            Player User2;
            TestFightHandler TestFight;

            User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);

            TestFight = new TestFightHandler(User1, User2);
            int TieDiceValue = TestFight.FindTie();
            Assert.AreEqual(-1, TieDiceValue);
        }
        [Test]
        public void FullFightTie()
        {
            Player User1;
            Player User2;
            TestFightHandler TestFight;
            int EloUser1;
            int EloUser2;


            User1 = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);

            TestFight = new TestFightHandler(User1, User2);
            FightHandler.BattleStatus StatusOfFight = TestFight.FullFightWinnerPlayer2_RiggedDice();
            EloUser1 = User1.Elo;
            EloUser2 = User2.Elo;

            Assert.AreEqual(FightHandler.BattleStatus.Tie, StatusOfFight);
            Assert.AreEqual(99, EloUser2);
            Assert.AreEqual(99, EloUser1);
        }*/
        [Test]
        public void TestShuffleDeck()
        {
            TestFightHandler TestFight;
            Player User;
            Player User2;
            Random RiggedDice;

            RiggedDice = new Random(1);
            RefreshExampleCombatLists();
            User = new Player("Lukas", 0, 0, 0, 20, 100,Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);
            TestFight = new TestFightHandler(User, User2);
            TestFight.TestShuffleRiggedDice(RiggedDice);
            ExampleShuffle();

            CollectionAssert.AreEqual(ExampleCombatListPlayer1, TestFight.ReturnTempDeck1_List());
            CollectionAssert.AreEqual(ExampleCombatListPlayer2, TestFight.ReturnTempDeck2_List());
         
        } 
        private void ExampleShuffle()
        {
            Random Dice = new Random(1);
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (ExampleCombatListPlayer1.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, ExampleCombatListPlayer1.Count - 1);
                Card Tempcard = ExampleCombatListPlayer1[ShuffleIndex];
                ExampleCombatListPlayer1.RemoveAt(ShuffleIndex);
                ExampleCombatListPlayer1.Add(Tempcard);
            }
            for (int i = 0; i < Dice.Next(0, 100); i++)
            {
                if (ExampleCombatListPlayer2.Count == 0)
                {
                    break;
                }
                int ShuffleIndex = Dice.Next(0, ExampleCombatListPlayer2.Count - 1);
                Card Tempcard = ExampleCombatListPlayer2[ShuffleIndex];
                ExampleCombatListPlayer2.RemoveAt(ShuffleIndex);
                ExampleCombatListPlayer2.Add(Tempcard);
            }
        }
        [Test]
        public void TestFightOneRoundAttackerWin()
        {
            TestFightHandler TestFight;
            Player User;
            Player User2;
            Random RiggedDice;
            List<Card> BeforePlayer1;
            List<Card> BeforePlayer2;
            List<Card> AfterPlayer1;
            List<Card> AfterPlayer2;

            BeforePlayer1 = new List<Card>();
            BeforePlayer2 = new List<Card>();

            FillListwith5Cards(ref BeforePlayer1, Ork);
            FillListwith5Cards(ref BeforePlayer2, WaterSpell);

            AfterPlayer1 = new List<Card>(BeforePlayer1);
            AfterPlayer2 = new List<Card>(BeforePlayer2);

            RiggedDice = new Random(1);
            RefreshExampleCombatLists();
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);
            TestFight = new TestFightHandler(User, User2);

            TestFight.TestFightOneRoundRiggedDice(RiggedDice,ref AfterPlayer1, ref AfterPlayer2);

            BeforePlayer1.Add(BeforePlayer2[0]);
            BeforePlayer2.RemoveAt(0);

            CollectionAssert.AreEqual(BeforePlayer1, AfterPlayer1);
            CollectionAssert.AreEqual(BeforePlayer2, AfterPlayer2);

        }
        [Test]
        public void TestFightOneRoundDefenderWin()
        {
            TestFightHandler TestFight;
            Player User;
            Player User2;
            Random RiggedDice;
            List<Card> BeforePlayer1;
            List<Card> BeforePlayer2;
            List<Card> AfterPlayer1;
            List<Card> AfterPlayer2;

            BeforePlayer1 = new List<Card>();
            BeforePlayer2 = new List<Card>();

            FillListwith5Cards(ref BeforePlayer1, Ork);
            FillListwith5Cards(ref BeforePlayer2, FireSpell);

            AfterPlayer1 = new List<Card>(BeforePlayer1);
            AfterPlayer2 = new List<Card>(BeforePlayer2);

            RiggedDice = new Random(1);
            RefreshExampleCombatLists();
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);
            TestFight = new TestFightHandler(User, User2);

            TestFight.TestFightOneRoundRiggedDice(RiggedDice, ref AfterPlayer1, ref AfterPlayer2);

            BeforePlayer2.Add(BeforePlayer1[0]);
            BeforePlayer1.RemoveAt(0);

            CollectionAssert.AreEqual(BeforePlayer1, AfterPlayer1);
            CollectionAssert.AreEqual(BeforePlayer2, AfterPlayer2);
        }
        [Test]
        public void TestFightOneRoundTiedDefenderWin()
        {
            TestFightHandler TestFight;
            Player User;
            Player User2;
            Random RiggedDice;
            List<Card> BeforePlayer1;
            List<Card> BeforePlayer2;
            List<Card> AfterPlayer1;
            List<Card> AfterPlayer2;

            BeforePlayer1 = new List<Card>();
            BeforePlayer2 = new List<Card>();

            FillListwith5Cards(ref BeforePlayer1, Ork);
            FillListwith5Cards(ref BeforePlayer2, Ork);

            AfterPlayer1 = new List<Card>(BeforePlayer1);
            AfterPlayer2 = new List<Card>(BeforePlayer2);

            RiggedDice = new Random(1);
            RefreshExampleCombatLists();
            User = new Player("Lukas", 0, 0, 0, 20, 100, Stack, DeckUser1Full);
            User2 = new Player("Lam", 0, 0, 0, 20, 100, Stack, DeckUser2Full);
            TestFight = new TestFightHandler(User, User2);

            TestFight.TestFightOneRoundRiggedDice(RiggedDice, ref AfterPlayer1, ref AfterPlayer2);

            BeforePlayer2.Add(BeforePlayer1[0]);
            BeforePlayer1.RemoveAt(0);

            CollectionAssert.AreEqual(BeforePlayer1, AfterPlayer1);
            CollectionAssert.AreEqual(BeforePlayer2, AfterPlayer2);
        }
    }
}

using System;
using System.Collections.Generic;
using MCTG.Cards;
using MCTG;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using NUnit.Framework;
using System.Reflection;
//https://stackoverflow.com/questions/33826500/how-to-run-a-nunit-test
namespace NunitTests
{
    public class TestCards{
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
            FireElve = new FireElve(10, "hans", 5, Card.CardelEmentEnum.normal);
            FireSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.fire);
            Kraken = new Kraken(10, "spell", 2, Card.CardelEmentEnum.fire);
        }
        [Test]
        public void TestOrkVsWizzard_0()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card Wizzard = new Wizzard(10, "Test", 5, Card.CardelEmentEnum.fire);
            int DamageVsWizzard;

            DamageVsWizzard = Ork.CalculateDamage(Wizzard);

            Assert.AreEqual(0, DamageVsWizzard);
        }
        public void TestOrkVsNonWizzard_10()
        {
            int DamageVsNonWizzard;

            DamageVsNonWizzard = Ork.CalculateDamage(Goblin);

            Assert.AreEqual(10, DamageVsNonWizzard);
        }
        [Test]
        public void TestDragonVsFireElve_0()
        {
            int DamageVsFireElve;

            DamageVsFireElve = Dragon.CalculateDamage(FireElve);

            Assert.AreEqual(0, DamageVsFireElve);
        }
        [Test]
        public void TestFireElve()
        {
            Assert.Pass();
        }
        [Test]
        public void TestGoblinVsDragon_0()
        {
            int DamageVsDragon;

            DamageVsDragon = Goblin.CalculateDamage(Dragon);

            Assert.AreEqual(0, DamageVsDragon);
        }
        [Test]
        public void TestKnightVSWater_0()
        {
            int DamageVsWater;

            DamageVsWater = knight.CalculateDamage(WaterSpell);

            Assert.AreEqual(0, DamageVsWater);
        }
        [Test]
        public void TestKrakenVsFire_0()
        {
            int KrakenVsFire;

            KrakenVsFire = FireSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, KrakenVsFire);
        }
        [Test]
        public void TestKrakenVSWater_0()
        {
            int KrakenVsWater;

            KrakenVsWater = WaterSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, KrakenVsWater);
        }
        [Test]
        public void TestKrakenVsNormal()
        {
            int KrakenVsNormal;

            KrakenVsNormal = NormalSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, KrakenVsNormal);
        }
        [Test]
        public void TestWizzard()
        {
            Assert.Pass();
        }
        //elemental
        [Test]
        public void TestElementalDamageCalculation_FireVsFire_10()
        {
            int FireVsFire;

            FireVsFire = FireSpell.CalculateDamage(FireSpell);

            Assert.AreEqual(10, FireVsFire);
        }
        [Test]
        public void TestElementalDamageCalculation_FireVsWater_5()
        {
            int FireVsWater;

            FireVsWater = FireSpell.CalculateDamage(WaterSpell);

            Assert.AreEqual(5, FireVsWater);
        }
        [Test]
        public void TestElementalDamageCalculation_FireVsNormal_20()
        {
            int FireVsNormal;

            FireVsNormal = FireSpell.CalculateDamage(NormalSpell);

            Assert.AreEqual(20, FireVsNormal);
        }
        [Test]
        public void TestElementalDamageCalculation_WaterVsWater_10()
        {
            int WaterVsWater;

            WaterVsWater = WaterSpell.CalculateDamage(WaterSpell);

            Assert.AreEqual(10, WaterVsWater);
        }
        [Test]
        public void TestElementalDamageCalculation_WaterVsNormal_5()
        {
            int WaterVsNormal;

            WaterVsNormal = WaterSpell.CalculateDamage(NormalSpell);

            Assert.AreEqual(5, WaterVsNormal);
        }
        [Test]
        public void TestElementalDamageCalculation_WaterVsFire_20()
        {
            int WaterVsFire;

            WaterVsFire = WaterSpell.CalculateDamage(FireSpell);

            Assert.AreEqual(20, WaterVsFire);
        }
        public void TestElementalDamageCalculation_NormalVsNormal_10()
        {
            int NormalVsNormal;

            NormalVsNormal = NormalSpell.CalculateDamage(NormalSpell);

            Assert.AreEqual(10, NormalVsNormal);
        }
        [Test]
        public void TestElementalDamageCalculation_NormalVsWater_20()
        {
            int NormalVsWater;

            NormalVsWater = NormalSpell.CalculateDamage(WaterSpell);

            Assert.AreEqual(20, NormalVsWater);
        }
        [Test]
        public void TestElementalDamageCalculation_NormalVsFire_5()
        {
            int NormalVsFIre;

            NormalVsFIre = NormalSpell.CalculateDamage(FireSpell);

            Assert.AreEqual(5, NormalVsFIre);
        }
    }
}


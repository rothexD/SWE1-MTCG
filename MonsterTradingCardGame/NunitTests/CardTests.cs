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
            Ork = new Ork(10, "hans", "1", Card.CardelEmentEnum.normal);
            Wizzard = new Wizzard(10, "Test", "1", Card.CardelEmentEnum.fire);
            WaterSpell = new WaterSpell(10, "spell", "1", Card.CardelEmentEnum.water);
            Goblin = new Goblin(10, "hans", "1", Card.CardelEmentEnum.normal);
            Dragon = new Dragon(10, "Test", "1", Card.CardelEmentEnum.fire);
            knight = new Knight(10, "Test", "1", Card.CardelEmentEnum.fire);
            NormalSpell = new NormalSpell(10, "spell", "1", Card.CardelEmentEnum.normal);
            FireElve = new FireElve(10, "hans", "1", Card.CardelEmentEnum.normal);
            FireSpell = new FireSpell(10, "spell", "1", Card.CardelEmentEnum.fire);
            Kraken = new Kraken(10, "spell", "1", Card.CardelEmentEnum.fire);
        }
        [Test]
        public void TestOrkVsWizzard_0()
        {
            int DamageVsWizzard;

            DamageVsWizzard = Ork.CalculateDamage(Wizzard);

            Assert.AreEqual(0, DamageVsWizzard);
        }
        [Test]
        public void TestOrkVsNonWizard_10()
        {
            int DamageVskraken;

            DamageVskraken = Ork.CalculateDamage(Kraken);

            Assert.AreEqual(10, DamageVskraken);
        }
        [Test]
        public void TestOrkVsSpell_5()
        {
            int DamageVsFirespell;

            DamageVsFirespell = Ork.CalculateDamage(FireSpell);

            Assert.AreEqual(5, DamageVsFirespell);
        }
        [Test]
        public void TestDragonVsFireElve_0()
        {
            int DamageVsFireElve;

            DamageVsFireElve = Dragon.CalculateDamage(FireElve);

            Assert.AreEqual(0, DamageVsFireElve);
        }
        [Test]
        public void TestDragonVsNonFireElve_10()
        {
            int DamageVsNonFireElve;

            DamageVsNonFireElve = Dragon.CalculateDamage(Ork);

            Assert.AreEqual(10, DamageVsNonFireElve);
        }
        [Test]
        public void TestDragonVsSpell_10()
        {
            int DamageVsSpell;

            DamageVsSpell = Dragon.CalculateDamage(FireSpell);

            Assert.AreEqual(10, DamageVsSpell);
        }
        [Test]
        public void TestFireElveVsMonster_10()
        {
            int DamageVsMonster;
            DamageVsMonster = FireElve.CalculateDamage(Ork);
            Assert.AreEqual(10, DamageVsMonster);
        }
        [Test]
        public void TestFireElveVsSpell_5()
        {
            int DamageVsSpell;
            DamageVsSpell = FireElve.CalculateDamage(FireSpell);
            Assert.AreEqual(5, DamageVsSpell);
        }
        [Test]
        public void TestGoblinVsDragon_0()
        {
            int DamageVsDragon;

            DamageVsDragon = Goblin.CalculateDamage(Dragon);

            Assert.AreEqual(0, DamageVsDragon);
        }
        [Test]
        public void TestGoblinVsNonDragon_10()
        {
            int DamageVsNonDragon;

            DamageVsNonDragon = Goblin.CalculateDamage(Ork);

            Assert.AreEqual(10, DamageVsNonDragon);
        }
        [Test]
        public void TestGoblinVsSpell_20()
        {
            int DamageVsDragon;

            DamageVsDragon = Goblin.CalculateDamage(WaterSpell);

            Assert.AreEqual(20, DamageVsDragon);
        }
        [Test]
        public void TestKnightVSWaterSpell_0()
        {
            int DamageVsWater;

            DamageVsWater = knight.CalculateDamage(WaterSpell);

            Assert.AreEqual(0, DamageVsWater);
        }
        [Test]
        public void TestKnightVSNonWaterSpell_10()
        {
            int DamageVsNonWater;

            DamageVsNonWater = knight.CalculateDamage(FireSpell);

            Assert.AreEqual(10, DamageVsNonWater);
        }
        [Test]
        public void TestKnightVSMonster_10()
        {
            int DamageVsMonster;

            DamageVsMonster = knight.CalculateDamage(Ork);

            Assert.AreEqual(10, DamageVsMonster);
        }

        [Test]
        public void TestKrakenVsSpell_10()
        {
            int DamageVsSpell;
            DamageVsSpell = Kraken.CalculateDamage(FireSpell);
            Assert.AreEqual(10, DamageVsSpell);
        }
        [Test]
        public void TestKrakenVsMonster_10()
        {
            int DamageVsMonster;

            DamageVsMonster = Kraken.CalculateDamage(Ork);

            Assert.AreEqual(10, DamageVsMonster);
        }
        [Test]
        public void TestWizzardVsMonster_10()
        {
            int DamageVsMonster;
            DamageVsMonster = Wizzard.CalculateDamage(Ork);
            Assert.AreEqual(10, DamageVsMonster);
        }
        [Test]
        public void TestWizzardVsSpell_10()
        {
            int DamageVsSpell;
            DamageVsSpell = Wizzard.CalculateDamage(FireSpell);
            Assert.AreEqual(10, DamageVsSpell);
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


        //Spells
        [Test]
        public void TestFireSpellVsKraken_0()
        {
            int FireVSKraken;

            FireVSKraken = FireSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, FireVSKraken);
        }
        [Test]
        public void TestWaterSpellVSKraken_0()
        {
            int WaterVsKraken;

            WaterVsKraken = WaterSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, WaterVsKraken);
        }
        [Test]
        public void TestNormalSpellVsKraken()
        {
            int NormalVsKraken;

            NormalVsKraken = NormalSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, NormalVsKraken);
        }
    }
}


using System;
using System.Collections.Generic;
using MCTG.Cards;
using MCTG;
using MCTG.Cards.MonsterCards;
using MCTG.Cards.SpellCards;
using NUnit.Framework;
using System.Reflection;
//https://stackoverflow.com/questions/33826500/how-to-run-a-nunit-test
namespace MonsterTradingCardGame.tests
{
    public class TestCards{
        [Test]
        public void OrkTest()
        {
            Card Ork = new Ork(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card Wizzard = new Wizzard(10, "Test", 5, Card.CardelEmentEnum.fire);
            Card WaterSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.water);

            int DamageVsWizzard = Ork.CalculateDamage(Wizzard);
            int DamageVsWaterSpell = Ork.CalculateDamage(WaterSpell);

            Assert.AreEqual(0, DamageVsWizzard);
            Assert.AreEqual(20, DamageVsWaterSpell);
        }
        [Test]
        public void TestDragon()
        {
            Card FireElve = new FireElve(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card Dragon = new Dragon(10, "Test", 5, Card.CardelEmentEnum.fire);

            int DamageVsWizzard = Dragon.CalculateDamage(FireElve);

            Assert.AreEqual(0, DamageVsWizzard);
        }
        [Test]
        public void TestFireElve()
        {
            Assert.Pass();
        }
        [Test]
        public void TestGoblin()
        {
            Card Goblin = new Goblin(10, "hans", 5, Card.CardelEmentEnum.normal);
            Card Dragon = new Dragon(10, "Test", 5, Card.CardelEmentEnum.fire);

            int DamageVsDragon = Goblin.CalculateDamage(Dragon);

            Assert.AreEqual(0, DamageVsDragon);
        }
        [Test]
        public void TestKnight()
        {
            Card knight = new Knight(10, "Test", 5, Card.CardelEmentEnum.fire);
            Card WaterSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.water);
            Card NormalSpell = new NormalSpell(10, "spell", 2, Card.CardelEmentEnum.normal);

            int DamageVsWater = knight.CalculateDamage(WaterSpell);
            int DamageVsNormal = knight.CalculateDamage(NormalSpell);

            Assert.AreEqual(0, DamageVsWater);
            Assert.AreEqual(20, DamageVsNormal);
        }
        [Test]
        public void TestKraken()
        {
            Card WaterSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.water);
            Card NormalSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.normal);
            Card FireSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.fire);
            Card Kraken = new Kraken(10, "spell", 2, Card.CardelEmentEnum.fire);


            int KrakenVsFire = FireSpell.CalculateDamage(Kraken);
            int KrakenVsWater = WaterSpell.CalculateDamage(Kraken);
            int KrakenVsNormal = NormalSpell.CalculateDamage(Kraken);

            Assert.AreEqual(0, KrakenVsFire);
            Assert.AreEqual(0, KrakenVsWater);
            Assert.AreEqual(0, KrakenVsNormal);
        }
        [Test]
        public void TestWizzard()
        {
            Assert.Pass();
        }
        [Test]
        public void TestElementalDamageCalculation()
        {
            Card WaterSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.water);
            Card NormalSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.normal);
            Card FireSpell = new WaterSpell(10, "spell", 2, Card.CardelEmentEnum.fire);
            
            int FireVsFire = FireSpell.CalculateDamage(FireSpell);
            int FireVsWater = FireSpell.CalculateDamage(WaterSpell);
            int FireVsNormal = FireSpell.CalculateDamage(NormalSpell);

            int WaterVsFire = WaterSpell.CalculateDamage(FireSpell);
            int WaterVsNormal = WaterSpell.CalculateDamage(NormalSpell);

            int NormalVsFire = NormalSpell.CalculateDamage(FireSpell);
            int NormalVsWater = NormalSpell.CalculateDamage(WaterSpell);


            Assert.AreEqual(10, FireVsFire);
            Assert.AreEqual(5, FireVsWater);
            Assert.AreEqual(20, FireVsNormal);
            Assert.AreEqual(20, WaterVsFire);
            Assert.AreEqual(5, WaterVsNormal);
            Assert.AreEqual(5, NormalVsFire);
            Assert.AreEqual(20, NormalVsWater);
        }
    }
}

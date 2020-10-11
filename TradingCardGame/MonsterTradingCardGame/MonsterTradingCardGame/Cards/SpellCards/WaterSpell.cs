using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Cards.SpellCards
{
    class WaterSpell : Card
    {
        WaterSpell(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "WaterSpell";
            this.CardStyle = "Spell";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            return Convert.ToInt32(this.BaseDamage * ElementMultiplikator(OpponentsCard));
        }
    }
}

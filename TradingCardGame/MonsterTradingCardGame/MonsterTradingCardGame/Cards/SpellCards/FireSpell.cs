using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Cards.SpellCards
{
    class FireSpell : Card
    {
        FireSpell(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "FireSpell";
            this.CardStyle = "Spell";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            return Convert.ToInt32(this.BaseDamage * ElementMultiplikator(OpponentsCard));
        }
    }
}

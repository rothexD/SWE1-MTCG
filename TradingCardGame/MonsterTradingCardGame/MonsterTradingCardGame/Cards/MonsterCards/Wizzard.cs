using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Cards.MonsterCards
{
    class Wizzard : Card
    {
        Wizzard(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "Wizzard";
            this.CardStyle = "Monster";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if(OpponentsCard.CardStyle== "Monster")
            {
                return this.BaseDamage;
            }
            else if (OpponentsCard.CardStyle == "Spell")
            {
                return Convert.ToInt32(this.BaseDamage * ElementMultiplikator(OpponentsCard));
            }
            else
            {
                return -1;
            }
        }
    }
}

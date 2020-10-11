using System;
using System.Collections.Generic;
using System.Text;
using MonsterTradingCardGame.Cards;

namespace MonsterTradingCardGame.Cards.MonsterCards
{
    class Ork : Card
    {
        Ork(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "Ork";
            this.CardStyle = "Monster";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if(OpponentsCard.CardStyle == "monster")
            {
                if (OpponentsCard.CardType == "Wizzard")
                {
                    return 0;
                }
                return this.BaseDamage;
            }else if(OpponentsCard.CardStyle == "Spell")
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

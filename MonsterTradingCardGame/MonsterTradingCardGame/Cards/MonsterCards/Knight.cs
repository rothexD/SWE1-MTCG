﻿using System;

namespace MCTG.Cards.MonsterCards
{
    public class Knight : Card
    {
        public Knight(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "Knight";
            this.CardStyle = "Monster";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if (OpponentsCard.CardStyle == "Monster")
            {
                return this.BaseDamage;
            }
            else if (OpponentsCard.CardStyle == "Spell")
            {
                if (OpponentsCard.CardType == "WaterSpell")
                {
                    return 0;
                }
                return Convert.ToInt32(this.BaseDamage * ElementMultiplikator(OpponentsCard));
            }
            else
            {
                return -1;
            }
        }
    }
}

﻿using System;

namespace MCTG.Cards.MonsterCards
{
    public class FireElve : Card
    {
        public FireElve(int Basedamage, string Name, string CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "FireElve";
            this.CardStyle = "Monster";
            this.CardElement = Element;
            this.AttackMoveName = "Sneaky Dagger";
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if (OpponentsCard.CardStyle == "Monster")
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

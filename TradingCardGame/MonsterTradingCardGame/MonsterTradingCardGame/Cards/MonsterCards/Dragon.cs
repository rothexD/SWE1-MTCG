using System;
using System.Collections.Generic;
using System.Text;

namespace MonsterTradingCardGame.Cards.MonsterCards
{
    class Dragon : Card
    {
        private Random RandomNumberGenerator = new Random();
        Dragon(int Basedamage, string Name,int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "Dragon";
            this.CardType = "Monster";
            this.CardElement = Element;
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if (OpponentsCard.CardStyle == "Monster")
            {
                if (OpponentsCard.CardType == "FireElve")
                {
                    int randomInt = RandomNumberGenerator.Next(0, 100);
                    if (randomInt > 20)
                    {
                        return 0;
                    }
                    else
                    {
                        return this.BaseDamage;
                    }
                }
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

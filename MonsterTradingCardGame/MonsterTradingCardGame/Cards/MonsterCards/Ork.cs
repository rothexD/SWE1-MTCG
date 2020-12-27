using System;

namespace MCTG.Cards.MonsterCards
{
    public class Ork : Card
    {
        public Ork(int Basedamage, string Name, string CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "Ork";
            this.CardStyle = "Monster";
            this.CardElement = Element;
            this.AttackMoveName = "berserkers rage";
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if (OpponentsCard.CardStyle == "Monster")
            {
                if (OpponentsCard.CardType == "Wizzard")
                {
                    return 0;
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

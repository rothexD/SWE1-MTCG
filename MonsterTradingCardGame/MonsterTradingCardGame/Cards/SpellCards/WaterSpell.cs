using System;

namespace MCTG.Cards.SpellCards
{
    public class WaterSpell : Card
    {
        public WaterSpell(int Basedamage, string Name, string CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "WaterSpell";
            this.CardStyle = "Spell";
            this.CardElement = Element;
            this.AttackMoveName = "Magic";
        }
        public override int CalculateDamage(Card OpponentsCard)
        {
            if (OpponentsCard.CardType == "Kraken")
            {
                return 0;
            }
            return Convert.ToInt32(this.BaseDamage * ElementMultiplikator(OpponentsCard));
        }
    }
}

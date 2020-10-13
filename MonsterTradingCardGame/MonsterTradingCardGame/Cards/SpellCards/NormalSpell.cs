using System;

namespace MCTG.Cards.SpellCards
{
    public class NormalSpell : Card
    {
        public NormalSpell(int Basedamage, string Name, int CardId, CardelEmentEnum Element) : base(CardId)
        {
            this.BaseDamage = Basedamage;
            this.CardName = Name;
            this.CardType = "NormalSpell";
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

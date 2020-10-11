namespace MCTG.Cards
{
    public abstract class Card
    {
        public enum CardelEmentEnum
        {
            fire,
            water,
            normal
        }
        public int BaseDamage { get; protected set; }
        public string CardType { get; protected set; }
        public string CardName { get; protected set; }
        public string CardStyle { get; protected set; }
        public CardelEmentEnum CardElement { get; protected set; }
        public int CardId { get; private set; }

        protected float ElementMultiplikator(Card OpponentCard)
        {
            if (this.CardElement == OpponentCard.CardElement)
            {
                return 1;
            }
            else if (this.CardElement == CardelEmentEnum.fire && OpponentCard.CardElement == CardelEmentEnum.normal)
            {
                return 2;
            }
            else if (this.CardElement == CardelEmentEnum.fire && OpponentCard.CardElement == CardelEmentEnum.water)
            {
                return 0.5f;
            }
            else if (this.CardElement == CardelEmentEnum.water && OpponentCard.CardElement == CardelEmentEnum.fire)
            {
                return 2f;
            }
            else if (this.CardElement == CardelEmentEnum.water && OpponentCard.CardElement == CardelEmentEnum.normal)
            {
                return 0.5f;
            }
            else if (this.CardElement == CardelEmentEnum.normal && OpponentCard.CardElement == CardelEmentEnum.water)
            {
                return 2f;
            }
            else if (this.CardElement == CardelEmentEnum.normal && OpponentCard.CardElement == CardelEmentEnum.fire)
            {
                return 0.5f;
            }
            else
            {
                return 0;
            }
        }
        public abstract int CalculateDamage(Card Opponentscard);
        protected Card(int CardId)
        {
            this.CardId = CardId;
        }

    }
}

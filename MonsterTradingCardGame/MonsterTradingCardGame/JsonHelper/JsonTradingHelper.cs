namespace MCTG.JsonHelper
{
    public class JsonTradingHelper
    {
        public string TradeID { get; set; }
        public string CardToTradeID_fk { get; set; }
        public string OriginalOwner_fk { get; set; }
        public string typeToTrade { get; set; }
        public string minDamage { get; set; }
        public string TimestampOffer { get; set; }
    }
}

using MCTG.Cards;
using MCTG.FightHandlers;
using System.Collections.Generic;
namespace MCTG.Players
{
    public class Player
    {
        public string UserName { get; set; }
        public string Elo { get; set; }
        public List<Card> Deck { get; set; }
        public string assignedBattleLog { get; set; }
        public FightHandler.BattleStatus status { get; set; }

        public Player()
        {
            status = FightHandler.BattleStatus.Fighthasnothappened;
            assignedBattleLog = "";
        }
    }
}
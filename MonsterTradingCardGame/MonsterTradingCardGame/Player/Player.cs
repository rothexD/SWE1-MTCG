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
        public string AssignedBattleLog { get; set; }
        public FightHandler.BattleStatus Status { get; set; }

        public Player()
        {
            Status = FightHandler.BattleStatus.Fighthasnothappened;
            AssignedBattleLog = "";
        }
    }
}
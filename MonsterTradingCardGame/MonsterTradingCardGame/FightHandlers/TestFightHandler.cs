using System;
using System.Collections.Generic;
using System.Text;
using MCTG.Cards;
using MCTG.Players;
namespace MCTG.FightHandlers
{
    public class TestFightHandler : FightHandler
    {
        public TestFightHandler(Player Player1, Player Player2) : base(Player1,Player2)
        {
        }

        public void TestShuffleRiggedDice(Random RiggedDice)
        {
            Random OldDice = Dice;
            Dice = RiggedDice;
            ShuffleTempLists();
            Dice = OldDice;
            return;
        }
        public void TestFightOneRoundRiggedDice(Random RiggedDice, ref List<Card> Player1, ref List<Card> Player2)
        {
            Random OldDice = Dice;
            Dice = RiggedDice;
            FightOneRound(ref Player1, ref Player2);
            Dice = OldDice;
            return;
        }
        public List<Card> ReturnTempDeck1_List()
        {
            return Player1TempDeck;
        }
        public List<Card> ReturnTempDeck2_List()
        {
            return Player2TempDeck;
        }
        public BattleStatus FullFightWinnerPlayer1_RiggedDice()
        {
            Random OldDice = Dice;
            Dice = new Random(1);
            this.Fight();
            Dice = OldDice;
            return MostRecentStatus;
        }
        public BattleStatus FullFightWinnerPlayer2_RiggedDice()
        {
            Random OldDice = Dice;
            Dice = new Random(999999999);
            this.Fight();
            Dice = OldDice;           
            return MostRecentStatus;
        }
        public BattleStatus FullFightWinnerTie_RiggedDice()
        {
            Random OldDice = Dice;
            Dice = new Random(999999999);
            this.Fight();
            Dice = OldDice;
            return MostRecentStatus;
        }
        public int FindTie()
        {
            Random OldDice = Dice;
            int x =1;
            for(int i=0; i< 1000000; i++)
            {
                x = (x * 2656) % sizeof(int);
                this.Fight();
                if (MostRecentStatus == BattleStatus.Tie)
                {
                    return x;
                }
            }         
            Dice = OldDice;
            return -1;
        }
    }
}

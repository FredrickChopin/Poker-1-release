using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using static Poker_1.GlobalVars;
using MoreLinq;
// put calling and betting into a moves method (done)
// write descriptions for functions
// need to improve RankHand so it adapts to multiple rank[0] but better(done)
// need to improve pairs and threes (done)
// RankHand now works for combinedArrays of lengths that are <7 (Perhaps useful in Later computer's decision making)
// RankHand now also sees 2,3,4,5,14 as straights (done)
// need to add BlindBetting : (done)
// change deal initial
// change deal card
// change computer move (was not needed)
// need to improve rank hands (perhaps pairs, threes and fours) (done)
// Disable betting, chcking and folding if one's loses his money (done)

namespace Poker_1
{
    public static class GlobalVars
    {
        public static int bigStake = 50;
        public static int money = 10000;
        public static bool pStarts = true;
        public static int miniRound = 1;
        public static Random rnd = new Random();
    }

    public static class Moves
    {

        public static void Call(ref int pMoney, ref int cMoney, ref int pMoneyBet, ref int cMoneyBet, char caller)
        {
            int difference = Math.Max(cMoneyBet, pMoneyBet) - Math.Min(cMoneyBet, pMoneyBet);
            if (caller == 'c')
            {
                cMoneyBet += difference;
                cMoney -= difference;
            }
            else
            {
                pMoneyBet += difference;
                pMoney -= difference;
            }
        }

        public static void Bet(ref int pMoney, ref int cMoney, ref int pMoneyBet, ref int cMoneyBet, char better, int amount)
        {
            if (better == 'c')
            {
                cMoneyBet += amount;
                cMoney -= amount;
            }
            else
            {
                pMoneyBet += amount;
                pMoney -= amount;
            }
        }

        public static void Fold(NumericUpDown numericBet, Card[] pHand, Card[] computerHand, List<Card> table, PictureBox[] picturesPHand, PictureBox[] picturesTable, PictureBox[] picturesCHand, Label lblCMoneyValue, Label lblPMoneyValue, Label lblCMoneyBetValue, Label lblPMoneyBetValue, ref int cMoney, ref int pMoney, ref int cMoneyBet, ref int pMoneyBet, int bigStake, Button btnBet, Button btnCall, Button btnFold, Button btnCheck, ToolStripMenuItem continueToolStripMenuItem, char winner)
        {
            //folding results in the player having to press continue
            Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
            Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
            Card.UpdateAll
            (
            numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue,
            lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
            bigStake
            );
        }

        public static void MakeComputerMove(ref int pMoney, ref int cMoney, ref int pMoneyBet, ref int cMoneyBet, NumericUpDown numericBet, Card[] cards, Card[] pHand, Card[] computerHand, List<Card> table, PictureBox[] picturesPHand, PictureBox[] picturesTable, PictureBox[] picturesCHand, Label lblCMoneyValue, Label lblPMoneyValue, Label lblCMoneyBetValue, Label lblPMoneyBetValue, Button btnBet, Button btnCall, Button btnFold, Button btnCheck, ToolStripMenuItem continueToolStripMenuItem)
        {
            int randonNum;
            if (pMoney == 0 || cMoney == 0)
            {
                if (pMoneyBet > cMoneyBet)
                {
                    MessageBox.Show("computer called");
                    Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                }
                else
                {
                    MessageBox.Show("computer checked");
                }
                MessageBox.Show("All in");
                Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                continueToolStripMenuItem.Enabled = false;
                while (table.Count < 5)
                {
                    //Thread.Sleep(1000);
                    Card.DealAnotherCard(table, cards);
                }
                Card.UpdateAll
                    (
                    numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                    bigStake
                    );
                Card.SetComputerCardPicturesToRed(picturesCHand);
                int[] computerRank = Card.RankHand(table, computerHand);
                int[] playerRank = Card.RankHand(table, pHand);
                char winner = Card.DetermineWinner(playerRank, computerRank);
                Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                Card.UpdateAll
                     (
                     numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                     bigStake
                     );
                Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);

            }
            else if (pStarts)
            {
                if (pMoneyBet == cMoneyBet)
                {
                    randonNum = rnd.Next(1, 11);
                    if (randonNum > 5 && pMoney >= cMoney / 100 && miniRound < 3)
                    {
                        int bigNumOfRandom = 100;
                        int amountToBet = 0;
                        do
                        {
                            amountToBet = rnd.Next(1, bigNumOfRandom) * cMoney / 100 + (pMoneyBet - cMoneyBet);
                            bigNumOfRandom--;
                        } while (amountToBet > Math.Min(pMoney, cMoney));
                        Bet(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c', amountToBet);
                        btnBet.Enabled = false;
                        btnCall.Enabled = true;
                        btnCheck.Enabled = false;
                        string whatToShow = string.Format("computer has betted " + amountToBet);
                        MessageBox.Show(whatToShow);
                        miniRound++;
                    }
                    else
                    {
                        if (table.Count < 5)
                        {
                            Card.DealAnotherCard(table, cards);
                            Card.UpdateAll
                                (
                                numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                bigStake
                                );
                            Card.SetComputerCardPicturesToRed(picturesCHand);
                            MessageBox.Show("Computer Checked");
                            miniRound = 1;
                            btnCheck.Enabled = true;
                            btnBet.Enabled = true;
                            btnCall.Enabled = false;
                        }
                        else
                        {
                            Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                            int[] computerRank = Card.RankHand(table, computerHand);
                            int[] playerRank = Card.RankHand(table, pHand);
                            char winner = Card.DetermineWinner(playerRank, computerRank);
                            Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                            Card.UpdateAll
                                 (
                                 numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                 bigStake
                                 );
                        }
                    }
                }
                else
                {
                    randonNum = rnd.Next(1, 11);
                    if (randonNum > 8)
                    {
                        MessageBox.Show("computer folded");
                        Fold(numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, ref cMoney, ref pMoney, ref cMoneyBet, ref pMoneyBet, bigStake, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem, 'p');
                    }
                    else if (randonNum > 4 && randonNum < 9)
                    {
                        Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                        MessageBox.Show("computer called");
                        if (table.Count == 5)
                        {
                            Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                            int[] computerRank = Card.RankHand(table, computerHand);
                            int[] playerRank = Card.RankHand(table, pHand);
                            char winner = Card.DetermineWinner(playerRank, computerRank);
                            Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                            Card.UpdateAll
                                (
                                numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                bigStake
                                );
                        }
                        else
                        {
                            Card.DealAnotherCard(table, cards);
                            Card.UpdateAll
                                (
                                numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                bigStake
                                );
                            Card.SetComputerCardPicturesToRed(picturesCHand);
                            miniRound = 1;
                            btnCheck.Enabled = true;
                            btnBet.Enabled = true;
                        }
                    }
                    else
                    {
                        if (pMoney >= cMoney / 100)
                        {
                            try
                            {
                                int bigNumOfRandom = 100;
                                int amountToBet = 0;
                                do
                                {
                                    amountToBet = rnd.Next(1, bigNumOfRandom) * cMoney / 100 + (pMoneyBet - cMoneyBet);
                                    bigNumOfRandom--;
                                } while (amountToBet > Math.Min(pMoney, cMoney));
                                Bet(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c', amountToBet);
                                btnBet.Enabled = false;
                                btnCall.Enabled = true;
                                btnCheck.Enabled = false;
                                string whatToShow = string.Format("computer has betted " + amountToBet);
                                MessageBox.Show(whatToShow);
                                miniRound++;
                            }
                            catch
                            {
                                Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                                MessageBox.Show("computer called");
                                Card.DealAnotherCard(table, cards);
                                Card.UpdateAll
                                    (
                                    numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                    bigStake
                                    );
                                Card.SetComputerCardPicturesToRed(picturesCHand);
                                miniRound = 1;
                                btnCheck.Enabled = true;
                                btnBet.Enabled = true;
                            }
                        }
                        else
                        {
                            Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                            MessageBox.Show("computer called");
                            Card.DealAnotherCard(table, cards);
                            Card.UpdateAll
                                (
                                numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                bigStake
                                );
                            Card.SetComputerCardPicturesToRed(picturesCHand);
                            miniRound = 1;
                            btnCheck.Enabled = true;
                            btnBet.Enabled = true;
                        }
                        
                        
                    }
                }
            }

            //if !pStarts
            else
            { 
                switch (miniRound)
                {
                    case 1:
                        randonNum = rnd.Next(1, 11);
                        if (randonNum < 8)
                        {
                            //call
                            if (table.Count == 3)
                            {
                                Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                                MessageBox.Show("computer called");
                            }
                            else
                            {
                                MessageBox.Show("computer checked");
                            }
                            btnCall.Enabled = false;
                        }
                        else
                        {
                            //If the player has at least 1 precent money than the computer, bet a random amount between 1 to 20 precent.
                            if (pMoney >= cMoney / 100)
                            {
                                try
                                {
                                    int bigNumOfRandom = 21;
                                    int amountToBet = 0;
                                    do
                                    {
                                        amountToBet = rnd.Next(1, bigNumOfRandom) * cMoney / 100 + (pMoneyBet - cMoneyBet);
                                        bigNumOfRandom--;
                                    } while (amountToBet > Math.Min(pMoney, cMoney));
                                    Bet(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c', amountToBet);
                                    btnCheck.Enabled = false;
                                    btnBet.Enabled = false;
                                    btnCall.Enabled = true;
                                    string whatToShow = string.Format("computer has betted " + amountToBet);
                                    MessageBox.Show(whatToShow);
                                }
                                catch
                                {
                                    MessageBox.Show("computer checked");
                                    btnCall.Enabled = false;
                                }
                            }
                            else
                            {
                                if (table.Count == 3)
                                {
                                    Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                                    MessageBox.Show("computer called");
                                }
                                else
                                {
                                    MessageBox.Show("computer checked");
                                    btnCall.Enabled = false;
                                }
                            }
                            
                        }
                        miniRound++;
                        break;
                    case 3:
                        randonNum = rnd.Next(1, 11);
                        if (randonNum > 9)
                        {
                            Fold(numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, ref cMoney, ref pMoney, ref cMoneyBet, ref pMoneyBet, bigStake, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem, 'p');
                        }
                        else
                        {
                            Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'c');
                            MessageBox.Show("computer called");
                            if (table.Count == 5)
                            {
                                Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                                int[] computerRank = Card.RankHand(table, computerHand);
                                int[] playerRank = Card.RankHand(table, pHand);
                                char winner = Card.DetermineWinner(playerRank, computerRank);
                                Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                                Card.UpdateAll
                                    (
                                    numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                                    bigStake
                                    );
                            }
                            else
                            {
                                Card.DealAnotherCard(table, cards);
                                Card.UpdateAll(numericBet, pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand, lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,bigStake);
                                Card.SetComputerCardPicturesToRed(picturesCHand);
                                miniRound = 1;
                            }
                        }
                        break;
                }
            }
            if (cMoney == 0)
            {
                btnBet.Enabled = false;
            }
            Card.UpdateNumeric(numericBet, cMoney, pMoney, bigStake, cMoneyBet, pMoneyBet);
            Card.UpdateMoney(lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet);
        }

    }
   

    public class Card : Form
    {

        public Card(int suit, int rank)
        {
            this.suit = SuitByValue(suit);
            this.rank = RankByValue(rank);
        }

        private string rank;
        private char suit;
        private Bitmap resource;

        private int CardValue
        {
            get
            {
                int actualValue = 0;
                switch (rank)
                {
                    case "J":
                        actualValue = 11;
                        break;
                    case "Q":
                        actualValue = 12;
                        break;
                    case "K":
                        actualValue = 13;
                        break;
                    case "A":
                        actualValue = 14;
                        break;
                    default:
                        actualValue = int.Parse(rank);
                        break;
                }
                return actualValue;
            }
        }

        private char SuitByValue(int _enteredSuit)
        {
            char actualSuit = ' ';
            switch (_enteredSuit)
            {
                case 0:
                    actualSuit = '♦';
                    break;
                case 1:
                    actualSuit = '♣';
                    break;
                case 2:
                    actualSuit = '♥';
                    break;
                case 3:
                    actualSuit = '♠';
                    break;
            }
            return actualSuit;
        }

        private string RankByValue(int rank)
        {
            string value = null;
            if (rank < 11) value = Convert.ToString(rank);
            else
            {
                switch (rank)
                {
                    case 11:
                        value = "J";
                        break;
                    case 12:
                        value = "Q";
                        break;
                    case 13:
                        value = "K";
                        break;
                    case 14:
                        value = "A";
                        break;
                }
            }
            return value;
        }

        public static void SetAllCards(Card[] cards)
        {
            int timesLoopsRan = 0;
            for (int rankValue = 2; rankValue < 15; rankValue++)
            {
                for (int suitValue = 0; suitValue < 4; suitValue++)
                {
                    cards[timesLoopsRan] = new Card(suitValue, rankValue);
                    switch (timesLoopsRan)
                    {
                        case 0:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._2D);
                            break;
                        case 1:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._2C);
                            break;
                        case 2:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._2H);
                            break;
                        case 3:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._2S);
                            break;
                        case 4:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._3D);
                            break;
                        case 5:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._3C);
                            break;
                        case 6:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._3H);
                            break;
                        case 7:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._3S);
                            break;
                        case 8:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._4D);
                            break;
                        case 9:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._4C);
                            break;
                        case 10:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._4H);
                            break;
                        case 11:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._4S);
                            break;
                        case 12:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._5D);
                            break;
                        case 13:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._5C);
                            break;
                        case 14:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._5H);
                            break;
                        case 15:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._5S);
                            break;
                        case 16:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._6D);
                            break;
                        case 17:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._6C);
                            break;
                        case 18:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._6H);
                            break;
                        case 19:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._6S);
                            break;
                        case 20:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._7D);
                            break;
                        case 21:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._7C);
                            break;
                        case 22:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._7H);
                            break;
                        case 23:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._7S);
                            break;
                        case 24:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._8D);
                            break;
                        case 25:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._8C);
                            break;
                        case 26:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._8H);
                            break;
                        case 27:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._8S);
                            break;
                        case 28:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._9D);
                            break;
                        case 29:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._9C);
                            break;
                        case 30:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._9H);
                            break;
                        case 31:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._9S);
                            break;
                        case 32:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._10D);
                            break;
                        case 33:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._10C);
                            break;
                        case 34:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._10H);
                            break;
                        case 35:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources._10S);
                            break;
                        case 36:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.JD);
                            break;
                        case 37:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.JC);
                            break;
                        case 38:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.JH);
                            break;
                        case 39:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.JS);
                            break;
                        case 40:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.QD);
                            break;
                        case 41:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.QC);
                            break;
                        case 42:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.QH);
                            break;
                        case 43:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.QS);
                            break;
                        case 44:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.KD);
                            break;
                        case 45:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.KC);
                            break;
                        case 46:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.KH);
                            break;
                        case 47:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.KS);
                            break;
                        case 48:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.AD);
                            break;
                        case 49:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.AC);
                            break;
                        case 50:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.AH);
                            break;
                        case 51:
                            cards[timesLoopsRan].resource = new Bitmap(Properties.Resources.AS);
                            break;
                    }
                    timesLoopsRan++;
                }
            }
        }

        public static void DealAnotherCard(List<Card> table, Card[] cards)
        {
            switch (table.Count)
            {
                case 0:
                    for (int i = 4; i < 7; i++)
                    {
                        table.Add(cards[i]);
                    }
                    break;
                case 3:
                    table.Add(cards[7]);
                    break;
                case 4:
                    table.Add(cards[8]);
                    break;
            }
        }

        public static void DealInitial(Card[] cards, Card[] pHand, Card[] computerHand, List<Card> table)
        {
            pHand[0] = cards[0];
            pHand[1] = cards[1];
            computerHand[0] = cards[2];
            computerHand[1] = cards[3];
            table.Clear();
        }

        public static void UpdateImages(Card[] pHand, Card[] computerHand, List<Card> table, PictureBox[] picturesPHand, PictureBox[] picturesTable, PictureBox[] picturesCHand)
        {
            for (int i = 0; i < 2; i++)
            {
                picturesCHand[i].Image = computerHand[i].resource;
                picturesPHand[i].Image = pHand[i].resource;
            }
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    picturesTable[i].Image = table[i].resource;
                    picturesTable[i].Visible = true;
                }
                catch { }
            }
        }

        public static void ShuffleCards(Card[] cards)
        {
            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < cards.Length; t++)
            {
                Card tmp = cards[t];
                int r = rnd.Next(t, cards.Length);
                cards[t] = cards[r];
                cards[r] = tmp;
            }
        }

        public static void UpdateMoney(Label lblCMoneyValue, Label lblPMoneyValue, Label lblCMoneyBetValue, Label lblPMoneyBetValue, int cMoney, int pMoney, int cMoneyBet, int pMoneyBet)
        {
            lblCMoneyValue.Text = cMoney.ToString();
            lblPMoneyValue.Text = pMoney.ToString();
            lblCMoneyBetValue.Text = cMoneyBet.ToString();
            lblPMoneyBetValue.Text = pMoneyBet.ToString();
        }

        public static void UpdateNumeric(NumericUpDown numericBet, int cMoney, int pMoney, int bigStake, int cMoneyBet, int pMoneyBet)
        {
            numericBet.Increment = bigStake / 2;
            numericBet.Maximum = Math.Max(cMoney, pMoney);
            numericBet.Minimum = Math.Max(bigStake / 2, cMoneyBet - pMoneyBet + numericBet.Increment);     
        }

        public static void UpdateAll(NumericUpDown numericBet, Card[] pHand, Card[] computerHand, List<Card> table, PictureBox[] picturesPHand, PictureBox[] picturesTable, PictureBox[] picturesCHand, Label lblCMoneyValue, Label lblPMoneyValue, Label lblCMoneyBetValue, Label lblPMoneyBetValue, int cMoney, int pMoney, int cMoneyBet, int pMoneyBet, int bigStake)
        {
            UpdateImages(pHand, computerHand, table, picturesPHand, picturesTable, picturesCHand);
            UpdateMoney(lblCMoneyValue, lblPMoneyValue, lblCMoneyBetValue, lblPMoneyBetValue, cMoney, pMoney, cMoneyBet, pMoneyBet);
            UpdateNumeric(numericBet, cMoney, pMoney, bigStake, cMoneyBet, pMoneyBet);
        }

        public static void BetStakes(ref int pMoney, ref int cMoney, ref int pMoneyBet, ref int cMoneyBet, int bigStake)
        {
            if (pStarts)
            {
                pMoney -= bigStake / 2;
                pMoneyBet += bigStake / 2;
                cMoney -= bigStake;
                cMoneyBet += bigStake;
            }
            else
            {

                pMoney -= bigStake;
                pMoneyBet += bigStake;
                cMoney -= bigStake / 2;
                cMoneyBet += bigStake / 2;
            }
        }

        public static void SetComputerCardPicturesToRed(PictureBox[] computerHand)
        {
            foreach (PictureBox pic in computerHand)
            {
                pic.Image = Properties.Resources.red_back;
            }
        }

        public static void DisableBtnsEnableContinue(Button btnBet, Button btnCall, Button btnFold, Button btnCheck, ToolStripMenuItem continueToolStripMenuItem)
        {
            continueToolStripMenuItem.Enabled = true;
            btnBet.Enabled = false;
            btnCall.Enabled = false;
            btnFold.Enabled = false;
            btnCheck.Enabled = false;
        }

        public static int[] RankHand(List<Card> table, Card [] hand)
        {
            //combinedArray is the array of all available cards
            Card[] combinedArray = new Card[table.Count + hand.Length];
            hand.CopyTo(combinedArray, 0);
            table.CopyTo(combinedArray, 2);
            //Sorting by Card Value
            combinedArray = combinedArray.OrderByDescending(c => c.CardValue).ToArray();
            //initialazing and putting 0 into the array of rank that will be returned
            var rank = new int[6];
            for (int i = 0; i < 6; i++)
            {
                rank[i] = 0;
            }
            //[0] stands for strength hand
            //Now check for winning combinations
            /* 0 nothing
            * 1 pair
             * 2 two pair
             * 3 three of a kind
             * 4 straight
             * 5 flush
             * 6 fullhouse
             * 7 four of a kind
             * 8 straight flush*/
            var newArray = new Card[5]; //An array to input the hand the computer is checking each iteration
                                        //These integers are used to count pairs, threes and fours:
                                        //3 lists to save their values
            int timesPair = 0;
            int timesThree = 0;
            int timesFour = 0;
            var pairValues = new List<int>();
            var threeValues = new List<int>();
            var fourValues = new List<int>();

            for (int mainLoopIndex = 0; mainLoopIndex < combinedArray.Length - 4; mainLoopIndex++)
            {
                //inputting it into newArray
                newArray[0] = combinedArray[mainLoopIndex];
                newArray[1] = combinedArray[mainLoopIndex + 1];
                newArray[2] = combinedArray[mainLoopIndex + 2];
                newArray[3] = combinedArray[mainLoopIndex + 3];
                newArray[4] = combinedArray[mainLoopIndex + 4];

                //check for straight flush
                if ((newArray[0].CardValue - newArray[1].CardValue == 1) && (newArray[1].CardValue - newArray[2].CardValue == 1) && (newArray[2].CardValue - newArray[3].CardValue == 1) && (newArray[3].CardValue - newArray[4].CardValue == 1) && newArray[0].suit == newArray[1].suit && newArray[2].suit == newArray[3].suit && newArray[3].suit == newArray[1].suit && newArray[3].suit == newArray[4].suit)
                {
                    if (rank[0] < 8 || newArray[0].CardValue > rank[1])
                    {
                        rank[0] = 8;
                        rank[1] = newArray[0].CardValue;
                        rank[2] = 0;
                        rank[3] = 0;
                        rank[4] = 0;
                        rank[5] = 0;
                    }
                }

                if (newArray[0].CardValue - newArray[1].CardValue == 1 && newArray[1].CardValue - newArray[2].CardValue == 1 && newArray[2].CardValue - newArray[3].CardValue == 1 && newArray[3].CardValue - newArray[4].CardValue == 1)
                {
                    if (rank[0] < 4 || rank[1] > newArray[0].CardValue)
                    {
                        rank[0] = 4;
                        rank[1] = newArray[0].CardValue;
                        rank[2] = 0;
                        rank[3] = 0;
                        rank[4] = 0;
                        rank[5] = 0;
                    }
                }

                //checks for all pairs, threes and fours in newArray
                for (int j = 0; j < 4; j++)
                {
                    int counter = 0;
                    if (newArray[j].CardValue == newArray[j + 1].CardValue)
                    {
                        for (int k = j + 1; k < 5; k++)
                        {
                            if (newArray[j].CardValue == newArray[k].CardValue && !pairValues.Contains(newArray[j].CardValue) && !threeValues.Contains(newArray[j].CardValue) && !fourValues.Contains(newArray[j].CardValue))
                            {
                                counter++;
                            }
                        }
                        switch (counter)
                        {
                            case 1:
                                timesPair++;
                                pairValues.Add(newArray[j].CardValue);
                                break;
                            case 2:
                                timesThree++;
                                threeValues.Add(newArray[j].CardValue);
                                break;
                            case 3:
                                timesFour++;
                                fourValues.Add(newArray[j].CardValue);
                                break;
                        }
                    }
                }
            }

            //Check for four
            if (timesFour > 0 && rank[0] < 7)
            {
                rank[0] = 7;
                rank[1] = fourValues.Max();
                //gets a kicker
                for (int i = 0; i < combinedArray.Length; i++)
                {
                    if (combinedArray[i].CardValue != fourValues.Max())
                    {
                        rank[2] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                rank[2] = 0;
                rank[3] = 0;
                rank[4] = 0;
                rank[5] = 0;
            }

            //Check for full house
            if (rank[0] < 6 && timesPair >= 1 && timesThree >= 1)
            {
                rank[0] = 6;
                rank[1] = threeValues.Max();
                rank[2] = pairValues.Min();
                rank[3] = 0;
                rank[4] = 0;
                rank[5] = 0;
            }

            //Check for three
            if (rank[0] < 3 && timesThree == 1)
            {
                rank[0] = 3;
                rank[1] = threeValues.Max();
                //gets 2 kickers
                for (int i = 0; i < 6; i++)
                {
                    if (combinedArray[i].CardValue != threeValues.Max())
                    {
                        rank[2] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (combinedArray[i].CardValue != threeValues.Max() && combinedArray[i].CardValue != rank[2])
                    {
                        rank[3] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                rank[4] = 0;
                rank[5] = 0;
            }

            //Check for two pair
            if (rank[0] < 2 && timesPair >= 2)
            {
                rank[0] = 2;
                pairValues.Sort();
                pairValues.Reverse();
                rank[1] = pairValues[0];
                rank[2] = pairValues[1];
                //gets a kicker
                for (int i = 0; i < 6; i++)
                {
                    if (!pairValues.Contains(combinedArray[i].CardValue))
                    {
                        rank[3] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                rank[4] = 0;
                rank[5] = 0;
            }

            //Check for a pair
            if (rank[0] < 1 && timesPair == 1)
            {
                rank[0] = 1;
                rank[1] = pairValues[0];
                for (int i = 0; i < 6; i++)
                {
                    if (!pairValues.Contains(combinedArray[i].CardValue))
                    {
                        rank[2] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (!pairValues.Contains(combinedArray[i].CardValue) && combinedArray[i].CardValue != rank[2])
                    {
                        rank[3] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    if (!pairValues.Contains(combinedArray[i].CardValue) && combinedArray[i].CardValue != rank[2] && combinedArray[i].CardValue != rank[3])
                    {
                        rank[4] = combinedArray[i].CardValue;
                        i = 6;
                    }
                }
                rank[5] = 0;
            }

            //Sorting by suit and then by card value to check for flush
            combinedArray = combinedArray.OrderBy(card => card.suit).ThenByDescending(card => card.CardValue).ToArray();
            for (int i = 0; i < combinedArray.Length - 4; i++)
            {
                newArray[0] = combinedArray[i];
                newArray[1] = combinedArray[i + 1];
                newArray[2] = combinedArray[i + 2];
                newArray[3] = combinedArray[i + 3];
                newArray[4] = combinedArray[i + 4];
                //newArray.Select(c => c.suit).Distinct().Count == 1
                if (rank[0] < 5 && newArray.Select(c => c.suit).Distinct().Count() == 1)
                {
                    rank[0] = 5;
                    rank[1] = newArray[0].CardValue;
                    rank[2] = newArray[1].CardValue;
                    rank[3] = newArray[2].CardValue;
                    rank[4] = newArray[3].CardValue;
                    rank[5] = newArray[4].CardValue;
                }
            }

            //Check for high card
            combinedArray = combinedArray.OrderByDescending(c => c.CardValue).ToArray();
            if (rank[0] == 0)
            {
                //rank[0] = 0;
                rank[1] = combinedArray[0].CardValue;
                rank[2] = combinedArray[1].CardValue;
                rank[3] = combinedArray[2].CardValue;
                rank[4] = combinedArray[3].CardValue;
                rank[5] = combinedArray[4].CardValue;
            }

            //now account for straights that start with Ace
            var theStraight = combinedArray.Where(c => c.CardValue == 2 || c.CardValue == 3 || c.CardValue == 4 || c.CardValue == 5 || c.CardValue == 14).ToList();
            theStraight = theStraight.DistinctBy(c => c.CardValue).ToList();
            bool thereIsAFlush = theStraight.Select(c => c.suit).Distinct().Count() == 1;
            bool thereIsAtraight = theStraight.Count == 5;
            if (rank[0] < 8 && thereIsAFlush && thereIsAtraight)
            {
                rank[0] = 8;
                rank[1] = 5;
                rank[2] = 0;
                rank[3] = 0;
                rank[4] = 0;
                rank[5] = 0;
            }
            if (rank[0] < 4 && thereIsAtraight)
            {
                rank[0] = 4;
                rank[1] = 5;
                rank[2] = 0;
                rank[3] = 0;
                rank[4] = 0;
                rank[5] = 0;
            }
            return rank;
        }

        public static char DetermineWinner(int[] pRank, int[] cRank, int counter=0)
        {
            if (counter == 6)
            {
                PrintRanks(pRank, cRank);
                //MessageBox.Show(pRank[0].ToString());
                return 'd';
            }
            if (pRank[counter] < cRank[counter])
            {
                PrintRanks(pRank, cRank);
                //MessageBox.Show(pRank[0].ToString());
                return 'c';
            }
            if (pRank[counter] > cRank[counter])
            {
                PrintRanks(pRank, cRank);
                //MessageBox.Show(pRank[0].ToString());
                return 'p';
            }
            return DetermineWinner(pRank, cRank, counter + 1);
        }

        public static void PrintRanks(int[] pRank , int[] cRank)
        {
            string stringPHand = null;
            string stringCHand = null;
            foreach (int cell in pRank)
            {
                stringPHand += cell + ",";
            }
            MessageBox.Show(stringPHand, "Player's hand:");
            foreach (int cell in cRank)
            {
                stringCHand += cell + ",";
            }
            MessageBox.Show(stringCHand, "Computer's hand:");
        }

        public static void DistributeMoneyBasedOnWinner(ref int pMoney, ref int cMoney, ref int pMoneyBet, ref int cMoneyBet, char winner)
        {
            switch (winner)
            {
                case 'c':
                    cMoney += pMoneyBet + cMoneyBet;
                    MessageBox.Show("Computer won! \n Cntrl+c to continue");
                    break;
                case 'p':
                    pMoney += pMoneyBet + cMoneyBet;
                    MessageBox.Show("Player won! \n Cntrl+c to continue");
                    break;
                case 'd':
                    MessageBox.Show("It is a draw! \n Cntrl+c to continue");
                    pMoney += pMoneyBet;
                    cMoney += cMoneyBet;
                    break;
            }
            pMoneyBet = 0;
            cMoneyBet = 0;
        }

    }
}

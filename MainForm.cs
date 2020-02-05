using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Poker_1.GlobalVars;

namespace Poker_1
{

    public partial class MainForm : Form
    {

        Card[] cards = new Card[52];
        Card[] pHand = new Card[2];
        Card[] cHand = new Card[2];
        List<Card> table = new List<Card>();

        PictureBox[] picturesPHand = new PictureBox[2];
        PictureBox[] picturesTable = new PictureBox[5];
        PictureBox[] picturesCHand = new PictureBox[2];

        int cMoney = 10000;
        int cMoneyBet = 0;
        int pMoney = 10000;
        int pMoneyBet = 0;

        public MainForm()
        {
            InitializeComponent();
            picturesPHand[0] = picPHand1;
            picturesPHand[1] = picPHand2;
            picturesTable[0] = picTable1;
            picturesTable[1] = picTable2;
            picturesTable[2] = picTable3;
            picturesTable[3] = picTable4;
            picturesTable[4] = picTable5;
            picturesCHand[0] = picPCHand1;
            picturesCHand[1] = picPCHand2;
            pMoney = money;
            cMoney = money;
            Card.SetAllCards(cards);
            Card.ShuffleCards(cards);
            Card.DealInitial(cards, pHand, cHand, table);
            Card.BetStakes(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, bigStake);
            Card.UpdateAll
                (
                numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                bigStake
                );
            btnCheck.Enabled = false;
            Card.SetComputerCardPicturesToRed(picturesCHand);
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            Moves.Call(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'p');
            if (table.Count == 5)
            {
                Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                int[] computerRank = Card.RankHand(table, cHand);
                int[] playerRank = Card.RankHand(table, pHand);
                char winner = Card.DetermineWinner(playerRank, computerRank);
                Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                Card.UpdateAll
                (
                numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                bigStake
                );
            }
            else
            {
                if (miniRound == 3 || !pStarts)
                {
                    Card.DealAnotherCard(table, cards);
                    Card.UpdateAll
                     (
                     numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                     lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                     bigStake
                     );
                    Card.SetComputerCardPicturesToRed(picturesCHand);
                    miniRound = 1;
                    btnCheck.Enabled = true;
                    btnBet.Enabled = true;
                    btnCall.Enabled = false;
                }
                else
                {
                    miniRound++;
                    Moves.MakeComputerMove(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, numericBet, cards, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                }
            }
        }

        private void btnFold_Click(object sender, EventArgs e)
        {
            // switch to folding function
            Moves.Fold(numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue, lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, ref cMoney, ref pMoney, ref cMoneyBet, ref pMoneyBet, bigStake, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem, 'c');

        }

        public void continueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cMoney <= 0)
            {
                MessageBox.Show("The computer has lost all of his money!", "Game over!");
            }
            else if (pMoney <= 0)
            {
                MessageBox.Show("The player has lost all of his money!", "Game over!");
            }
            else
            {
                miniRound = 1;
                pStarts = !pStarts;
                Card.ShuffleCards(cards);
                Card.DealInitial(cards, pHand, cHand, table);
                Card.BetStakes(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, bigStake);
                Card.UpdateAll
                    (
                    numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                    lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                    bigStake
                    );
                Card.SetComputerCardPicturesToRed(picturesCHand);
                foreach (PictureBox box in picturesTable)
                {
                    box.Visible = false;
                }
                btnBet.Enabled = true;
                btnCall.Enabled = true;
                btnFold.Enabled = true;
                btnCheck.Enabled = true;
                if (!pStarts) Moves.MakeComputerMove(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, numericBet, cards, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                    lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
            }
            continueToolStripMenuItem.Enabled = false;
        }

        private void btnBet_Click(object sender, EventArgs e)
        {
            Moves.Bet(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, 'p', Convert.ToInt32(numericBet.Value));
            miniRound++;
            Card.UpdateNumeric(numericBet, cMoney, pMoney, bigStake, cMoneyBet, pMoneyBet);
            Card.UpdateMoney(lblComputerTotalMoneyValue, lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet);
            Moves.MakeComputerMove(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, numericBet, cards, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (pStarts)
            {

                miniRound++;
                Moves.MakeComputerMove(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, numericBet, cards, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
            lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
            }
            else
            {
                if (table.Count < 5)
                {
                    Card.DealAnotherCard(table, cards);
                    Card.UpdateAll
                     (
                     numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                     lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                     bigStake
                     );
                    Card.SetComputerCardPicturesToRed(picturesCHand);
                    miniRound = 1;
                    Moves.MakeComputerMove(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, numericBet, cards, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                }
                else
                {
                    Card.DisableBtnsEnableContinue(btnBet, btnCall, btnFold, btnCheck, continueToolStripMenuItem);
                    int[] computerRank = Card.RankHand(table, cHand);
                    int[] playerRank = Card.RankHand(table, pHand);
                    char winner = Card.DetermineWinner(playerRank, computerRank);
                    Card.DistributeMoneyBasedOnWinner(ref pMoney, ref cMoney, ref pMoneyBet, ref cMoneyBet, winner);
                    Card.UpdateAll
                    (
                    numericBet, pHand, cHand, table, picturesPHand, picturesTable, picturesCHand, lblComputerTotalMoneyValue,
                    lblPTotalMoneyValue, lblCMoneyInBetValue, lbPlMoneyInbetValue, cMoney, pMoney, cMoneyBet, pMoneyBet,
                    bigStake
                    );
                }
            }
        }
    }

}

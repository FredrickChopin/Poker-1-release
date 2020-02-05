using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Poker_1.GlobalVars;

namespace Poker_1
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            moneyNumericUpDown.Value = money;
            stakesNumericUpDown.Minimum = 0;
            stakesNumericUpDown.Maximum = money/100*20;
            stakesNumericUpDown.Value = bigStake;
        }

        private void confirm_Click(object sender, EventArgs e)
        {
            money = Convert.ToInt32(moneyNumericUpDown.Value);
            bigStake = Convert.ToInt32(stakesNumericUpDown.Value);
            MainForm form = new MainForm();            
            form.ShowDialog();
            this.Close();

        }
    }
}

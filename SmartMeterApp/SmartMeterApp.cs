using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartMeterApp
{
    public partial class SmartMeterApp : Form
    {
        public SmartMeterApp()
        {
            InitializeComponent();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            DialogResult dialogResult = settings.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                this.txtBxTop6Phase1.Text = "ok";
            }
            else if (dialogResult == DialogResult.Cancel)
            {
                this.txtBxTop6Phase1.Text = "cancel";
            }
        }
    }
}

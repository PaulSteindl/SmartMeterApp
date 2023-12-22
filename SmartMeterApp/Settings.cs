using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartMeterApp
{
    public partial class Settings : Form
    {
        public readonly static int IntervallMultiplier = 1000; // == Sekunde
        public static SettingsObject ActualSettings = new SettingsObject();
        private readonly static string _settingString = "SettingsSaved";
        private readonly static SettingsObject _defaultSettings = new SettingsObject
        {
            Top6Url = "http://192.168.188.6/wizard/public/api/measurements",
            Top7Url = "http://192.168.188.7/wizard/public/api/measurements",
            Username = "admin",
            Password = "Start123",
            IntervallRealtimeData = 1,
            IntervallLogging = 600,
            FlagTop6Phase1 = true,
            FlagTop6Phase2 = true,
            FlagTop6Phase3 = true,
            FlagTop7Phase1 = true,
            FlagTop7Phase2 = true,
            FlagTop7Phase3 = true,
            FlagOnTop = true
        };

        public Settings()
        {
            InitializeComponent();
            WriteSettingsIntoUI();
        }

        public void LockLoggerFields()
        {
            this.chxTop6Phase1.Enabled = false;
            this.chxTop6Phase2.Enabled = false;
            this.chxTop6Phase3.Enabled = false;

            this.chxTop7Phase1.Enabled = false;
            this.chxTop7Phase2.Enabled = false;
            this.chxTop7Phase3.Enabled = false;
        }

        public static void UpdateActualSettings()
        {
            ActualSettings = GetSettings();
        }

        private static SettingsObject GetSettings()
        {
            string settingsSavedString = Properties.Settings.Default[_settingString].ToString();
            if (!String.IsNullOrWhiteSpace(settingsSavedString))
            {
                return JsonSerializer.Deserialize<SettingsObject>(settingsSavedString);
            }

            return _defaultSettings;
        }

        private void WriteSettingsIntoUI()
        {
            SettingsObject settingsSaved = GetSettings();

            this.txtBxTop6Url.Text = settingsSaved.Top6Url;
            this.txtBxTop7Url.Text = settingsSaved.Top7Url;
            this.txtBxUsername.Text = settingsSaved.Username;
            this.txtBxPassword.Text = settingsSaved.Password;

            this.numUpDownIntervall.Value = settingsSaved.IntervallRealtimeData;
            this.numUpDownLogging.Value = settingsSaved.IntervallLogging;

            this.chxTop6Phase1.Checked = settingsSaved.FlagTop6Phase1;
            this.chxTop6Phase2.Checked = settingsSaved.FlagTop6Phase2;
            this.chxTop6Phase3.Checked = settingsSaved.FlagTop6Phase3;

            this.chxTop7Phase1.Checked = settingsSaved.FlagTop7Phase1;
            this.chxTop7Phase2.Checked = settingsSaved.FlagTop7Phase2;
            this.chxTop7Phase3.Checked = settingsSaved.FlagTop7Phase3;

            this.chxOnTop.Checked = settingsSaved.FlagOnTop;
        }

        // Saves to C:\Users\<Username>\AppData\Local\SmartMeterApp\SmartMeterApp.exe_Url_<Hash>\<Version>
        private void SaveSettings()
        {
            SettingsObject settings = new SettingsObject();

            settings.Top6Url = this.txtBxTop6Url.Text;
            settings.Top7Url = this.txtBxTop7Url.Text;
            settings.Username = this.txtBxUsername.Text;
            settings.Password = this.txtBxPassword.Text;

            settings.IntervallRealtimeData = Convert.ToInt32(this.numUpDownIntervall.Value);
            settings.IntervallLogging = Convert.ToInt32(this.numUpDownLogging.Value);

            settings.FlagTop6Phase1 = this.chxTop6Phase1.Checked;
            settings.FlagTop6Phase2 = this.chxTop6Phase2.Checked;
            settings.FlagTop6Phase3 = this.chxTop6Phase3.Checked;

            settings.FlagTop7Phase1 = this.chxTop7Phase1.Checked;
            settings.FlagTop7Phase2 = this.chxTop7Phase2.Checked;
            settings.FlagTop7Phase3 = this.chxTop7Phase3.Checked;

            settings.FlagOnTop = this.chxOnTop.Checked;

            Properties.Settings.Default[_settingString] = JsonSerializer.Serialize(settings);
            Properties.Settings.Default.Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

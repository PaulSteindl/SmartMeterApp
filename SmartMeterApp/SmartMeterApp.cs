using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.IO;

namespace SmartMeterApp
{
    public partial class SmartMeterApp : Form
    {
        private bool _loggingActive;
        private static string _loggingPath;
        private Timer _timerIntervall;
        private Timer _timerLogger;
        private static bool _isError;

        public SmartMeterApp()
        {
            InitializeComponent();
            Settings.UpdateActualSettings();

            _loggingActive = false;
            _isError = false;

            _timerIntervall = new Timer();
            _timerIntervall.Interval = Settings.ActualSettings.IntervallRealtimeData * Settings.IntervallMultiplier;
            _timerIntervall.Tick += new EventHandler(OnIntervallTimer);
            _timerIntervall.Start();
        }

        private async void OnIntervallTimer(object sender, EventArgs e)
        {
            try
            {
                RequestObject resultTop6 = await GetApiDataAsync(Settings.ActualSettings.Top6Url);
                RequestObject resultTop7 = await GetApiDataAsync(Settings.ActualSettings.Top7Url);

                UpdateUI(resultTop6, resultTop7);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private async void OnLogTimer(object sender, EventArgs e)
        {
            RequestObject resultTop6 = null;
            RequestObject resultTop7 = null;

            try
            {
                if (Settings.ActualSettings.FlagTop6Phase1 || Settings.ActualSettings.FlagTop6Phase2 || Settings.ActualSettings.FlagTop6Phase3)
                    resultTop6 = await GetApiDataAsync(Settings.ActualSettings.Top6Url);

                if (Settings.ActualSettings.FlagTop7Phase1 || Settings.ActualSettings.FlagTop7Phase2 || Settings.ActualSettings.FlagTop7Phase3)
                    resultTop7 = await GetApiDataAsync(Settings.ActualSettings.Top7Url);

                LogResult(resultTop6, resultTop7);
            }
            catch(Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void CreateFileCheck()
        {
            try
            {
                if (!File.Exists(_loggingPath))
                {
                    using (FileStream fs = File.Create(_loggingPath))
                    {
                        string csvHeader = "DateTime";

                        if (Settings.ActualSettings.FlagTop6Phase1) csvHeader += ";Top6Phase1";
                        if (Settings.ActualSettings.FlagTop6Phase2) csvHeader += ";Top6Phase2";
                        if (Settings.ActualSettings.FlagTop6Phase3) csvHeader += ";Top6Phase3";

                        if (Settings.ActualSettings.FlagTop7Phase1) csvHeader += ";Top7Phase1";
                        if (Settings.ActualSettings.FlagTop7Phase2) csvHeader += ";Top7Phase2";
                        if (Settings.ActualSettings.FlagTop7Phase3) csvHeader += ";Top7Phase3";

                        byte[] info = new UTF8Encoding(true).GetBytes(csvHeader + "\n");
                        fs.Write(info, 0, info.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new FileCreationException(e.Message);
            }
        }

        private void LogResult(RequestObject resultTop6, RequestObject resultTop7)
        {
            string errorAddition = ";ERR";

            try
            {
                using (StreamWriter writer = new StreamWriter(_loggingPath, append: true))
                {
                    string csvInfo = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                    if (resultTop6 != null)
                    {
                        if (Settings.ActualSettings.FlagTop6Phase1)
                        {
                            if (resultTop6.PA.HasValue && Settings.ActualSettings.FlagTop6Phase1) csvInfo += $";{resultTop6.PA.Value}";
                            else csvInfo += errorAddition;
                        }

                        if (Settings.ActualSettings.FlagTop6Phase2)
                        {
                            if (resultTop6.PB.HasValue && Settings.ActualSettings.FlagTop6Phase2) csvInfo += $";{resultTop6.PB.Value}";
                            else csvInfo += errorAddition;
                        }

                        if (Settings.ActualSettings.FlagTop6Phase3)
                        {
                            if (resultTop6.PC.HasValue && Settings.ActualSettings.FlagTop6Phase3) csvInfo += $";{resultTop6.PC.Value}";
                            else csvInfo += errorAddition;
                        }
                    }
                    else
                    {
                        if (Settings.ActualSettings.FlagTop6Phase1) csvInfo += errorAddition;
                        if (Settings.ActualSettings.FlagTop6Phase2) csvInfo += errorAddition;
                        if (Settings.ActualSettings.FlagTop6Phase3) csvInfo += errorAddition;
                    }

                    if (resultTop7 != null)
                    {
                        if (Settings.ActualSettings.FlagTop7Phase1)
                        {
                            if (resultTop7.PA.HasValue) csvInfo += $";{resultTop7.PA.Value}";
                            else csvInfo += errorAddition;
                        }

                        if (Settings.ActualSettings.FlagTop7Phase2)
                        {
                            if (resultTop7.PB.HasValue) csvInfo += $";{resultTop7.PB.Value}";
                            else csvInfo += errorAddition;
                        }

                        if (Settings.ActualSettings.FlagTop7Phase3)
                        {
                            if (resultTop7.PC.HasValue) csvInfo += $";{resultTop7.PC.Value}";
                            else csvInfo += errorAddition;
                        }
                    }
                    else
                    {
                        if (Settings.ActualSettings.FlagTop7Phase1) csvInfo += errorAddition;
                        if (Settings.ActualSettings.FlagTop7Phase2) csvInfo += errorAddition;
                        if (Settings.ActualSettings.FlagTop7Phase3) csvInfo += errorAddition;
                    }

                    writer.WriteLine(csvInfo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new FileWriteException(e.Message);
            }
        }

        private void UpdateColor(TextBox txtBox, double value)
        {
            if(value >= 0) 
            {
                txtBox.BackColor = Color.FromArgb(255, 100, 100);
            }
            else
            {
                txtBox.BackColor = Color.LightGreen;
            }

            txtBox.ForeColor = Color.Black;
        }

        private void UpdateUI(RequestObject resultTop6, RequestObject resultTop7)
        {
            if (resultTop6 != null)
            {
                if (resultTop6.PA.HasValue)
                {
                    this.txtBxTop6Phase1.Text = resultTop6.PA.Value.ToString("F4");
                    UpdateColor(this.txtBxTop6Phase1, resultTop6.PA.Value);
                }

                if (resultTop6.PB.HasValue)
                {
                    this.txtBxTop6Phase2.Text = resultTop6.PB.Value.ToString("F4");
                    UpdateColor(this.txtBxTop6Phase2, resultTop6.PB.Value);
                }

                if (resultTop6.PC.HasValue)
                {
                    this.txtBxTop6Phase3.Text = resultTop6.PC.Value.ToString("F4");
                    UpdateColor(this.txtBxTop6Phase3, resultTop6.PC.Value);
                }

                if (resultTop6.PT.HasValue)
                {
                    this.txtBxTop6Total.Text = resultTop6.PT.Value.ToString("F4");
                    UpdateColor(this.txtBxTop6Total, resultTop6.PT.Value);
                }
            }
            else
            {
                this.txtBxTop6Phase1.Text = "ERR";
                this.txtBxTop6Phase2.Text = "ERR";
                this.txtBxTop6Phase3.Text = "ERR";
                this.txtBxTop6Total.Text = "ERR";
            }

            if (resultTop7 != null)
            {
                if (resultTop7.PA.HasValue)
                {
                    this.txtBxTop7Phase1.Text = resultTop7.PA.Value.ToString("F4");
                    UpdateColor(this.txtBxTop7Phase1, resultTop7.PA.Value);
                }

                if (resultTop7.PB.HasValue)
                {
                    this.txtBxTop7Phase2.Text = resultTop7.PB.Value.ToString("F4");
                    UpdateColor(this.txtBxTop7Phase2, resultTop7.PB.Value);
                }

                if (resultTop7.PC.HasValue)
                {
                    this.txtBxTop7Phase3.Text = resultTop7.PC.Value.ToString("F4");
                    UpdateColor(this.txtBxTop7Phase3, resultTop7.PC.Value);
                }

                if (resultTop7.PT.HasValue)
                {
                    this.txtBxTop7Total.Text = resultTop7.PT.Value.ToString("F4");
                    UpdateColor(this.txtBxTop7Total, resultTop7.PT.Value);
                }
            }
            else
            {
                this.txtBxTop7Phase1.Text = "ERR";
                this.txtBxTop7Phase2.Text = "ERR";
                this.txtBxTop7Phase3.Text = "ERR";
                this.txtBxTop7Total.Text = "ERR";
            }

            if (resultTop6 != null && resultTop7 != null)
            {
                if (resultTop6.PT.HasValue && resultTop7.PT.HasValue)
                {
                    double total = resultTop6.PT.Value + resultTop7.PT.Value;
                    this.txtBxTotalPhase.Text = total.ToString("F7");
                    UpdateColor(this.txtBxTotalPhase, total);
                }
            }
            else
                this.txtBxTotalPhase.Text = "ERR";
        }

        private static async Task<RequestObject> GetApiDataAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string base64Credential = Convert.ToBase64String
                        (Encoding.ASCII.GetBytes($"{Settings.ActualSettings.Username}:{Settings.ActualSettings.Password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credential);

                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<RequestObject>(responseBody);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new RequestException(e.Message);
                }
            }
        }

        private static void HandleError(Exception e)
        {
            string error = $"{e.GetType()}: {e.Message}\n{e.InnerException}";

            Console.WriteLine($"Error: {error}");

            if (!_isError)
            {
                _isError = true;
                MessageBox.Show(error, "FEHLER", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void HandleInformation(string info)
        {
            MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();

            if (_loggingActive)
            {
                settings.LockLoggerFields();
            }

            DialogResult dialogResult = settings.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                Settings.UpdateActualSettings();
                this.TopMost = Settings.ActualSettings.FlagOnTop;
                _timerIntervall.Interval = Settings.ActualSettings.IntervallRealtimeData * Settings.IntervallMultiplier;
                if(_timerLogger != null)
                    _timerLogger.Interval = Settings.ActualSettings.IntervallLogging * Settings.IntervallMultiplier;

                HandleInformation("Einstellungen aktualisiert.");
            }

            _isError = false;
        }

        private void btnActivateLogging_Click(object sender, EventArgs e)
        {
            if(!_loggingActive)
            {
                string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                this.fileDialogLogging.FileName = $"SmartMeterLogging_{dateTimeNow}";

                if (this.fileDialogLogging.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        CreateFileCheck();
                    }
                    catch(Exception ex)
                    {
                        HandleError(ex);
                    }

                    _loggingPath = this.fileDialogLogging.FileName;

                    _timerLogger = new Timer();
                    _timerLogger.Interval = Settings.ActualSettings.IntervallLogging * Settings.IntervallMultiplier;
                    _timerLogger.Tick += new EventHandler(OnLogTimer);
                    _timerLogger.Start();

                    this.btnActivateLogging.Text = "Logging Deaktivieren";
                    this._loggingActive = true;
                }
            } 
            else if(_loggingActive)
            {
                _timerLogger.Stop();
                this.btnActivateLogging.Text = "Logging Aktivieren";
                this._loggingActive = false;
            }

            HandleInformation($"Logging aktiv: {this._loggingActive}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace NextPCName
{
    public class Settings
    {
        public string AdPath = "LDAP://DC=yourcompany,DC=local";
        public string Prefix = "PC";
        public int Digits = 3;
        public int MaxTotal = 50;
        public bool Confetti = false;

        // settings.ini path: AppData\Roaming\NextPCName
        private static string FileName =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NextPCName", "settings.ini");

        public void Save()
        {
            var dir = Path.GetDirectoryName(FileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllLines(FileName, new[]
            {
                $"ad_path={AdPath}",
                $"prefix={Prefix}",
                $"digits={Digits}",
                $"max_total={MaxTotal}",
                $"confetti={(Confetti ? "1" : "0")}"
            });
        }

        public void Load()
        {
            if (!File.Exists(FileName)) return;
            foreach (var line in File.ReadAllLines(FileName))
            {
                if (line.StartsWith("ad_path=")) AdPath = line.Substring(8).Trim();
                else if (line.StartsWith("prefix=")) Prefix = line.Substring(7).Trim();
                else if (line.StartsWith("digits=")) int.TryParse(line.Substring(7).Trim(), out Digits);
                else if (line.StartsWith("max_total=")) int.TryParse(line.Substring(10).Trim(), out MaxTotal);
                else if (line.StartsWith("confetti=")) Confetti = line.Substring(9).Trim() == "1";
            }
        }
    }

    public class SettingsForm : Form
    {
        public Settings Tmp;
        TextBox tbAdPath, tbPrefix, tbDigits, tbMaxTotal;
        CheckBox cbConfetti;
        Label lblExample;

        public SettingsForm(Settings s)
        {
            this.Tmp = new Settings();
            this.Tmp.AdPath = s.AdPath;
            this.Tmp.Prefix = s.Prefix;
            this.Tmp.Digits = s.Digits;
            this.Tmp.MaxTotal = s.MaxTotal;
            this.Tmp.Confetti = s.Confetti;

            this.Text = "Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(400, 300); // ændret fra 400,270 til 400,300
            this.MaximizeBox = false;

            Label lbl1 = new Label() { Text = "AD Path (e.g. LDAP://DC=company,DC=local):", Top = 14, Left = 20, Width = 360 };
            tbAdPath = new TextBox() { Top = lbl1.Bottom + 2, Left = 20, Width = 360, Text = Tmp.AdPath };

            Label lbl2 = new Label() { Text = "Name prefix:", Top = tbAdPath.Bottom + 12, Left = 20, Width = 90 };
            tbPrefix = new TextBox() { Top = lbl2.Top, Left = lbl2.Right + 12, Width = 70, Text = Tmp.Prefix };

            Label lbl3 = new Label() { Text = "Digits in name:", Top = tbPrefix.Top + 34, Left = 20, Width = 90 };
            tbDigits = new TextBox() { Top = lbl3.Top, Left = lbl3.Right + 12, Width = 50, Text = Tmp.Digits.ToString() };

            lblExample = new Label()
            {
                Text = "",
                Top = tbDigits.Bottom + 2,
                Left = 20,
                Width = 360,
                Height = 22,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };

            UpdateExample();

            tbPrefix.TextChanged += (s2, e2) => UpdateExample();
            tbDigits.TextChanged += (s2, e2) => UpdateExample();

            Label lbl4 = new Label() { Text = "Max names to fetch:", Top = lblExample.Bottom + 10, Left = 20, Width = 150 };
            tbMaxTotal = new TextBox() { Top = lbl4.Top, Left = lbl4.Right + 12, Width = 60, Text = Tmp.MaxTotal.ToString() };

            cbConfetti = new CheckBox()
            {
                Text = "Show confetti celebration after searching",
                Top = lbl4.Bottom + 16,
                Left = 20,
                Checked = Tmp.Confetti,
                Width = 320
            };

            Label lblConfettiHelp = new Label()
            {
                Text = "Turn on to display colorful confetti when names are found.",
                Left = 42,
                Top = cbConfetti.Top + 22,
                Width = 340,
                Height = 20,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.DimGray
            };

            Button btnOK = new Button() { Text = "Save", Left = 140, Top = lblConfettiHelp.Top + lblConfettiHelp.Height + 16, Width = 100 };
            btnOK.Click += (s2, e2) =>
            {
                Tmp.AdPath = tbAdPath.Text;
                Tmp.Prefix = tbPrefix.Text;
                int.TryParse(tbDigits.Text, out Tmp.Digits);
                int.TryParse(tbMaxTotal.Text, out Tmp.MaxTotal);
                Tmp.Confetti = cbConfetti.Checked;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.AddRange(new Control[]
            {
                lbl1, tbAdPath, lbl2, tbPrefix, lbl3, tbDigits, lblExample, lbl4, tbMaxTotal, cbConfetti, lblConfettiHelp, btnOK
            });
        }

        private void UpdateExample()
        {
            int d = 3;
            int.TryParse(tbDigits.Text, out d);
            string pfx = tbPrefix.Text.Trim();
            if (string.IsNullOrEmpty(pfx)) pfx = "PC";
            string ex1 = $"{pfx}{(d > 1 ? new string('0', d - 1) : "")}1";
            string ex2 = $"{pfx}{(d > 1 ? new string('0', d - 1) : "")}2";
            lblExample.Text = $"Example: Will look for {ex1}, {ex2}, ...";
        }
    }

    public class Form1 : Form
    {
        private Label lblOverskrift, lblInfo, lblStatus, lblAntal;
        private Button btnNext;
        private Panel jfPanel, konfettiPanel;
        private LinkLabel jfLabel;
        private TextBox tbAmount;
        private Panel namesPanel;
        private Random rand = new Random();

        private List<(int x, int y, int size, Color c)> konfetti = new();
        private Color[] konfettiFarver = new Color[]
        {
            Color.DeepPink, Color.Orange, Color.LimeGreen,
            Color.CornflowerBlue, Color.Gold, Color.Violet
        };

        private Settings settings = new Settings();
        private const int MaxShowOnScreen = 6;

        public Form1()
        {
            settings.Load();

            // Menu
            var menu = new MenuStrip();
            var mSettings = new ToolStripMenuItem("Settings");
            menu.Items.Add(mSettings);
            mSettings.Click += (s, e) =>
            {
                var f = new SettingsForm(settings);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    settings = f.Tmp;
                    settings.Save();
                }
            };
            this.Controls.Add(menu);
            this.MainMenuStrip = menu;

            this.Text = "NextPCName";
            this.Icon = new Icon("NextPCName.ico");
            this.ClientSize = new Size(600, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10);

            lblInfo = new Label
            {
                Text = "Find the next available PC name in Active Directory.",
                Top = 28,
                Left = 0,
                Width = this.ClientSize.Width,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 10, FontStyle.Italic)
            };
            this.Controls.Add(lblInfo);

            lblOverskrift = new Label
            {
                Text = "Next available PC name(s)",
                Left = 0,
                Top = lblInfo.Bottom + 2,
                Width = this.ClientSize.Width,
                Height = 38,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };
            this.Controls.Add(lblOverskrift);

            namesPanel = new Panel
            {
                Left = 0,
                Top = lblOverskrift.Bottom + 10,
                Width = this.ClientSize.Width,
                Height = 95,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(namesPanel);

            btnNext = new Button
            {
                Text = "Find next available name(s)",
                Width = 250,
                Height = 44,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(47, 191, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(btnNext);

            lblAntal = new Label
            {
                Text = "Amount:",
                Width = 65,
                Height = 24,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10)
            };
            tbAmount = new TextBox
            {
                Text = "1",
                Width = 38,
                Height = 24,
                Font = new Font("Segoe UI", 11),
                TextAlign = HorizontalAlignment.Center
            };
            tbAmount.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FetchNames(); } };
            this.Controls.Add(lblAntal);
            this.Controls.Add(tbAmount);

            btnNext.Click += (s, e) => FetchNames();

            lblStatus = new Label
            {
                Text = "",
                Left = 0,
                Top = this.ClientSize.Height - 85,
                Width = this.ClientSize.Width - 170,
                Height = 28,
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblStatus);

            // Confetti overlay
            konfettiPanel = new Panel()
            {
                BackColor = Color.Transparent,
                Left = 0,
                Top = 0,
                Width = this.ClientSize.Width,
                Height = this.ClientSize.Height,
                Visible = false
            };
            konfettiPanel.Paint += KonfettiPanel_Paint;
            this.Controls.Add(konfettiPanel);

            // JF-panel nederst til højre, nu mindre og link
            int jfPanelSize = 94, jfPanelHeight = 28, jfPanelPadding = 12;
            jfPanel = new Panel()
            {
                Width = jfPanelSize,
                Height = jfPanelHeight,
                Left = this.ClientSize.Width - jfPanelSize - jfPanelPadding,
                Top = this.ClientSize.Height - jfPanelHeight - jfPanelPadding,
                BackColor = Color.White,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            jfPanel.Paint += JfPanel_Paint;

            jfLabel = new LinkLabel()
            {
                Text = "Made by Lett3rs",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = Color.MediumSlateBlue,
                Font = new Font("Segoe UI", 8, FontStyle.Bold | FontStyle.Italic),
                BackColor = Color.Transparent
            };
            jfLabel.Click += (s, e) => Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/lett3rs",
                UseShellExecute = true
            });
            jfPanel.Controls.Add(jfLabel);
            this.Controls.Add(jfPanel);

            this.Resize += (s, e) => DoLayout();
            DoLayout();
        }

        private void DoLayout()
        {
            lblInfo.Width = this.ClientSize.Width;
            lblOverskrift.Top = lblInfo.Bottom + 2;
            lblOverskrift.Width = this.ClientSize.Width;

            namesPanel.Width = this.ClientSize.Width;
            namesPanel.Top = lblOverskrift.Bottom + 10;
            namesPanel.Height = 100;

            btnNext.Top = namesPanel.Bottom + 10;
            btnNext.Left = (this.ClientSize.Width - btnNext.Width) / 2;

            int midX = this.ClientSize.Width / 2;
            lblAntal.Top = btnNext.Bottom + 15;
            tbAmount.Top = btnNext.Bottom + 13;
            lblAntal.Left = midX - (lblAntal.Width + tbAmount.Width + 6) / 2;
            tbAmount.Left = lblAntal.Right + 6;

            lblStatus.Width = this.ClientSize.Width - 170;
            lblStatus.Top = this.ClientSize.Height - 85;

            konfettiPanel.Width = this.ClientSize.Width;
            konfettiPanel.Height = this.ClientSize.Height;

            jfPanel.Left = this.ClientSize.Width - jfPanel.Width - 12;
            jfPanel.Top = this.ClientSize.Height - jfPanel.Height - 12;
        }

        private async void FetchNames()
        {
            lblStatus.Text = "";
            ShowNames(new List<string>());
            btnNext.Enabled = false;

            int amount = 1;
            if (!int.TryParse(tbAmount.Text, out amount) || amount < 1)
            {
                lblStatus.Text = $"Enter an amount of at least 1.";
                btnNext.Enabled = true;
                return;
            }
            if (amount > settings.MaxTotal)
            {
                lblStatus.Text = $"Maximum is {settings.MaxTotal}. You can change this in Settings.";
                btnNext.Enabled = true;
                return;
            }

            await Task.Delay(120);

            List<string> nameList = GetNextPCNames(settings.Prefix, settings.Digits, amount);

            // ----------- Fejlhåndtering hvis AD fejler -----------
            if (nameList == null)
            {
                lblStatus.Text = "Could not connect to Active Directory! Please check your AD Path in Settings.";
                btnNext.Enabled = true;
                return;
            }
            if (nameList.Count == 0)
            {
                lblStatus.Text = "No available names found! Please check your AD Path in Settings.";
                btnNext.Enabled = true;
                return;
            }
            // ------------------------------------------------------

            try { Clipboard.SetText(string.Join(Environment.NewLine, nameList)); }
            catch { lblStatus.Text = "Could not copy to clipboard!"; }

            lblStatus.Text = $"{nameList.Count} name" + (nameList.Count > 1 ? "s" : "") + " copied to clipboard.";

            // Show names in grid, but only up to 6 on screen
            if (nameList.Count <= MaxShowOnScreen)
                ShowNames(nameList);
            else
                ShowNames(new List<string> { $"{nameList.Count} PC names copied to clipboard." });

            if (settings.Confetti) await ShowConfettiOverlayAsync();
            btnNext.Enabled = true;
        }

        private void ShowNames(List<string> names)
        {
            namesPanel.Controls.Clear();

            if (names.Count == 0) return;

            int n = names.Count;
            int cellW = namesPanel.Width / n;
            int cellH = namesPanel.Height;
            if (n >= 4)
            {
                cellW = namesPanel.Width / 3;
                cellH = namesPanel.Height / 2;
            }

            for (int i = 0; i < n; i++)
            {
                var lbl = new Label
                {
                    Text = names[i],
                    Width = (n >= 4 ? namesPanel.Width / 3 : namesPanel.Width / n) - 8,
                    Height = (n >= 4 ? namesPanel.Height / 2 : namesPanel.Height) - 6,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", n >= 4 ? 14 : 20, FontStyle.Bold),
                    ForeColor = Color.MediumSlateBlue,
                    Left = (n >= 4 ? (i % 3) * (namesPanel.Width / 3) : i * (namesPanel.Width / n)) + 4,
                    Top = (n >= 4 ? (i / 3) * (namesPanel.Height / 2) : 3)
                };
                namesPanel.Controls.Add(lbl);
            }
        }

        private async Task ShowConfettiOverlayAsync()
        {
            konfetti.Clear();
            int konfettiCount = 65;
            for (int i = 0; i < konfettiCount; i++)
            {
                konfetti.Add((
                    rand.Next(10, konfettiPanel.Width - 10),
                    rand.Next(10, konfettiPanel.Height - 10),
                    rand.Next(10, 22),
                    konfettiFarver[rand.Next(konfettiFarver.Length)]
                ));
            }
            konfettiPanel.Visible = true;
            konfettiPanel.BringToFront();
            await Task.Delay(1500);
            konfettiPanel.Visible = false;
            jfPanel.BringToFront();
        }

        private void KonfettiPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (var k in konfetti)
                using (var b = new SolidBrush(k.c))
                    e.Graphics.FillEllipse(b, k.x, k.y, k.size, k.size);
        }

        private void JfPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rect = new Rectangle(1, 1, jfPanel.Width - 3, jfPanel.Height - 3);
            using (var pen = new Pen(Color.MediumSlateBlue, 2))
                g.DrawRectangle(pen, rect);
            for (int i = 0; i < 12; i++)
            {
                int px = rand.Next(jfPanel.Width);
                int py = rand.Next(jfPanel.Height);
                using (var b = new SolidBrush(konfettiFarver[rand.Next(konfettiFarver.Length)]))
                    g.FillEllipse(b, px, py, 3, 3);
            }
        }

        private HashSet<string> GetComputerNamesFromAD()
        {
            try
            {
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (DirectoryEntry entry = new DirectoryEntry(settings.AdPath))
                using (DirectorySearcher searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = "(objectCategory=computer)";
                    searcher.PropertiesToLoad.Add("name");
                    searcher.PageSize = 1000;
                    foreach (SearchResult result in searcher.FindAll())
                    {
                        if (result.Properties.Contains("name"))
                        {
                            var nameObj = result.Properties["name"][0];
                            if (nameObj is string name && !string.IsNullOrEmpty(name))
                                names.Add(name);
                        }
                    }
                }
                return names;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to Active Directory using the path:\n" + settings.AdPath +
                    "\n\nError: " + ex.Message +
                    "\n\nPlease check your settings.",
                    "AD Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private List<string> GetNextPCNames(string prefix, int digits, int amount)
        {
            var usedNames = GetComputerNamesFromAD();
            if (usedNames == null)
                return null;
            if (usedNames.Count == 0)
                return new List<string>(); // AD virker men ingen computere fundet

            var available = new List<string>();
            for (int i = 1; i < 1000 && available.Count < amount; i++)
            {
                string name = $"{prefix}{i.ToString().PadLeft(digits, '0')}";
                if (!usedNames.Contains(name, StringComparer.OrdinalIgnoreCase))
                    available.Add(name);
            }
            return available;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

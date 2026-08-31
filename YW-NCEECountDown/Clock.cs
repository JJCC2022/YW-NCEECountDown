using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCEECountDown
{
    public partial class NCEECountDown : Form
    {
        int mode = 5;//倒计时显示模式
        int ChangeTime = 5;//每多少分钟换皮肤m
        int RunTime = 0;//run已经运行时长ms
        bool Class = false;//联动上课状态
        string[] NameList;
        int NameListLines = 0;
        int FormMode = 2;//窗口模式
        bool Mini = false;
        List<string> SubFolderPaths;
        private Point TargetLoc => new Point(Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2, this.Height / 2);

        string H;
        string M;
        string S;
        string C;
        double percent;

        //拖动窗口V2
        bool IsMouseDown = false;
        Point PointMouse = new Point();
        Point Location0 = new Point();
        Point Location1 = new Point();
        Point Location2 = new Point();
        int MoveDistanceX;
        int MoveDistanceY;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        public NCEECountDown()
        {
            InitializeComponent();
        }

        private List<string> GetSubFolders(string folderPath)
        {
            List<string> subFolders = new List<string>();
            try
            {
                string[] Directories = Directory.GetDirectories(folderPath);
                subFolders.AddRange(Directories);
            }
            catch (Exception) { }
            return subFolders;
        }

        /* 
         自定义规则：（X为顺序正整数，文件夹命名为1、2、3、...、X）
        label1:AppDomain.CurrentDomain.BaseDirectory + "Signs.txt"
        TTPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Title.png"
        LPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Left.png"
        RPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Right.png"
        this.BackColor:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\BGC.ini"（RGB三行，范围0-255，UTF-8编码）
        下个版本要做！窗体宽度进一步适配；程序位置保存；字体自动还原。
         */
        private async void NCEECountDown_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            this.Size = new Size(Time.Width * 7 / 4, Time.Height + Title.Height + label1.Height * 7 / 4);
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2, -this.Height);
            label1.Left = this.Width / 2 - label1.Width / 2;
            label1.Top = Time.Bottom;
            progressBar1.Width = this.Width;
            Title.Location = new Point(this.Width / 2 - Title.Width / 2, 0);
            Time.Location = new Point(this.Width / 2 - Time.Width / 2, Title.Bottom);
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Signs.txt"))
            {
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "Signs.txt").Close();
            }
            //Launch.Start();
            await FadeInAsync();
            Title.Text = "距 离 高 考 还 有";
            ThemeChange();
            run.Start();
            Launch.Stop();
            Status(sender, e);
            if (File.Exists(Thread.GetDomain().BaseDirectory + "Signs.txt"))
            {
                Stopwatch sw = new Stopwatch();
                var path = Thread.GetDomain().BaseDirectory + "Signs.txt";
                int lines = 0;
                sw.Restart();
                using (var sr = new StreamReader(path))
                {
                    var ls = "";
                    while ((ls = sr.ReadLine()) != null)
                    {
                        lines++;
                    }
                }
                sw.Stop();
                NameListLines = lines;
                NameList = File.ReadAllLines(Thread.GetDomain().BaseDirectory + "Signs.txt");
                try
                {
                    if (File.Exists(Thread.GetDomain().BaseDirectory + "NCEEConfig.ini"))
                    {
                        var ini = new IniReader(Thread.GetDomain().BaseDirectory + "NCEEConfig.ini");
                        mode = ini.GetInt("mode", 1);
                        FormMode = ini.GetInt("FormMode", 2);
                        //Anchor
                    }
                }
                catch (Exception) { }
            }
            while (!this.IsDisposed && !this.Disposing)
            {
                if (!Mini)
                {
                    ThemeChange();
                    if (NameListLines != 0)
                    {
                        int RanNum = new Random().Next(0, NameListLines);
                        label1.Text = NameList[RanNum];
                    }

                    // 使用 Task.Delay 替代同步阻塞或 Task.Run+Sleep，避免占用线程池与阻塞 UI
                    int delayMs = Math.Max(0, ChangeTime) * 60 * 1000;
                    if (delayMs == 0) delayMs = 1000; // 防止意外的 0 值导致紧凑循环
                    await Task.Delay(delayMs);
                }
                else
                {
                    // Mini 模式下每 200ms 检查一次状态，避免忙等
                    await Task.Delay(200);
                }
            }
        }

        private async void run_Tick(object sender, EventArgs e)
        {
            RunTime += run.Interval;
            TimeSpan ts = GetTimeUntilJune7();
            int days = ts.Days;
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;
            H = days.ToString();
            M = hours < 10 & hours > 0 ? "0" + hours : hours.ToString();
            S = minutes < 10 ? "0" + minutes : minutes.ToString();
            C = seconds < 10 ? "0" + seconds : seconds.ToString();
            percent = Math.Round((double)(days * 1440 * 60 + hours * 60 * 60 + minutes * 60 + seconds) / 5256 / 60, 2);
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJClock.ini");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "JJClock.ini"))
            {
                Class = true;
                if (Mini & FormMode == 1)
                {
                    Mini = false;
                    LPic.Visible = true;
                    RPic.Visible = true;
                    Time.Visible = true;
                    Title.Visible = true;
                    ThemeChange();
                }
            }
            else
            {
                Class = false;
                if (Opacity <= 0) await FadeInAsync();
            }
            if (Class & FormMode == 1)
            {
                if (this.Opacity >= 1) await FadeOutAsync();
            }
            else if (this.Location != TargetLoc & !Running & !Mini & FormMode == 1) await FadeInAsync();
            Status(sender, e);
            int R = 10;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(this.Width - R, this.Height - R, R, R, 0, 90);
            path.AddArc(0, this.Height - R, R, R, 90, 90);
            path.AddArc(0, 0, R, R, 180, 90);
            path.AddArc(this.Width - R, 0, R, R, 270, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);
        }
        bool Running = false;
        private async Task FadeInAsync()
        {
            if (Running) { label1.Text = "警告：程序负载过高"; return; }
            Running = true;
            try
            {
                if (this.Opacity <= 0)
                {
                    this.Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2, -this.Height);
                    if (NameListLines != 0)
                    {
                        int RanNum = new Random().Next(0, NameListLines);
                        label1.Text = NameList[RanNum];
                    }
                }
                while (this.Top < this.Height / 2 || this.Opacity < 1.0)
                {
                    if (this.Location != TargetLoc)
                    {
                        this.Top = Math.Min(this.Height / 2, this.Top + 5);
                        await Task.Delay(5);
                    }
                    if (this.Opacity < 1.0)
                    {
                        this.Opacity = Math.Min(1.0, this.Opacity + 0.01);
                    }
                    await Task.Delay(5);
                }
            }
            finally { Running = false; }
        }

        private async Task FadeOutAsync()
        {
            if (Running) { label1.Text = "警告：程序负载过高"; return; }
            Running = true;
            try
            {
                while (this.Opacity > 0)
                {
                    this.Opacity = Math.Max(0.0, this.Opacity - 0.01);
                    await Task.Delay(5);
                }
            }
            finally { Running = false; }

        }

        private TimeSpan GetTimeUntilJune7()
        {
            DateTime now = DateTime.Now;
            int year = now.Year;
            DateTime target = new DateTime(year, 6, 7, 0, 0, 0);
            if (now >= target)
            {
                target = target.AddYears(1);
            }
            return target - now;
        }

        private void NCEECountDown_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F4) && (e.Alt == true))
            {
                e.Handled = true;//seewo特调
            }
        }

        private async void Launch_Tick(object sender, EventArgs e)
        {
            await FadeInAsync();
            run.Start();
            Launch.Stop();
            Status(sender, e);
            if (File.Exists(Thread.GetDomain().BaseDirectory + "Signs.txt"))
            {
                Stopwatch sw = new Stopwatch();
                var path = Thread.GetDomain().BaseDirectory + "Signs.txt";
                int lines = 0;
                sw.Restart();
                using (var sr = new StreamReader(path))
                {
                    var ls = "";
                    while ((ls = sr.ReadLine()) != null)
                    {
                        lines++;
                    }
                }
                sw.Stop();
                NameListLines = lines;
                NameList = File.ReadAllLines(Thread.GetDomain().BaseDirectory + "Signs.txt");
            }
        }
        private int PrevFormMode = -1;
        private void Status(object sender, EventArgs e)//UI适配
        {
            if (mode == 1)
            {
                Time.Text = H + "天";
            }
            if (mode == 2)
            {
                Time.Text = H + "天(" + percent.ToString() + "%)";
            }
            if (mode == 3)
            {
                Time.Text = H + "天" + M + "时";
            }
            if (mode == 4)
            {
                Time.Text = H + "天" + M + "时(" + percent.ToString() + "%)";
            }
            if (mode == 5)
            {
                Time.Text = H + "天" + M + "时" + S + "分";
            }
            if (mode == 6)
            {
                Time.Text = H + "天" + M + "时" + S + "分(" + percent.ToString() + "%)";
            }
            if (mode == 7)
            {
                Time.Text = H + "天" + M + "时" + S + "分" + C + "秒";
            }
            if (mode == 8)
            {
                Time.Text = H + "天" + M + "时" + S + "分" + C + "秒(" + percent.ToString() + "%)";
            }
            int MaxWidth = Math.Max(Time.Width * 3 / 2, Title.Width * 4 / 3);
            if (Mini)
            {

            }
            else
            {
                this.Size = new Size(Math.Max(MaxWidth + 30, label1.Width + LPic.Width + RPic.Width + 30), Time.Height + Title.Height + label1.Height * 7 / 4);
                //label1.Text = this.Location.ToString() + "/" + TargetLoc.ToString();测试
                if (this.Location != TargetLoc & FormMode == 1 & this.Opacity == 1)
                {
                    this.Location = TargetLoc;
                }
            }
            label1.Left = this.Width / 2 - label1.Width / 2;
            label1.Top = Time.Bottom;
            progressBar1.Width = this.Width;
            Title.Location = new Point(this.Width / 2 - Title.Width / 2, 10);
            TTPic.Size = Title.Size;
            TTPic.Location = Title.Location;
            Time.Location = new Point(this.Width / 2 - Time.Width / 2, Title.Bottom);
            if (PrevFormMode != FormMode)
            {
                if (FormMode == 1)
                {
                    SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
                else if (FormMode == 2)
                {
                    SetWindowPos(this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
                }
                PrevFormMode = FormMode;
            }
            //if (FormMode == 1) SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            //else if (FormMode == 2) SetWindowPos(this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MouseEventArgs Mouse_e = (MouseEventArgs)e;
            if (Mouse_e.Button == MouseButtons.Left)
            {
                if (NameListLines != 0)
                {
                    int RanNum = new Random().Next(0, NameListLines);
                    label1.Text = NameList[RanNum];
                }
            }
            else if (Mouse_e.Button == MouseButtons.Right)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Signs.txt"))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Signs.txt");
                }
            }
        }

        private void Time_Click(object sender, EventArgs e)
        {
            MouseEventArgs Mouse_e = (MouseEventArgs)e;
            if (Mouse_e.Button == MouseButtons.Left)
            {
                mode += 1;
                if (mode == 9) mode = 1;
                Status(sender, e);
            }
            else if (Mouse_e.Button == MouseButtons.Right)
            {
                Process.Start("https://github.com/JJCC2022/YW-NCEECountDown/blob/master/README.md");
            }
        }

        private void NCEECountDown_Click(object sender, EventArgs e)
        {
        }

        private void LPic_Click(object sender, EventArgs e)
        {
            MouseEventArgs Mouse_e = (MouseEventArgs)e;
            if (Mouse_e.Button == MouseButtons.Left)
            {
                ThemeChange();
            }
            else if (Mouse_e.Button == MouseButtons.Right)
            {
                ThemeEdit();
            }
        }

        private void RPic_Click(object sender, EventArgs e)
        {
            LPic_Click(sender, e);
        }

        private void Title_Click(object sender, EventArgs e)
        {
            MouseEventArgs Mouse_e = (MouseEventArgs)e;
            if (Mouse_e.Button == MouseButtons.Left)
            {
                ThemeChange();
            }
            else if (Mouse_e.Button == MouseButtons.Right)
            {
                ThemeEdit();
            }
        }

        private void TTPic_Click(object sender, EventArgs e)
        {
            Title_Click(sender, e);
        }
        PrivateFontCollection PrivateFonts = new PrivateFontCollection();
        int TheNum = 0;
        private void ThemeChange()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Bg\\1"))
            {
                SubFolderPaths = GetSubFolders(AppDomain.CurrentDomain.BaseDirectory + "Bg");
                if (SubFolderPaths.Count != 0)
                {
                    if (TheNum < SubFolderPaths.Count) TheNum += 1;
                    else TheNum = 1;
                    string LeftPath = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\Left.png";
                    string RightPath = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\Right.png";
                    string BGC = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\BGC.ini";
                    string TTP = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\Title.png";
                    string OMNI = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\OMN.ico";
                    string FTU = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (TheNum) + "\\Font.ttf";
                    if (File.Exists(LeftPath))
                    {
                        LPic.Image = Image.FromFile(LeftPath);
                    }
                    else LPic.Image = null;
                    if (File.Exists(RightPath))
                    {
                        RPic.Image = Image.FromFile(RightPath);
                    }
                    else RPic.Image = null;
                    if (File.Exists(OMNI))
                    {
                        OMN.Image = Image.FromFile(OMNI);
                    }
                    else { OMN.Image = this.Icon.ToBitmap(); }
                    if (File.Exists(TTP))
                    {
                        TTPic.Visible = true;
                        TTPic.Image = Image.FromFile(TTP);
                    }
                    else TTPic.Visible = false;
                    if (File.Exists(FTU))
                    {
                        PrivateFonts = new PrivateFontCollection();
                        PrivateFonts.AddFontFile(FTU);
                        var family = PrivateFonts.Families.FirstOrDefault();
                        if (family != null)
                        {
                            Time.Font = new Font(family, Time.Font.Size, Time.Font.Style);
                        }
                    }

                    if (File.Exists(BGC))
                    {
                        string colorStr = File.ReadAllText(BGC).Trim();
                        var rawLines = File.ReadAllLines(BGC);
                        var rgb = new List<int>();
                        foreach (var raw in rawLines)
                        {
                            var s = (raw ?? string.Empty).Trim();
                            if (string.IsNullOrEmpty(s)) continue;
                            int v;
                            if (!int.TryParse(s, out v))
                            {
                                // 非数字则视为无效，直接放弃解析
                                rgb.Clear();
                                break;
                            }
                            // 限定范围 0-255
                            if (v < 0) v = 0;
                            if (v > 255) v = 255;
                            rgb.Add(v);
                            if (rgb.Count == 3) break; // 只需要前三个有效行
                        }
                        if (rgb.Count == 3)
                        {
                            this.BackgroundImage = null;
                            this.BackColor = Color.FromArgb(255, rgb[0], rgb[1], rgb[2]);
                        }
                        else
                        {
                            Random rnd = new Random();
                            var color = Color.FromArgb(255, rnd.Next(50, 216), rnd.Next(50, 216), rnd.Next(50, 216));
                            this.BackgroundImage = null;
                            this.BackColor = color;
                        }
                    }
                    else
                    {
                        Random rnd = new Random();
                        var color = Color.FromArgb(255, rnd.Next(50, 216), rnd.Next(50, 216), rnd.Next(50, 216));
                        this.BackgroundImage = null;
                        this.BackColor = color;
                    }
                }
            }
            else
            {
                Random rnd = new Random();
                var color = Color.FromArgb(255, rnd.Next(50, 216), rnd.Next(50, 216), rnd.Next(50, 216));
                this.BackgroundImage = null;
                this.BackColor = color;
            }
        }

        private void ThemeEdit()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Bg"))
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Bg");
            }
            else
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Bg\\1");
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "Bg\\1");
            }
        }

        private void OMN_MouseDown(object sender, MouseEventArgs e)
        {
            PointMouse = e.Location;
            Location1 = this.Location;
            Location2 = this.Location;
            if (FormMode == 2 || Mini) IsMouseDown = true;
            MoveDistanceX = 0;
            MoveDistanceY = 0;
        }

        private void OMN_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                Location0 = this.PointToScreen(e.Location);
                Location0.Offset(-PointMouse.X, -PointMouse.Y);
                Location2 = this.Location;
                this.Location = Location0;
            }
        }

        private void OMN_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
            MoveDistanceX = Math.Abs(Location2.X - Location1.X);
            MoveDistanceY = Math.Abs(Location2.Y - Location1.Y);
            if (MoveDistanceX + MoveDistanceY <= 20)
            {
                MouseEventArgs Mouse_e = (MouseEventArgs)e;
                if (Mouse_e.Button == MouseButtons.Left)
                {
                    if (!Mini)
                    {
                        Mini = true;
                        this.Size = new Size(OMN.Right + OMN.Left, OMN.Bottom + OMN.Top);//测
                        LPic.Visible = false;
                        RPic.Visible = false;
                        Time.Visible = false;
                        Title.Visible = false;
                        TTPic.Visible = false;
                    }
                    else
                    {
                        Mini = false;
                        LPic.Visible = true;
                        RPic.Visible = true;
                        Time.Visible = true;
                        Title.Visible = true;
                        ThemeChange();
                    }
                }
                else if (Mouse_e.Button == MouseButtons.Right)
                {
                    if (FormMode <= 1) FormMode += 1;
                    else FormMode = 1;
                }
            }
        }

        private void OMN_DoubleClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NCEECountDown_FormClosing(object sender, FormClosingEventArgs e)
        {
            //处理配置保存操作
            try
            {
                if (File.Exists(Thread.GetDomain().BaseDirectory + "NCEEConfig.ini"))
                {
                    File.Delete(Thread.GetDomain().BaseDirectory + "NCEEConfig.ini");
                }

                StringBuilder iniContent = new StringBuilder();
                iniContent.AppendLine("[Pinned]这是高考倒计时的配置文件，请不要随意更改以防程序出现不稳定问题~");
                iniContent.AppendLine("[Pinned]Application=" + Thread.GetDomain().BaseDirectory + Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName));
                iniContent.AppendLine("[Pinned]Version=" + Assembly.GetExecutingAssembly().GetName().Version);
                iniContent.AppendLine("[Pinned]CreateTime=" + DateTime.Now.ToFileTime().ToString());
                iniContent.AppendLine("mode=" + mode);
                iniContent.AppendLine("FormMode=" + FormMode);
                //iniContent.AppendLine("AnchorX=" + ？);取左右距边界最小值，用正负区分
                //iniContent.AppendLine("AnchorY=" + ？);取上下距边界最小值，用正负区分
                File.WriteAllText(Thread.GetDomain().BaseDirectory + "NCEEConfig.ini", iniContent.ToString(), Encoding.UTF8);
            }
            catch (Exception)
            {

            }
        }

        public class IniReader
        {
            private Dictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public IniReader(string filePath)
            {
                if (File.Exists(filePath))
                {
                    foreach (var line in File.ReadAllLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith(";")))
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                            _data[parts[0].Trim()] = parts[1].Trim().Trim('"');
                    }
                }
            }

            public string this[string key] => _data.TryGetValue(key, out var v) ? v : null;
            public string Get(string key, string def = null) => _data.TryGetValue(key, out var v) ? v : def;
            public int GetInt(string key, int def = 0) => int.TryParse(Get(key), out var v) ? v : def;
            public bool GetBool(string key, bool def = false)
            {
                var val = Get(key)?.ToLower();
                if (val == "true" || val == "1" || val == "yes" || val == "on") return true;
                if (val == "false" || val == "0" || val == "no" || val == "off") return false;
                return def;
            }
            public DateTime GetDateTime(string key, DateTime defaultValue)
            {
                string value = Get(key);
                if (string.IsNullOrEmpty(value))
                    return defaultValue;
                if (long.TryParse(value, out long fileTime))
                {
                    try
                    {
                        return DateTime.FromFileTime(fileTime);
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
                return defaultValue;
            }
        }

    }
}

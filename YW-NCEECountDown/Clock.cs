using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NCEECountDown
{
    public partial class NCEECountDown : Form
    {
        int mode = 5;
        bool Class = false;
        string[] NameList;
        int NameListLines = 0;
        int Mode = 1;
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

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWCP_ROUND = 3;

        private List<string> GetSubFolders(string folderPath)
        {
            List<string> subFolders = new List<string>();
            try
            {
                string[] Directories = Directory.GetDirectories(folderPath);
                subFolders.AddRange(Directories);
            }
            catch (Exception ex) { }
            return subFolders;
        }

        /* 
         自定义规则：（X为顺序正整数，文件夹命名为1、2、3、...、X）
        label1:AppDomain.CurrentDomain.BaseDirectory + "Signs.txt"
        TTPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Title.png"
        LPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Left.png"
        RPic:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\Right.png"
        this.BackColor:AppDomain.CurrentDomain.BaseDirectory + "Bg\\X\\BGC.ini"（RGB三行，范围0-255，UTF-8编码）
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
            ThemeChange();
            //Launch.Start();
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

        private async void run_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = GetTimeUntilJune7();
            int days = ts.Days;
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;
            H = days.ToString();
            M = hours < 10 & hours > 0 ? "0" + hours : hours.ToString();
            S = minutes < 10 ? "0" + minutes : minutes.ToString();
            C = seconds < 10 ? "0" + seconds : seconds.ToString();
            percent = Math.Round((double)(days * 1440 * 60 + hours * 60 * 60 + minutes * 60 + seconds) / 5256 / 60, 5);
            string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JJClock.ini");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "JJClock.ini"))
            {
                Class = true;
                if (Mini & Mode == 1)
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
                if(Opacity<=0)await FadeInAsync();
            }
            if (Class & Mode == 1)
            {
                if (this.Opacity > 0) this.Opacity -= 0.1;
                else
                {
                    ThemeChange();
                    if (NameListLines != 0)
                    {
                        int RanNum = new Random().Next(0, NameListLines);
                        label1.Text = NameList[RanNum];
                    }
                }
            }
            else if (this.Location != TargetLoc & !Running & !Mini & Mode == 1) await FadeInAsync();
            Status(sender, e);
            int cornerPreference = DWMWCP_ROUND;
            DwmSetWindowAttribute(this.Handle, DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreference, sizeof(int));
        }
        bool Running = false;
        private async Task FadeInAsync()
        {
            if (Running) { label1.Text = "警告：程序负载过高"; return; }
            Running = true;
            try
            {
                if (this.Opacity <= 0) this.Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2, -this.Height);
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

        private void FadeOut(object sender, EventArgs e)
        {
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
                Time.Text = H + "天" + M + "时" + S + "分";
            }
            if (mode == 4)
            {
                Time.Text = H + "天" + M + "时" + S + "分(" + percent.ToString() + "%)";
            }
            if (mode == 5)
            {
                Time.Text = H + "天" + M + "时" + S + "分" + C + "秒";
            }
            if (mode == 6)
            {
                Time.Text = H + "天" + M + "时" + S + "分" + C + "秒(" + percent.ToString() + "%)";
            }
            int MaxWidth = Math.Max(Time.Width * 4 / 3, Title.Width * 4 / 3);
            if (Mini)
            {

            }
            else
            {
                this.Size = new Size(Math.Max(MaxWidth + 30, label1.Width + LPic.Width + RPic.Width + 30), Time.Height + Title.Height + label1.Height * 7 / 4);
                //label1.Text = this.Location.ToString() + "/" + TargetLoc.ToString();测试
                if (this.Location != TargetLoc & Mode == 1&this.Opacity==1)
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

            if (Mode == 1) SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
            else if (Mode == 2) SetWindowPos(this.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
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
                if (mode == 7) mode = 1;
                Status(sender, e);
            }
            else if (Mouse_e.Button == MouseButtons.Right)
            {
                Process.Start("https://github.com/JJCC2022/YW-NCEECountDown/blob/main/README.md");
            }
        }

        private void NCEECountDown_Click(object sender, EventArgs e)
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

        private void LPic_Click(object sender, EventArgs e)
        {
            NCEECountDown_Click(sender, e);
        }

        private void RPic_Click(object sender, EventArgs e)
        {
            NCEECountDown_Click(sender, e);
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

        private void ThemeChange()
        {
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Bg\\1"))
            {
                SubFolderPaths = GetSubFolders(AppDomain.CurrentDomain.BaseDirectory + "Bg");
                Random BgRan = new Random();
                if (SubFolderPaths.Count != 0)
                {
                    int RanNum = BgRan.Next(0, SubFolderPaths.Count);
                    string LeftPath = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\Left.png";
                    string RightPath = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\Right.png";
                    string BGC = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\BGC.ini";
                    string TTP = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\Title.png";
                    string OMNI = AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\OMN.ico";
                    string FTU= AppDomain.CurrentDomain.BaseDirectory + "Bg\\" + (RanNum + 1) + "\\Font.ttf";
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
                        OMN.BackColor = Color.Transparent;
                    }
                    else { OMN.Image = null; OMN.BackColor = Color.DarkRed; }
                    if (File.Exists(TTP))
                    {
                        TTPic.Visible = true;
                        TTPic.Image = Image.FromFile(TTP);
                    }
                    else TTPic.Visible = false;
                    if (File.Exists(FTU))
                    {
                        //Time.Font= new Font(new PrivateFontCollection().AddFontFile(FTU).Families[0], Time.Font.Size, Time.Font.Style);
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
            if (Mode == 2 || Mini) IsMouseDown = true;
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
                    if (Mode <= 1) Mode += 1;
                    else Mode = 1;
                }
            }
        }

        private void OMN_DoubleClick(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

namespace NCEECountDown
{
    partial class NCEECountDown
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Time = new System.Windows.Forms.Label();
            this.run = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Launch = new System.Windows.Forms.Timer(this.components);
            this.Title = new System.Windows.Forms.Label();
            this.LPic = new System.Windows.Forms.PictureBox();
            this.RPic = new System.Windows.Forms.PictureBox();
            this.TTPic = new System.Windows.Forms.PictureBox();
            this.OMN = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TTPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OMN)).BeginInit();
            this.SuspendLayout();
            // 
            // Time
            // 
            this.Time.AutoSize = true;
            this.Time.BackColor = System.Drawing.Color.Transparent;
            this.Time.Cursor = System.Windows.Forms.Cursors.Default;
            this.Time.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Time.Location = new System.Drawing.Point(64, 93);
            this.Time.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(508, 80);
            this.Time.TabIndex = 0;
            this.Time.Text = "XXX天XX时XX分";
            this.Time.Click += new System.EventHandler(this.Time_Click);
            // 
            // run
            // 
            this.run.Interval = 200;
            this.run.Tick += new System.EventHandler(this.run_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(184, 184);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 30);
            this.label1.TabIndex = 6;
            this.label1.Text = "奋勇拼搏，决战高考";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar1.Maximum = 4000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(649, 10);
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // Launch
            // 
            this.Launch.Interval = 1;
            this.Launch.Tick += new System.EventHandler(this.Launch_Tick);
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Cursor = System.Windows.Forms.Cursors.Default;
            this.Title.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Title.Location = new System.Drawing.Point(64, 13);
            this.Title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(484, 80);
            this.Title.TabIndex = 10;
            this.Title.Text = "距 离 高 考 仅 剩";
            this.Title.Click += new System.EventHandler(this.Title_Click);
            // 
            // LPic
            // 
            this.LPic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LPic.BackColor = System.Drawing.Color.Transparent;
            this.LPic.Location = new System.Drawing.Point(12, 114);
            this.LPic.Name = "LPic";
            this.LPic.Size = new System.Drawing.Size(100, 100);
            this.LPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LPic.TabIndex = 11;
            this.LPic.TabStop = false;
            this.LPic.Click += new System.EventHandler(this.LPic_Click);
            // 
            // RPic
            // 
            this.RPic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RPic.BackColor = System.Drawing.Color.Transparent;
            this.RPic.Location = new System.Drawing.Point(533, 114);
            this.RPic.Name = "RPic";
            this.RPic.Size = new System.Drawing.Size(100, 100);
            this.RPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.RPic.TabIndex = 12;
            this.RPic.TabStop = false;
            this.RPic.Click += new System.EventHandler(this.RPic_Click);
            // 
            // TTPic
            // 
            this.TTPic.BackColor = System.Drawing.Color.Transparent;
            this.TTPic.Location = new System.Drawing.Point(78, 16);
            this.TTPic.Name = "TTPic";
            this.TTPic.Size = new System.Drawing.Size(448, 61);
            this.TTPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TTPic.TabIndex = 13;
            this.TTPic.TabStop = false;
            this.TTPic.Visible = false;
            this.TTPic.Click += new System.EventHandler(this.TTPic_Click);
            // 
            // OMN
            // 
            this.OMN.BackColor = System.Drawing.Color.Maroon;
            this.OMN.Location = new System.Drawing.Point(12, 12);
            this.OMN.Name = "OMN";
            this.OMN.Size = new System.Drawing.Size(24, 24);
            this.OMN.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OMN.TabIndex = 14;
            this.OMN.TabStop = false;
            this.OMN.DoubleClick += new System.EventHandler(this.OMN_DoubleClick);
            this.OMN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OMN_MouseDown);
            this.OMN.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OMN_MouseMove);
            this.OMN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OMN_MouseUp);
            // 
            // NCEECountDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(645, 223);
            this.Controls.Add(this.OMN);
            this.Controls.Add(this.TTPic);
            this.Controls.Add(this.RPic);
            this.Controls.Add(this.LPic);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.progressBar1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "NCEECountDown";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.Text = "高考倒计时";
            this.Load += new System.EventHandler(this.NCEECountDown_Load);
            this.Click += new System.EventHandler(this.NCEECountDown_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NCEECountDown_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.LPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TTPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OMN)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Time;
        private System.Windows.Forms.Timer run;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer Launch;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.PictureBox LPic;
        private System.Windows.Forms.PictureBox RPic;
        private System.Windows.Forms.PictureBox TTPic;
        private System.Windows.Forms.PictureBox OMN;
    }
}


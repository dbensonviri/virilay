namespace ConfigManager
{
    partial class Configurator
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this._username = new System.Windows.Forms.TextBox();
            this._opacity = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this._fadetime = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._savebutton = new System.Windows.Forms.Button();
            this._outputdirection = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this._outputcount = new System.Windows.Forms.NumericUpDown();
            this._cancelbutton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._startwidth = new System.Windows.Forms.NumericUpDown();
            this._fontsize = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this._oauth1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._oauth3 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this._oauth2 = new System.Windows.Forms.TextBox();
            this.clientidget = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this._opacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._fadetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._outputcount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._startwidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._fontsize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Channel";
            // 
            // _username
            // 
            this._username.Location = new System.Drawing.Point(16, 63);
            this._username.Margin = new System.Windows.Forms.Padding(4);
            this._username.Name = "_username";
            this._username.Size = new System.Drawing.Size(240, 31);
            this._username.TabIndex = 7;
            // 
            // _opacity
            // 
            this._opacity.Location = new System.Drawing.Point(16, 217);
            this._opacity.Margin = new System.Windows.Forms.Padding(4);
            this._opacity.Name = "_opacity";
            this._opacity.Size = new System.Drawing.Size(150, 31);
            this._opacity.TabIndex = 5;
            this._opacity.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 188);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Opacity";
            // 
            // _fadetime
            // 
            this._fadetime.Location = new System.Drawing.Point(178, 217);
            this._fadetime.Margin = new System.Windows.Forms.Padding(4);
            this._fadetime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._fadetime.Name = "_fadetime";
            this._fadetime.Size = new System.Drawing.Size(150, 31);
            this._fadetime.TabIndex = 6;
            this._fadetime.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this._fadetime.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(172, 188);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Fade Time";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 31);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 25);
            this.label5.TabIndex = 9;
            this.label5.Text = "Output Count";
            // 
            // _savebutton
            // 
            this._savebutton.Location = new System.Drawing.Point(426, 302);
            this._savebutton.Margin = new System.Windows.Forms.Padding(4);
            this._savebutton.Name = "_savebutton";
            this._savebutton.Size = new System.Drawing.Size(112, 37);
            this._savebutton.TabIndex = 12;
            this._savebutton.Text = "Save";
            this._savebutton.UseVisualStyleBackColor = true;
            this._savebutton.Click += new System.EventHandler(this._savebutton_Click);
            // 
            // _outputdirection
            // 
            this._outputdirection.DisplayMember = "0";
            this._outputdirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._outputdirection.FormattingEnabled = true;
            this._outputdirection.Items.AddRange(new object[] {
            "up",
            "down"});
            this._outputdirection.Location = new System.Drawing.Point(178, 62);
            this._outputdirection.Margin = new System.Windows.Forms.Padding(4);
            this._outputdirection.MaxDropDownItems = 2;
            this._outputdirection.Name = "_outputdirection";
            this._outputdirection.Size = new System.Drawing.Size(148, 33);
            this._outputdirection.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(167, 25);
            this.label6.TabIndex = 13;
            this.label6.Text = "Output Direction";
            // 
            // _outputcount
            // 
            this._outputcount.Location = new System.Drawing.Point(16, 60);
            this._outputcount.Margin = new System.Windows.Forms.Padding(4);
            this._outputcount.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this._outputcount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._outputcount.Name = "_outputcount";
            this._outputcount.Size = new System.Drawing.Size(150, 31);
            this._outputcount.TabIndex = 1;
            this._outputcount.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this._outputcount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _cancelbutton
            // 
            this._cancelbutton.Location = new System.Drawing.Point(538, 302);
            this._cancelbutton.Margin = new System.Windows.Forms.Padding(4);
            this._cancelbutton.Name = "_cancelbutton";
            this._cancelbutton.Size = new System.Drawing.Size(112, 37);
            this._cancelbutton.TabIndex = 15;
            this._cancelbutton.Text = "Quit";
            this._cancelbutton.UseVisualStyleBackColor = true;
            this._cancelbutton.Click += new System.EventHandler(this._cancelbutton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 108);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 25);
            this.label2.TabIndex = 17;
            this.label2.Text = "Width";
            // 
            // _startwidth
            // 
            this._startwidth.Location = new System.Drawing.Point(16, 137);
            this._startwidth.Margin = new System.Windows.Forms.Padding(4);
            this._startwidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this._startwidth.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this._startwidth.Name = "_startwidth";
            this._startwidth.Size = new System.Drawing.Size(150, 31);
            this._startwidth.TabIndex = 3;
            this._startwidth.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this._startwidth.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // _fontsize
            // 
            this._fontsize.Location = new System.Drawing.Point(178, 138);
            this._fontsize.Margin = new System.Windows.Forms.Padding(4);
            this._fontsize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._fontsize.Name = "_fontsize";
            this._fontsize.Size = new System.Drawing.Size(150, 31);
            this._fontsize.TabIndex = 4;
            this._fontsize.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this._fontsize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(172, 106);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(103, 25);
            this.label8.TabIndex = 21;
            this.label8.Text = "Font Size";
            // 
            // _oauth1
            // 
            this._oauth1.Location = new System.Drawing.Point(106, 140);
            this._oauth1.Margin = new System.Windows.Forms.Padding(4);
            this._oauth1.Name = "_oauth1";
            this._oauth1.Size = new System.Drawing.Size(148, 31);
            this._oauth1.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 140);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 37);
            this.button1.TabIndex = 8;
            this.button1.Text = "Get";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 188);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(142, 25);
            this.label9.TabIndex = 27;
            this.label9.Text = "AccessToken";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._oauth3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this._oauth2);
            this.groupBox1.Controls.Add(this.clientidget);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._username);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this._oauth1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(22, 23);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(288, 344);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Twitch";
            // 
            // _oauth3
            // 
            this._oauth3.Location = new System.Drawing.Point(106, 277);
            this._oauth3.Margin = new System.Windows.Forms.Padding(6);
            this._oauth3.Name = "_oauth3";
            this._oauth3.ReadOnly = true;
            this._oauth3.Size = new System.Drawing.Size(152, 31);
            this._oauth3.TabIndex = 32;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(20, 273);
            this.button2.Margin = new System.Windows.Forms.Padding(6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 44);
            this.button2.TabIndex = 31;
            this.button2.Text = "Get";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 106);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 25);
            this.label10.TabIndex = 30;
            this.label10.Text = "OAuth";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // _oauth2
            // 
            this._oauth2.Location = new System.Drawing.Point(106, 227);
            this._oauth2.Margin = new System.Windows.Forms.Padding(6);
            this._oauth2.Name = "_oauth2";
            this._oauth2.Size = new System.Drawing.Size(150, 31);
            this._oauth2.TabIndex = 29;
            // 
            // clientidget
            // 
            this.clientidget.Location = new System.Drawing.Point(18, 223);
            this.clientidget.Margin = new System.Windows.Forms.Padding(6);
            this.clientidget.Name = "clientidget";
            this.clientidget.Size = new System.Drawing.Size(84, 44);
            this.clientidget.TabIndex = 28;
            this.clientidget.Text = "Get";
            this.clientidget.UseVisualStyleBackColor = true;
            this.clientidget.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this._outputcount);
            this.groupBox2.Controls.Add(this._fontsize);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this._fadetime);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this._outputdirection);
            this.groupBox2.Controls.Add(this._startwidth);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this._opacity);
            this.groupBox2.Location = new System.Drawing.Point(322, 23);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox2.Size = new System.Drawing.Size(342, 267);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Virilay";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(332, 296);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(68, 25);
            this.linkLabel2.TabIndex = 32;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "About";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(333, 321);
            this.linkLabel3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(56, 25);
            this.linkLabel3.TabIndex = 33;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Help";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // Configurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 387);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._cancelbutton);
            this.Controls.Add(this._savebutton);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Configurator";
            this.Text = "Configurator";
            ((System.ComponentModel.ISupportInitialize)(this._opacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._fadetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._outputcount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._startwidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._fontsize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _username;
        private System.Windows.Forms.NumericUpDown _opacity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown _fadetime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button _savebutton;
        public System.Windows.Forms.ComboBox _outputdirection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown _outputcount;
        private System.Windows.Forms.Button _cancelbutton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown _startwidth;
        private System.Windows.Forms.NumericUpDown _fontsize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _oauth1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button clientidget;
        private System.Windows.Forms.TextBox _oauth2;
        private System.Windows.Forms.TextBox _oauth3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
    }
}


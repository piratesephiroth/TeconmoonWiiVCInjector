namespace TeconMoon_s_WiiVC_Injector
{
    partial class SDCardMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDCardMenu));
            this.DriveBox = new System.Windows.Forms.ComboBox();
            this.SDCardText = new System.Windows.Forms.Label();
            this.ReloadDrives = new System.Windows.Forms.Button();
            this.NintendontUpdate = new System.Windows.Forms.Button();
            this.ActionStatus = new System.Windows.Forms.Label();
            this.GenerateConfig = new System.Windows.Forms.Button();
            this.LanguageBox = new System.Windows.Forms.ComboBox();
            this.LanguageText = new System.Windows.Forms.Label();
            this.VideoText = new System.Windows.Forms.Label();
            this.VideoForceMode = new System.Windows.Forms.ComboBox();
            this.VideoTypeMode = new System.Windows.Forms.ComboBox();
            this.VideoWidth = new System.Windows.Forms.TrackBar();
            this.VideoWidthText = new System.Windows.Forms.Label();
            this.WidthNumber = new System.Windows.Forms.Label();
            this.MemcardText = new System.Windows.Forms.Label();
            this.MemcardBlocks = new System.Windows.Forms.ComboBox();
            this.MemcardMulti = new System.Windows.Forms.CheckBox();
            this.NintendontOptions = new System.Windows.Forms.CheckedListBox();
            this.Format = new System.Windows.Forms.LinkLabel();
            this.gamepadSlotLabel = new System.Windows.Forms.Label();
            this.wiiUGamepadSlotBox = new System.Windows.Forms.ComboBox();
            this.nintendontConfigGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.VideoWidth)).BeginInit();
            this.nintendontConfigGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // DriveBox
            // 
            this.DriveBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DriveBox.FormattingEnabled = true;
            this.DriveBox.Location = new System.Drawing.Point(14, 29);
            this.DriveBox.Name = "DriveBox";
            this.DriveBox.Size = new System.Drawing.Size(253, 21);
            this.DriveBox.TabIndex = 0;
            this.DriveBox.SelectedIndexChanged += new System.EventHandler(this.DriveBox_SelectedIndexChanged);
            // 
            // SDCardText
            // 
            this.SDCardText.Location = new System.Drawing.Point(11, 9);
            this.SDCardText.Name = "SDCardText";
            this.SDCardText.Size = new System.Drawing.Size(145, 17);
            this.SDCardText.TabIndex = 1;
            this.SDCardText.Text = "Choose your SD Card";
            // 
            // ReloadDrives
            // 
            this.ReloadDrives.AutoSize = true;
            this.ReloadDrives.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ReloadDrives.Location = new System.Drawing.Point(14, 56);
            this.ReloadDrives.Name = "ReloadDrives";
            this.ReloadDrives.Size = new System.Drawing.Size(98, 23);
            this.ReloadDrives.TabIndex = 1;
            this.ReloadDrives.Text = "Reload Drive List";
            this.ReloadDrives.UseVisualStyleBackColor = true;
            this.ReloadDrives.Click += new System.EventHandler(this.ReloadDrives_Click);
            // 
            // NintendontUpdate
            // 
            this.NintendontUpdate.AutoSize = true;
            this.NintendontUpdate.Location = new System.Drawing.Point(14, 103);
            this.NintendontUpdate.Name = "NintendontUpdate";
            this.NintendontUpdate.Size = new System.Drawing.Size(253, 28);
            this.NintendontUpdate.TabIndex = 3;
            this.NintendontUpdate.Text = "Download Latest Nintendont from GitHub";
            this.NintendontUpdate.UseVisualStyleBackColor = true;
            this.NintendontUpdate.Click += new System.EventHandler(this.NintendontUpdate_Click);
            // 
            // ActionStatus
            // 
            this.ActionStatus.Location = new System.Drawing.Point(5, 82);
            this.ActionStatus.Name = "ActionStatus";
            this.ActionStatus.Size = new System.Drawing.Size(253, 20);
            this.ActionStatus.TabIndex = 5;
            this.ActionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GenerateConfig
            // 
            this.GenerateConfig.AutoSize = true;
            this.GenerateConfig.Location = new System.Drawing.Point(4, 323);
            this.GenerateConfig.Name = "GenerateConfig";
            this.GenerateConfig.Size = new System.Drawing.Size(253, 28);
            this.GenerateConfig.TabIndex = 12;
            this.GenerateConfig.Text = "Generate Nintendont Config File (nincfg.bin)";
            this.GenerateConfig.UseVisualStyleBackColor = true;
            this.GenerateConfig.Click += new System.EventHandler(this.GenerateConfig_Click);
            // 
            // LanguageBox
            // 
            this.LanguageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageBox.FormattingEnabled = true;
            this.LanguageBox.Items.AddRange(new object[] {
            "Automatic",
            "English",
            "German",
            "French",
            "Spanish",
            "Italian",
            "Dutch"});
            this.LanguageBox.Location = new System.Drawing.Point(6, 236);
            this.LanguageBox.Name = "LanguageBox";
            this.LanguageBox.Size = new System.Drawing.Size(124, 21);
            this.LanguageBox.TabIndex = 9;
            // 
            // LanguageText
            // 
            this.LanguageText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LanguageText.Location = new System.Drawing.Point(6, 213);
            this.LanguageText.Name = "LanguageText";
            this.LanguageText.Size = new System.Drawing.Size(124, 18);
            this.LanguageText.TabIndex = 16;
            this.LanguageText.Text = "Language";
            this.LanguageText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VideoText
            // 
            this.VideoText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VideoText.Location = new System.Drawing.Point(135, 136);
            this.VideoText.Name = "VideoText";
            this.VideoText.Size = new System.Drawing.Size(124, 18);
            this.VideoText.TabIndex = 15;
            this.VideoText.Text = "Video Mode";
            this.VideoText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VideoForceMode
            // 
            this.VideoForceMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoForceMode.FormattingEnabled = true;
            this.VideoForceMode.Items.AddRange(new object[] {
            "Auto",
            "Force",
            "Force (Deflicker)",
            "None"});
            this.VideoForceMode.Location = new System.Drawing.Point(136, 157);
            this.VideoForceMode.Name = "VideoForceMode";
            this.VideoForceMode.Size = new System.Drawing.Size(123, 21);
            this.VideoForceMode.TabIndex = 6;
            this.VideoForceMode.SelectedIndexChanged += new System.EventHandler(this.VideoForceMode_SelectedIndexChanged);
            // 
            // VideoTypeMode
            // 
            this.VideoTypeMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VideoTypeMode.FormattingEnabled = true;
            this.VideoTypeMode.Items.AddRange(new object[] {
            "Auto",
            "NTSC",
            "MPAL",
            "PAL50",
            "PAL60"});
            this.VideoTypeMode.Location = new System.Drawing.Point(136, 184);
            this.VideoTypeMode.Name = "VideoTypeMode";
            this.VideoTypeMode.Size = new System.Drawing.Size(123, 21);
            this.VideoTypeMode.TabIndex = 7;
            this.VideoTypeMode.SelectedIndexChanged += new System.EventHandler(this.VideoTypeMode_SelectedIndexChanged);
            // 
            // VideoWidth
            // 
            this.VideoWidth.AutoSize = false;
            this.VideoWidth.BackColor = System.Drawing.SystemColors.Control;
            this.VideoWidth.Enabled = false;
            this.VideoWidth.LargeChange = 10;
            this.VideoWidth.Location = new System.Drawing.Point(136, 234);
            this.VideoWidth.Maximum = 720;
            this.VideoWidth.Minimum = 640;
            this.VideoWidth.Name = "VideoWidth";
            this.VideoWidth.Size = new System.Drawing.Size(123, 26);
            this.VideoWidth.SmallChange = 2;
            this.VideoWidth.TabIndex = 10;
            this.VideoWidth.TickFrequency = 2;
            this.VideoWidth.Value = 640;
            this.VideoWidth.Scroll += new System.EventHandler(this.VideoWidth_Scroll);
            // 
            // VideoWidthText
            // 
            this.VideoWidthText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.VideoWidthText.Enabled = false;
            this.VideoWidthText.Location = new System.Drawing.Point(136, 213);
            this.VideoWidthText.Name = "VideoWidthText";
            this.VideoWidthText.Size = new System.Drawing.Size(123, 18);
            this.VideoWidthText.TabIndex = 17;
            this.VideoWidthText.Text = "Video Width";
            this.VideoWidthText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WidthNumber
            // 
            this.WidthNumber.Location = new System.Drawing.Point(132, 229);
            this.WidthNumber.Name = "WidthNumber";
            this.WidthNumber.Size = new System.Drawing.Size(127, 14);
            this.WidthNumber.TabIndex = 15;
            this.WidthNumber.Text = "Auto";
            this.WidthNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MemcardText
            // 
            this.MemcardText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MemcardText.Location = new System.Drawing.Point(6, 136);
            this.MemcardText.Name = "MemcardText";
            this.MemcardText.Size = new System.Drawing.Size(124, 18);
            this.MemcardText.TabIndex = 14;
            this.MemcardText.Text = "Memcard Blocks";
            this.MemcardText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MemcardBlocks
            // 
            this.MemcardBlocks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MemcardBlocks.FormattingEnabled = true;
            this.MemcardBlocks.Items.AddRange(new object[] {
            "59",
            "123",
            "251",
            "507 (Unstable)",
            "1019 (Unstable)",
            "2043 (Unstable)"});
            this.MemcardBlocks.Location = new System.Drawing.Point(6, 157);
            this.MemcardBlocks.Name = "MemcardBlocks";
            this.MemcardBlocks.Size = new System.Drawing.Size(124, 21);
            this.MemcardBlocks.TabIndex = 5;
            // 
            // MemcardMulti
            // 
            this.MemcardMulti.Location = new System.Drawing.Point(6, 184);
            this.MemcardMulti.Name = "MemcardMulti";
            this.MemcardMulti.Size = new System.Drawing.Size(124, 26);
            this.MemcardMulti.TabIndex = 8;
            this.MemcardMulti.Text = "Memcard Multi";
            this.MemcardMulti.UseVisualStyleBackColor = false;
            // 
            // NintendontOptions
            // 
            this.NintendontOptions.CheckOnClick = true;
            this.NintendontOptions.FormattingEnabled = true;
            this.NintendontOptions.Items.AddRange(new object[] {
            "Memcard Emulation",
            "Cheats",
            "Cheat Path",
            "Unlock Read Speed",
            "Wiimote CC Rumble",
            "TRI Arcade Mode",
            "BBA Emulation",
            "Auto Video Width",
            "Patch PAL50",
            "Force Widescreen",
            "Force Progressive",
            "Skip IPL",
            "OSReport",
            "Log"});
            this.NintendontOptions.Location = new System.Drawing.Point(6, 19);
            this.NintendontOptions.MultiColumn = true;
            this.NintendontOptions.Name = "NintendontOptions";
            this.NintendontOptions.Size = new System.Drawing.Size(253, 109);
            this.NintendontOptions.TabIndex = 4;
            this.NintendontOptions.SelectedIndexChanged += new System.EventHandler(this.NintendontOptions_SelectedIndexChanged);
            this.NintendontOptions.DoubleClick += new System.EventHandler(this.NintendontOptions_DoubleClick);
            // 
            // Format
            // 
            this.Format.AutoSize = true;
            this.Format.Location = new System.Drawing.Point(118, 61);
            this.Format.Name = "Format";
            this.Format.Size = new System.Drawing.Size(155, 13);
            this.Format.TabIndex = 2;
            this.Format.TabStop = true;
            this.Format.Text = "Use this to format your SD Card";
            this.Format.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Format_LinkClicked);
            // 
            // gamepadSlotLabel
            // 
            this.gamepadSlotLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gamepadSlotLabel.Location = new System.Drawing.Point(6, 267);
            this.gamepadSlotLabel.Name = "gamepadSlotLabel";
            this.gamepadSlotLabel.Size = new System.Drawing.Size(124, 18);
            this.gamepadSlotLabel.TabIndex = 18;
            this.gamepadSlotLabel.Text = "Wii U Gamepad Slot";
            this.gamepadSlotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // wiiUGamepadSlotBox
            // 
            this.wiiUGamepadSlotBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.wiiUGamepadSlotBox.FormattingEnabled = true;
            this.wiiUGamepadSlotBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.wiiUGamepadSlotBox.Location = new System.Drawing.Point(6, 290);
            this.wiiUGamepadSlotBox.Name = "wiiUGamepadSlotBox";
            this.wiiUGamepadSlotBox.Size = new System.Drawing.Size(124, 21);
            this.wiiUGamepadSlotBox.TabIndex = 11;
            // 
            // nintendontConfigGroupBox
            // 
            this.nintendontConfigGroupBox.Controls.Add(this.NintendontOptions);
            this.nintendontConfigGroupBox.Controls.Add(this.wiiUGamepadSlotBox);
            this.nintendontConfigGroupBox.Controls.Add(this.GenerateConfig);
            this.nintendontConfigGroupBox.Controls.Add(this.gamepadSlotLabel);
            this.nintendontConfigGroupBox.Controls.Add(this.LanguageBox);
            this.nintendontConfigGroupBox.Controls.Add(this.LanguageText);
            this.nintendontConfigGroupBox.Controls.Add(this.MemcardMulti);
            this.nintendontConfigGroupBox.Controls.Add(this.VideoForceMode);
            this.nintendontConfigGroupBox.Controls.Add(this.MemcardBlocks);
            this.nintendontConfigGroupBox.Controls.Add(this.VideoText);
            this.nintendontConfigGroupBox.Controls.Add(this.MemcardText);
            this.nintendontConfigGroupBox.Controls.Add(this.VideoTypeMode);
            this.nintendontConfigGroupBox.Controls.Add(this.WidthNumber);
            this.nintendontConfigGroupBox.Controls.Add(this.VideoWidth);
            this.nintendontConfigGroupBox.Controls.Add(this.VideoWidthText);
            this.nintendontConfigGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nintendontConfigGroupBox.Location = new System.Drawing.Point(8, 139);
            this.nintendontConfigGroupBox.Name = "nintendontConfigGroupBox";
            this.nintendontConfigGroupBox.Size = new System.Drawing.Size(265, 361);
            this.nintendontConfigGroupBox.TabIndex = 13;
            this.nintendontConfigGroupBox.TabStop = false;
            this.nintendontConfigGroupBox.Text = "Nintendont Configuration";
            // 
            // SDCardMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(280, 512);
            this.Controls.Add(this.nintendontConfigGroupBox);
            this.Controls.Add(this.Format);
            this.Controls.Add(this.ActionStatus);
            this.Controls.Add(this.NintendontUpdate);
            this.Controls.Add(this.ReloadDrives);
            this.Controls.Add(this.SDCardText);
            this.Controls.Add(this.DriveBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDCardMenu";
            this.ShowInTaskbar = false;
            this.Text = "Nintendont SD Card Menu...";
            this.Load += new System.EventHandler(this.SDCardMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.VideoWidth)).EndInit();
            this.nintendontConfigGroupBox.ResumeLayout(false);
            this.nintendontConfigGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox DriveBox;
        private System.Windows.Forms.Label SDCardText;
        private System.Windows.Forms.Button ReloadDrives;
        private System.Windows.Forms.Button NintendontUpdate;
        private System.Windows.Forms.Label ActionStatus;
        private System.Windows.Forms.Button GenerateConfig;
        private System.Windows.Forms.ComboBox LanguageBox;
        private System.Windows.Forms.Label LanguageText;
        private System.Windows.Forms.Label VideoText;
        private System.Windows.Forms.ComboBox VideoForceMode;
        private System.Windows.Forms.ComboBox VideoTypeMode;
        private System.Windows.Forms.TrackBar VideoWidth;
        private System.Windows.Forms.Label VideoWidthText;
        private System.Windows.Forms.Label WidthNumber;
        private System.Windows.Forms.Label MemcardText;
        private System.Windows.Forms.ComboBox MemcardBlocks;
        private System.Windows.Forms.CheckBox MemcardMulti;
        private System.Windows.Forms.CheckedListBox NintendontOptions;
        private System.Windows.Forms.LinkLabel Format;
        private System.Windows.Forms.Label gamepadSlotLabel;
        private System.Windows.Forms.ComboBox wiiUGamepadSlotBox;
        private System.Windows.Forms.GroupBox nintendontConfigGroupBox;
    }
}
using TeconMoon_s_WiiVC_Injector.Properties;

namespace TeconMoon_s_WiiVC_Injector
{
    partial class SettingsForm
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
            this.BannersRepositoryLabel = new System.Windows.Forms.Label();
            this.BannersRepository = new System.Windows.Forms.TextBox();
            this.OutputDirLabel = new System.Windows.Forms.Label();
            this.OutputDir = new System.Windows.Forms.TextBox();
            this.OutputDirButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BannersRepositoryLabel
            // 
            this.BannersRepositoryLabel.Location = new System.Drawing.Point(12, 9);
            this.BannersRepositoryLabel.Name = "BannersRepositoryLabel";
            this.BannersRepositoryLabel.Size = new System.Drawing.Size(118, 16);
            this.BannersRepositoryLabel.TabIndex = 0;
            this.BannersRepositoryLabel.Text = "Banners Repository";
            // 
            // BannersRepository
            // 
            this.BannersRepository.Location = new System.Drawing.Point(12, 28);
            this.BannersRepository.Name = "BannersRepository";
            this.BannersRepository.Size = new System.Drawing.Size(345, 20);
            this.BannersRepository.TabIndex = 1;
            this.BannersRepository.Text = global::TeconMoon_s_WiiVC_Injector.Properties.Settings.Default.BannersRepository;
            // 
            // OutputDirLabel
            // 
            this.OutputDirLabel.Location = new System.Drawing.Point(12, 60);
            this.OutputDirLabel.Name = "OutputDirLabel";
            this.OutputDirLabel.Size = new System.Drawing.Size(262, 15);
            this.OutputDirLabel.TabIndex = 2;
            this.OutputDirLabel.Text = "Output Directory (leave empty to always ask)";
            // 
            // OutputDir
            // 
            this.OutputDir.Location = new System.Drawing.Point(12, 78);
            this.OutputDir.Name = "OutputDir";
            this.OutputDir.Size = new System.Drawing.Size(255, 20);
            this.OutputDir.TabIndex = 3;
            this.OutputDir.Text = global::TeconMoon_s_WiiVC_Injector.Properties.Settings.Default.OutputPathFixed;
            // 
            // OutputDirButton
            // 
            this.OutputDirButton.Location = new System.Drawing.Point(273, 74);
            this.OutputDirButton.Name = "OutputDirButton";
            this.OutputDirButton.Size = new System.Drawing.Size(84, 26);
            this.OutputDirButton.TabIndex = 4;
            this.OutputDirButton.Text = "Browse...";
            this.OutputDirButton.UseVisualStyleBackColor = false;
            this.OutputDirButton.Click += new System.EventHandler(this.OutputDirButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(183, 257);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(84, 26);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = false;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(273, 257);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(84, 26);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(369, 295);
            this.Controls.Add(this.OutputDirLabel);
            this.Controls.Add(this.OutputDir);
            this.Controls.Add(this.BannersRepositoryLabel);
            this.Controls.Add(this.BannersRepository);
            this.Controls.Add(this.OutputDirButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "Application Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label BannersRepositoryLabel;
        private System.Windows.Forms.TextBox BannersRepository;
        private System.Windows.Forms.Label OutputDirLabel;
        private System.Windows.Forms.TextBox OutputDir;
        private System.Windows.Forms.Button OutputDirButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button Cancel;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using TeconMoon_s_WiiVC_Injector.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace TeconMoon_s_WiiVC_Injector
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        //Load Drives and set drive variable on load
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            BannersRepository.Text = Settings.Default.BannersRepository;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Settings.Default.BannersRepository = BannersRepository.Text;
            Settings.Default.OutputPathFixed = OutputDir.Text;
            Settings.Default.Save();
            Close();
        }

        private void OutputDirButton_Click(object sender, EventArgs e)
        {
            var outputFolderSelect = new CommonOpenFileDialog("Specify your output folder")
            {
                InitialDirectory = Settings.Default.OutputPath,
                IsFolderPicker = true,
                EnsurePathExists = true
            };

            //Specify Path Variables to be called later
            if (outputFolderSelect.ShowDialog() == CommonFileDialogResult.Ok)
            {
                OutputDir.Text = outputFolderSelect.FileName;
            }
        }
    }
}

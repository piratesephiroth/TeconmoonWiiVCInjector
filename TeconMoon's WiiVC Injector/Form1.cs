﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Media;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using TGASharpLib;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace TeconMoon_s_WiiVC_Injector
{
    public partial class WiiVC_Injector : Form
    {
        public WiiVC_Injector()
        {
            InitializeComponent();
            this.Text = string.Format(this.Text, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //Check for if .Net v3.5 component is installed
            CheckForNet35();
            //Delete Temporary Root Folder if it exists
            if (Directory.Exists(TempRootPath))
            {
                Directory.Delete(TempRootPath, true);
            }
            Directory.CreateDirectory(TempRootPath);
            //Extract Tools to temp folder
            File.WriteAllBytes(TempRootPath + "TOOLDIR.zip", Properties.Resources.TOOLDIR);
            ZipFile.ExtractToDirectory(TempRootPath + "TOOLDIR.zip", TempRootPath);
            File.Delete(TempRootPath + "TOOLDIR.zip");
            //Create Source and Build directories
            Directory.CreateDirectory(TempSourcePath);
            Directory.CreateDirectory(TempBuildPath);
        }

        //Testing
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(String pathName, StringBuilder shortName, int cbShortName);
        public string ShortenPath(string pathtomakesafe)
        {
            StringBuilder sb = new StringBuilder(1000);
            long n = GetShortPathName(pathtomakesafe, sb, 1000);
            if (n == 0) // check for errors
            {
                return Marshal.GetLastWin32Error().ToString();
            }
            else
            {
                return sb.ToString();
            }
        }


        //Specify public variables for later use (ASK ALAN)
        string SystemType = "wii";
        string TitleIDHex;
        string TitleIDText;
        string InternalGameName;
        bool FlagWBFS;
        bool FlagNKIT;
        bool FlagNASOS;
        bool FlagGameSpecified;
        bool FlagGC2Specified;
        bool FlagBootSoundSpecified;
        bool HideProcess = true;
        string CucholixRepoID = "";
        string DRCUSE = "1";
        string nfspatchflag = "";
        string wiimmfiOption = " --wiimmfi";
        ProcessStartInfo Launcher;
        string LauncherExeFile;
        string LauncherExeArgs;
        string JNUSToolDownloads = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\JNUSToolDownloads\\";
        readonly static string TempRootPath = Path.GetTempPath() + "WiiVCInjector\\";
        readonly static string TempSourcePath = TempRootPath + "SOURCETEMP\\";
        readonly string TempBuildPath = TempRootPath + "BUILDDIR\\";
        readonly string TempToolsPath = TempRootPath + "TOOLDIR\\";
        readonly string TempIconPath = TempSourcePath + "iconTex.png";
        readonly string TempBannerPath = TempSourcePath + "bootTvTex.png";
        readonly string TempDrcPath = TempSourcePath + "bootDrcTex.png";
        readonly string TempLogoPath = TempSourcePath + "bootLogoTex.png";
        readonly string TempSoundPath = TempSourcePath + "bootSound.wav";
        string selectedOutputPath;

        //call options
        public void LaunchProgram()
        {
            Launcher = new ProcessStartInfo(LauncherExeFile);
            Launcher.Arguments = LauncherExeArgs;
            if (HideProcess)
            {
                Launcher.WindowStyle = ProcessWindowStyle.Hidden;
            }
            Process.Start(Launcher).WaitForExit();
        }
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public static string GetFullPath(string fileName)
        {
            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
        public void CleanUp()
        {
            var sourceFilesToDelete = Directory.EnumerateFiles(TempSourcePath, "*.*", System.IO.SearchOption.AllDirectories);
            var buildFilesToDelete = Directory.EnumerateFiles(TempBuildPath, "*.*", System.IO.SearchOption.AllDirectories);
            foreach (var file in sourceFilesToDelete)
                File.Delete(file);
            foreach (var file in buildFilesToDelete)
                File.Delete(file);

            IconPreviewBox.Image = null;
            IconSourceDirectory.Text = "Icon file has not been specified";
            IconSourceDirectory.ForeColor = Color.Red;

            BannerPreviewBox.Image = null;
            BannerSourceDirectory.Text = "Banner file has not been specified";
            BannerSourceDirectory.ForeColor = Color.Red;
        }

        public void DownloadFromRepo(string RepoID)
        {
            var client = new WebClient();
            client.BaseAddress = Properties.Settings.Default.BannersRepository + SystemType + "/" + RepoID + "/";

            string filename = "iconTex.png";
            string pathname = TempRootPath + filename;
            if (File.Exists(pathname)) { File.Delete(pathname); }
            client.DownloadFile(filename, pathname);

            IconPreviewBox.Image = ImageSourceLoad(TempIconPath, pathname);
            IconSourceDirectory.Text = filename + "downloaded from Cucholix's Repo";
            IconSourceDirectory.ForeColor = Color.Black;

            filename = "bootTvTex.png";
            pathname = TempRootPath + filename;
            if (File.Exists(pathname)) { File.Delete(pathname); }
            client.DownloadFile(filename, pathname);

            BannerPreviewBox.Image = ImageSourceLoad(TempBannerPath, pathname);
            BannerSourceDirectory.Text = filename + "downloaded from Cucholix's Repo";
            BannerSourceDirectory.ForeColor = Color.Black;

            client.Dispose();
        }

        //Called from RepoDownload_Click to check if files exist before downloading
        private bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
        private void CheckForNet35()
        {
            if (Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v3.5") == null)
            {
                MessageBox.Show(".NET Framework 3.5 was not detected on your machine, which is required by programs used during the build process." +
                                "\n\nYou should be able to enable this in \"Programs and Features\" under \"Turn Windows features on or off\", or download it from Microsoft." +
                                "\n\nClick OK to close the injector and open \"Programs and Features\"...", ".NET Framework v3.5 not found..."
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Exclamation);
                HideProcess = false;
                LauncherExeFile = "appwiz.cpl";
                LauncherExeArgs = "";
                LaunchProgram();
                Environment.Exit(0);
            }
        }

        //Cleanup when program is closed
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // If Windows isn't shutting down, ask the user if they want to close
            if ((e.CloseReason != CloseReason.WindowsShutDown) &&
                (DialogResult.No == MessageBox.Show(this, "Are you sure you want to close?"
                    , "Closing"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Question)))
            {
                e.Cancel = true;
                return;
            }

            //Otherwise try to delete the TempRootPath
            if(Directory.Exists(TempRootPath))
                Directory.Delete(TempRootPath, true);
        }

        private void SystemType_Common_Checked()
        {
            GameSourceDirectory.Text = "Game file has not been specified";
            GameSourceDirectory.ForeColor = Color.Red;
            FlagGameSpecified = false;

            GC2SourceDirectory.Text = "2nd GameCube Disc Image has not been specified";
            GC2SourceDirectory.ForeColor = Color.Red;
            FlagGC2Specified = false;

            GameNameLabel.Text = "";
            TitleIDLabel.Text = "";
            TitleIDHex = "";
            PackedTitleLine1.Text = "";
            PackedTitleIDLine.Text = "";
            CucholixRepoID = "";
            RepoDownload.Enabled = false;
        }

        //Radio Buttons for desired injection type (Check with Alan on having one command to clear variables instead of specifying them all 4 times)
        private void WiiRetail_CheckedChanged(object sender, EventArgs e)
        {
            if (WiiRetail.Checked == false) return;

            SystemType_Common_Checked();

            WiiVMC.Enabled = true;
            Wiimmfi.Enabled = true;
            GameSourceButton.Enabled = true;
            GameSourceButton.Text = "Game...";
            OpenGame.Filter = "Wii Dumps (*.iso,*.wbfs,*.iso.dec)|*.iso;*.wbfs;*.iso.dec";
            SystemType = "wii";
            GC2SourceButton.Enabled = false;
            if (NoGamePadEmu.Checked == false & CCEmu.Checked == false & HorWiiMote.Checked == false & VerWiiMote.Checked == false & ForceCC.Checked == false & ForceNoCC.Checked == false)
            {
                NoGamePadEmu.Checked = true;
                GamePadEmuLayout.Enabled = true;
                DRCUSE = "1";
            }
            Force43NINTENDONT.Checked = false;
            Force43NINTENDONT.Enabled = false;
            ForceInterlacedNINTENDONT.Checked = false;
            ForceInterlacedNINTENDONT.Enabled = false;
            CustomMainDol.Checked = false;
            CustomMainDol.Enabled = false;
            DisableNintendontAutoboot.Checked = false;
            DisableNintendontAutoboot.Enabled = false;
            DisablePassthrough.Checked = false;
            DisablePassthrough.Enabled = false;
            DisableGamePad.Checked = false;
            DisableGamePad.Enabled = false;
            C2WPatchFlag.Checked = false;
            C2WPatchFlag.Enabled = false;
            if (ForceCC.Checked) { DisableTrimming.Checked = false; DisableTrimming.Enabled = false; } else { DisableTrimming.Enabled = true; }
            Force43NAND.Checked = false;
            Force43NAND.Enabled = false;
        }

        private void WiiHomebrew_CheckedChanged(object sender, EventArgs e)
        {
            if (WiiHomebrew.Checked == false) return;
            SystemType_Common_Checked();

            WiiVMC.Checked = false;
            WiiVMC.Enabled = false;
            Wiimmfi.Checked = false;
            Wiimmfi.Enabled = false;
            GameSourceButton.Enabled = true;
            GameSourceButton.Text = "Game...";
            OpenGame.FileName = "boot.dol";
            OpenGame.Filter = "DOL Files (*.dol)|*.dol";
            SystemType = "dol";
            DRCUSE = "65537";
            GC2SourceButton.Enabled = false;
            NoGamePadEmu.Checked = false;
            CCEmu.Checked = false;
            HorWiiMote.Checked = false;
            VerWiiMote.Checked = false;
            ForceCC.Checked = false;
            ForceNoCC.Checked = false;
            GamePadEmuLayout.Enabled = false;
            LRPatch.Checked = false;
            LRPatch.Enabled = false;

            Force43NINTENDONT.Checked = false;
            Force43NINTENDONT.Enabled = false;
            ForceInterlacedNINTENDONT.Checked = false;
            ForceInterlacedNINTENDONT.Enabled = false;
            CustomMainDol.Checked = false;
            CustomMainDol.Enabled = false;
            DisableNintendontAutoboot.Checked = false;
            DisableNintendontAutoboot.Enabled = false;

            DisablePassthrough.Enabled = true;
            DisableGamePad.Enabled = true;
            C2WPatchFlag.Enabled = true;
            DisableTrimming.Checked = false;
            DisableTrimming.Enabled = false;
            Force43NAND.Checked = false;
            Force43NAND.Enabled = false;
        }

        private void WiiNAND_CheckedChanged(object sender, EventArgs e)
        {
            if (WiiNAND.Checked == false) return;
            SystemType_Common_Checked();

            WiiVMC.Checked = false;
            WiiVMC.Enabled = false;
            Wiimmfi.Checked = false;
            Wiimmfi.Enabled = false;
            GameSourceButton.Enabled = false;
            GameSourceButton.Text = "TitleID...";
            GC2SourceButton.Enabled = false;
            Force43NINTENDONT.Checked = false;
            Force43NINTENDONT.Enabled = false;
            ForceInterlacedNINTENDONT.Checked = false;
            ForceInterlacedNINTENDONT.Enabled = false;
            CustomMainDol.Checked = false;
            CustomMainDol.Enabled = false;
            DisableNintendontAutoboot.Checked = false;
            DisableNintendontAutoboot.Enabled = false;
            DisablePassthrough.Checked = false;
            DisablePassthrough.Enabled = false;
            DisableGamePad.Checked = false;
            DisableGamePad.Enabled = false;
            C2WPatchFlag.Checked = false;
            C2WPatchFlag.Enabled = false;
            DisableTrimming.Checked = false;
            DisableTrimming.Enabled = false;
            Force43NAND.Enabled = true;
            if (NoGamePadEmu.Checked == false & CCEmu.Checked == false & HorWiiMote.Checked == false & VerWiiMote.Checked == false & ForceCC.Checked == false & ForceNoCC.Checked == false)
            {
                NoGamePadEmu.Checked = true;
                GamePadEmuLayout.Enabled = true;
                DRCUSE = "1";
            }

            /* Loop until the loop is broken by entering a blank ID or 4 characters */
            while (true)
            {
                GameSourceDirectory.Text = Microsoft.VisualBasic.Interaction.InputBox("Enter your installed Wii Channel's 4-letter Title ID. If you don't know it, open a WAD for the channel in something like ShowMiiWads to view it.", "Enter your WAD's Title ID", "XXXX", 0, 0);

                if (GameSourceDirectory.Text.Length == 0)
                {
                    GameSourceDirectory.Text = "Title ID specification cancelled, reselect vWii NAND Title Launcher to specify";
                    return;
                }
                else if (GameSourceDirectory.Text.Length == 4)
                {
                    GameSourceDirectory.ForeColor = Color.Black;
                    GameSourceDirectory.Text = GameSourceDirectory.Text.ToUpper();
                    FlagGameSpecified = true;

                    SystemType = "wiiware";
                    GameNameLabel.Text = "N/A";
                    TitleIDLabel.Text = "N/A";
                    TitleIDText = GameSourceDirectory.Text;
                    CucholixRepoID = GameSourceDirectory.Text;
                    RepoDownload.Enabled = true;

                    char[] HexIDBuild = GameSourceDirectory.Text.ToCharArray();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (char c in HexIDBuild)
                    {
                        stringBuilder.Append(((Int16)c).ToString("X"));
                    }
                    PackedTitleIDLine.Text = "00050002" + stringBuilder.ToString();

                    return;
                }
                else
                {
                    MessageBox.Show("Only 4 characters can be used, try again. Example: The Star Fox 64 (USA) Channel's Title ID is NADE01, so you would specify NADE as the Title ID"
                                    , "Invalid Title ID"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Warning);
                    /* Keep looping */
                }
            }
        }

        private void GCRetail_CheckedChanged(object sender, EventArgs e)
        {
            if (GCRetail.Checked == false) return;

            SystemType_Common_Checked();
            WiiVMC.Checked = false;
            WiiVMC.Enabled = false;
            Wiimmfi.Checked = false;
            Wiimmfi.Enabled = false;
            GameSourceButton.Enabled = true;
            GameSourceButton.Text = "Game...";
            OpenGame.Filter = "GameCube Dumps (*.gcm,*.iso)|*.gcm;*.iso";
            SystemType = "gcn";
            DRCUSE = "65537";
            GC2SourceButton.Enabled = true;
            NoGamePadEmu.Checked = false;
            CCEmu.Checked = false;
            HorWiiMote.Checked = false;
            VerWiiMote.Checked = false;
            ForceCC.Checked = false;
            ForceNoCC.Checked = false;
            GamePadEmuLayout.Enabled = false;
            LRPatch.Checked = false;
            LRPatch.Enabled = false;
            Force43NINTENDONT.Enabled = true;
            ForceInterlacedNINTENDONT.Enabled = true;
            CustomMainDol.Enabled = true;
            DisableNintendontAutoboot.Enabled = true;
            DisablePassthrough.Checked = false;
            DisablePassthrough.Enabled = false;
            DisableGamePad.Enabled = true;
            C2WPatchFlag.Checked = false;
            C2WPatchFlag.Enabled = false;
            DisableTrimming.Checked = false;
            DisableTrimming.Enabled = false;
            Force43NAND.Checked = false;
            Force43NAND.Enabled = false;
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog(this);
            }
        }
        private void SDCardStuff_Click(object sender, EventArgs e)
        {
            new SDCardMenu().Show();
        }

        /* Takes in a text box and the correct hash and performs the pattern
         * that was being used for each. Returns the result so that further 
         * actions can be taken based on it.
         */
        private bool HashTest(TextBox box, string goodHash)
        {
            bool savedHash = (Registry.CurrentUser.CreateSubKey("WiiVCInjector").GetValue(box.Name) != null);

            if (savedHash)
                box.Text = Registry.CurrentUser.OpenSubKey("WiiVCInjector").GetValue(box.Name).ToString();

            box.Text = box.Text.ToUpper();
            byte[] tmpSource = Encoding.ASCII.GetBytes(box.Text);
            byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            string boxHash = BitConverter.ToString(tmpHash);
            bool rv = (boxHash == goodHash);

            /* Clean up the text box */
            box.ReadOnly = rv;
            box.BackColor = rv ? Color.Lime : Color.White;

            /* If we're good then save it */
            if(!savedHash)
            {
                if (rv)
                {
                    Registry.CurrentUser.CreateSubKey("WiiVCInjector").SetValue(box.Name, box.Text);
                    Registry.CurrentUser.CreateSubKey("WiiVCInjector").Close();

                    MessageBox.Show("The " + box.Name + " has been verified."
                    , "Success"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The " + box.Name + " you have provided is incorrect" + "\n" + "(MD5 Hash verification failed)"
                                    , "Invalid " + box.Name
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
                }
            }

            return rv;
        }

        //Performs actions when switching tabs
        private void MainTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Disables Radio buttons when switching away from the main tab
            if (MainTabs.SelectedTab == SourceFilesTab)
            {
                WiiRetail.Enabled = true;
                WiiHomebrew.Enabled = true;
                WiiNAND.Enabled = true;
                GCRetail.Enabled = true;
            }
            else
            {
                WiiRetail.Enabled = false;
                WiiHomebrew.Enabled = false;
                WiiNAND.Enabled = false;
                GCRetail.Enabled = false;
            }

            //If we're not on the build tab, there's nothing else to do here.
            if (MainTabs.SelectedTab != BuildTab) return;

            HashTest(WiiUCommonKey, "35-AC-59-94-97-22-79-33-1D-97-09-4F-A2-FB-97-FC");
            HashTest(TitleKey, "F9-4B-D8-8E-BB-7A-A9-38-67-E6-30-61-5F-27-1C-9F");

            //Final check for if all requirements are good

            bool BuildFlagSource = (FlagGameSpecified && (IconPreviewBox.Image != null) && (BannerPreviewBox.Image != null));
            SourceCheck.ForeColor = BuildFlagSource ? Color.Green : Color.Red;

            bool BuildFlagMeta = (PackedTitleLine1.Text != "" & PackedTitleIDLine.TextLength == 16);
            MetaCheck.ForeColor = BuildFlagMeta ? Color.Green : Color.Red;

            bool BuildFlagAdvance;
            if (CustomMainDol.Checked == false)
                BuildFlagAdvance = true;
            else
                BuildFlagAdvance = (Path.GetExtension(OpenMainDol.FileName) == ".dol");

            AdvanceCheck.ForeColor = BuildFlagAdvance ? Color.Green : Color.Red;

            bool BuildFlagKeys = (WiiUCommonKey.ReadOnly && TitleKey.ReadOnly && (!C2WPatchFlag.Checked || AncastKey.ReadOnly));
            KeysCheck.ForeColor = BuildFlagKeys ? Color.Green : Color.Red;

            //Enable Build Button
            TheBigOneTM.Enabled = (BuildFlagSource && BuildFlagMeta & BuildFlagAdvance && BuildFlagKeys);
        }

        private string readerReadString(BinaryReader reader)
        {
            string TempString = "";
            char TempChar;

            while ((int)(TempChar = reader.ReadChar()) != 0)
                TempString = TempString + TempChar;

            return TempString;
        }

        //Events for the "Required Source Files" Tab
        private void GameSourceButton_Click(object sender, EventArgs e)
        {
            int TitleIDInt = 0;
            long GameType = 0;
            if (OpenGame.ShowDialog() == DialogResult.OK)
            {
                // delete any previous files
                CleanUp();

                byte[] idBytes = new byte[4];
                //Get values from game file
                using (var reader = new BinaryReader(File.OpenRead(OpenGame.FileName)))
                {
                    reader.BaseStream.Position = 0;
                    TitleIDInt = reader.ReadInt32();
                    idBytes = BitConverter.GetBytes(TitleIDInt);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(idBytes);
                    }
                    string idString = new string(Encoding.ASCII.GetChars(idBytes));
                    
                    //WBFS Check
                    if (idString == "WBFS") //Performs actions if the header indicates a WBFS file
                    {
                        FlagWBFS = true;

                        reader.BaseStream.Position = 0x200;
                        TitleIDInt = reader.ReadInt32();

                        reader.BaseStream.Position = 0x218;
                        GameType = reader.ReadInt64();

                        reader.BaseStream.Position = 0x220;
                        InternalGameName = readerReadString(reader);

                        reader.BaseStream.Position = 0x200;
                        CucholixRepoID = readerReadString(reader);

                        RepoDownload.Enabled = true;
                    }
                    else
                    {
                        if (TitleIDInt == 65536) //Performs actions if the header indicates a DOL file
                        {
                            reader.BaseStream.Position = 0x2A0;
                            TitleIDInt = reader.ReadInt32();
                            InternalGameName = "N/A";
                        }
                        else //Performs actions if the header indicates a normal Wii / GC iso
                        {
                            FlagWBFS = false;
                            FlagNKIT = false;
                            FlagNASOS = false;
                            uint startOffset = 0;
                            // NASOS check
                            if (idString == "WII5")
                            {
                                FlagNASOS = true;
                                startOffset = 0x1182800;
                            }
                            else if (idString == "WII9")
                            {
                                FlagNASOS = true;
                                startOffset = 0x1FB5000;
                            }
                            // read game info
                            reader.BaseStream.Position = startOffset;
                            TitleIDInt = reader.ReadInt32();

                            reader.BaseStream.Position = startOffset + 0x18;
                            GameType = reader.ReadInt64();

                            reader.BaseStream.Position = startOffset + 0x20;
                            InternalGameName = readerReadString(reader);

                            reader.BaseStream.Position = startOffset + 0x00;
                            CucholixRepoID = readerReadString(reader);

                            RepoDownload.Enabled = true;

                            // NKIT check
                            if (!FlagNASOS)
                            {
                                reader.BaseStream.Position = 0x200;
                                /* TODO: It should be possible to replace this with ReadChars(4), 
                                but would need to be tested. */
                                idBytes = reader.ReadBytes(4);
                                idString = new string(Encoding.ASCII.GetChars(idBytes));
                                if (idString == "NKIT")
                                {
                                    FlagNKIT = true;
                                }
                            }
                        }
                    }
                }

                //Ensure GameType matches SystemType
                if ((SystemType == "wii" && GameType == 2745048157) ||
                    (SystemType == "gcn" && GameType == 4440324665927270400))
                {
                    GameNameLabel.Text = InternalGameName;
                    var GameTitle = StringUtil.RemoveSpecialChars(GameTdb.GetName(CucholixRepoID));
                    PackedTitleLine1.Text = !string.IsNullOrEmpty(GameTitle) ? GameTitle : InternalGameName;
                    //Convert pulled Title ID Int to Hex for use with Wii U Title ID
                    idBytes = BitConverter.GetBytes(TitleIDInt);
                    if (!BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(idBytes);
                    }
                    TitleIDHex = BitConverter.ToString(idBytes).Replace("-", "");
                    PackedTitleIDLine.Text = ("00050002" + TitleIDHex);

                    if (SystemType == "dol")
                    {
                        TitleIDLabel.Text = TitleIDHex;
                        TitleIDText = "BOOT";
                    }
                    else
                    {
                        TitleIDText = string.Join("", System.Text.RegularExpressions.Regex.Split(TitleIDHex, "(?<=\\G..)(?!$)").Select(x => (char)Convert.ToByte(x, 16)));
                        TitleIDLabel.Text = (TitleIDText + " / " + TitleIDHex);
                    }

                    GameSourceDirectory.Text = OpenGame.FileName;
                    GameSourceDirectory.ForeColor = Color.Black;
                    FlagGameSpecified = true;
                    return;
                }
                else
                {
                    MessageBox.Show("This is not a valid " + SystemType + " image. It will not be loaded."
                                    , "Error"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
                    /* Fall through to the fail state on the outside of all the if blocks */
                }
            }

            /* The success state returns on its own, but all others will reach here */
            GameSourceDirectory.Text = "Game file has not been specified";
            GameSourceDirectory.ForeColor = Color.Red;
            FlagGameSpecified = false;
            GameNameLabel.Text = "";
            TitleIDLabel.Text = "";
            TitleIDHex = "";
            CucholixRepoID = "";
            RepoDownload.Enabled = false;
            PackedTitleLine1.Text = "";
            PackedTitleIDLine.Text = "";

            return;
        }

        private Image ImageSourceLoad(string tmpPNG, string filename)
        {
            if (File.Exists(tmpPNG)) { File.Delete(tmpPNG); }

            Image tmpimage;
            if (Path.GetExtension(filename) == ".tga")
                tmpimage = (new TGA(filename)).ToBitmap();
            else
                tmpimage = Image.FromFile(filename);

            tmpimage.Save(tmpPNG, System.Drawing.Imaging.ImageFormat.Png);

            return tmpimage;
        }

        private void IconSourceButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure your icon is 128x128 (1:1) to prevent distortion"
                            , "Icon Size Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
            if (OpenIcon.ShowDialog() == DialogResult.OK)
            {
                IconPreviewBox.Image = ImageSourceLoad(TempIconPath, OpenIcon.FileName);
                IconSourceDirectory.Text = OpenIcon.FileName;
                IconSourceDirectory.ForeColor = Color.Black;
            }
            else
            {
                IconPreviewBox.Image = null;
                IconSourceDirectory.Text = "Icon has not been specified";
                IconSourceDirectory.ForeColor = Color.Red;
            }
        }

        private void BannerSourceButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure your Banner is 1280x720 (16:9) to prevent distortion"
                            , "Banner Size Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
            if (OpenBanner.ShowDialog() == DialogResult.OK)
            {
                BannerPreviewBox.Image = ImageSourceLoad(TempBannerPath, OpenBanner.FileName);
                BannerSourceDirectory.Text = OpenBanner.FileName;
                BannerSourceDirectory.ForeColor = Color.Black;
            }
            else
            {
                BannerPreviewBox.Image = null;
                BannerSourceDirectory.Text = "Banner has not been specified";
                BannerSourceDirectory.ForeColor = Color.Red;
            }
        }
        private void RepoDownload_Click(object sender, EventArgs e)
        {
            IEnumerable<string> ids = GameTdb.GetAlternativeIds(CucholixRepoID);
            foreach (var id in ids)
            {
                if (RemoteFileExists(Properties.Settings.Default.BannersRepository + SystemType + "/" + id + "/iconTex.png"))
                {
                    DownloadFromRepo(id);
                    return;
                }
            }

            if (MessageBox.Show("Cucholix's Repo does not have assets for your game. You will need to provide your own. Would you like to visit the GBAtemp request thread?"
                    , "Game not found on Repo"
                    , MessageBoxButtons.YesNo
                    , MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                Process.Start("https://gbatemp.net/threads/483080/");
            }
        }

        //Events for the "Optional Source Files" Tab
        private void GC2SourceButton_Click(object sender, EventArgs e)
        {
            if (OpenGC2.ShowDialog() == DialogResult.OK)
            {
                using (var reader = new BinaryReader(File.OpenRead(OpenGC2.FileName)))
                {
                    reader.BaseStream.Position = 0x18;
                    long GC2GameType = reader.ReadInt64();
                    if (GC2GameType != 4440324665927270400)
                    {
                        MessageBox.Show("This is not a GameCube image. It will not be loaded."
                                        , "Error"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Error);
                    }
                    else
                    {
                        GC2SourceDirectory.Text = OpenGC2.FileName;
                        GC2SourceDirectory.ForeColor = Color.Black;
                        FlagGC2Specified = true;
                        return;
                    }
                }
            }

            /* Reset to these defaults unless we got the right input and returned already */
            GC2SourceDirectory.Text = "2nd GameCube Disc Image has not been specified";
            GC2SourceDirectory.ForeColor = Color.Red;
            FlagGC2Specified = false;
        }
        private void DrcSourceButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure your GamePad Banner is 854x480 (16:9) to prevent distortion"
                            , "Banner Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
            if (OpenDrc.ShowDialog() == DialogResult.OK)
            {
                DrcPreviewBox.Image = ImageSourceLoad(TempDrcPath, OpenDrc.FileName);
                DrcSourceDirectory.Text = OpenDrc.FileName;
                DrcSourceDirectory.ForeColor = Color.Black;
            }
            else
            {
                DrcPreviewBox.Image = null;
                DrcSourceDirectory.Text = "GamePad Banner has not been specified";
                DrcSourceDirectory.ForeColor = Color.Red;
            }
        }
        private void LogoSourceButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure your Logo is 170x42 to prevent distortion"
                            , "Logo Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
            if (OpenLogo.ShowDialog() == DialogResult.OK)
            {
                LogoPreviewBox.Image = ImageSourceLoad(TempLogoPath, OpenLogo.FileName);
                LogoSourceDirectory.Text = OpenLogo.FileName;
                LogoSourceDirectory.ForeColor = Color.Black;
            }
            else
            {
                LogoPreviewBox.Image = null;
                LogoSourceDirectory.Text = "Boot Logo has not been specified";
                LogoSourceDirectory.ForeColor = Color.Red;
            }
        }
        private void BootSoundButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Your sound file will be cut off if it's longer than 6 seconds to prevent the Wii U from not loading it. When the Wii U plays the boot sound, it will fade out once it's done loading the game (usually after about 5 seconds). You can not change this."
                            , "Boot Sound Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information);
            if (OpenBootSound.ShowDialog() == DialogResult.OK)
            {
                using (var reader = new BinaryReader(File.OpenRead(OpenBootSound.FileName)))
                {
                    reader.BaseStream.Position = 0x00;
                    long WAVHeader1 = reader.ReadInt32();
                    reader.BaseStream.Position = 0x08;
                    long WAVHeader2 = reader.ReadInt32();
                    if (WAVHeader1 == 1179011410 & WAVHeader2 == 1163280727)
                    {
                        BootSoundDirectory.Text = OpenBootSound.FileName;
                        BootSoundDirectory.ForeColor = Color.Black;
                        BootSoundPreviewButton.Enabled = true;
                        FlagBootSoundSpecified = true;
                    }
                    else
                    {
                        MessageBox.Show("This is not a valid WAV file. It will not be loaded. \nConsider converting it with something like Audacity."
                                        , "Not a WAV File"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Error);
                        BootSoundDirectory.Text = "Boot Sound has not been specified";
                        BootSoundDirectory.ForeColor = Color.Red;
                        BootSoundPreviewButton.Enabled = false;
                        FlagBootSoundSpecified = false;
                    }
                }
            }
            else
            {
                if (BootSoundPreviewButton.Text != "Stop Sound")
                {
                    BootSoundDirectory.Text = "Boot Sound has not been specified";
                    BootSoundDirectory.ForeColor = Color.Red;
                    BootSoundPreviewButton.Enabled = false;
                    FlagBootSoundSpecified = false;
                }
            }
        }
        private void BootSoundPreviewButton_Click(object sender, EventArgs e)
        {
            var simpleSound = new SoundPlayer(OpenBootSound.FileName);
            if (BootSoundPreviewButton.Text == "Stop Sound")
            {
                simpleSound.Stop();
                BootSoundPreviewButton.Text = "Play Sound";
            }
            else
            {
                if (ToggleBootSoundLoop.Checked)
                {
                    simpleSound.PlayLooping();
                    BootSoundPreviewButton.Text = "Stop Sound";
                }
                else
                {
                    simpleSound.Play();
                }
            }
        }

        //Events for the "GamePad/Meta Options" Tab
        private void EnablePackedLine2_CheckedChanged(object sender, EventArgs e)
        {
            if (EnablePackedLine2.Checked)
            {
                PackedTitleLine2.Text = "";
                PackedTitleLine2.BackColor = Color.White;
                PackedTitleLine2.ReadOnly = false;
            }
            else
            {
                PackedTitleLine2.Text = "(Optional) Line 2";
                PackedTitleLine2.BackColor = Color.Silver;
                PackedTitleLine2.ReadOnly = true;
            }

        }
        //Radio Buttons for GamePad Emulation Mode
        private void NoGamePadEmu_CheckedChanged(object sender, EventArgs e)
        {
            if (NoGamePadEmu.Checked)
            {
                DRCUSE = "1";
                nfspatchflag = "";
                LRPatch.Checked = false;
                LRPatch.Enabled = false;
            }
        }
        private void CCEmu_CheckedChanged(object sender, EventArgs e)
        {
            if (CCEmu.Checked)
            {
                DRCUSE = "65537";
                nfspatchflag = "";
                LRPatch.Enabled = true;
            }
        }
        private void HorWiiMote_CheckedChanged(object sender, EventArgs e)
        {
            if (HorWiiMote.Checked)
            {
                DRCUSE = "65537";
                nfspatchflag = " -horizontal";
                LRPatch.Checked = false;
                LRPatch.Enabled = false;
            }
        }
        private void VerWiiMote_CheckedChanged(object sender, EventArgs e)
        {
            if (VerWiiMote.Checked)
            {
                DRCUSE = "65537";
                nfspatchflag = " -wiimote";
                LRPatch.Checked = false;
                LRPatch.Enabled = false;
            }
        }
        private void ForceCC_CheckedChanged(object sender, EventArgs e)
        {
            if (ForceCC.Checked)
            {
                DRCUSE = "65537";
                nfspatchflag = " -instantcc";
                DisableTrimming.Checked = false;
                DisableTrimming.Enabled = false;
                LRPatch.Enabled = true;
            }
        }
        private void ForceWiiMote_CheckedChanged(object sender, EventArgs e)
        {
            if (ForceNoCC.Checked)
            {
                DRCUSE = "65537";
                nfspatchflag = " -nocc";
                LRPatch.Checked = false;
                LRPatch.Enabled = false;
            }
        }
        private void TutorialLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.google.com");
        }

        //Events for the Advanced Tab
        private void Force43NINTENDONT_CheckedChanged(object sender, EventArgs e)
        {
            if (Force43NINTENDONT.Checked || ForceInterlacedNINTENDONT.Checked)
            {
                CustomMainDol.Checked = false;
                CustomMainDol.Enabled = false;
                DisableNintendontAutoboot.Checked = false;
                DisableNintendontAutoboot.Enabled = false;
            }
            else
            {
                CustomMainDol.Enabled = true;
                DisableNintendontAutoboot.Enabled = true;
            }
        }
        //ForceInterlacedNINTENDONT.Checked = false;
        //ForceInterlacedNINTENDONT.Enabled = false;
        private void ForceInterlacedNINTENDONT_CheckedChanged(object sender, EventArgs e)
        {
            if (ForceInterlacedNINTENDONT.Checked || Force43NINTENDONT.Checked)
            {
                CustomMainDol.Checked = false;
                CustomMainDol.Enabled = false;
                DisableNintendontAutoboot.Checked = false;
                DisableNintendontAutoboot.Enabled = false;
            }
            else
            {
                CustomMainDol.Enabled = true;
                DisableNintendontAutoboot.Enabled = true;
            }
        }
        private void CustomMainDol_CheckedChanged(object sender, EventArgs e)
        {
            if (CustomMainDol.Checked)
            {
                MainDolSourceButton.Enabled = true;
                MainDolSourceButton.Visible = true;
                MainDolLabel.Text = "<- Specify custom main.dol file";
                MainDolLabel.Visible = true;

                Force43NINTENDONT.Checked = false;
                Force43NINTENDONT.Enabled = false;
                ForceInterlacedNINTENDONT.Checked = false;
                ForceInterlacedNINTENDONT.Enabled = false;
                DisableNintendontAutoboot.Checked = false;
                DisableNintendontAutoboot.Enabled = false;

            }
            else
            {
                MainDolSourceButton.Enabled = false;
                MainDolSourceButton.Visible = false;
                MainDolLabel.ResetText();
                MainDolLabel.Visible = false;

                Force43NINTENDONT.Enabled = true;
                ForceInterlacedNINTENDONT.Enabled = true;
                DisableNintendontAutoboot.Enabled = true;
                OpenMainDol.FileName = null;
            }
        }
        private void NintendontAutoboot_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableNintendontAutoboot.Checked)
            {
                Force43NINTENDONT.Checked = false;
                Force43NINTENDONT.Enabled = false;
                ForceInterlacedNINTENDONT.Checked = false;
                ForceInterlacedNINTENDONT.Enabled = false;
                CustomMainDol.Checked = false;
                CustomMainDol.Enabled = false;
            }
            else
            {
                Force43NINTENDONT.Enabled = true;
                ForceInterlacedNINTENDONT.Enabled = true;
                CustomMainDol.Enabled = true;
            }
        }
        private void MainDolSourceButton_Click(object sender, EventArgs e)
        {
            if (OpenMainDol.ShowDialog() == DialogResult.OK)
            {
                MainDolLabel.Text = OpenMainDol.FileName;
            }
            else
            {
                MainDolLabel.Text = "<- Specify custom main.dol file";
            }
        }
        private void DisableGamePad_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableGamePad.Checked)
            {
                if (SystemType == "gcn")
                {
                    DRCUSE = "1";
                }
                else if (SystemType == "dol")
                {
                    DRCUSE = "1";
                }
            }
            else
            {
                if (SystemType == "gcn")
                {
                    DRCUSE = "65537";
                }
                else if (SystemType == "dol")
                {
                    DRCUSE = "65537";
                }
            }
        }
        private void C2WPatchFlag_CheckedChanged(object sender, EventArgs e)
        {
            if (C2WPatchFlag.Checked)
            {
                SaveAncastKeyButton.Enabled = true;

                //If key is correct, lock text box for edits
                HashTest(AncastKey, "31-8D-1F-9D-98-FB-08-E7-7C-7F-E1-77-AA-49-05-43");
            }
            else
            {
                SaveAncastKeyButton.Enabled = false;
            }
        }
        private void SaveAncastKeyButton_Click(object sender, EventArgs e)
        {
            HashTest(AncastKey, "31-8D-1F-9D-98-FB-08-E7-7C-7F-E1-77-AA-49-05-43");
        }

        private void sign_c2w_patcher_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/FIX94/sign_c2w_patcher");
        }
        private void DisableTrimming_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableTrimming.Checked)
            {
                WiiVMC.Checked = false;
                WiiVMC.Enabled = false;
                Wiimmfi.Checked = false;
                Wiimmfi.Enabled = false;
            }
            else
            {
                if (SystemType == "wii")
                {
                    WiiVMC.Enabled = true;
                    Wiimmfi.Enabled = true;
                }
                else
                {
                    WiiVMC.Checked = false;
                    WiiVMC.Enabled = false;
                    Wiimmfi.Checked = false;
                    Wiimmfi.Enabled = false;
                }
            }
        }

        //Events for the "Build Title" Tab
        private void SaveCommonKeyButton_Click(object sender, EventArgs e)
        {
            HashTest(WiiUCommonKey, "35-AC-59-94-97-22-79-33-1D-97-09-4F-A2-FB-97-FC");
        }

        private void SaveTitleKeyButton_Click(object sender, EventArgs e)
        {
            HashTest(TitleKey, "F9-4B-D8-8E-BB-7A-A9-38-67-E6-30-61-5F-27-1C-9F");
        }

        private void processGCNdisc(string inFile, string outFile)
        {
            if (FlagNKIT)
            {
                if (Directory.Exists(TempToolsPath + "NKIT\\Processed\\Temp"))
                {
                    Directory.Delete(TempToolsPath + "NKIT\\Processed\\Temp", true);
                }
                BuildStatus.Text = "Unscrubbing disc for NFS Conversion...";
                BuildStatus.Refresh();
                LauncherExeFile = TempToolsPath + "NKIT\\ConvertToISO.exe";
                LauncherExeArgs = "\"" + inFile + "\"";
                LaunchProgram(); // CONVERT DISC TO ISO
                File.Move(Directory.GetFiles(TempToolsPath + "NKIT\\Processed\\GameCube_MatchFail", "*.iso")[0], TempSourcePath + "TEMPISOBASE\\files\\" + outFile);
            }
            else
            {
                File.Copy(inFile, TempSourcePath + "TEMPISOBASE\\files\\" + outFile);
            }
        }

        //Events for the actual "Build" Button
        private void TheBigOneTM_Click(object sender, EventArgs e)
        {
            //Initialize Build Process
            //Disable form elements so navigation can't be attempted during build process
            MainTabs.Enabled = false;
            //Check for free space
            var drive = new DriveInfo(TempRootPath);
            long freeSpaceInBytes = drive.AvailableFreeSpace;
            long gamesize = 0;
            /* If wii or gcn, get actual game size */
            if ((SystemType == "wii") || (SystemType == "gcn"))
                gamesize = new FileInfo(OpenGame.FileName).Length;

            if (freeSpaceInBytes < ((gamesize * 2) + 6000000000))
            {
                DialogResult dialogResult = MessageBox.Show("Your hard drive may be low on space. The conversion process involves temporary files" +
                                                            "that can amount to more than double the size of your game + 5GB. If you continue without" +
                                                            "clearing some hard drive space, the conversion may fail. Do you want to continue anyway?",
                                                            "Check your hard drive space"
                                                            , MessageBoxButtons.YesNo
                                                            , MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.No)
                {
                    goto BuildProcessFin;
                }
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.OutputPathFixed))
            {
                selectedOutputPath = Properties.Settings.Default.OutputPathFixed;
            }
            else
            {
                var outputFolderSelect = new CommonOpenFileDialog("Specify your output folder")
                {
                    InitialDirectory = Properties.Settings.Default.OutputPath,
                    IsFolderPicker = true,
                    EnsurePathExists = true
                };

                //Specify Path Variables to be called later
                if (outputFolderSelect.ShowDialog() == CommonFileDialogResult.Cancel)
                {
                    MessageBox.Show("Output folder selection has been cancelled, conversion will not continue."
                                    , "Cancelled"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Warning);
                    goto BuildProcessFin;
                }
                selectedOutputPath = outputFolderSelect.FileName;
                Properties.Settings.Default.OutputPath = selectedOutputPath;
                Properties.Settings.Default.Save();
            }
            BuildProgress.Value = 2;
            //////////////////////////

            //Download base files with JNUSTool, store them for future use

            var downloadedFiles = new string[]
            {
                JNUSToolDownloads + "0005001010004000\\code\\deint.txt",
                JNUSToolDownloads + "0005001010004000\\code\\font.bin",
                JNUSToolDownloads + "0005001010004001\\code\\c2w.img",
                JNUSToolDownloads + "0005001010004001\\code\\boot.bin",
                JNUSToolDownloads + "0005001010004001\\code\\dmcu.d.hex",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\cos.xml",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\frisbiiU.rpx",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\fw.img",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\fw.tmd",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\htk.bin",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\code\\nn_hai_user.rpl",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\content\\assets\\shaders\\cafe\\banner.gsh",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\content\\assets\\shaders\\cafe\\fade.gsh",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\meta\\bootMovie.h264",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\meta\\bootLogoTex.tga",
                JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]\\meta\\bootSound.btsnd"
            };

            var fileHashes = new string[]
            {
                "E707A62EE5491DD16E5494631EA9870A",
                "CDDAC70FDDB9428F220B048102DAAD40",
                "FC5EE480F58796C3681BEE78BD3E5D1C",
                "F4D5F095CBA9504A5CB8A94A4781114C",
                "E32FCBCC817C443E0832DE5CA9032808",
                "42215713D951C2023F90164ED9DF900F",
                "69E191E8B0DF1D5304B36F1375C4F127",
                "3CAF52A9A440EEE4F125A3AD22E305C8",
                "AE4E06CAD3BEF60AE5C49E22CCDC3254",
                "C99CAF5995E395F39C3FCAB4A8AF20E0",
                "C4BF586BA0071BD8477986C1AA37E1F1",
                "5F2FA196DFC158F0FCC69272073AE07E",
                "307221985A7B46F0386A2637DC15DA3E",
                "CA0DAC3E3C5654209C754357EF5A2507",
                "67B312145ECB70514D5BD36FCAAE0193",
                "43CD445B8569A445F97ECCC098C93B38"
            };

            var filesToDownload = new string[]
            {
                "0005001010004000 -file /code/deint.txt",
                "0005001010004000 -file /code/font.bin",
                "0005001010004001 -file /code/c2w.img",
                "0005001010004001 -file /code/boot.bin",
                "0005001010004001 -file /code/dmcu.d.hex",
                "00050000101b0700 " + TitleKey.Text + " -file /code/cos.xml",
                "00050000101b0700 " + TitleKey.Text + " -file /code/frisbiiU.rpx",
                "00050000101b0700 " + TitleKey.Text + " -file /code/fw.img",
                "00050000101b0700 " + TitleKey.Text + " -file /code/fw.tmd",
                "00050000101b0700 " + TitleKey.Text + " -file /code/htk.bin",
                "00050000101b0700 " + TitleKey.Text + " -file /code/nn_hai_user.rpl",
                "00050000101b0700 " + TitleKey.Text + " -file /content/assets/shaders/cafe/banner.gsh",
                "00050000101b0700 " + TitleKey.Text + " -file /content/assets/shaders/cafe/fade.gsh*",
                "00050000101b0700 " + TitleKey.Text + " -file /meta/bootMovie.h264",
                "00050000101b0700 " + TitleKey.Text + " -file /meta/bootLogoTex.tga",
                "00050000101b0700 " + TitleKey.Text + " -file /meta/bootSound.btsnd"
            };

            BuildStatus.Text = "Checking if the necessary files are present...";
            BuildStatus.Refresh();
            BuildProgress.Value = 10;

            // create config file for jnustool
            string[] JNUSToolConfig = { "http://ccs.cdn.wup.shop.nintendo.net/ccs/download", WiiUCommonKey.Text };
            File.WriteAllLines(TempToolsPath + "JAR\\config", JNUSToolConfig);
            Directory.SetCurrentDirectory(TempToolsPath + "JAR");
            LauncherExeFile = "JNUSTool.exe";

            bool internetPresent = CheckForInternetConnection();

            for (int i = 0; i < downloadedFiles.Length; i++)
            {
                // check if file exists and is correct
                if (File.Exists(downloadedFiles[i]) && GetMD5Checksum(downloadedFiles[i]) == fileHashes[i])
                {
                    continue;
                }

                if (!internetPresent)
                {
                    DialogResult dialogResult = MessageBox.Show("Your internet connection could not be verified, do you wish to try and download" +
                                                                "the necessary base files from Nintendo anyway? (This is a one-time download)"
                                                                , "Internet Connection Verification Failed"
                                                                , MessageBoxButtons.YesNo
                                                                , MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.No)
                    {
                        goto BuildProcessFin;
                    }
                }

                // if not, download it
                BuildStatus.Text = "(One-Time Download) Downloading base files from Nintendo...";
                BuildStatus.Refresh();
                LauncherExeArgs = filesToDownload[i];
                LaunchProgram();
                BuildProgress.Value += 2;

            }

            // if any files were downloaded, store them in ProgramData
            if (BuildProgress.Value > 10)
            {
                BuildStatus.Text = "Saving files from Nintendo for future use...";
                BuildStatus.Refresh();

                if (Directory.Exists("Rhythm Heaven Fever [VAKE01]"))
                {
                    FileSystem.CopyDirectory("Rhythm Heaven Fever [VAKE01]", JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]", true);
                    Directory.Delete("Rhythm Heaven Fever [VAKE01]", true);
                }
                if (Directory.Exists("0005001010004000"))
                {
                    FileSystem.CopyDirectory("0005001010004000", JNUSToolDownloads + "0005001010004000", true);
                    Directory.Delete("0005001010004000", true);
                }
                if (Directory.Exists("0005001010004001"))
                {
                    FileSystem.CopyDirectory("0005001010004001", JNUSToolDownloads + "0005001010004001", true);
                    Directory.Delete("0005001010004001", true);
                }

                // repeat loop to check if all files were downloaded properly
                bool JNUSFail = false;
                for (int i = 0; i < downloadedFiles.Length; i++)
                {
                    // check if file exists and is correct
                    if (File.Exists(downloadedFiles[i]) && GetMD5Checksum(downloadedFiles[i]) == fileHashes[i])
                    {
                        continue;
                    }
                    JNUSFail = true;
                }

                if (JNUSFail)
                {
                    MessageBox.Show("Failed to download base files using JNUSTool, conversion will not continue"
                                    , "Error"
                                    , MessageBoxButtons.OK
                                    , MessageBoxIcon.Error);
                    goto BuildProcessFin;
                }
            }
            File.Delete("config");
            Directory.SetCurrentDirectory(TempRootPath);
            ///////////////////////////////////

            //Copy downloaded files to the build directory
            BuildStatus.Text = "Copying base files to temporary build directory...";
            BuildStatus.Refresh();
            FileSystem.CopyDirectory(JNUSToolDownloads + "Rhythm Heaven Fever [VAKE01]", TempBuildPath);
            if (C2WPatchFlag.Checked)
            {
                FileSystem.CopyDirectory(JNUSToolDownloads + "0005001010004000", TempBuildPath);
                FileSystem.CopyDirectory(JNUSToolDownloads + "0005001010004001", TempBuildPath);
                string[] AncastKeyCopy = { AncastKey.Text };
                File.WriteAllLines(TempToolsPath + "C2W\\starbuck_key.txt", AncastKeyCopy);
                File.Copy(TempBuildPath + "code\\c2w.img", TempToolsPath + "C2W\\c2w.img");
                Directory.SetCurrentDirectory(TempToolsPath + "C2W");
                LauncherExeFile = "c2w_patcher.exe";
                LauncherExeArgs = "-nc";
                LaunchProgram();
                File.Delete(TempBuildPath + "code\\c2w.img");
                File.Copy(TempToolsPath + "C2W\\c2p.img", TempBuildPath + "code\\c2w.img", true);
                File.Delete(TempToolsPath + "C2W\\c2p.img");
                File.Delete(TempToolsPath + "C2W\\c2w.img");
                File.Delete(TempToolsPath + "C2W\\starbuck_key.txt");
            }
            BuildProgress.Value = 50;
            //////////////////////////////////////////////

            //Generate app.xml & meta.xml
            BuildStatus.Text = "Generating app.xml and meta.xml";
            BuildStatus.Refresh();
            string[] AppXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<app type=\"complex\" access=\"777\">", "  <version type=\"unsignedInt\" length=\"4\">16</version>", "  <os_version type=\"hexBinary\" length=\"8\">000500101000400A</os_version>", "  <title_id type=\"hexBinary\" length=\"8\">" + PackedTitleIDLine.Text + "</title_id>", "  <title_version type=\"hexBinary\" length=\"2\">0000</title_version>", "  <sdk_version type=\"unsignedInt\" length=\"4\">21204</sdk_version>", "  <app_type type=\"hexBinary\" length=\"4\">8000002E</app_type>", "  <group_id type=\"hexBinary\" length=\"4\">" + TitleIDHex + "</group_id>", "  <os_mask type=\"hexBinary\" length=\"32\">0000000000000000000000000000000000000000000000000000000000000000</os_mask>", "  <common_id type=\"hexBinary\" length=\"8\">0000000000000000</common_id>", "</app>"};
            File.WriteAllLines(TempBuildPath + "code\\app.xml", AppXML);
            if (EnablePackedLine2.Checked)
            {
                string[] MetaXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<menu type=\"complex\" access=\"777\">", "  <version type=\"unsignedInt\" length=\"4\">33</version>", "  <product_code type=\"string\" length=\"32\">WUP-N-" + TitleIDText + "</product_code>", "  <content_platform type=\"string\" length=\"32\">WUP</content_platform>", "  <company_code type=\"string\" length=\"8\">0001</company_code>", "  <mastering_date type=\"string\" length=\"32\"></mastering_date>", "  <logo_type type=\"unsignedInt\" length=\"4\">0</logo_type>", "  <app_launch_type type=\"hexBinary\" length=\"4\">00000000</app_launch_type>", "  <invisible_flag type=\"hexBinary\" length=\"4\">00000000</invisible_flag>", "  <no_managed_flag type=\"hexBinary\" length=\"4\">00000000</no_managed_flag>", "  <no_event_log type=\"hexBinary\" length=\"4\">00000002</no_event_log>", "  <no_icon_database type=\"hexBinary\" length=\"4\">00000000</no_icon_database>", "  <launching_flag type=\"hexBinary\" length=\"4\">00000004</launching_flag>", "  <install_flag type=\"hexBinary\" length=\"4\">00000000</install_flag>", "  <closing_msg type=\"unsignedInt\" length=\"4\">0</closing_msg>", "  <title_version type=\"unsignedInt\" length=\"4\">0</title_version>", "  <title_id type=\"hexBinary\" length=\"8\">" + PackedTitleIDLine.Text + "</title_id>", "  <group_id type=\"hexBinary\" length=\"4\">" + TitleIDHex + "</group_id>", "  <boss_id type=\"hexBinary\" length=\"8\">0000000000000000</boss_id>", "  <os_version type=\"hexBinary\" length=\"8\">000500101000400A</os_version>", "  <app_size type=\"hexBinary\" length=\"8\">0000000000000000</app_size>", "  <common_save_size type=\"hexBinary\" length=\"8\">0000000000000000</common_save_size>", "  <account_save_size type=\"hexBinary\" length=\"8\">0000000000000000</account_save_size>", "  <common_boss_size type=\"hexBinary\" length=\"8\">0000000000000000</common_boss_size>", "  <account_boss_size type=\"hexBinary\" length=\"8\">0000000000000000</account_boss_size>", "  <save_no_rollback type=\"unsignedInt\" length=\"4\">0</save_no_rollback>", "  <join_game_id type=\"hexBinary\" length=\"4\">00000000</join_game_id>", "  <join_game_mode_mask type=\"hexBinary\" length=\"8\">0000000000000000</join_game_mode_mask>", "  <bg_daemon_enable type=\"unsignedInt\" length=\"4\">0</bg_daemon_enable>", "  <olv_accesskey type=\"unsignedInt\" length=\"4\">3921400692</olv_accesskey>", "  <wood_tin type=\"unsignedInt\" length=\"4\">0</wood_tin>", "  <e_manual type=\"unsignedInt\" length=\"4\">0</e_manual>", "  <e_manual_version type=\"unsignedInt\" length=\"4\">0</e_manual_version>", "  <region type=\"hexBinary\" length=\"4\">00000002</region>", "  <pc_cero type=\"unsignedInt\" length=\"4\">128</pc_cero>", "  <pc_esrb type=\"unsignedInt\" length=\"4\">6</pc_esrb>", "  <pc_bbfc type=\"unsignedInt\" length=\"4\">192</pc_bbfc>", "  <pc_usk type=\"unsignedInt\" length=\"4\">128</pc_usk>", "  <pc_pegi_gen type=\"unsignedInt\" length=\"4\">128</pc_pegi_gen>", "  <pc_pegi_fin type=\"unsignedInt\" length=\"4\">192</pc_pegi_fin>", "  <pc_pegi_prt type=\"unsignedInt\" length=\"4\">128</pc_pegi_prt>", "  <pc_pegi_bbfc type=\"unsignedInt\" length=\"4\">128</pc_pegi_bbfc>", "  <pc_cob type=\"unsignedInt\" length=\"4\">128</pc_cob>", "  <pc_grb type=\"unsignedInt\" length=\"4\">128</pc_grb>", "  <pc_cgsrr type=\"unsignedInt\" length=\"4\">128</pc_cgsrr>", "  <pc_oflc type=\"unsignedInt\" length=\"4\">128</pc_oflc>", "  <pc_reserved0 type=\"unsignedInt\" length=\"4\">192</pc_reserved0>", "  <pc_reserved1 type=\"unsignedInt\" length=\"4\">192</pc_reserved1>", "  <pc_reserved2 type=\"unsignedInt\" length=\"4\">192</pc_reserved2>", "  <pc_reserved3 type=\"unsignedInt\" length=\"4\">192</pc_reserved3>", "  <ext_dev_nunchaku type=\"unsignedInt\" length=\"4\">0</ext_dev_nunchaku>", "  <ext_dev_classic type=\"unsignedInt\" length=\"4\">0</ext_dev_classic>", "  <ext_dev_urcc type=\"unsignedInt\" length=\"4\">0</ext_dev_urcc>", "  <ext_dev_board type=\"unsignedInt\" length=\"4\">0</ext_dev_board>", "  <ext_dev_usb_keyboard type=\"unsignedInt\" length=\"4\">0</ext_dev_usb_keyboard>", "  <ext_dev_etc type=\"unsignedInt\" length=\"4\">0</ext_dev_etc>", "  <ext_dev_etc_name type=\"string\" length=\"512\"></ext_dev_etc_name>", "  <eula_version type=\"unsignedInt\" length=\"4\">0</eula_version>", "  <drc_use type=\"unsignedInt\" length=\"4\">" + DRCUSE + "</drc_use>", "  <network_use type=\"unsignedInt\" length=\"4\">0</network_use>", "  <online_account_use type=\"unsignedInt\" length=\"4\">0</online_account_use>", "  <direct_boot type=\"unsignedInt\" length=\"4\">0</direct_boot>", "  <reserved_flag0 type=\"hexBinary\" length=\"4\">00010001</reserved_flag0>", "  <reserved_flag1 type=\"hexBinary\" length=\"4\">00080023</reserved_flag1>", "  <reserved_flag2 type=\"hexBinary\" length=\"4\">" + TitleIDHex + "</reserved_flag2>", "  <reserved_flag3 type=\"hexBinary\" length=\"4\">00000000</reserved_flag3>", "  <reserved_flag4 type=\"hexBinary\" length=\"4\">00000000</reserved_flag4>", "  <reserved_flag5 type=\"hexBinary\" length=\"4\">00000000</reserved_flag5>", "  <reserved_flag6 type=\"hexBinary\" length=\"4\">00000003</reserved_flag6>", "  <reserved_flag7 type=\"hexBinary\" length=\"4\">00000005</reserved_flag7>", "  <longname_ja type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_ja>", "  <longname_en type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_en>", "  <longname_fr type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_fr>", "  <longname_de type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_de>", "  <longname_it type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_it>", "  <longname_es type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_es>", "  <longname_zhs type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_zhs>", "  <longname_ko type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_ko>", "  <longname_nl type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_nl>", "  <longname_pt type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_pt>", "  <longname_ru type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_ru>", "  <longname_zht type=\"string\" length=\"512\">" + PackedTitleLine1.Text, PackedTitleLine2.Text + "</longname_zht>", "  <shortname_ja type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ja>", "  <shortname_en type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_en>", "  <shortname_fr type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_fr>", "  <shortname_de type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_de>", "  <shortname_it type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_it>", "  <shortname_es type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_es>", "  <shortname_zhs type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_zhs>", "  <shortname_ko type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ko>", "  <shortname_nl type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_nl>", "  <shortname_pt type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_pt>", "  <shortname_ru type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ru>", "  <shortname_zht type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_zht>", "  <publisher_ja type=\"string\" length=\"256\"></publisher_ja>", "  <publisher_en type=\"string\" length=\"256\"></publisher_en>", "  <publisher_fr type=\"string\" length=\"256\"></publisher_fr>", "  <publisher_de type=\"string\" length=\"256\"></publisher_de>", "  <publisher_it type=\"string\" length=\"256\"></publisher_it>", "  <publisher_es type=\"string\" length=\"256\"></publisher_es>", "  <publisher_zhs type=\"string\" length=\"256\"></publisher_zhs>", "  <publisher_ko type=\"string\" length=\"256\"></publisher_ko>", "  <publisher_nl type=\"string\" length=\"256\"></publisher_nl>", "  <publisher_pt type=\"string\" length=\"256\"></publisher_pt>", "  <publisher_ru type=\"string\" length=\"256\"></publisher_ru>", "  <publisher_zht type=\"string\" length=\"256\"></publisher_zht>", "  <add_on_unique_id0 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id0>", "  <add_on_unique_id1 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id1>", "  <add_on_unique_id2 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id2>", "  <add_on_unique_id3 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id3>", "  <add_on_unique_id4 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id4>", "  <add_on_unique_id5 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id5>", "  <add_on_unique_id6 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id6>", "  <add_on_unique_id7 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id7>", "  <add_on_unique_id8 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id8>", "  <add_on_unique_id9 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id9>", "  <add_on_unique_id10 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id10>", "  <add_on_unique_id11 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id11>", "  <add_on_unique_id12 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id12>", "  <add_on_unique_id13 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id13>", "  <add_on_unique_id14 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id14>", "  <add_on_unique_id15 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id15>", "  <add_on_unique_id16 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id16>", "  <add_on_unique_id17 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id17>", "  <add_on_unique_id18 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id18>", "  <add_on_unique_id19 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id19>", "  <add_on_unique_id20 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id20>", "  <add_on_unique_id21 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id21>", "  <add_on_unique_id22 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id22>", "  <add_on_unique_id23 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id23>", "  <add_on_unique_id24 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id24>", "  <add_on_unique_id25 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id25>", "  <add_on_unique_id26 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id26>", "  <add_on_unique_id27 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id27>", "  <add_on_unique_id28 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id28>", "  <add_on_unique_id29 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id29>", "  <add_on_unique_id30 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id30>", "  <add_on_unique_id31 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id31>", "</menu>" };
                File.WriteAllLines(TempBuildPath + "meta\\meta.xml", MetaXML);
            }
            else
            {
                string[] MetaXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<menu type=\"complex\" access=\"777\">", "  <version type=\"unsignedInt\" length=\"4\">33</version>", "  <product_code type=\"string\" length=\"32\">WUP-N-" + TitleIDText + "</product_code>", "  <content_platform type=\"string\" length=\"32\">WUP</content_platform>", "  <company_code type=\"string\" length=\"8\">0001</company_code>", "  <mastering_date type=\"string\" length=\"32\"></mastering_date>", "  <logo_type type=\"unsignedInt\" length=\"4\">0</logo_type>", "  <app_launch_type type=\"hexBinary\" length=\"4\">00000000</app_launch_type>", "  <invisible_flag type=\"hexBinary\" length=\"4\">00000000</invisible_flag>", "  <no_managed_flag type=\"hexBinary\" length=\"4\">00000000</no_managed_flag>", "  <no_event_log type=\"hexBinary\" length=\"4\">00000002</no_event_log>", "  <no_icon_database type=\"hexBinary\" length=\"4\">00000000</no_icon_database>", "  <launching_flag type=\"hexBinary\" length=\"4\">00000004</launching_flag>", "  <install_flag type=\"hexBinary\" length=\"4\">00000000</install_flag>", "  <closing_msg type=\"unsignedInt\" length=\"4\">0</closing_msg>", "  <title_version type=\"unsignedInt\" length=\"4\">0</title_version>", "  <title_id type=\"hexBinary\" length=\"8\">" + PackedTitleIDLine.Text + "</title_id>", "  <group_id type=\"hexBinary\" length=\"4\">" + TitleIDHex + "</group_id>", "  <boss_id type=\"hexBinary\" length=\"8\">0000000000000000</boss_id>", "  <os_version type=\"hexBinary\" length=\"8\">000500101000400A</os_version>", "  <app_size type=\"hexBinary\" length=\"8\">0000000000000000</app_size>", "  <common_save_size type=\"hexBinary\" length=\"8\">0000000000000000</common_save_size>", "  <account_save_size type=\"hexBinary\" length=\"8\">0000000000000000</account_save_size>", "  <common_boss_size type=\"hexBinary\" length=\"8\">0000000000000000</common_boss_size>", "  <account_boss_size type=\"hexBinary\" length=\"8\">0000000000000000</account_boss_size>", "  <save_no_rollback type=\"unsignedInt\" length=\"4\">0</save_no_rollback>", "  <join_game_id type=\"hexBinary\" length=\"4\">00000000</join_game_id>", "  <join_game_mode_mask type=\"hexBinary\" length=\"8\">0000000000000000</join_game_mode_mask>", "  <bg_daemon_enable type=\"unsignedInt\" length=\"4\">0</bg_daemon_enable>", "  <olv_accesskey type=\"unsignedInt\" length=\"4\">3921400692</olv_accesskey>", "  <wood_tin type=\"unsignedInt\" length=\"4\">0</wood_tin>", "  <e_manual type=\"unsignedInt\" length=\"4\">0</e_manual>", "  <e_manual_version type=\"unsignedInt\" length=\"4\">0</e_manual_version>", "  <region type=\"hexBinary\" length=\"4\">00000002</region>", "  <pc_cero type=\"unsignedInt\" length=\"4\">128</pc_cero>", "  <pc_esrb type=\"unsignedInt\" length=\"4\">6</pc_esrb>", "  <pc_bbfc type=\"unsignedInt\" length=\"4\">192</pc_bbfc>", "  <pc_usk type=\"unsignedInt\" length=\"4\">128</pc_usk>", "  <pc_pegi_gen type=\"unsignedInt\" length=\"4\">128</pc_pegi_gen>", "  <pc_pegi_fin type=\"unsignedInt\" length=\"4\">192</pc_pegi_fin>", "  <pc_pegi_prt type=\"unsignedInt\" length=\"4\">128</pc_pegi_prt>", "  <pc_pegi_bbfc type=\"unsignedInt\" length=\"4\">128</pc_pegi_bbfc>", "  <pc_cob type=\"unsignedInt\" length=\"4\">128</pc_cob>", "  <pc_grb type=\"unsignedInt\" length=\"4\">128</pc_grb>", "  <pc_cgsrr type=\"unsignedInt\" length=\"4\">128</pc_cgsrr>", "  <pc_oflc type=\"unsignedInt\" length=\"4\">128</pc_oflc>", "  <pc_reserved0 type=\"unsignedInt\" length=\"4\">192</pc_reserved0>", "  <pc_reserved1 type=\"unsignedInt\" length=\"4\">192</pc_reserved1>", "  <pc_reserved2 type=\"unsignedInt\" length=\"4\">192</pc_reserved2>", "  <pc_reserved3 type=\"unsignedInt\" length=\"4\">192</pc_reserved3>", "  <ext_dev_nunchaku type=\"unsignedInt\" length=\"4\">0</ext_dev_nunchaku>", "  <ext_dev_classic type=\"unsignedInt\" length=\"4\">0</ext_dev_classic>", "  <ext_dev_urcc type=\"unsignedInt\" length=\"4\">0</ext_dev_urcc>", "  <ext_dev_board type=\"unsignedInt\" length=\"4\">0</ext_dev_board>", "  <ext_dev_usb_keyboard type=\"unsignedInt\" length=\"4\">0</ext_dev_usb_keyboard>", "  <ext_dev_etc type=\"unsignedInt\" length=\"4\">0</ext_dev_etc>", "  <ext_dev_etc_name type=\"string\" length=\"512\"></ext_dev_etc_name>", "  <eula_version type=\"unsignedInt\" length=\"4\">0</eula_version>", "  <drc_use type=\"unsignedInt\" length=\"4\">" + DRCUSE + "</drc_use>", "  <network_use type=\"unsignedInt\" length=\"4\">0</network_use>", "  <online_account_use type=\"unsignedInt\" length=\"4\">0</online_account_use>", "  <direct_boot type=\"unsignedInt\" length=\"4\">0</direct_boot>", "  <reserved_flag0 type=\"hexBinary\" length=\"4\">00010001</reserved_flag0>", "  <reserved_flag1 type=\"hexBinary\" length=\"4\">00080023</reserved_flag1>", "  <reserved_flag2 type=\"hexBinary\" length=\"4\">" + TitleIDHex + "</reserved_flag2>", "  <reserved_flag3 type=\"hexBinary\" length=\"4\">00000000</reserved_flag3>", "  <reserved_flag4 type=\"hexBinary\" length=\"4\">00000000</reserved_flag4>", "  <reserved_flag5 type=\"hexBinary\" length=\"4\">00000000</reserved_flag5>", "  <reserved_flag6 type=\"hexBinary\" length=\"4\">00000003</reserved_flag6>", "  <reserved_flag7 type=\"hexBinary\" length=\"4\">00000005</reserved_flag7>", "  <longname_ja type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_ja>", "  <longname_en type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_en>", "  <longname_fr type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_fr>", "  <longname_de type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_de>", "  <longname_it type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_it>", "  <longname_es type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_es>", "  <longname_zhs type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_zhs>", "  <longname_ko type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_ko>", "  <longname_nl type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_nl>", "  <longname_pt type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_pt>", "  <longname_ru type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_ru>", "  <longname_zht type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</longname_zht>", "  <shortname_ja type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ja>", "  <shortname_en type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_en>", "  <shortname_fr type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_fr>", "  <shortname_de type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_de>", "  <shortname_it type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_it>", "  <shortname_es type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_es>", "  <shortname_zhs type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_zhs>", "  <shortname_ko type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ko>", "  <shortname_nl type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_nl>", "  <shortname_pt type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_pt>", "  <shortname_ru type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_ru>", "  <shortname_zht type=\"string\" length=\"512\">" + PackedTitleLine1.Text + "</shortname_zht>", "  <publisher_ja type=\"string\" length=\"256\"></publisher_ja>", "  <publisher_en type=\"string\" length=\"256\"></publisher_en>", "  <publisher_fr type=\"string\" length=\"256\"></publisher_fr>", "  <publisher_de type=\"string\" length=\"256\"></publisher_de>", "  <publisher_it type=\"string\" length=\"256\"></publisher_it>", "  <publisher_es type=\"string\" length=\"256\"></publisher_es>", "  <publisher_zhs type=\"string\" length=\"256\"></publisher_zhs>", "  <publisher_ko type=\"string\" length=\"256\"></publisher_ko>", "  <publisher_nl type=\"string\" length=\"256\"></publisher_nl>", "  <publisher_pt type=\"string\" length=\"256\"></publisher_pt>", "  <publisher_ru type=\"string\" length=\"256\"></publisher_ru>", "  <publisher_zht type=\"string\" length=\"256\"></publisher_zht>", "  <add_on_unique_id0 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id0>", "  <add_on_unique_id1 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id1>", "  <add_on_unique_id2 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id2>", "  <add_on_unique_id3 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id3>", "  <add_on_unique_id4 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id4>", "  <add_on_unique_id5 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id5>", "  <add_on_unique_id6 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id6>", "  <add_on_unique_id7 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id7>", "  <add_on_unique_id8 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id8>", "  <add_on_unique_id9 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id9>", "  <add_on_unique_id10 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id10>", "  <add_on_unique_id11 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id11>", "  <add_on_unique_id12 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id12>", "  <add_on_unique_id13 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id13>", "  <add_on_unique_id14 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id14>", "  <add_on_unique_id15 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id15>", "  <add_on_unique_id16 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id16>", "  <add_on_unique_id17 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id17>", "  <add_on_unique_id18 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id18>", "  <add_on_unique_id19 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id19>", "  <add_on_unique_id20 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id20>", "  <add_on_unique_id21 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id21>", "  <add_on_unique_id22 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id22>", "  <add_on_unique_id23 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id23>", "  <add_on_unique_id24 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id24>", "  <add_on_unique_id25 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id25>", "  <add_on_unique_id26 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id26>", "  <add_on_unique_id27 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id27>", "  <add_on_unique_id28 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id28>", "  <add_on_unique_id29 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id29>", "  <add_on_unique_id30 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id30>", "  <add_on_unique_id31 type=\"hexBinary\" length=\"4\">00000000</add_on_unique_id31>", "</menu>" };
                File.WriteAllLines(TempBuildPath + "meta\\meta.xml", MetaXML);
            }
            BuildProgress.Value = 52;
            /////////////////////////////

            //Convert PNG files to TGA
            BuildStatus.Text = "Converting all image sources to expected TGA specification...";
            BuildStatus.Refresh();
            LauncherExeFile = TempToolsPath + "IMG\\png2tgacmd.exe";
            LauncherExeArgs = "-i \"" + TempIconPath + "\" -o \"" + TempBuildPath + "meta\" --width=128 --height=128 --tga-bpp=32 --tga-compression=none";
            LaunchProgram();
            LauncherExeFile = TempToolsPath + "IMG\\png2tgacmd.exe";
            LauncherExeArgs = "-i \"" + TempBannerPath + "\" -o \"" + TempBuildPath + "meta\" --width=1280 --height=720 --tga-bpp=24 --tga-compression=none";
            LaunchProgram();
            if (DrcPreviewBox.Image == null)
            {
                File.Copy(TempBannerPath, TempDrcPath);
            }
            LauncherExeFile = TempToolsPath + "IMG\\png2tgacmd.exe";
            LauncherExeArgs = "-i \"" + TempDrcPath + "\" -o \"" + TempBuildPath + "meta\" --width=854 --height=480 --tga-bpp=24 --tga-compression=none";
            LaunchProgram();
            if (LogoPreviewBox.Image != null)
            {
                LauncherExeFile = TempToolsPath + "IMG\\png2tgacmd.exe";
                LauncherExeArgs = "-i \"" + TempLogoPath + "\" -o \"" + TempBuildPath + "meta\" --width=170 --height=42 --tga-bpp=32 --tga-compression=none";
                LaunchProgram();
            }
            if (DrcPreviewBox.Image == null) { File.Delete(TempDrcPath); }
            BuildProgress.Value = 55;
            //////////////////////////

            //Convert Boot Sound if provided by user
            if (FlagBootSoundSpecified)
            {
                BuildStatus.Text = "Converting user-provided sound to btsnd format...";
                BuildStatus.Refresh();
                LauncherExeFile = TempToolsPath + "SOX\\sox.exe";
                LauncherExeArgs = "\"" + OpenBootSound.FileName + "\" -b 16 \"" + TempSoundPath + "\" channels 2 rate 48k trim 0 6";
                LaunchProgram();
                File.Delete(TempBuildPath + "meta\\bootSound.btsnd");
                LauncherExeFile = TempToolsPath + "JAR\\wav2btsnd.exe";
                LauncherExeArgs = "-in \"" + TempSoundPath + "\" -out \"" + TempBuildPath + "meta\\bootSound.btsnd\"" + (ToggleBootSoundLoop.Checked ? "":"-noLoop");
                LaunchProgram();
                File.Delete(TempSoundPath);
            }
            BuildProgress.Value = 60;
            ////////////////////////////////////////

            //Build ISO based on type and user specification
            BuildStatus.Text = "Processing game for NFS Conversion...";
            BuildStatus.Refresh();

            string OGfilepath = OpenGame.FileName;

            if (SystemType == "wii")
            {
                /* Convert the wbfs file to iso. This is not named game.iso in to allow the nkit/nasos/trimming to work */
                if (FlagWBFS)
                {
                    LauncherExeFile = TempToolsPath + "EXE\\wbfs_file.exe";
                    LauncherExeArgs = "\"" + OGfilepath + "\" convert \"" + TempSourcePath + "wbfsconvert.iso\"";
                    LaunchProgram();
                    OGfilepath = TempSourcePath + "wbfsconvert.iso";
                }
                if (FlagNKIT || FlagNASOS)
                {
                    if (Directory.Exists(TempToolsPath + "NKIT\\Processed"))
                    {
                        Directory.Delete(TempToolsPath + "NKIT\\Processed", true);
                    }
                    BuildStatus.Text = "Unscrubbing game for NFS Conversion...";
                    BuildStatus.Refresh();
                    LauncherExeFile = TempToolsPath + "NKIT\\ConvertToISO.exe";
                    LauncherExeArgs = "\"" + OGfilepath + "\"";
                    LaunchProgram();
                    OGfilepath = TempSourcePath + "game.iso";
                    if(FlagNKIT)
                        File.Move(Directory.GetFiles(TempToolsPath + "NKIT\\Processed\\Temp", "*.tmp")[0], TempSourcePath + "game.iso");
                    else
                        File.Move(Directory.GetFiles(TempToolsPath + "NKIT\\Processed\\Wii_MatchFail", "*.iso")[0], TempSourcePath + "game.iso");


                }
                if (DisableTrimming.Checked == false)
                {
                    BuildStatus.Text = "Extracting game for NFS Conversion...";
                    BuildStatus.Refresh();
                    LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
                    LauncherExeArgs = "extract " + "\"" + OGfilepath + "\"" + " --DEST " + TempSourcePath + "ISOEXTRACT" + " --psel data,-update -ovv";
                    LaunchProgram(); // EXTRACT WII ISO
                    if (ForceCC.Checked)
                    {
                        LauncherExeFile = TempToolsPath + "EXE\\GetExtTypePatcher.exe";
                        LauncherExeArgs = "\"" + TempSourcePath + "ISOEXTRACT\\sys\\main.dol\" -nc";
                        LaunchProgram();
                    }
                    if (WiiVMC.Checked)
                    {
                        MessageBox.Show("The Wii Video Mode Changer will now be launched. I recommend using the Smart Patcher option. \n\n" +
                                        "If you're scared and don't know what you're doing, close the patcher window and nothing will be patched." +
                                        "\n\nClick OK to continue..."
                                        , "Information"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Information);
                        HideProcess = false;
                        LauncherExeFile = TempToolsPath + "EXE\\wii-vmc.exe";
                        LauncherExeArgs = "\"" + TempSourcePath + "ISOEXTRACT\\sys\\main.dol\"";
                        LaunchProgram();
                        HideProcess = true;
                        MessageBox.Show("Conversion will now continue..."
                                        , "Information"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Information);
                    }
                    BuildStatus.Text = "Rebuilding iso for NFS Conversion...";
                    BuildStatus.Refresh();
                    LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
                    if (!Wiimmfi.Checked)
                    {
                        wiimmfiOption = "";
                    }
                    LauncherExeArgs = "copy " + TempSourcePath + "ISOEXTRACT" + " --DEST " + TempSourcePath + "game.iso" + " -ovv --links --iso" + wiimmfiOption;
                    LaunchProgram(); // REBUILD WII ISO
                    if (File.Exists(TempSourcePath + "wbfsconvert.iso")) { File.Delete(TempSourcePath + "wbfsconvert.iso"); }
                    OGfilepath = TempSourcePath + "game.iso";
                }
            }
            else if (SystemType == "dol")
            {
                FileSystem.CreateDirectory(TempSourcePath + "TEMPISOBASE");
                FileSystem.CopyDirectory(TempToolsPath + "BASE", TempSourcePath + "TEMPISOBASE");
                File.Copy(OGfilepath, TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
                LauncherExeArgs = "copy " + TempSourcePath + "TEMPISOBASE" + " --DEST " + TempSourcePath + "game.iso" + " -ovv --links --iso";
                LaunchProgram();
                Directory.Delete(TempSourcePath + "TEMPISOBASE", true);
                OGfilepath = TempSourcePath + "game.iso";
            }
            else if (SystemType == "wiiware")
            {
                FileSystem.CreateDirectory(TempSourcePath + "TEMPISOBASE");
                FileSystem.CopyDirectory(TempToolsPath + "BASE", TempSourcePath + "TEMPISOBASE");
                if (Force43NAND.Checked)
                {
                    File.Copy(TempToolsPath + "DOL\\FIX94_wiivc_chan_booter_force43.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }
                else
                {
                    File.Copy(TempToolsPath + "DOL\\FIX94_wiivc_chan_booter.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }
                string[] TitleTXT = { GameSourceDirectory.Text };
                File.WriteAllLines(TempSourcePath + "TEMPISOBASE\\files\\title.txt", TitleTXT);
                LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
                LauncherExeArgs = "copy " + TempSourcePath + "TEMPISOBASE" + " --DEST " + TempSourcePath + "game.iso" + " -ovv --links --iso";
                LaunchProgram();
                Directory.Delete(TempSourcePath + "TEMPISOBASE", true);
                OGfilepath = TempSourcePath + "game.iso";
            }
            else if (SystemType == "gcn")
            {
                FileSystem.CreateDirectory(TempSourcePath + "TEMPISOBASE");
                FileSystem.CopyDirectory(TempToolsPath + "BASE", TempSourcePath + "TEMPISOBASE");
                if (Force43NINTENDONT.Checked)
                {
                    if (ForceInterlacedNINTENDONT.Checked)
                    {
                        File.Copy(TempToolsPath + "DOL\\nintendont_force_43_interlaced_autobooter.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                    }
                    else
                    {
                        File.Copy(TempToolsPath + "DOL\\nintendont_force_4_by_3_autobooter.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                    }
                }

                else if (ForceInterlacedNINTENDONT.Checked)
                {
                    File.Copy(TempToolsPath + "DOL\\nintendont_force_interlaced_autobooter.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }
                else if (CustomMainDol.Checked)
                {
                    File.Copy(OpenMainDol.FileName, TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }
                else if (DisableNintendontAutoboot.Checked)
                {
                    File.Copy(TempToolsPath + "DOL\\nintendont_forwarder.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }
                else
                {
                    File.Copy(TempToolsPath + "DOL\\nintendont_default_autobooter.dol", TempSourcePath + "TEMPISOBASE\\sys\\main.dol");
                }

                processGCNdisc(OGfilepath, "game.iso");

                if (FlagGC2Specified)
                    processGCNdisc(OpenGC2.FileName, "disc2.iso");

                LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
                LauncherExeArgs = "copy " + TempSourcePath + "TEMPISOBASE" + " --DEST " + TempSourcePath + "game.iso" + " -ovv --links --iso";
                LaunchProgram(); // BUILD FINAL GAMECUBE ISO
                Directory.Delete(TempSourcePath + "TEMPISOBASE", true);
                OGfilepath = TempSourcePath + "game.iso";
            }
        
            LauncherExeFile = TempToolsPath + "WIT\\wit.exe";
            LauncherExeArgs = "extract " + OGfilepath + " --psel data --psel -update --files +tmd.bin --files +ticket.bin --dest " + TempSourcePath + "TIKTEMP" + " -vv1";
            LaunchProgram();
            File.Copy(TempSourcePath + "TIKTEMP\\tmd.bin", TempBuildPath + "code\\rvlt.tmd");
            File.Copy(TempSourcePath + "TIKTEMP\\ticket.bin", TempBuildPath + "code\\rvlt.tik");
            Directory.Delete(TempSourcePath + "TIKTEMP", true);
            BuildProgress.Value = 70;
            ////////////////////////////////////////////////

            //Convert ISO to NFS format
            BuildStatus.Text = "Converting processed game to NFS format...";
            BuildStatus.Refresh();
            Directory.SetCurrentDirectory(TempBuildPath + "content");
            LauncherExeFile = TempToolsPath + "EXE\\nfs2iso2nfs.exe";

            string lrpatchflag = LRPatch.Checked ? " -lrpatch" : "";

            switch (SystemType)
            {
                case "wii":
                    LauncherExeArgs = "-enc" + nfspatchflag + lrpatchflag + " -iso \"" + OGfilepath + "\"";
                    break;

                case "dol":
                    LauncherExeArgs = "-enc -homebrew" + (DisablePassthrough.Checked ? "" : "-passthrough") + " -iso \"" + OGfilepath + "\"";
                    break;

                case "wiiware":
                    LauncherExeArgs = "-enc -homebrew" + nfspatchflag + lrpatchflag + " -iso \"" + OGfilepath + "\"";
                    break;

                case "gcn":
                    LauncherExeArgs = "-enc -homebrew -passthrough -iso \"" + OGfilepath + "\"";
                    break;
            }
            LaunchProgram();

            if ((DisableTrimming.Checked == false) || (FlagWBFS))
            {
                File.Delete(OGfilepath);
            }
            BuildProgress.Value = 85;
            ///////////////////////////

            //Encrypt contents with NUSPacker
            BuildStatus.Text = "Encrypting contents into installable WUP Package...";
            BuildStatus.Refresh();
            Directory.SetCurrentDirectory(TempRootPath);
            string sanitizedGameName = SanitizeFilename(PackedTitleLine1.Text);
            string outputPath = selectedOutputPath + "\\" + sanitizedGameName + " WUP-N-" + TitleIDText + "_" + PackedTitleIDLine.Text;
            LauncherExeFile = TempToolsPath + "JAR\\NUSPacker.exe";
            LauncherExeArgs = "-in BUILDDIR -out \"" + outputPath + "\" -encryptKeyWith " + WiiUCommonKey.Text;
            LaunchProgram();
            BuildProgress.Value = 100;
            /////////////////////////////////

            //Delete Temp Directories
            Directory.SetCurrentDirectory(Application.StartupPath);
            DeleteFolder(TempBuildPath, true);
            DeleteFolder(TempRootPath + "output", true);
            DeleteFolder(TempRootPath + "tmp", true);
            Directory.CreateDirectory(TempBuildPath);
            /////////////////////////

            //END
            BuildStatus.Text = "Conversion complete...";
            BuildStatus.Refresh();

            DialogResult finalDialogResult = MessageBox.Show("Conversion Complete! Your packed game can be found here:\n" + outputPath + "\n\n" +
                                                            "Install your title using WUP Installer GX2 with signature patches enabled (CBHC, Haxchi, etc)." +
                                                            "Make sure you have signature patches enabled when launching your title.\n\n" +
                                                            "Open the output folder now?"
                                                            , PackedTitleLine1.Text + "Conversion Complete"
                                                            , MessageBoxButtons.YesNo
                                                            , MessageBoxIcon.Information);

            if (finalDialogResult == DialogResult.Yes)
            {
                Process.Start(outputPath);
            }

            MainTabs.SelectedTab = SourceFilesTab;

        BuildProcessFin:
            BuildProgress.Value = 0;
            BuildStatus.Text = "";
            BuildStatus.Refresh();
            MainTabs.Enabled = true;
            /////
        }

        private string SanitizeFilename(string str)
        {
            var invalids = Path.GetInvalidFileNameChars();
            return string.Join("_", str.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        private static string GetMD5Checksum(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var md5 = MD5.Create();
                byte[] checksum = md5.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        private static void DeleteFolder(string path, bool recursive)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }

    }
}

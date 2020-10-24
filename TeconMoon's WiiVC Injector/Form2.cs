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

namespace TeconMoon_s_WiiVC_Injector
{
    public partial class SDCardMenu : Form
    {
        public SDCardMenu()
        {
            InitializeComponent();
        }
        string SelectedDriveLetter;
        bool DriveSpecified;

        //Load Drives and set drive variable on load
        private void SDCardMenu_Load(object sender, EventArgs e)
        {
            ReloadDriveList();
            SpecifyDrive();
            MemcardBlocks.SelectedIndex = 0;
            VideoForceMode.SelectedIndex = 0;
            VideoTypeMode.SelectedIndex = 0;
            LanguageBox.SelectedIndex = 0;
            NintendontOptions.SetItemChecked(0, true);
            NintendontOptions.SetItemChecked(7, true);
        }

        //Callable voids for commands
        public void SpecifyDrive()
        {
            if (DriveBox.SelectedValue != null)
            {
                SelectedDriveLetter = DriveBox.SelectedValue.ToString().Substring(0, 3);
                DriveSpecified = true;
            }
            else
            {
                SelectedDriveLetter = "";
                DriveSpecified = false;
            }
        }
        public void ReloadDriveList()
        {
            DriveBox.DataSource = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Removable).Select(d => d.Name + " (" + d.VolumeLabel + ")").ToList();
        }
        public void CheckForBoxes()
        {
            // memory card emulation
            if (NintendontOptions.GetItemChecked(0))
            {
                MemcardText.Enabled = true;
                MemcardBlocks.Enabled = true;
                MemcardMulti.Enabled = true;
            }
            else
            {
                MemcardText.Enabled = false;
                MemcardBlocks.Enabled = false;
                MemcardMulti.Enabled = false;
            }

            // video width
            if (NintendontOptions.GetItemChecked(7))
            {
                VideoWidth.Enabled = false;
                VideoWidthText.Enabled = false;
                WidthNumber.Text = "Auto";
            }
            else
            {
                VideoWidth.Enabled = true;
                VideoWidthText.Enabled = true;
                WidthNumber.Text = VideoWidth.Value.ToString();
            }
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
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

        //Reload Drives when selected
        private void ReloadDrives_Click(object sender, EventArgs e)
        {
            ReloadDriveList();
            SpecifyDrive();
        }
        //Specify Drive variable when a drive is selected
        private void DriveBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecifyDrive();
        }
        

        //Changing config options
        public void NintendontOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForBoxes();
        }
        private void NintendontOptions_DoubleClick(object sender, EventArgs e)
        {
            CheckForBoxes();
        }
        private void VideoForceMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoForceMode.SelectedIndex == 0)
            {
                VideoTypeMode.SelectedIndex = 0;
                VideoTypeMode.Enabled = false;
            }
            else if (VideoForceMode.SelectedIndex == 3)
            {
                VideoTypeMode.SelectedIndex = 0;
                VideoTypeMode.Enabled = false;
            }
            else
            {
                VideoTypeMode.SelectedIndex = 1;
                VideoTypeMode.Enabled = true;
            }
        }
        private void VideoWidth_Scroll(object sender, EventArgs e)
        {
            WidthNumber.Text = VideoWidth.Value.ToString();
        }
        private void VideoTypeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VideoForceMode.SelectedIndex != 0 & VideoForceMode.SelectedIndex != 3 & VideoTypeMode.SelectedIndex == 0)
            {
                VideoTypeMode.SelectedIndex = 1;
            }
        }

        //Buttons that make changes to SD Card
        private void NintendontUpdate_Click(object sender, EventArgs e)
        {
            string downloadPath = Path.GetTempPath() + "WiiVCInjector\\SOURCETEMP\\Download\\";
            string tempPath = downloadPath + "apps\\nintendont\\";
            string SDPath = SelectedDriveLetter + "apps\\nintendont";

            if (CheckForInternetConnection() == false)
            {
                DialogResult dialogResult = MessageBox.Show("Your internet connection could not be verified, do you wish to try and download Nintendont anyway?"
                                                            , "Internet Connection Verification Failed"
                                                            , MessageBoxButtons.YesNo
                                                            , MessageBoxIcon.Question
                                                            , MessageBoxDefaultButton.Button1
                                                            , (MessageBoxOptions)0x40000);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            ActionStatus.Text = "Downloading...";
            ActionStatus.Refresh();
            Directory.CreateDirectory(tempPath);
            var client = new WebClient();
            client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/loader/loader.dol", tempPath + "boot.dol");
            client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/nintendont/meta.xml", tempPath + "meta.xml");
            client.DownloadFile("https://raw.githubusercontent.com/FIX94/Nintendont/master/nintendont/icon.png", tempPath + "icon.png");
            ActionStatus.Text = "";

            if (DriveSpecified)
            {
                if (Directory.Exists(SelectedDriveLetter + "apps\\nintendont"))
                {
                    Directory.Delete(SelectedDriveLetter + "apps\\nintendont", true);
                }
                Directory.CreateDirectory(SelectedDriveLetter + "apps\\nintendont");


                DirectoryInfo dir = new DirectoryInfo(tempPath);
                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo file in files)
                {
                    string outPath = Path.Combine(SDPath, file.Name);
                    file.CopyTo(outPath, true);
                }

                MessageBox.Show("Download complete."
                                , "Success"
                                , MessageBoxButtons.OK
                                , MessageBoxIcon.Information
                                , MessageBoxDefaultButton.Button1
                                , (MessageBoxOptions)0x40000);

            }
            else // if no removable drive is specified
            {
                DialogResult dialogResult = MessageBox.Show("SD Card not specified.\nDo you wish to save Nintendont somewhere else?"
                                                            , "Drive not specified"
                                                            , MessageBoxButtons.YesNo
                                                            , MessageBoxIcon.Question
                                                            , MessageBoxDefaultButton.Button1
                                                            , (MessageBoxOptions)0x40000);

                if (dialogResult == DialogResult.Yes)   // if YES, ask where to save the file
                {
                    DateTime dateTime = DateTime.UtcNow.Date;

                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save Nintendont zip file",
                        CheckPathExists = true,
                        DefaultExt = "zip",
                        Filter = "Zip Files (*.zip)|*.zip",
                        FilterIndex = 2,
                        RestoreDirectory = true,
                        FileName = "Nintendont-" + dateTime.ToString("dd.MMM.yyyy") +  ".zip"
                    };

                    // if a path is decided, store it
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string sourcePath = downloadPath;
                        string zipPath = saveFileDialog.FileName;

                        if (File.Exists(zipPath))
                        {
                            File.Delete(zipPath);
                        }

                        ZipFile.CreateFromDirectory(sourcePath, zipPath);
                        MessageBox.Show("Download complete."
                                        , "Success"
                                        , MessageBoxButtons.OK
                                        , MessageBoxIcon.Information
                                        , MessageBoxDefaultButton.Button1
                                        , (MessageBoxOptions)0x40000);
                    }
                    

                    // else, stop the saving process
                    else
                    {
                        return;
                    }

                }
                //  if the user doesn't want to save, stop
                else
                {
                    return;
                }
            }
        }

        public struct ConfigFile
        {
            public uint magicBytes;
            public uint version;
            public uint config;
            public uint videoMode;
            public uint language;       // mainly for PAL gamecube games
            public byte[] gamePath;
            public byte[] cheatPath;
            public uint maxPads;        // old wii only
            public uint gameID;
            public byte memCardBlocks;
            public sbyte videoScale;
            public sbyte videoOffset;
            public byte networkProfile; // wii only
        }

        public enum ninconfig
        {
            NIN_CFG_CHEATS = 1,
            NIN_CFG_DEBUGGER = (1 << 1), // Only for Wii Version
            NIN_CFG_DEBUGWAIT = (1 << 2),   // Only for Wii Version
            NIN_CFG_MEMCARDEMU = (1 << 3), // ENABLED for Wii U and newer Wii
            NIN_CFG_CHEAT_PATH = (1 << 4),
            NIN_CFG_FORCE_WIDE = (1 << 5),
            NIN_CFG_FORCE_PROG = (1 << 6),
            NIN_CFG_AUTO_BOOT = (1 << 7),
            NIN_CFG_HID = (1 << 8),
            NIN_CFG_REMLIMIT = (1 << 8),
            NIN_CFG_OSREPORT = (1 << 9),
            NIN_CFG_USB = (1 << 10),       // old bit for WiiU Widescreen
            NIN_CFG_LED = (1 << 11),       // Only for Wii Version
            NIN_CFG_LOG = (1 << 12),

            NIN_CFG_MC_MULTI = (1 << 13),
            NIN_CFG_NATIVE_SI = (1 << 14),   // Only for Wii Version
            NIN_CFG_WIIU_WIDE = (1 << 15),   // Only for Wii U Version
            NIN_CFG_ARCADE_MODE = (1 << 16),
            NIN_CFG_CC_RUMBLE = (1 << 17),
            NIN_CFG_SKIP_IPL = (1 << 18),
            NIN_CFG_BBA_EMU = (1 << 19),
        };

        enum ninvideomode
        {
            NIN_VID_AUTO = (0 << 16),
            NIN_VID_FORCE = (1 << 16),
            NIN_VID_NONE = (2 << 16),
            NIN_VID_FORCE_DF = (4 << 16),
            NIN_VID_MASK = NIN_VID_AUTO | NIN_VID_FORCE | NIN_VID_NONE | NIN_VID_FORCE_DF,

            NIN_VID_FORCE_PAL50 = (1 << 0), 
            NIN_VID_FORCE_PAL60 = (1 << 1),
            NIN_VID_FORCE_NTSC = (1 << 2),
            NIN_VID_FORCE_MPAL = (1 << 3),
            NIN_VID_FORCE_MASK = NIN_VID_FORCE_PAL50 | NIN_VID_FORCE_PAL60 | NIN_VID_FORCE_NTSC | NIN_VID_FORCE_MPAL,

            NIN_VID_PROG = (1 << 4),   //important to prevent blackscreens
            NIN_VID_PATCH_PAL50 = (1 << 5), //different force behaviour
        };

        enum ninlanguage : uint
        {
            NIN_LAN_ENGLISH = 0,
            NIN_LAN_GERMAN = 1,
            NIN_LAN_FRENCH = 2,
            NIN_LAN_SPANISH = 3,
            NIN_LAN_ITALIAN = 4,
            NIN_LAN_DUTCH = 5,

            /* Auto will use English for E/P region codes and 
               only other languages when these region codes are used: D/F/S/I/J  */
            NIN_LAN_AUTO = 0xFFFFFFFF,
        };

        private void GenerateConfig_Click(object sender, EventArgs e)
        {

            ConfigFile nintendontCfg = new ConfigFile();
            
            nintendontCfg.magicBytes = 0x01070CF6;
            nintendontCfg.version = 9;
            nintendontCfg.config = 0;
            nintendontCfg.videoMode = 0;
            nintendontCfg.language = 0;
            nintendontCfg.gamePath = new byte[256];
            nintendontCfg.cheatPath = new byte[256];
            nintendontCfg.maxPads = 0;
            nintendontCfg.gameID = 0;
            nintendontCfg.memCardBlocks = 0;
            nintendontCfg.videoScale = 0;
            nintendontCfg.videoOffset = 0;
            nintendontCfg.networkProfile = 0;

            nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_PROG; // always required?

            // Memory Card Emulation
            if (NintendontOptions.GetItemChecked(0))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_MEMCARDEMU;
            }
            // Cheats
            if (NintendontOptions.GetItemChecked(1))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_CHEATS;
            }
            // Cheat Path
            if (NintendontOptions.GetItemChecked(2))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_CHEAT_PATH;
            }
            // Unlock Disc Read Speed
            if (NintendontOptions.GetItemChecked(3))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_REMLIMIT;
            }
            // Wii Remote / Classic Controller Rumble
            if (NintendontOptions.GetItemChecked(4))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_CC_RUMBLE;
            }
            // Triforce Arcade Mode
            if (NintendontOptions.GetItemChecked(5))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_ARCADE_MODE;
            }
            // Broadband Adapter Emulation
            if (NintendontOptions.GetItemChecked(6))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_BBA_EMU;
            }
            // AUTO VIDEO WIDTH
            if (NintendontOptions.GetItemChecked(7))
            {
                //nintendontCfg.videoMode &= (uint)ninvideomode.NIN_VID_MASK;
            }
            // Patch PAL 50
            if (NintendontOptions.GetItemChecked(8))
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_PATCH_PAL50;

                if(VideoTypeMode.SelectedIndex == 0 || VideoTypeMode.SelectedIndex == 3)
                {
                    nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_PAL50;
                }
            }
            // Force Widescreen
            if (NintendontOptions.GetItemChecked(9))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_FORCE_WIDE;
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_WIIU_WIDE;
                //nintendontCfg.config |= (uint)ninconfig.NIN_CFG_USB;
            }
            // Force Progressive Scan
            if (NintendontOptions.GetItemChecked(10))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_FORCE_PROG;
            }
            // Skip IPL
            if (NintendontOptions.GetItemChecked(11))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_SKIP_IPL;
            }
            // OSReport
            if (NintendontOptions.GetItemChecked(12))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_OSREPORT;
            }
            // Log
            if (NintendontOptions.GetItemChecked(13))
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_LOG;
            }


            //Memcard Multi
            if (MemcardMulti.Checked)
            {
                nintendontCfg.config |= (uint)ninconfig.NIN_CFG_MC_MULTI;
            }


            // VIDEO MODES
            // Auto
            if (VideoForceMode.SelectedIndex == 0)
            {
                // do nothing, it's 0
            }
            // Force
            if (VideoForceMode.SelectedIndex == 1)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE;
            }
            // Force (Deflicker)
            if (VideoForceMode.SelectedIndex == 2)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_DF;
            }
            // None
            if (VideoForceMode.SelectedIndex == 3)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_NONE;
            }


            // VIDEO FORCE OPTIONS
            // None
            if (VideoTypeMode.SelectedIndex == 0)
            {
                // do nothing, it's 0;
            }
            // NTSC
            if (VideoTypeMode.SelectedIndex == 1)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_NTSC;
            }
            // MPAL
            if (VideoTypeMode.SelectedIndex == 2)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_MPAL;
            }
            // PAL50
            if (VideoTypeMode.SelectedIndex == 3)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_PAL50;
            }
            // PAL60
            if (VideoTypeMode.SelectedIndex == 4)
            {
                nintendontCfg.videoMode |= (uint)ninvideomode.NIN_VID_FORCE_PAL60;
            }


            // LANGUAGE SELECTION
            if (LanguageBox.SelectedIndex == 0)
            {
                nintendontCfg.language = 0xFFFFFFFF;
            }
            else
            {
                nintendontCfg.language = (uint)LanguageBox.SelectedIndex;
            }

            // MEMCARD BLOCKS
            nintendontCfg.memCardBlocks = (byte)MemcardBlocks.SelectedIndex;

            // VIDEO WIDTH
            if (NintendontOptions.GetItemChecked(7))
            {
                nintendontCfg.videoScale = 0;
            }
            else
            {
                nintendontCfg.videoScale = (sbyte)(VideoWidth.Value - 600);
            }


            // SAVING THE FILE
            string savePath = SelectedDriveLetter + "nincfg.bin";

            // if removable drive isdn't specified, save file manually
            if (DriveSpecified == false)
            {
                DialogResult dialogResult = MessageBox.Show("SD not specified.\nDo you wish to save the file somewhere else?"
                                                            , "Drive not specified"
                                                            , MessageBoxButtons.YesNo
                                                            , MessageBoxIcon.Question
                                                            , MessageBoxDefaultButton.Button1
                                                            , (MessageBoxOptions)0x40000);
                if (dialogResult == DialogResult.Yes)   // if YES, ask where to save the file
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Title = "Save nincfg.bin",
                        CheckPathExists = true,
                        DefaultExt = "bin",
                        Filter = "nintendont config files (*.bin)|*.bin",
                        FilterIndex = 2,
                        RestoreDirectory = true,
                        FileName = "nincfg.bin"
                    };

                    // if a path is decided, store it
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        savePath = saveFileDialog.FileName;
                    }

                    // else, stop the saving process
                    else
                    {
                        return;
                    }

                }
                //  if the user doesn't want to save, stop
                else
                {
                    return;
                }
            }

            // write it
            using (BinaryWriter cfgFile = new BinaryWriter(File.Open(savePath, FileMode.Create) ) )
            {
                byte[] magicBytes = BitConverter.GetBytes(nintendontCfg.magicBytes);
                byte[] version = BitConverter.GetBytes(nintendontCfg.version);
                byte[] config = BitConverter.GetBytes(nintendontCfg.config);
                byte[] videoMode = BitConverter.GetBytes(nintendontCfg.videoMode);
                byte[] language = BitConverter.GetBytes(nintendontCfg.language);
                byte[] maxPads = BitConverter.GetBytes(nintendontCfg.maxPads);
                byte[] gameID = BitConverter.GetBytes(nintendontCfg.gameID);


                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(magicBytes, 0, magicBytes.Length);
                    Array.Reverse(version, 0, version.Length);
                    Array.Reverse(config, 0, config.Length);
                    Array.Reverse(videoMode, 0, videoMode.Length);
                    Array.Reverse(language, 0, language.Length);
                    Array.Reverse(maxPads, 0, maxPads.Length);
                    Array.Reverse(gameID, 0, gameID.Length);
                }

                cfgFile.Write(magicBytes);
                cfgFile.Write(version);
                cfgFile.Write(config);
                cfgFile.Write(videoMode);
                cfgFile.Write(language);
                cfgFile.Write(maxPads);
                cfgFile.Write(gameID);
                cfgFile.Write(nintendontCfg.gamePath);
                cfgFile.Write(nintendontCfg.cheatPath);
                cfgFile.Write(nintendontCfg.memCardBlocks);
                cfgFile.Write(nintendontCfg.videoScale);
                cfgFile.Write(nintendontCfg.videoOffset);
                cfgFile.Write(nintendontCfg.networkProfile);

            }

            MessageBox.Show("Config generation complete."
                            , "Information"
                            , MessageBoxButtons.OK
                            , MessageBoxIcon.Information
                            , MessageBoxDefaultButton.Button1
                            , (MessageBoxOptions)0x40000);
        }

        private void Format_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.ridgecrop.demon.co.uk/index.htm?guiformat.htm");
            Process.Start("http://www.ridgecrop.demon.co.uk/guiformat.exe");
        }
    }
}

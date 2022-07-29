﻿using System;
using System.Windows.Forms;

namespace TeconMoon_s_WiiVC_Injector
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            _ = Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WiiVC_Injector());
        }
    }
}

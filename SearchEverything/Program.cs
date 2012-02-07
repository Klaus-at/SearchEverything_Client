using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SearchEverything
{
    static class Program
    {
        public static SearchForm MainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new SearchForm();
            Application.Run(MainForm);
        }

    }
}
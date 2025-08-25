using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeoulHotel.Forms;

namespace SeoulHotel
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Properties.Settings.Default.RememberMe)
            {
                Application.Run(new Management_Form());
            }
            else
            {
                Application.Run(new Login());
            }
        }
    }
}

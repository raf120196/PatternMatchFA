using System;
using System.Windows.Forms;

namespace PatternMatchFA
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new RegularExpressions());
        }
    }
}

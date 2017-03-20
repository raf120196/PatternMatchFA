using System;
using System.Windows.Forms;

namespace PatternMatchFA
{
    public partial class ForLog : Form
    {
        string log_text = String.Empty;
        public ForLog()
        {                        
        }

        public ForLog(string s)
        {
            InitializeComponent();
            log.Show();
            log_text = s;
            log.Text = log_text;
        }
    }
}

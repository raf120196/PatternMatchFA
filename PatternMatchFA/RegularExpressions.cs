using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PatternMatchFA
{
    public partial class RegularExpressions : Form
    {
        RegExpr regex = new RegExpr();
        bool isFirst = true;
        StringBuilder sb = null;

        public RegularExpressions()
        {
            InitializeComponent();
        }

        private void compile_Click(object sender, EventArgs e)
        {
            sb = new StringBuilder(10000);
            if (pattern.Text.Length == 0)
            {
                MessageBox.Show("Сначала нужно ввести шаблон.");
                pattern.Select();
                pattern.Focus();
                return;
            }

            try
            {
                TypeError errCode = regex.CompilationWithLog(this.pattern.Text, sb);
                if (errCode != TypeError.ERROR_NO)
                {
                    string errorText = pattern.Text.Substring(regex.GetLastErrorPosition(), regex.GetLastErrorLength());
                    string mess = "Произошла ошибка во время обработки.\nТип: {0}\nВ: {1}\nПодстрока: {2}";
                    mess = String.Format(mess, errCode.ToString(), regex.GetLastErrorPosition(), errorText);
                    pattern.Select(regex.GetLastErrorPosition(), regex.GetLastErrorLength());
                    pattern.Focus();
                    MessageBox.Show(mess);
                    pattern.Select();
                    pattern.Focus();
                    return;
                }
                this.info1.Clear();
                MessageBox.Show("Регулярное выражение успешно обработано.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка во время обработки.\n\n" + ex.Message);
                pattern.Select();
                pattern.Focus();
                return;
            }

            isFirst = true;
        }

        private void findall_Click(object sender, EventArgs e)
        {
            if (!regex.IsReady())
            {
                MessageBox.Show("Сначала нужно обработать шаблон.");
                pattern.Focus();
                return;
            }

            info1.Clear();
            info1.AcceptChanges();

            int foundStartPos = -1;
            int foundEndPos = -1;
            int startPos = 0;
            int lengthOfMatch = -1;
            int count = 1;

            do
            {
                bool isFind = regex.FindMatch(text.Text, startPos, text.Text.Length - 1, ref foundStartPos, ref foundEndPos);
                if (isFind)
                {
                    string s = "{Empty String}";
                    lengthOfMatch = foundEndPos - foundStartPos + 1;
                    if (lengthOfMatch > 0)
                    {
                        s = text.Text.Substring(foundStartPos, lengthOfMatch);
                    }
                    info1.Set.AddSetRow(count++, s, foundStartPos, foundEndPos, lengthOfMatch);
                    info1.AcceptChanges();
                    startPos = foundEndPos + 1;
                }
                else
                {
                    break;
                }
            } while (startPos < text.Text.Length);
        }

        private void findnext_Click(object sender, EventArgs e)
        {
            if (!regex.IsReady())
            {
                MessageBox.Show("Сначала нужно обработать шаблон.");
                pattern.Focus();
                return;
            }
            int foundStartPos = -1;
            int foundEndPos = -1;
            int startPos = -1;

            if (isFirst)
            {
                startPos = text.SelectionStart;
                isFirst = false;
            }
            else
            {
                startPos = text.SelectionStart + 1;
            }

            bool isFind = regex.FindMatch(text.Text, startPos, text.Text.Length - 1, ref foundStartPos, ref foundEndPos);
            if (isFind)
            {
                int lengthOfMatch = foundEndPos - foundStartPos + 1;
                if (lengthOfMatch == 0)
                {
                    MessageBox.Show("Найдена пустая строка в позиции " + foundStartPos.ToString() + ".");
                }
                else
                {
                    text.Select(foundStartPos, lengthOfMatch);
                    text.Focus();
                    text.ScrollToCaret();
                }
            }
            else
            {
                MessageBox.Show("Совпадений не найдено.");
            }
        }

        private void findfirst_Click(object sender, EventArgs e)
        {
            if (!regex.IsReady())
            {
                MessageBox.Show("Сначала нужно обработать шаблон.");
                pattern.Focus();
                return;
            }
            int foundStartPos = -1;
            int foundEndPos = -1;

            bool isFind = regex.FindMatch(text.Text, 0, text.Text.Length - 1, ref foundStartPos, ref foundEndPos);
            if (isFind)
            {
                int lengthOfMatch = foundEndPos - foundStartPos + 1;
                if (lengthOfMatch == 0)
                {
                    MessageBox.Show("Найдена пустая строка в позиции " + foundStartPos.ToString() + ".");
                }
                else
                {
                    text.Select(foundStartPos, lengthOfMatch);
                    text.Focus();
                    text.ScrollToCaret();
                }
            }
            else
            {
                MessageBox.Show("Совпадений не найдено.");
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataRowView d = (DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem;
                Info.SetRow row = (Info.SetRow)d.Row;

                text.Select(row.Начальная_позиция, row.Длина);
                text.Focus();
                text.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void text_KeyDown(object sender, KeyEventArgs e)
        {
            isFirst = true;
        }

        private void text_MouseDown(object sender, MouseEventArgs e)
        {
            isFirst = true;
        }

        private void text_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = findall;
        }

        private void pattern_Enter(object sender, EventArgs e)
        {
            this.AcceptButton = compile;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            regex.IsGreedy = checkBox1.Checked;
        }

        private void log_Click(object sender, EventArgs e)
        {
            if (sb == null)
            {
                MessageBox.Show("Сначала нужно ввести и обработать шаблон.");
                pattern.Focus();
                return;
            }

            ForLog secondform = new ForLog(sb.ToString());
            secondform.ShowDialog();
        }
    }
}

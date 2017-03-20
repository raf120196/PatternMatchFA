namespace PatternMatchFA
{
    partial class RegularExpressions
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegularExpressions));
            this.compile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pattern = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.text = new System.Windows.Forms.TextBox();
            this.findfirst = new System.Windows.Forms.Button();
            this.findnext = new System.Windows.Forms.Button();
            this.findall = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.номерDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.найденныйОбразDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.начальнаяПозицияDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.конечнаяПозицияDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.длинаDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.info1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.info1 = new PatternMatchFA.Info();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.log = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.info1BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.info1)).BeginInit();
            this.SuspendLayout();
            // 
            // compile
            // 
            this.compile.Location = new System.Drawing.Point(602, 7);
            this.compile.Name = "compile";
            this.compile.Size = new System.Drawing.Size(75, 23);
            this.compile.TabIndex = 0;
            this.compile.Text = "Обработать";
            this.compile.UseVisualStyleBackColor = true;
            this.compile.Click += new System.EventHandler(this.compile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Регулярное выражение";
            // 
            // pattern
            // 
            this.pattern.Location = new System.Drawing.Point(145, 9);
            this.pattern.Name = "pattern";
            this.pattern.Size = new System.Drawing.Size(441, 20);
            this.pattern.TabIndex = 2;
            this.pattern.Enter += new System.EventHandler(this.pattern_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Текст для поиска";
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(15, 107);
            this.text.Multiline = true;
            this.text.Name = "text";
            this.text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.text.Size = new System.Drawing.Size(662, 198);
            this.text.TabIndex = 4;
            this.text.Enter += new System.EventHandler(this.text_Enter);
            this.text.KeyDown += new System.Windows.Forms.KeyEventHandler(this.text_KeyDown);
            this.text.MouseDown += new System.Windows.Forms.MouseEventHandler(this.text_MouseDown);
            // 
            // findfirst
            // 
            this.findfirst.Location = new System.Drawing.Point(696, 107);
            this.findfirst.Name = "findfirst";
            this.findfirst.Size = new System.Drawing.Size(96, 23);
            this.findfirst.TabIndex = 5;
            this.findfirst.Text = "Найти первое";
            this.findfirst.UseVisualStyleBackColor = true;
            this.findfirst.Click += new System.EventHandler(this.findfirst_Click);
            // 
            // findnext
            // 
            this.findnext.Location = new System.Drawing.Point(697, 147);
            this.findnext.Name = "findnext";
            this.findnext.Size = new System.Drawing.Size(95, 23);
            this.findnext.TabIndex = 6;
            this.findnext.Text = "Найти далее";
            this.findnext.UseVisualStyleBackColor = true;
            this.findnext.Click += new System.EventHandler(this.findnext_Click);
            // 
            // findall
            // 
            this.findall.Location = new System.Drawing.Point(697, 190);
            this.findall.Name = "findall";
            this.findall.Size = new System.Drawing.Size(95, 23);
            this.findall.TabIndex = 7;
            this.findall.Text = "Найти всё";
            this.findall.UseVisualStyleBackColor = true;
            this.findall.Click += new System.EventHandler(this.findall_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.номерDataGridViewTextBoxColumn,
            this.найденныйОбразDataGridViewTextBoxColumn,
            this.начальнаяПозицияDataGridViewTextBoxColumn,
            this.конечнаяПозицияDataGridViewTextBoxColumn,
            this.длинаDataGridViewTextBoxColumn});
            this.dataGridView1.DataMember = "Set";
            this.dataGridView1.DataSource = this.info1BindingSource;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView1.Location = new System.Drawing.Point(15, 351);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(662, 150);
            this.dataGridView1.TabIndex = 8;
            // 
            // номерDataGridViewTextBoxColumn
            // 
            this.номерDataGridViewTextBoxColumn.DataPropertyName = "Номер";
            this.номерDataGridViewTextBoxColumn.HeaderText = "Номер";
            this.номерDataGridViewTextBoxColumn.Name = "номерDataGridViewTextBoxColumn";
            // 
            // найденныйОбразDataGridViewTextBoxColumn
            // 
            this.найденныйОбразDataGridViewTextBoxColumn.DataPropertyName = "Найденный образ";
            this.найденныйОбразDataGridViewTextBoxColumn.HeaderText = "Найденный образ";
            this.найденныйОбразDataGridViewTextBoxColumn.Name = "найденныйОбразDataGridViewTextBoxColumn";
            this.найденныйОбразDataGridViewTextBoxColumn.Width = 200;
            // 
            // начальнаяПозицияDataGridViewTextBoxColumn
            // 
            this.начальнаяПозицияDataGridViewTextBoxColumn.DataPropertyName = "Начальная позиция";
            this.начальнаяПозицияDataGridViewTextBoxColumn.HeaderText = "Начальная позиция";
            this.начальнаяПозицияDataGridViewTextBoxColumn.Name = "начальнаяПозицияDataGridViewTextBoxColumn";
            // 
            // конечнаяПозицияDataGridViewTextBoxColumn
            // 
            this.конечнаяПозицияDataGridViewTextBoxColumn.DataPropertyName = "Конечная позиция";
            this.конечнаяПозицияDataGridViewTextBoxColumn.HeaderText = "Конечная позиция";
            this.конечнаяПозицияDataGridViewTextBoxColumn.Name = "конечнаяПозицияDataGridViewTextBoxColumn";
            // 
            // длинаDataGridViewTextBoxColumn
            // 
            this.длинаDataGridViewTextBoxColumn.DataPropertyName = "Длина";
            this.длинаDataGridViewTextBoxColumn.HeaderText = "Длина";
            this.длинаDataGridViewTextBoxColumn.Name = "длинаDataGridViewTextBoxColumn";
            // 
            // info1BindingSource
            // 
            this.info1BindingSource.DataSource = this.info1;
            this.info1BindingSource.Position = 0;
            // 
            // info1
            // 
            this.info1.DataSetName = "Info";
            this.info1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Результаты";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(499, 311);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(178, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Найти длиннейшую подстроку";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // log
            // 
            this.log.Location = new System.Drawing.Point(697, 255);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(96, 35);
            this.log.TabIndex = 11;
            this.log.Text = "Показать журнал";
            this.log.UseVisualStyleBackColor = true;
            this.log.Click += new System.EventHandler(this.log_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(15, 48);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(224, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Поиск с помощью конечных полугрупп";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // RegularExpressions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 507);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.log);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.findall);
            this.Controls.Add(this.findnext);
            this.Controls.Add(this.findfirst);
            this.Controls.Add(this.text);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.compile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegularExpressions";
            this.Text = "Поиск шаблонов в тексте с помощью конечных автоматов";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.info1BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.info1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button compile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pattern;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox text;
        private System.Windows.Forms.Button findfirst;
        private System.Windows.Forms.Button findnext;
        private System.Windows.Forms.Button findall;
        private System.Windows.Forms.DataGridView dataGridView1;
        private Info info1;
        private System.Windows.Forms.DataGridViewTextBoxColumn номерDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn найденныйОбразDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn начальнаяПозицияDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn конечнаяПозицияDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn длинаDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource info1BindingSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button log;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}
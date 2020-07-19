namespace CrudGeneratorForm
{
    partial class FrmPrincipal
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
            this.txtConexao = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConectar = new System.Windows.Forms.Button();
            this.cbxTabelas = new System.Windows.Forms.CheckedListBox();
            this.btnGerar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.btnMarcarTabelas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtConexao
            // 
            this.txtConexao.Location = new System.Drawing.Point(15, 25);
            this.txtConexao.Name = "txtConexao";
            this.txtConexao.Size = new System.Drawing.Size(257, 20);
            this.txtConexao.TabIndex = 0;
            this.txtConexao.Text = "Data Source=(LocalDb)\\v11.0;Initial Catalog=TesteGerador;Integrated Security=True" +
    ";Pooling=False";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Conexão";
            // 
            // btnConectar
            // 
            this.btnConectar.Location = new System.Drawing.Point(15, 97);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(114, 23);
            this.btnConectar.TabIndex = 2;
            this.btnConectar.Text = "Obter Tabelas";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // cbxTabelas
            // 
            this.cbxTabelas.FormattingEnabled = true;
            this.cbxTabelas.Location = new System.Drawing.Point(15, 126);
            this.cbxTabelas.Name = "cbxTabelas";
            this.cbxTabelas.Size = new System.Drawing.Size(257, 154);
            this.cbxTabelas.TabIndex = 4;
            // 
            // btnGerar
            // 
            this.btnGerar.Location = new System.Drawing.Point(15, 287);
            this.btnGerar.Name = "btnGerar";
            this.btnGerar.Size = new System.Drawing.Size(75, 23);
            this.btnGerar.TabIndex = 5;
            this.btnGerar.Text = "Gerar CRUD";
            this.btnGerar.UseVisualStyleBackColor = true;
            this.btnGerar.Click += new System.EventHandler(this.btnGerar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "NameSpace:";
            // 
            // txtNameSpace
            // 
            this.txtNameSpace.Location = new System.Drawing.Point(15, 65);
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.Size = new System.Drawing.Size(257, 20);
            this.txtNameSpace.TabIndex = 7;
            this.txtNameSpace.Text = "CrudGeneratorTest";
            // 
            // btnMarcarTabelas
            // 
            this.btnMarcarTabelas.Location = new System.Drawing.Point(135, 97);
            this.btnMarcarTabelas.Name = "btnMarcarTabelas";
            this.btnMarcarTabelas.Size = new System.Drawing.Size(106, 23);
            this.btnMarcarTabelas.TabIndex = 8;
            this.btnMarcarTabelas.Text = "Marcar Todas";
            this.btnMarcarTabelas.UseVisualStyleBackColor = true;
            this.btnMarcarTabelas.Click += new System.EventHandler(this.btnMarcarTabelas_Click);
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 421);
            this.Controls.Add(this.btnMarcarTabelas);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGerar);
            this.Controls.Add(this.cbxTabelas);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtConexao);
            this.Name = "FrmPrincipal";
            this.Text = "CRUD Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConexao;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.CheckedListBox cbxTabelas;
        private System.Windows.Forms.Button btnGerar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNameSpace;
        private System.Windows.Forms.Button btnMarcarTabelas;
    }
}


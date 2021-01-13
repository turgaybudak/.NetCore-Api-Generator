
namespace AspDotNetCoreAraci
{
    partial class AspDotNetAraci
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AspDotNetAraci));
            this.ProjeAdi = new System.Windows.Forms.Label();
            this.ConString = new System.Windows.Forms.Label();
            this.txtProjeAdi = new System.Windows.Forms.TextBox();
            this.txtConString = new System.Windows.Forms.TextBox();
            this.btnKlasor = new System.Windows.Forms.Button();
            this.btnHazirla = new System.Windows.Forms.Button();
            this.Bar = new System.Windows.Forms.ProgressBar();
            this.lstTablolar = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ProjeAdi
            // 
            this.ProjeAdi.AutoSize = true;
            this.ProjeAdi.Location = new System.Drawing.Point(34, 42);
            this.ProjeAdi.Name = "ProjeAdi";
            this.ProjeAdi.Size = new System.Drawing.Size(49, 13);
            this.ProjeAdi.TabIndex = 0;
            this.ProjeAdi.Text = "Proje Adi";
            // 
            // ConString
            // 
            this.ConString.AutoSize = true;
            this.ConString.Location = new System.Drawing.Point(34, 77);
            this.ConString.Name = "ConString";
            this.ConString.Size = new System.Drawing.Size(91, 13);
            this.ConString.TabIndex = 1;
            this.ConString.Text = "Connection String";
            // 
            // txtProjeAdi
            // 
            this.txtProjeAdi.Location = new System.Drawing.Point(137, 42);
            this.txtProjeAdi.Name = "txtProjeAdi";
            this.txtProjeAdi.Size = new System.Drawing.Size(100, 20);
            this.txtProjeAdi.TabIndex = 2;
            this.txtProjeAdi.TextChanged += new System.EventHandler(this.txtProjeAdi_TextChanged);
            // 
            // txtConString
            // 
            this.txtConString.Location = new System.Drawing.Point(137, 74);
            this.txtConString.Name = "txtConString";
            this.txtConString.Size = new System.Drawing.Size(100, 20);
            this.txtConString.TabIndex = 3;
            this.txtConString.TextChanged += new System.EventHandler(this.txtConString_TextChanged);
            // 
            // btnKlasor
            // 
            this.btnKlasor.Location = new System.Drawing.Point(37, 313);
            this.btnKlasor.Name = "btnKlasor";
            this.btnKlasor.Size = new System.Drawing.Size(75, 23);
            this.btnKlasor.TabIndex = 4;
            this.btnKlasor.Text = "Klasor";
            this.btnKlasor.UseVisualStyleBackColor = true;
            this.btnKlasor.Click += new System.EventHandler(this.btnKlasor_Click);
            // 
            // btnHazirla
            // 
            this.btnHazirla.Location = new System.Drawing.Point(162, 313);
            this.btnHazirla.Name = "btnHazirla";
            this.btnHazirla.Size = new System.Drawing.Size(75, 23);
            this.btnHazirla.TabIndex = 5;
            this.btnHazirla.Text = "Hazırla";
            this.btnHazirla.UseVisualStyleBackColor = true;
            this.btnHazirla.Click += new System.EventHandler(this.btnHazirla_Click);
            // 
            // Bar
            // 
            this.Bar.BackColor = System.Drawing.Color.FloralWhite;
            this.Bar.Location = new System.Drawing.Point(-1, -4);
            this.Bar.Name = "Bar";
            this.Bar.Size = new System.Drawing.Size(284, 10);
            this.Bar.TabIndex = 6;
            // 
            // lstTablolar
            // 
            this.lstTablolar.FormattingEnabled = true;
            this.lstTablolar.Location = new System.Drawing.Point(37, 101);
            this.lstTablolar.Name = "lstTablolar";
            this.lstTablolar.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstTablolar.Size = new System.Drawing.Size(199, 199);
            this.lstTablolar.TabIndex = 7;
            // 
            // AspDotNetAraci
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.ClientSize = new System.Drawing.Size(283, 362);
            this.Controls.Add(this.lstTablolar);
            this.Controls.Add(this.Bar);
            this.Controls.Add(this.btnHazirla);
            this.Controls.Add(this.btnKlasor);
            this.Controls.Add(this.txtConString);
            this.Controls.Add(this.txtProjeAdi);
            this.Controls.Add(this.ConString);
            this.Controls.Add(this.ProjeAdi);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AspDotNetAraci";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asp.Net Core Aracı";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProjeAdi;
        private System.Windows.Forms.Label ConString;
        private System.Windows.Forms.TextBox txtProjeAdi;
        private System.Windows.Forms.TextBox txtConString;
        private System.Windows.Forms.Button btnKlasor;
        private System.Windows.Forms.Button btnHazirla;
        private System.Windows.Forms.ProgressBar Bar;
        private System.Windows.Forms.ListBox lstTablolar;
    }
}


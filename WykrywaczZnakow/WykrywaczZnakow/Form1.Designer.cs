namespace WykrywaczZnakow
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.zdjecieGlowneBox = new System.Windows.Forms.PictureBox();
            this.zdjecieCannyBox = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btn_wczytajZdjecie = new System.Windows.Forms.Button();
            this.btn_wykryjZnaki = new System.Windows.Forms.Button();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.zdjecieWykrytyZnak = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieGlowneBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieCannyBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieWykrytyZnak)).BeginInit();
            this.SuspendLayout();
            // 
            // zdjecieGlowneBox
            // 
            this.zdjecieGlowneBox.Location = new System.Drawing.Point(12, 73);
            this.zdjecieGlowneBox.Name = "zdjecieGlowneBox";
            this.zdjecieGlowneBox.Size = new System.Drawing.Size(450, 320);
            this.zdjecieGlowneBox.TabIndex = 1;
            this.zdjecieGlowneBox.TabStop = false;
            this.zdjecieGlowneBox.Click += new System.EventHandler(this.ZdjecieGlowneBox_Click);
            // 
            // zdjecieCannyBox
            // 
            this.zdjecieCannyBox.Location = new System.Drawing.Point(488, 73);
            this.zdjecieCannyBox.Name = "zdjecieCannyBox";
            this.zdjecieCannyBox.Size = new System.Drawing.Size(250, 150);
            this.zdjecieCannyBox.TabIndex = 2;
            this.zdjecieCannyBox.TabStop = false;
            this.zdjecieCannyBox.Click += new System.EventHandler(this.ZdjecieCannyBox_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(762, 73);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(450, 320);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // btn_wczytajZdjecie
            // 
            this.btn_wczytajZdjecie.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_wczytajZdjecie.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_wczytajZdjecie.Location = new System.Drawing.Point(12, 27);
            this.btn_wczytajZdjecie.Name = "btn_wczytajZdjecie";
            this.btn_wczytajZdjecie.Size = new System.Drawing.Size(131, 40);
            this.btn_wczytajZdjecie.TabIndex = 5;
            this.btn_wczytajZdjecie.Text = "Otwórz";
            this.btn_wczytajZdjecie.UseVisualStyleBackColor = true;
            this.btn_wczytajZdjecie.Click += new System.EventHandler(this.Btn_wczytajZdjecie_Click);
            // 
            // btn_wykryjZnaki
            // 
            this.btn_wykryjZnaki.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_wykryjZnaki.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btn_wykryjZnaki.Location = new System.Drawing.Point(149, 27);
            this.btn_wykryjZnaki.Name = "btn_wykryjZnaki";
            this.btn_wykryjZnaki.Size = new System.Drawing.Size(147, 40);
            this.btn_wykryjZnaki.TabIndex = 6;
            this.btn_wykryjZnaki.Text = "Rozpocznij";
            this.btn_wykryjZnaki.UseVisualStyleBackColor = true;
            this.btn_wykryjZnaki.Click += new System.EventHandler(this.Btn_wykryjZnaki_Click);
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(12, 409);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(1212, 23);
            this.progressBar2.TabIndex = 8;
            this.progressBar2.Click += new System.EventHandler(this.ProgressBar2_Click);
            // 
            // zdjecieWykrytyZnak
            // 
            this.zdjecieWykrytyZnak.Location = new System.Drawing.Point(488, 243);
            this.zdjecieWykrytyZnak.Name = "zdjecieWykrytyZnak";
            this.zdjecieWykrytyZnak.Size = new System.Drawing.Size(250, 150);
            this.zdjecieWykrytyZnak.TabIndex = 9;
            this.zdjecieWykrytyZnak.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Font = new System.Drawing.Font("Impact", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(12, 459);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 34);
            this.label1.TabIndex = 10;
            this.label1.Click += new System.EventHandler(this.Label1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1234, 559);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zdjecieWykrytyZnak);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.btn_wykryjZnaki);
            this.Controls.Add(this.btn_wczytajZdjecie);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.zdjecieCannyBox);
            this.Controls.Add(this.zdjecieGlowneBox);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieGlowneBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieCannyBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zdjecieWykrytyZnak)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox zdjecieGlowneBox;
        private System.Windows.Forms.PictureBox zdjecieCannyBox;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btn_wczytajZdjecie;
        private System.Windows.Forms.Button btn_wykryjZnaki;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.PictureBox zdjecieWykrytyZnak;
        private System.Windows.Forms.Label label1;
    }
}


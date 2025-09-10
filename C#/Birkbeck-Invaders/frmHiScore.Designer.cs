namespace Birkbeck_Invaders
{
    partial class frmHiScore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHiScore));
            this.lblEalingInvaders = new System.Windows.Forms.Label();
            this.lblHoF = new System.Windows.Forms.Label();
            this.txtHiScore = new System.Windows.Forms.TextBox();
            this.lblEnterName = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblEalingInvaders
            // 
            this.lblEalingInvaders.AutoSize = true;
            this.lblEalingInvaders.Font = new System.Drawing.Font("Tempus Sans ITC", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEalingInvaders.ForeColor = System.Drawing.Color.Green;
            this.lblEalingInvaders.Location = new System.Drawing.Point(223, 36);
            this.lblEalingInvaders.Name = "lblEalingInvaders";
            this.lblEalingInvaders.Size = new System.Drawing.Size(495, 73);
            this.lblEalingInvaders.TabIndex = 0;
            this.lblEalingInvaders.Text = "EALING ATTACK!";
            // 
            // lblHoF
            // 
            this.lblHoF.AutoSize = true;
            this.lblHoF.Font = new System.Drawing.Font("Tempus Sans ITC", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHoF.ForeColor = System.Drawing.Color.Green;
            this.lblHoF.Location = new System.Drawing.Point(337, 90);
            this.lblHoF.Name = "lblHoF";
            this.lblHoF.Size = new System.Drawing.Size(186, 37);
            this.lblHoF.TabIndex = 1;
            this.lblHoF.Text = "Hall Of Fame";
            // 
            // txtHiScore
            // 
            this.txtHiScore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.txtHiScore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHiScore.Font = new System.Drawing.Font("Tempus Sans ITC", 28F, System.Drawing.FontStyle.Bold);
            this.txtHiScore.ForeColor = System.Drawing.Color.Gray;
            this.txtHiScore.Location = new System.Drawing.Point(249, 284);
            this.txtHiScore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHiScore.Name = "txtHiScore";
            this.txtHiScore.Size = new System.Drawing.Size(306, 81);
            this.txtHiScore.TabIndex = 3;
            this.txtHiScore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHiScore.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHiScore_KeyDown);
            // 
            // lblEnterName
            // 
            this.lblEnterName.AutoSize = true;
            this.lblEnterName.Font = new System.Drawing.Font("Tempus Sans ITC", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnterName.ForeColor = System.Drawing.Color.Green;
            this.lblEnterName.Location = new System.Drawing.Point(283, 209);
            this.lblEnterName.Name = "lblEnterName";
            this.lblEnterName.Size = new System.Drawing.Size(324, 37);
            this.lblEnterName.TabIndex = 4;
            this.lblEnterName.Text = "Please Enter Your Name";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Font = new System.Drawing.Font("Tempus Sans ITC", 18F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.Green;
            this.button1.Location = new System.Drawing.Point(288, 538);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(232, 40);
            this.button1.TabIndex = 5;
            this.button1.Text = "New Game";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Tempus Sans ITC", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(184, 606);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(573, 26);
            this.label1.TabIndex = 6;
            this.label1.Tag = "";
            this.label1.Text = "Press \'S\' To Toggle Background Sound (for slower machines)";
            // 
            // frmHiScore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(832, 641);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblEnterName);
            this.Controls.Add(this.txtHiScore);
            this.Controls.Add(this.lblHoF);
            this.Controls.Add(this.lblEalingInvaders);
            this.Font = new System.Drawing.Font("Tempus Sans ITC", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmHiScore";
            this.Text = "Ealing Attack Hall of Fame";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEalingInvaders;
        private System.Windows.Forms.Label lblHoF;
        private System.Windows.Forms.TextBox txtHiScore;
        private System.Windows.Forms.Label lblEnterName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}
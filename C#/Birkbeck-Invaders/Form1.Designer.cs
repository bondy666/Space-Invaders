namespace Birkbeck_Invaders
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblScore = new System.Windows.Forms.Label();
            this.lblLIVES = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.performanceCounter1 = new System.Diagnostics.PerformanceCounter();
            this.performanceCounter2 = new System.Diagnostics.PerformanceCounter();
            this.spaceshipPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spaceshipPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.Location = new System.Drawing.Point(895, 9);
            this.lblScore.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(57, 20);
            this.lblScore.TabIndex = 0;
            this.lblScore.Text = "SCORE: ";
            // 
            // lblLIVES
            // 
            this.lblLIVES.AutoSize = true;
            this.lblLIVES.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLIVES.Location = new System.Drawing.Point(47, 9);
            this.lblLIVES.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLIVES.Name = "lblLIVES";
            this.lblLIVES.Size = new System.Drawing.Size(51, 20);
            this.lblLIVES.TabIndex = 1;
            this.lblLIVES.Text = "LIVES : ";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Font = new System.Drawing.Font("Impact", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLevel.Location = new System.Drawing.Point(457, 9);
            this.lblLevel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(51, 20);
            this.lblLevel.TabIndex = 2;
            this.lblLevel.Text = "LEVEL : ";
            // 
            // spaceshipPictureBox
            // 
            this.spaceshipPictureBox.BackColor = System.Drawing.Color.Black;
            this.spaceshipPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("spaceshipPictureBox.Image")));
            this.spaceshipPictureBox.Location = new System.Drawing.Point(50, 60);
            this.spaceshipPictureBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.spaceshipPictureBox.Name = "spaceshipPictureBox";
            this.spaceshipPictureBox.Size = new System.Drawing.Size(98, 49);
            this.spaceshipPictureBox.TabIndex = 3;
            this.spaceshipPictureBox.TabStop = false;
            this.spaceshipPictureBox.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1050, 658);
            this.Controls.Add(this.spaceshipPictureBox);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lblLIVES);
            this.Controls.Add(this.lblScore);
            this.Font = new System.Drawing.Font("Tempus Sans ITC", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.Name = "Form1";
            this.Text = "Ealing Attack!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblLIVES;
        private System.Windows.Forms.Label lblLevel;
        private System.Diagnostics.PerformanceCounter performanceCounter1;
        private System.Diagnostics.PerformanceCounter performanceCounter2;
        private System.Windows.Forms.PictureBox spaceshipPictureBox;
    }
}


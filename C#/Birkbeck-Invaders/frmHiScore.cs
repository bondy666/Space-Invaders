using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace Birkbeck_Invaders
{
    
    public partial class frmHiScore : Form
    {
        // List to store player names and scores
        private List<Tuple<string, int>> highScores = new List<Tuple<string, int>>();
        private List<int> scoreList = new List<int>(); // List to store scores only
        public  int hiscore;
        private const string HighscoreFile = "highscores.txt";
        private PictureBox pbSpaceShip;
        private Timer animationTimer;
        private int spaceshipSpeed = 5; // pixels per timer tick
        private bool HallofFame = false;
        public frmHiScore(int currentscore)
        {
            InitializeComponent();
            hiscore = currentscore;
            LoadHighScores();
            SetupSpaceship();
        }

        private void SetupSpaceship()
        {
            // Create and configure the spaceship PictureBox
            pbSpaceShip = new PictureBox();
            pbSpaceShip.Size = new Size(64, 64);
            pbSpaceShip.Location = new Point(-64, 100); // Start off-screen left
            pbSpaceShip.SizeMode = PictureBoxSizeMode.StretchImage;
            // Load spaceship image
            pbSpaceShip.Image = Image.FromFile("saucer2.jpg");
            pbSpaceShip.BackColor = Color.Black;
            // Add to form
            this.Controls.Add(pbSpaceShip);
            // Setup timer for animation
            animationTimer = new Timer();
            animationTimer.Interval = 30; // ~33 FPS
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Move spaceship to the right
            pbSpaceShip.Left += spaceshipSpeed;
            // Reset position when it goes off-screen
            if (pbSpaceShip.Left > this.Width)
            {
                pbSpaceShip.Left = -pbSpaceShip.Width;
            }
        }

        private void LoadHighScores()
        {
            // Add score and sort
           
            highScores.Clear();

            if (System.IO.File.Exists(HighscoreFile))
            {
                foreach (var line in System.IO.File.ReadAllLines(HighscoreFile))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                        {
                            scoreList.Add(score);
                            highScores.Add(new Tuple<string, int>(parts[0], score));
                        }
                    }

                foreach (var score in scoreList)
                    {
                        if (hiscore > score) HallofFame = true;
                    }

            }
            if (!HallofFame)
            {
                txtHiScore.Visible = false; // Hide TextBox if not a high score
                lblEnterName.Visible = false;
                DisplayHighScores();
            }
            else
            {
                txtHiScore.Visible = true;
                lblEnterName.Visible = true;

            }


        }

        private void txtHiScore_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if(txtHiScore.Text.Length < 1)
                {
                    return;
                }
                // LoadHighScores();
                string name = txtHiScore.Text;
                highScores.Add(new Tuple<string, int>(name, hiscore));   
                highScores.Sort((a, b) => b.Item2.CompareTo(hiscore));
                if (highScores.Count > 6) highScores = highScores.GetRange(0, 5); // Keep top 6
                SaveHighscores();

                // Sort scores in descending order
                highScores = highScores.OrderByDescending(hiscore => hiscore.Item2).Take(5).ToList();
                txtHiScore.Visible = false; // Hide the TextBox after entering the name
                lblEnterName.Visible = false;
                DisplayHighScores();
                e.Handled = true; // Optional: Prevent further processing
            }
        }

        private void DisplayHighScores()
        {
            int yPosition = 170; // Starting Y position for labels
            if (!HallofFame)
            {
                txtHiScore.Visible = false; // Hide the TextBox after entering the name
                lblEnterName.Visible = false;
            }
            if (highScores.Count > 6) highScores = highScores.GetRange(0, 5); // Keep top 6
            foreach (var score in highScores)
            {
                int namecount = score.Item1.ToString().Length;
                string dotdotdot = new string('.',32 - namecount); // Adjust number of dots based on name length
                // Create a new label for each score
                System.Windows.Forms.Label scoreLabel = new System.Windows.Forms.Label()
                {
                    Text = $"{score.Item1}: " + dotdotdot + $" {score.Item2}",
                    AutoSize = true,
                    Location = new System.Drawing.Point(100, yPosition),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Tempus Sans ITC", 28, System.Drawing.FontStyle.Bold)
                };

                // Add the label to the form
                this.Controls.Add(scoreLabel);

                // Increment Y position for the next label
                yPosition += 40;
            }
        }

        private void SaveHighscores()
        {
            var lines = new List<string>();
            foreach (var entry in highScores)
                lines.Add($"{entry.Item1},{entry.Item2}");
            System.IO.File.WriteAllLines(HighscoreFile, lines);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 mainForm = new Form1();
            mainForm.Show();    
        }
    }
}


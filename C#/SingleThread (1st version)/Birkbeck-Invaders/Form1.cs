using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Birkbeck_Invaders
{
    public partial class Form1 : Form
    {
        private PictureBox turret;
            private int turretSpeed = 4; // Smoother movement
        private List<PictureBox> bullets = new List<PictureBox>();
        private Timer bulletTimer;
        // Aliens
        private List<PictureBox> aliens = new List<PictureBox>();
        private Timer alienTimer;
        private int alienDirection = 1; // 1 = right, -1 = left
        private int alienSpeed = 5;
        //  Base
        private List<BasePart> bases = new List<BasePart>();
        // Lasers
        private List<PictureBox> alienLasers = new List<PictureBox>();
        private Timer alienLaserTimer;
        private Random rand = new Random();
        private int lives = 3; // Add lives field

        private bool movingLeft = false;
        private bool movingRight = false;
        private Timer turretMoveTimer;

        private bool canFire = true;
        private Timer fireCooldownTimer;

        private PictureBox GameOver = new PictureBox
        {
            Size = new Size(200, 120),
            BackColor = Color.Black,
            Location = new Point(350, 260),
            ImageLocation = "GameOver.gif"
        };

        private System.Media.SoundPlayer TurretExplode;
        private System.Media.SoundPlayer backgroundPlayer1;
        private System.Media.SoundPlayer backgroundPlayer2;
        private int currentBackground = 1;
        private bool duhduhPlaying = true;
        private Timer backgroundSwitchTimer;

        /*var GameOver = new PictureBox
        {
            Size = new Size(200, 120),
            BackColor = Color.Black,
            Location = new Point(350, 260),
            ImageLocation = "GameOver.gif"
        };
        */
        private const string HighscoreFile = "highscores.txt";
        private List<(string Name, int Score)> highscores = new List<(string, int)>();
        private int currentScore = 0; // Track score during game

            public class BasePart
        {
            public PictureBox PictureBox { get; set; }
            public int Health { get; set; }
        }
        public Form1()
        {
            InitializeComponent();

            turretMoveTimer = new Timer();
            turretMoveTimer.Interval = 5;
            turretMoveTimer.Tick += TurretMoveTimer_Tick;
            turretMoveTimer.Start();

            InitializeBases();      // <-- Bases first!
            InitializeTurret();     // <-- Turret after bases
            InitializeBulletTimer();
            InitializeAliens();
            InitializeAlienTimer();
            InitializeAlienLaserTimer();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;


            fireCooldownTimer = new Timer();
            fireCooldownTimer.Interval = 500; // 250 ms cooldown (adjust as needed)
            fireCooldownTimer.Tick += (s, e) =>
            {
                canFire = true;
                fireCooldownTimer.Stop();
            };

            backgroundPlayer1 = new System.Media.SoundPlayer("5.wav");
            backgroundPlayer2 = new System.Media.SoundPlayer("4.wav");
            if (duhduhPlaying == true)
            { 

                // Start the first background sound
                backgroundPlayer1.Play();


                // Set up a timer to switch duh-duh sound every half second
                backgroundSwitchTimer = new Timer();
                backgroundSwitchTimer.Interval = 500;
                backgroundSwitchTimer.Tick += (s, e) =>
                {
                
                    if (currentBackground == 1)
                    {
                        backgroundPlayer1.Stop();
                        backgroundPlayer2.Play();
                        currentBackground = 2;
                    }
                    else
                    {
                        backgroundPlayer2.Stop();
                        backgroundPlayer1.Play();
                        currentBackground = 1;
                    }

                };
                backgroundSwitchTimer.Start();

            }
            LoadHighscores();
        }

        private void InitializeTurret()
        {
            int turretWidth = 60;
            int turretHeight = 40;
            int spawnX = (this.ClientSize.Width - turretWidth) / 2; // Default center
            int spawnY = this.ClientSize.Height - turretHeight;

            // Find the base with the highest health
            if (bases.Count > 0)
            {
                BasePart bestBase = bases[0];
                foreach (var basePart in bases)
                {
                    if (basePart.Health > bestBase.Health)
                        bestBase = basePart;
                }
                // Center turret under the best base
                spawnX = bestBase.PictureBox.Left + (bestBase.PictureBox.Width - turretWidth) / 2;
                spawnY = bestBase.PictureBox.Bottom + 5; // 5 pixels below the base
                                                         // Ensure turret doesn't go off screen
                spawnX = Math.Max(0, Math.Min(spawnX, this.ClientSize.Width - turretWidth));
            }

            turret = new PictureBox
            {
                Size = new Size(turretWidth, turretHeight),
                BackColor = Color.Black,
                Location = new Point(spawnX, spawnY),
                ImageLocation = "man.gif",
            };
            this.Controls.Add(turret);
        }

        private void InitializeAliens()
        {
            int rows = 5;
            int cols = 8;
            int alienWidth = 50;
            int alienHeight = 30;
            int spacingX = 10;
            int spacingY = 10;
            int startX = 30;
            int startY = 30;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var alien = new PictureBox
                    {
                        Size = new Size(alienWidth, alienHeight),
                        BackColor = Color.Green,
                        Location = new Point(
                            startX + col * (alienWidth + spacingX),
                            startY + row * (alienHeight + spacingY)
                        ),
                        ImageLocation = "invader3.gif"
                    };
                    aliens.Add(alien);
                    this.Controls.Add(alien);
                }
            }
        }

        private void InitializeBases()
        {
            int baseCount = 4;
            int baseWidth = 70;
            int baseHeight = 50;
            int spacing = (this.ClientSize.Width - baseCount * baseWidth) / (baseCount + 1);
            int baseY = this.ClientSize.Height - 120;

            for (int i = 0; i < baseCount; i++)
            {
                int baseX = spacing + i * (baseWidth + spacing);

                // Main house body
                var house = new PictureBox
                {
                    Size = new Size(baseWidth, baseHeight - 20),
                    BackColor = Color.SandyBrown,
                    Location = new Point(baseX, baseY + 20)
                };
                this.Controls.Add(house);
                bases.Add(new BasePart { PictureBox = house, Health = 4 }); // 4 hits to destroy

                // Roof (triangle using a bitmap)
                var roof = new PictureBox
                {
                    Size = new Size(baseWidth, 20),
                    Location = new Point(baseX, baseY)
                };
                Bitmap roofBmp = new Bitmap(baseWidth, 20);
                using (Graphics g = Graphics.FromImage(roofBmp))
                {
                    Point[] triangle = {
                new Point(0, 20),
                new Point(baseWidth / 2, 0),
                new Point(baseWidth, 20)
            };
                    g.FillPolygon(Brushes.Red, triangle);
                }
                roof.Image = roofBmp;
                this.Controls.Add(roof);
                bases.Add(new BasePart { PictureBox = roof, Health = 3 }); // 3 hits to destroy
            }
        }


        private void InitializeBulletTimer()
        {
            bulletTimer = new Timer();
            bulletTimer.Interval = 40; // ms
            bulletTimer.Tick += BulletTimer_Tick;
            bulletTimer.Start();
        }

        private void InitializeAlienTimer()
        {
            alienTimer = new Timer();
            alienTimer.Interval = 40; // ms
            alienTimer.Tick += AlienTimer_Tick;
            alienTimer.Start();
        }



        private void InitializeAlienLaserTimer()
        {
            alienLaserTimer = new Timer();
            alienLaserTimer.Interval = 500; // ms, adjust for firing rate
            alienLaserTimer.Tick += AlienLaserTimer_Tick;
            alienLaserTimer.Start();
        }

        private void TurretMoveTimer_Tick(object sender, EventArgs e)
        {
            if (turret == null) return;
            if (movingLeft && turret.Left - turretSpeed >= 0)
                turret.Left -= turretSpeed;
            if (movingRight && turret.Left + turretSpeed + turret.Width <= this.ClientSize.Width)
                turret.Left += turretSpeed;
        }

        private void StartNewGame()
        {
            // Reset game state
            currentScore = 0;
            lives = 3;
            foreach (var alien in aliens) this.Controls.Remove(alien);
            aliens.Clear();
            foreach (var bullet in bullets) this.Controls.Remove(bullet);
            bullets.Clear();
            foreach (var basePart in bases) this.Controls.Remove(basePart.PictureBox);
            bases.Clear();
            foreach (var laser in alienLasers) this.Controls.Remove(laser);
            alienLasers.Clear();





            InitializeComponent();

            turretMoveTimer = new Timer();
            turretMoveTimer.Interval = 5;
            turretMoveTimer.Tick += TurretMoveTimer_Tick;
            turretMoveTimer.Start();

            InitializeBases();      // <-- Bases first!
            InitializeTurret();     // <-- Turret after bases
            InitializeBulletTimer();
            InitializeAliens();
            InitializeAlienTimer();
            InitializeAlienLaserTimer();











          //  InitializeBases();      // <-- Bases first!
          //  InitializeTurret();     // <-- Turret after bases
          //  InitializeAliens();

          //  bulletTimer.Start();
         //   alienTimer.Start();
         //   alienLaserTimer.Start();
        }   

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (turret == null) return;
            if (e.KeyCode == Keys.Left)
                movingLeft = true;
            else if (e.KeyCode == Keys.Right)
                movingRight = true;
            else if (e.KeyCode == Keys.Space)
            {
                if (canFire)
                {
                    FireBullet();
                    canFire = false;
                    fireCooldownTimer.Start();
                }
            }

          

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
                movingLeft = false;
            else if (e.KeyCode == Keys.Right)
                movingRight = false;

        }

        private void FireBullet()
        {
            var bullet = new PictureBox
            {
                Size = new Size(6, 18),
                BackColor = Color.Yellow,
                Location = new Point(
                    turret.Left + turret.Width / 2 - 3,
                    turret.Top - 18
                )
            };
            bullets.Add(bullet);
            this.Controls.Add(bullet);
            bullet.BringToFront();

            // Play fire sound
            try
            {
                var player = new System.Media.SoundPlayer("fire.wav");
                player.Play();
            }
            catch { /* Optional: handle missing file or playback error */ }
        }

        private void BulletTimer_Tick(object sender, EventArgs e)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Top -= 10;
                if (bullet.Top + bullet.Height < 0)
                {
                    this.Controls.Remove(bullet);
                    bullets.RemoveAt(i);
                    continue;
                }

                // Check collision with aliens
                bool bulletRemoved = false;
                for (int j = aliens.Count - 1; j >= 0; j--)
                {
                    var alien = aliens[j];
                    if (bullet.Bounds.IntersectsWith(alien.Bounds))
                    {
                        // Play alient hit sound
                        var player = new System.Media.SoundPlayer("alienthit.wav");
                        player.Play();

                        this.Controls.Remove(bullet);
                        bullets.RemoveAt(i);
                        this.Controls.Remove(alien);
                        aliens.RemoveAt(j);
                        currentScore += 10; // Add points per alien
                        lblScore.Text = "SCORE : " + currentScore;
                        bulletRemoved = true;
                        break;
                    }
                }
                if (bulletRemoved) continue;

                // Check collision with bases (house or roof)
                for (int k = bases.Count - 1; k >= 0; k--)
                {
                    var basePart = bases[k];
                    if (bullet.Bounds.IntersectsWith(basePart.PictureBox.Bounds))
                    {
                        this.Controls.Remove(bullet);
                        bullets.RemoveAt(i);

                        basePart.Health--;
                        if (basePart.Health <= 0)
                        {
                            this.Controls.Remove(basePart.PictureBox);
                            bases.RemoveAt(k);
                        }
                        else
                        {
                            // Change color to indicate damage
                            basePart.PictureBox.BackColor = Color.FromArgb(
                                255,
                                Math.Max(0, basePart.PictureBox.BackColor.R - 40),
                                Math.Max(0, basePart.PictureBox.BackColor.G - 40),
                                Math.Max(0, basePart.PictureBox.BackColor.B - 40)
                            );
                        }
                        bulletRemoved = true;
                        break;
                    }
                }
                if (bulletRemoved) continue;

                // Check collision with alien lasers
                for (int l = alienLasers.Count - 1; l >= 0; l--)
                {
                    var laser = alienLasers[l];
                    if (bullet.Bounds.IntersectsWith(laser.Bounds))
                    {
                        // Simple explosion effect: flash a red dot
                        var explosion = new PictureBox
                        {
                            Size = new Size(16, 16),
                            BackColor = Color.Red,
                            Location = new Point(
                                (bullet.Left + laser.Left) / 2,
                                (bullet.Top + laser.Top) / 2
                            )
                        };
                        this.Controls.Add(explosion);
                        explosion.BringToFront();
                        var timer = new Timer { Interval = 100 };
                        timer.Tick += (s, evt) =>
                        {
                            this.Controls.Remove(explosion);
                            timer.Stop();
                            timer.Dispose();
                        };
                        timer.Tick += (s, evt) =>
                        {
                            this.Controls.Remove(explosion);
                            timer.Stop();
                            timer.Dispose();
                        };
                        timer.Start();

                        this.Controls.Remove(bullet);
                        bullets.RemoveAt(i);
                        this.Controls.Remove(laser);
                        alienLasers.RemoveAt(l);
                        bulletRemoved = true;
                        break;
                    }
                }
                if (bulletRemoved) continue;
            }
            MoveAlienLasers();
        }

        private void AlienTimer_Tick(object sender, EventArgs e)
        {
            if (aliens.Count == 0) return;

            // Check for game over: aliens reach bottom or hit turret
            foreach (var alien in aliens)
            {
                if (alien.Bottom >= this.ClientSize.Height)
                {
                    EndGame();
                    return;
                }
                if (turret != null && alien.Bounds.IntersectsWith(turret.Bounds))
                {
                    EndGame();
                    return;
                }
            }

            // Calculate minX, maxX, maxY for aliens
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            foreach (var alien in aliens)
            {
                if (alien.Left < minX) minX = alien.Left;
                if (alien.Right > maxX) maxX = alien.Right;
                if (alien.Bottom > maxY) maxY = alien.Bottom;
            }

            // --- Dynamic speed adjustment ---
            int topMargin = 30; // Where aliens start
            int bottomMargin = 60; // How close to bottom before max speed
            int minInterval = 10; // Fastest speed (ms)
            int maxInterval = 80; // Slowest speed (ms)
            int range = this.ClientSize.Height - topMargin - bottomMargin;
            int distance = Math.Max(0, maxY - topMargin);
            double progress = Math.Min(1.0, (double)distance / range);
            int newInterval = (int)(maxInterval - (maxInterval - minInterval) * progress);
            alienTimer.Interval = Math.Max(minInterval, newInterval);
            // --- End dynamic speed adjustment ---

            bool bounced = false;
            // Bounce at edges
            if (alienDirection == 1 && maxX + alienSpeed > this.ClientSize.Width)
            {
                alienDirection = -1;
                bounced = true;
            }
            else if (alienDirection == -1 && minX - alienSpeed < 0)
            {
                alienDirection = 1;
                bounced = true;
            }

            // Move aliens
            foreach (var alien in aliens)
            {
                alien.Left += alienDirection * alienSpeed;
                if (bounced)
                {
                    alien.Top += 30; // Drop down by 30 pixels 
                }
            }
        }

        private void AlienLaserTimer_Tick(object sender, EventArgs e)
        {
            if (aliens.Count == 0) return;
            // Pick a random alien from the bottom row
            int maxY = int.MinValue;
            foreach (var alien in aliens)
                if (alien.Bottom > maxY) maxY = alien.Bottom;
            var shooters = aliens.FindAll(a => a.Bottom == maxY);
            if (shooters.Count == 0) return;
            var shooter = shooters[rand.Next(shooters.Count)];
            var laser = new PictureBox
            {
                Size = new Size(4, 18),
                BackColor = Color.Cyan,
                Location = new Point(
                    shooter.Left + shooter.Width / 2 - 2,
                    shooter.Bottom
                )
            };
            alienLasers.Add(laser);
            this.Controls.Add(laser);
            laser.BringToFront();
        }

        private void MoveAlienLasers()
        {
            for (int i = alienLasers.Count - 1; i >= 0; i--)
            {
                var laser = alienLasers[i];
                laser.Top += 10;
                if (laser.Top > this.ClientSize.Height)
                {
                    this.Controls.Remove(laser);
                    alienLasers.RemoveAt(i);
                    continue;
                }

                // Collision with bases
                for (int k = bases.Count - 1; k >= 0; k--)
                {
                    var basePart = bases[k];
                    if (laser.Bounds.IntersectsWith(basePart.PictureBox.Bounds))
                    {
                        this.Controls.Remove(laser);
                        alienLasers.RemoveAt(i);

                        basePart.Health--;
                        if (basePart.Health <= 0)
                        {
                            this.Controls.Remove(basePart.PictureBox);
                            bases.RemoveAt(k);
                        }
                        else
                        {
                            basePart.PictureBox.BackColor = Color.FromArgb(
                                255,
                                Math.Max(0, basePart.PictureBox.BackColor.R - 40),
                                Math.Max(0, basePart.PictureBox.BackColor.G - 40),
                                Math.Max(0, basePart.PictureBox.BackColor.B - 40)
                            );
                        }
                        break;
                    }
                }
                // Collision with turret
                if (turret != null && laser.Bounds.IntersectsWith(turret.Bounds))
                {
                    // Explosion effect at turret location
                    duhduhPlaying = false;
                    backgroundSwitchTimer.Stop();
                    TurretExplode = new System.Media.SoundPlayer("turretexplode.wav");

                    // Turret explosion sound
                    TurretExplode.Play();
                    var explosion = new PictureBox
                    {
                        Size = turret.Size,
                        BackColor = Color.OrangeRed,
                        Location = turret.Location,
                        ImageLocation = "explode.gif"
                    };
                    this.Controls.Add(explosion);
                    explosion.BringToFront();

                    var timer = new Timer { Interval = 1000 };
                    timer.Tick += (s, evt) =>
                    {
                        this.Controls.Remove(explosion);
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();

                    this.Controls.Remove(laser);
                    alienLasers.RemoveAt(i);

                    this.Controls.Remove(turret);
                    turret = null;

                    lives--;
                    if (lives > 0)
                    {
                        // Respawn turret after short delay
                        var respawnTimer = new Timer { Interval = 1500 };
                        respawnTimer.Tick += (s, evt) =>
                        {
                            respawnTimer.Stop();
                            respawnTimer.Dispose();
                            duhduhPlaying = true;
                            backgroundSwitchTimer.Start();
                            InitializeTurret();
                        };
                        respawnTimer.Start();
                    }
                    else
                    {
                        // Stop the game and show Game Over
                        duhduhPlaying = false;
                        backgroundSwitchTimer.Stop();
                        bulletTimer.Stop();
                        alienTimer.Stop();
                        alienLaserTimer.Stop();

                        TurretExplode = new System.Media.SoundPlayer("endgame.wav");

                        // Turret explosion sound
                        TurretExplode.Play();



                        this.Controls.Add(GameOver);
                        GameOver.BringToFront();

                        EndGame();

                        break;
                    }

  
                }
            }
        }



        private void EndGame()
        {
            // Stop all timers
            bulletTimer.Stop();
            alienTimer.Stop();
            alienLaserTimer.Stop();
            backgroundSwitchTimer.Stop();

            // Play end game sound

                var endPlayer = new System.Media.SoundPlayer("endgame.wav");
                endPlayer.Play();


            // Remove turret if present
            if (turret != null)
            {
                this.Controls.Remove(turret);
                turret = null;
            }

            // Prompt for player name
            string name = "Player";
            using (var prompt = new Form())
            {
                prompt.Width = 250;
                prompt.Height = 150;
                var textLabel = new Label() { Left = 10, Top = 20, Text = "Enter your name:" };
                var textBox = new TextBox() { Left = 10, Top = 50, Width = 200 };
                var confirmation = new Button() { Text = "OK", Left = 10, Width = 200, Top = 80 };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.ShowDialog();
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                    name = textBox.Text;
            }

            // Add score and sort
            highscores.Add((name, currentScore));
            highscores.Sort((a, b) => b.Score.CompareTo(a.Score));
            if (highscores.Count > 10) highscores = highscores.GetRange(0, 10); // Keep top 10
            SaveHighscores();

            ShowHighscores();

        }

        private void LoadHighscores()
{
    highscores.Clear();
    if (System.IO.File.Exists(HighscoreFile))
    {
        foreach (var line in System.IO.File.ReadAllLines(HighscoreFile))
        {
            var parts = line.Split(',');
            if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                highscores.Add((parts[0], score));
        }
    }
}



        private void SaveHighscores()
{
    var lines = new List<string>();
    foreach (var entry in highscores)
        lines.Add($"{entry.Name},{entry.Score}");
    System.IO.File.WriteAllLines(HighscoreFile, lines);
}
        private void ShowHighscores()
{
    string msg = "Highscores:\n";
    int rank = 1;
    foreach (var entry in highscores)
        msg += $"{rank++}. {entry.Name} - {entry.Score}\n";
    MessageBox.Show(msg, "Highscores");


            GameOver.Dispose();


 StartNewGame();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Birkbeck_Invaders
{

    public partial class Form1 : Form                                                       
    {

        // Turret
        private Rectangle turret;
        private int turretSpeed = 3; // Smoother movement
        private List<Rectangle> bullets = new List<Rectangle>();

        // Aliens
        private List<Rectangle> aliens = new List<Rectangle>();
        private int alienDirection = 1; // 1 = right, -1 = left
        private int alienSpeed = 2;

        //  Base
        private List<BasePart> bases = new List<BasePart>();

        // Lasers
        private List<Rectangle> alienLasers = new List<Rectangle>();
        private Random rand = new Random();

        //  Spaceship
        private bool movingLeft = false;
        private bool movingRight = false;
        private Timer turretMoveTimer;

        // Movement flags
        private bool canFire = true;
        private Timer fireCooldownTimer;

        // Load Sound
        private System.Media.SoundPlayer TurretExplode;
        private System.Media.SoundPlayer backgroundPlayer1;
        private System.Media.SoundPlayer backgroundPlayer2;
        private System.Media.SoundPlayer Firesound;
        private System.Media.SoundPlayer saucerHitSound;
        private System.Media.SoundPlayer alienHitSound;
        private System.Media.SoundPlayer saucerSound;
        private System.Media.SoundPlayer endgameSound;
        private int currentBackground = 1;
        private bool duhduhPlaying = true;
        private Timer backgroundSwitchTimer;

        // Spaceship
        private Random random = new Random();
        private Timer flightStartTimer = new Timer();
        private int spaceshipSpeed = 2;
        private Timer movementTimer = new Timer();

        // Game state
        private int Games = 0;
        private int Level = 1;
        private int currentScore = 0;
        private int lives = 3;
        private bool IsGameRunning = true;

        // Image Resources
        private Image turretImage;
        private Image alienImage;
        private Image spaceshipImage;
        private Image[] baseImages = new Image[7];
        private Image explosionImage;
        private PictureBox explosionEffect;


        public class BasePart
        {
            public Rectangle Bounds { get; set; }
            public int Health { get; set; }
        }



        public Form1()
        {
            Games++;
            currentBackground = 2;
            InitializeComponent();
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering
            InitializeResources();
            InitizeSounds();
            InitializeBases();
            InitializeTurret();
            InitializeAliens();
            initializeTurretMoveTimer();


            // set up spaceship timers
            initializeSpaceshipTimer();
            flightStartTimer.Start();

            backgroundSwitchTimer = new Timer();
            backgroundSwitchTimer.Interval = 500;
            backgroundSwitchTimer.Tick += (s, e) =>
            {
                if (currentBackground == 1)
                {
                    StopSound(backgroundPlayer1);
                    PlaySound(backgroundPlayer2);
                    currentBackground = 2;
                }
                else
                {
                    StopSound(backgroundPlayer2);
                    PlaySound(backgroundPlayer1);
                    currentBackground = 1;
                }
            };
            StartBackgroundMusic();
            StartGameLoop();

            lblLIVES.Text = "LIVES : " + lives;
            lblLevel.Text = "LEVEL : " + Level;
            this.KeyPreview = true;

            // Controls
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;

            // Fire cooldown timer
            fireCooldownTimer = new Timer();
            fireCooldownTimer.Interval = 500;
            fireCooldownTimer.Tick += (s, e) =>
            {
                canFire = true;
                fireCooldownTimer.Stop();
            };

        }

        private void InitializeResources()
        {
            turretImage = Image.FromFile("base.gif");
            alienImage = Image.FromFile("invader3g.jpg");
            spaceshipImage = spaceshipPictureBox.Image;
            explosionImage = Image.FromFile("explode.gif");

            for (int i = 1; i <= 6; i++)
            {
                baseImages[i] = Image.FromFile($"hs{i}.png");
            }
            baseImages[0] = baseImages[1]; // Default image

            // The spaceship is the only PictureBox we keep for its properties, but we hide it.
            spaceshipPictureBox.Visible = false;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            // Use NearestNeighbor interpolation for a sharp, pixelated look, which is ideal for retro-style games.
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;


            // Manually draw all game objects in a specific order to ensure correct layering.
            // Aliens are drawn first, followed by projectiles, bases, and finally the player's turret and the spaceship.
            foreach (var alien in aliens)
            {
                g.DrawImage(alienImage, alien);
            }

            foreach (var bullet in bullets)
            {
                g.FillRectangle(Brushes.Yellow, bullet);
            }

            foreach (var laser in alienLasers)
            {
                g.FillRectangle(Brushes.Cyan, laser);
            }

            foreach (var basePart in bases)
            {
                // The base image is selected based on its remaining health.
                int healthIndex = Math.Max(1, 7 - basePart.Health);
                g.DrawImage(baseImages[healthIndex], basePart.Bounds);
            }

            if (lives > 0 && !turret.IsEmpty)
            {
                g.DrawImage(turretImage, turret);
            }

            if (spaceshipPictureBox.Tag != null && (bool)spaceshipPictureBox.Tag) // spaceship is active
            {
                g.DrawImage(spaceshipImage, spaceshipPictureBox.Bounds);
            }
        }

        /// <summary>
        /// The main game loop, running asynchronously to avoid blocking the UI thread.
        /// It orchestrates all game state updates and triggers a redraw on each frame.
        /// </summary>
        private async void StartGameLoop()
        {
            while (IsGameRunning)
            {
                // Update the state of all game elements.
                UpdateBullets();
                UpdateAliens();
                UpdateAlienLasers();
                UpdateSpaceship();
                HandleCollisions();

                // Randomly trigger alien firing.
                if (rand.Next(100) < 5) // alien firing rate
                {
                    FireAlienLaser();
                }

                this.Invalidate(); // Request a redraw of the form to display the updated game state.
                await Task.Delay(16); // Wait for approximately 16ms to achieve a frame rate of ~60 FPS.
            }
        }

        private void initializeTurretMoveTimer()
        {
            turretMoveTimer = new Timer();
            turretMoveTimer.Interval =  4;
            turretMoveTimer.Tick += TurretMoveTimer_Tick;
            turretMoveTimer.Start();
        }
        private void TurretMoveTimer_Tick(object sender, EventArgs e)
        {
            if (turret.IsEmpty) return;

            int newX = turret.X;
            if (movingLeft && turret.Left - turretSpeed >= 0)
                newX -= turretSpeed;
            if (movingRight && turret.Left + turretSpeed + turret.Width <= this.ClientSize.Width)
                newX += turretSpeed;

            turret = new Rectangle(newX, turret.Y, turret.Width, turret.Height);
        }

        private void UpdateBullets()
        {
            // Iterate backwards to safely remove items from the list while iterating.
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bullet.Y -= 10; // Move bullet upwards.
                bullets[i] = bullet;

                // Remove bullets that have gone off-screen.
                if (bullet.Top + bullet.Height < 0)
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Manages the movement and behavior of the alien swarm.
        /// </summary>
        private void UpdateAliens()
        {
            if (aliens.Count == 0) return;

            // Check for game-ending conditions.
            foreach (var alien in aliens)
            {
                // If aliens reach the bottom of the screen.
                if (alien.Bottom >= this.ClientSize.Height)
                {
                    EndGame();
                    return;
                }
                // If an alien collides with the turret.
                if (!turret.IsEmpty && alien.IntersectsWith(turret))
                {
                    EndGame();
                    return;
                }

                // Check for collision with bases, destroying the base part on impact.
                for (int k = bases.Count - 1; k >= 0; k--)
                {
                    var basePart = bases[k];
                    if (alien.IntersectsWith(basePart.Bounds))
                    {
                        bases.RemoveAt(k);
                    }
                }
            }
            // Determine the current boundaries of the entire alien swarm.
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
            // The speed of the aliens and the background music increases as they get closer to the bottom of the screen.
            int topMargin = 30; // Where aliens start
            int bottomMargin = 60; // How close to bottom before max speed
            int range = this.ClientSize.Height - topMargin - bottomMargin;
            int distance = Math.Max(0, maxY - topMargin);
            double progress = Math.Min(1.0, (double)distance / range);
            if (backgroundSwitchTimer.Enabled)
            {
                backgroundSwitchTimer.Interval = Math.Max(100, 800 - (int)(600 * progress)); // Speed up background sound switch too
            }


            // --- End dynamic speed adjustment ---

            bool bounced = false;
            // Check if the swarm has reached the screen edges and needs to reverse direction.
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

            // Update the position of each alien.
            for (int i = 0; i < aliens.Count; i++)
            {
                var alien = aliens[i];
                alien.X += alienDirection * alienSpeed;
                if (bounced)
                {
                    alien.Y += 30; // If the swarm bounced, move all aliens down.
                }
                aliens[i] = alien;
            }
        }

        /// <summary>
        /// Manages alien laser movement, collisions, and player death sequence.
        /// </summary>
        private void UpdateAlienLasers()
        {
            // Iterate backwards to safely remove items from the list.
            for (int i = alienLasers.Count - 1; i >= 0; i--)
            {
                var laser = alienLasers[i];
                laser.Y += 10; // Move laser downwards.
                alienLasers[i] = laser;

                // Remove lasers that go off-screen.
                if (laser.Top > this.ClientSize.Height)
                {
                    alienLasers.RemoveAt(i);
                    continue;
                }

                // Check for collision with bases.
                for (int k = bases.Count - 1; k >= 0; k--)
                {
                    var basePart = bases[k];
                    if (laser.IntersectsWith(basePart.Bounds))
                    {
                        alienLasers.RemoveAt(i);
                        basePart.Health--;
                        if (basePart.Health <= 0)
                        {
                            bases.RemoveAt(k);
                        }
                        goto nextLaser; // Laser is destroyed, so skip to the next one.
                    }
                }
                // Check for collision with the player's turret.
                if (!turret.IsEmpty && laser.IntersectsWith(turret))
                {
                    // Stop the background music and play the explosion sound.
                    backgroundSwitchTimer.Stop();
                    PlaySound(TurretExplode);

                    // Create and display an explosion effect at the turret's location.
                    if (explosionEffect == null)
                    {
                        explosionEffect = new PictureBox
                        {
                            Size = turret.Size,
                            Image = explosionImage,
                            BackColor = Color.Transparent,
                        };
                        this.Controls.Add(explosionEffect);
                    }
                    explosionEffect.Location = turret.Location;
                    explosionEffect.Visible = true;
                    explosionEffect.BringToFront();


                    // Hide the explosion effect after a short delay.
                    var timer = new Timer { Interval = 1000 };
                    timer.Tick += (s, evt) =>
                    {
                        explosionEffect.Visible = false;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();

                    alienLasers.RemoveAt(i);
                    turret = Rectangle.Empty; // Destroy the turret.

                    lives--;
                    lblLIVES.Text = "LIVES : " + lives;
                    if (lives > 0)
                    {
                        // If the player has lives remaining, respawn the turret after a delay.
                        var respawnTimer = new Timer { Interval = 1500 };
                        respawnTimer.Tick += (s, evt) =>
                        {
                            respawnTimer.Stop();
                            respawnTimer.Dispose();
                            backgroundSwitchTimer.Start();
                            InitializeTurret();
                        };
                        backgroundSwitchTimer.Start(); //dudduh
                        respawnTimer.Start();

                    }
                    else
                    {
                        // If no lives are left, end the game.
                        duhduhPlaying = false;
                        // Stop all timers
     
                        backgroundSwitchTimer.Stop();
                        StopSound(backgroundPlayer1);
                        StopSound(backgroundPlayer2);

                        // Turret explosion sound
                        PlaySound(TurretExplode);
                        EndGame();

                        break; // Exit the loop as the game is over.
                    }
                }
            nextLaser:; // Label to jump to the next iteration of the outer loop.
            }
        }

        private void UpdateSpaceship()
        {
            if (spaceshipPictureBox.Tag == null || !(bool)spaceshipPictureBox.Tag) return;

            // Move the spaceship
            spaceshipPictureBox.Left += spaceshipSpeed;

            // If it goes off-screen
            if (spaceshipPictureBox.Left > this.ClientSize.Width)
            {
                movementTimer.Stop();
                spaceshipPictureBox.Tag = false;
                backgroundSwitchTimer.Start(); // Resume the background music.

                // restart the flight timer with a new random interval
                flightStartTimer.Stop();
                flightStartTimer.Interval = GetRandomInterval();
                flightStartTimer.Start();
            }
        }


        /// <summary>
        /// Handles all collisions involving player bullets.
        /// </summary>
        private void HandleCollisions()
        {
            // Iterate backwards through bullets to allow safe removal.
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                var bullet = bullets[i];
                bool bulletRemoved = false;

                // Check for collision with the spaceship.
                if (spaceshipPictureBox.Tag != null && (bool)spaceshipPictureBox.Tag && bullet.IntersectsWith(spaceshipPictureBox.Bounds))
                {
                    PlaySound(saucerHitSound);
                    spaceshipPictureBox.Tag = false; // Deactivate the spaceship.
                    currentScore += 50;
                    lblScore.Text = "SCORE : " + currentScore;
                    flightStartTimer.Interval = GetRandomInterval();
                    flightStartTimer.Start();
                    bulletRemoved = true;

                    // restart ominous background sound
                    StartBackgroundMusic();

                    // Create a temporary PictureBox to display the score popup.
                    PictureBox scorePopup = new PictureBox
                    {
                        Image = Image.FromFile("50.png"),
                        SizeMode = PictureBoxSizeMode.AutoSize,
                        Location = new Point(spaceshipPictureBox.Location.X, spaceshipPictureBox.Location.Y),
                        BackColor = Color.Transparent
                    };

                    this.Controls.Add(scorePopup);
                    scorePopup.BringToFront();

                    // Use a timer to automatically remove the score popup.
                    var popupTimer = new Timer { Interval = 750 }; // Display for 0.75 seconds
                    popupTimer.Tick += (s, evt) =>
                    {
                        popupTimer.Stop();
                        this.Controls.Remove(scorePopup);
                        scorePopup.Dispose();
                        popupTimer.Dispose();
                    };
                    
                    popupTimer.Start();
                }

                // Check for collision with aliens.
                if (!bulletRemoved)
                {
                    for (int j = aliens.Count - 1; j >= 0; j--)
                    {
                        if (bullet.IntersectsWith(aliens[j]))
                        {
                            PlaySound(alienHitSound);
                            aliens.RemoveAt(j);
                            currentScore += 10;
                            lblScore.Text = "SCORE : " + currentScore;
                            bulletRemoved = true;

                            // If all aliens are defeated, advance to the next level.
                            if (aliens.Count == 0)
                            {
                                Level++;
                                alienSpeed += 1;
                                var levelTimer = new Timer { Interval = 1500 };
                                levelTimer.Tick += (s, evt) =>
                                {
                                    levelTimer.Stop();
                                    levelTimer.Dispose();
                                    InitializeBases();
                                    InitializeAliens();
                                };
                                levelTimer.Start();
                                lblLIVES.Text = "LIVES : " + lives;
                                lblScore.Text = "SCORE : " + currentScore;
                                lblLevel.Text = "LEVEL : " + Level;
                            }
                            break; // Bullet can only hit one alien.
                        }
                    }
                }

                // Check for collision with bases.
                if (!bulletRemoved)
                {
                    for (int k = bases.Count - 1; k >= 0; k--)
                    {
                        var basePart = bases[k];
                        if (bullet.IntersectsWith(basePart.Bounds))
                        {
                            basePart.Health--;
                            if (basePart.Health <= 0)
                            {
                                bases.RemoveAt(k);
                            }
                            bulletRemoved = true;
                            break; // Bullet is destroyed upon hitting a base.
                        }
                    }
                }

                // Check for collision with alien lasers.
                if (!bulletRemoved)
                {
                    for (int l = alienLasers.Count - 1; l >= 0; l--)
                    {
                        if (bullet.IntersectsWith(alienLasers[l]))
                        {
                            alienLasers.RemoveAt(l);
                            bulletRemoved = true;
                            break; // Both projectiles are destroyed.
                        }
                    }
                }

                if (bulletRemoved)
                {
                    bullets.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Creates a laser projectile fired by a random alien from the bottom-most row.
        /// </summary>
        private void FireAlienLaser()
        {
            if (aliens.Count == 0) return;
            // Identify the lowest row of aliens.
            int maxY = int.MinValue;
            foreach (var alien in aliens)
                if (alien.Bottom > maxY) maxY = alien.Bottom;
            
            // Find all aliens in that row that can shoot.
            var shooters = aliens.FindAll(a => a.Bottom == maxY);
            if (shooters.Count == 0) return;

            // Pick a random shooter and create a laser.
            var shooter = shooters[rand.Next(shooters.Count)];
            var laser = new Rectangle(
                    shooter.Left + shooter.Width / 2 - 2,
                    shooter.Bottom,
                    4,
                    18);
            alienLasers.Add(laser);
        }

        /// <summary>
        /// Manages the iconic, alternating two-tone background sound that speeds up over time.
        /// </summary>
        private void StartBackgroundMusic()
        {
            if (!duhduhPlaying) return;

            // Play the first sound immediately.
            currentBackground = 1;
            PlaySound(backgroundPlayer1);
            backgroundSwitchTimer.Start();
        }

        private void StopBackgroundMusic()
        {
            backgroundSwitchTimer.Stop();
            StopSound(backgroundPlayer1);
            StopSound(backgroundPlayer2);
        }


        private void initializeSpaceshipTimer()
        {
            flightStartTimer.Interval = GetRandomInterval();
            flightStartTimer.Tick += FlightStartTimer_Tick;

            movementTimer.Interval = 50; // ~50 FPS
            movementTimer.Tick += movementTimer_Tick;
        }


        private int GetRandomInterval() 
        {
            return random.Next(15000, 20000);
        }

        private void FlightStartTimer_Tick(object sender, EventArgs e)
        {
            // If the spaceship is not currently active, start a new flight.
            if (spaceshipPictureBox.Tag == null || !(bool)spaceshipPictureBox.Tag)
            {

                StartNewFlight();
            }
        }

        private void StartNewFlight()
        {
            // Place the spaceship off-screen to the left and at a random vertical position
            spaceshipPictureBox.Location = new Point(-spaceshipPictureBox.Width, 30);
            spaceshipPictureBox.Tag = true; // Mark as active
            // Stop the main background music and play the saucer's unique sound.
            StopBackgroundMusic();
            PlaySound(saucerSound);
            movementTimer.Start();
     
        }


        /// <summary>
        /// Initializes or respawns the player's turret.
        /// </summary>
        private void InitializeTurret()
        {
            int turretWidth = 60;
            int turretHeight = 40;
            int spawnX = (this.ClientSize.Width - turretWidth) / 2; // Default to center.
            int spawnY = this.ClientSize.Height - turretHeight - 40;

            // If bases exist, try to spawn the turret under the one with the most health.
            if (bases.Count > 0)
            {
                BasePart bestBase = bases[0];
                if (Games > 1)
                {
                    foreach (var basePart in bases)
                    {
                        basePart.Health = 4; // Reset health for all bases on new level.
                    }
                }
                foreach (var basePart in bases)
                {
                    if (basePart.Health > bestBase.Health)
                        bestBase = basePart;
                }
                // Center turret under the healthiest base.
                spawnX = bestBase.Bounds.Left + (bestBase.Bounds.Width - turretWidth) / 2;
                spawnY = bestBase.Bounds.Bottom + 5; // 5 pixels below the base
                
                // Ensure turret doesn't spawn off-screen.
                spawnX = Math.Max(0, Math.Min(spawnX, this.ClientSize.Width - turretWidth));
            }

            turret = new Rectangle(spawnX, spawnY, turretWidth, turretHeight);
        }

        private void InitializeAliens()
        {
            aliens.Clear();
            int rows = 5;
            int cols = 8;
            int alienWidth = 50;
            int alienHeight = 33;
            int spacingX = 10;
            int spacingY = 10;
            int startX = 30;
            int startY = 80;


            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var alien = new Rectangle(
                            startX + col * (alienWidth + spacingX),
                            startY + row * (alienHeight + spacingY),
                            alienWidth,
                            alienHeight);
                    aliens.Add(alien);
                }
            }
        }

        private void InitizeSounds()
        {
            backgroundPlayer1 = new System.Media.SoundPlayer("5.wav");
            backgroundPlayer1.Load();
            backgroundPlayer2 = new System.Media.SoundPlayer("4.wav");
            backgroundPlayer2.Load();
            TurretExplode = new System.Media.SoundPlayer("turretexplode.wav");
            TurretExplode.Load();
            Firesound = new System.Media.SoundPlayer("fire.wav");
            Firesound.Load();
            saucerHitSound = new System.Media.SoundPlayer("saucerhit.wav");
            saucerHitSound.Load();
            alienHitSound = new System.Media.SoundPlayer("alienhit.wav");
            alienHitSound.Load();
            saucerSound = new System.Media.SoundPlayer("saucer.wav");
            saucerSound.Load();
            endgameSound = new System.Media.SoundPlayer("endgame.wav");
            endgameSound.Load();
        }


        private void InitializeBases()
        {
            bases.Clear();
            int baseCount = 5;
            int baseWidth = 70;
            int baseHeight = 70;

            int spacing = (this.ClientSize.Width - baseCount * baseWidth) / (baseCount + 1);
            int baseY = this.ClientSize.Height - 120;

            for (int i = 0; i < baseCount; i++)
            {
                int baseX = spacing + i * (baseWidth + spacing);

                var baseRect = new Rectangle(baseX + 40, baseY, baseWidth, baseHeight);
                bases.Add(new BasePart { Bounds = baseRect, Health = 7 });
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (turret.IsEmpty) return;
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
            else if (e.KeyCode == Keys.S)
            {
                duhduhPlaying = !duhduhPlaying;
                if (duhduhPlaying)
                {
                    StartBackgroundMusic();
                }
                else
                {
                    StopBackgroundMusic();
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
            var bullet = new Rectangle(
                    turret.Left + turret.Width / 2 - 3,
                    turret.Top - 18,
                    6,
                    18);
            bullets.Add(bullet);

            // Play fire sound
            PlaySound(Firesound);

        }

        /// <summary>
        /// Plays a sound on a background thread to prevent blocking the main game loop.
        /// </summary>
        /// <param name="player">The SoundPlayer instance to play.</param>
        private void PlaySound(System.Media.SoundPlayer player)
        {
            Task.Run(() => player.Play());
        }

        private void StopSound(System.Media.SoundPlayer player)
        {
            player.Stop();
        }


        private void movementTimer_Tick(object sender, EventArgs e)
        {
            UpdateSpaceship();
        }


        /// <summary>
        /// Handles the entire game-over sequence.
        /// </summary>
        private async void EndGame()
        {
            if (!IsGameRunning) return; // Prevent multiple calls.
            IsGameRunning = false; // Stop the main game loop.
            turret = Rectangle.Empty;

            // Stop all game-related timers.
            turretMoveTimer.Stop();
            fireCooldownTimer.Stop();
            backgroundSwitchTimer.Stop();
            flightStartTimer.Stop();
            movementTimer.Stop();

            // Stop background sounds and play the final game-over sound.
            backgroundPlayer1.Stop();
            backgroundPlayer2.Stop();
            PlaySound(endgameSound);
            await Task.Delay(2000); // Wait for the sound to play.

            // Use Invoke to safely update the UI from this async method.
            this.Invoke((MethodInvoker)delegate
            {
                PictureBox gameOver = new PictureBox();
                try
                {
                    // Display the "Game Over" GIF.
                    gameOver.Image = Image.FromFile("gameover.gif");
                    gameOver.SizeMode = PictureBoxSizeMode.AutoSize;
                    this.Controls.Add(gameOver);
                    gameOver.BringToFront();

                    // Center the image on the form.
                    int x = (this.ClientSize.Width - gameOver.Width) / 2;
                    int y = (this.ClientSize.Height - gameOver.Height) / 2;
                    gameOver.Location = new Point(x, y);
                }
                catch (System.IO.FileNotFoundException)
                {
                    // If the GIF is missing, display a text label as a fallback.
                    Label gameOverLabel = new Label
                    {
                        Text = "GAME OVER",
                        Font = new Font("Arial", 24, FontStyle.Bold),
                        ForeColor = Color.Red,
                        AutoSize = true
                    };
                    this.Controls.Add(gameOverLabel);
                    gameOverLabel.BringToFront();
                    int x = (this.ClientSize.Width - gameOverLabel.Width) / 2;
                    int y = (this.ClientSize.Height - gameOverLabel.Height) / 2;
                    gameOverLabel.Location = new Point(x, y);
                }
            });

            await Task.Delay(3000); // Pause to show the "Game Over" message.

            // Transition to the high score form.
            this.Invoke((MethodInvoker)delegate {
                this.Hide();
                frmHiScore newForm = new frmHiScore(currentScore);
                newForm.FormClosed += (s, args) => this.Close(); // Ensure the application exits when the high score form is closed.
                newForm.Show();
            });
        }
    }
}
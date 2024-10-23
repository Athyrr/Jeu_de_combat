using Jeu_de_combat;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace Jeu_de_combat
{
    /// <summary>
    /// Defines the display module.
    /// </summary>
    public class GameDisplay
    {
        #region Remove QuickEdit Mode Data
        // Code found on the Internet to remove the Visual Studio's QuickEdit Mode, which stops the program if the player clicks on the window with his mouse.
        private const uint ENABLE_QUICK_EDIT = 0x0040; 
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080; 
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        #endregion

        #region Data
        /// <summary>
        /// Grid height
        /// </summary>
        const int xMax = 51; 
        /// <summary>
        /// Grid width 
        /// </summary>
        const int yMax = 7;  

        /// <summary>
        /// Grid where each char element represents the screen game "pixels"
        /// </summary>
        static char[,] _grid = new char[xMax + 1, yMax + 1];
        /// <summary>
        /// Grid where each ConsoleColor element represents each pixels color
        /// </summary>
        static ConsoleColor[,] _colorGrid = new ConsoleColor[xMax + 1, yMax + 1];

        /// <summary>
        /// Assigned column to print the game screen.
        /// </summary>
        const int gridLeft = 10;
        /// <summary>
        /// Assigned line to print the buttons
        /// </summary>
        const int butTop = yMax + 2;
        /// <summary>
        /// Assigned line to print the text
        /// </summary>
        const int textTop = yMax + 5;
        /// <summary>
        /// Assigned line to print the left player
        /// </summary>
        const int charLeft = 4;
        /// <summary>
        /// Assigned line to print the right player
        /// </summary>
        const int charRight = 41;

        /// <summary>
        /// Default text which appears when the buttons don't have a specified text
        /// </summary>
        const string _defaultText = "Choose with arrows and enter the wanted action";

        /// <summary>
        /// Actual selected button index
        /// </summary>
        static int _buttonIndex = 0;
        /// <summary>
        /// Actual text printed
        /// </summary>
        static string _actualText = string.Empty;
        #endregion

        #region Sprites
        /// <summary>
        /// White color (default)
        /// </summary>
        const ConsoleColor defaultColor = ConsoleColor.White;
        /// <summary>
        /// Gray color (default) for elements inside the game screen
        /// </summary>
        const ConsoleColor gridTextC = ConsoleColor.Gray;
        

        /// <summary>
        /// Life point sprite
        /// </summary>
        const string lifePoint = "*";
        /// <summary>
        /// Life point sprite color
        /// </summary>
        const ConsoleColor lifeC = ConsoleColor.Red;

        /// <summary>
        /// Grid borders sprite
        /// </summary>
        static string _gridBorders = "";
        #endregion

        #region Initialization
        /// <summary>
        /// Initialisation of the GameDisplay module
        /// </summary>
        public static void Init()
        {
            #region Remove QuickEdit Mode 
            IntPtr consoleHandle = GetStdHandle(-10);
            uint consoleMode = 0;
            consoleMode &= ~ENABLE_QUICK_EDIT;
            consoleMode |= ENABLE_EXTENDED_FLAGS;
            SetConsoleMode(consoleHandle, consoleMode);
            #endregion

            #region Application Window Initialisation
            Console.SetWindowSize(72, 20);
            //Console.SetBufferSize(72, 20); 
            Console.Title = "Monomachia"; // Rename the application window
            Console.CursorVisible = false; // Remove the cursor visibility
            #endregion

            #region Grid Borders Sprite construction by code
            for (int y = 0; y <= yMax; y++)
            {
                for (int x = 0; x <= xMax; x++)
                {
                    if ((y == 0 || y == yMax) && (x == 0 || x == xMax))
                        _gridBorders += "+";
                    else if (y == 0 || y == yMax)
                        _gridBorders += '-';
                    else if (x == 0 || x == xMax)
                        _gridBorders += "|";
                    else if (x < xMax)
                        _gridBorders += ' ';

                    _colorGrid[x, y] = defaultColor;
                }

                if (y < _grid.GetLength(1) - 1)
                    _gridBorders += '\n';
            }
            SpriteToGrid(_gridBorders, 0, 0);
            #endregion
        }
        #endregion

        #region Menus display
        /// <summary>
        /// Menu selection 
        /// </summary>
        /// <returns>Selected button choice</returns>
        public static string DisplayMenu()
        {
            // Title text sprite
            string title = "MMMM OOOO N  N OOOO MMMM  AA  CCCC H  H  /   AA " +
                         "\nM  M O  O NN N O  O M  M A  A C    HHHH     A  A" +
                         "\nM  M O  O N NN O  O M  M AAAA C    HHHH  I  AAAA" +
                         "\nM  M OOOO N  N OOOO M  M A  A CCCC H  H  I  A  A";

            Fade(false, true);
            SoundManager.Play("bg_menu.mp3", true); // Play the menu theme

            // Title text animation
            for (int y = yMax + 1; y > 1; y--)
            {
                SpriteToGrid(title, 2, y, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(250);
            }
            return Selector(["Quit", "Play", "Credits"], [_defaultText]); // Buttons set up
        }

        /// <summary>
        /// Gamemode selection
        /// </summary>
        /// <returns> Selected button choice </returns>
        public static string DisplayGameModeSelection()
        {
            // "VS" text sprite
            string vs = "V   V  SSSSS"
                + "\nV   V  S    "
                + "\n V V   SSSSS"
                + "\n V V       S"
                + "\n  V    SSSSS";

            Fade();
            FloorAnim();

            // Arrival animation of two characters, the VS text
            for (int i = 10; i > 0; i--)
            {
                SpriteToGrid(Damager.SpriteLeft, charLeft - i, 2, Damager.SpriteColor, 1, 0);
                SpriteToGrid(Tank.SpriteRight, charRight + i, 2, Tank.SpriteColor, -1, 0);
                SpriteToGrid(vs, 20, 2 - i, gridTextC, 0, -1);
                PrintGrid();
                Thread.Sleep(100);
            }

            return Selector(["Player vs AI", "AI vs AI"], [_defaultText]); // Buttons set up
        }

        /// <summary>
        /// Character selection
        /// </summary>
        /// <param name="text"></param>
        /// <returns> Selected button choice </returns>
        public static string DisplayCharacterSelection(string text)
        {
            string randomSprite = " ????? \n     ? \n   ??? \n   ?   \n"; // Interrogation point sprite 
            Fade();

            // Animation of the three characters and the random choice sprites 
            for (int y = yMax + 1; y >= 2; y--)
            {
                SpriteToGrid(Damager.SpriteLeft, 6, y, Damager.SpriteColor, 0, 1);
                SpriteToGrid(Healer.SpriteLeft, 16, y, Healer.SpriteColor, 0, 1);
                SpriteToGrid(Tank.SpriteRight, 27, y, Tank.SpriteColor, 0, 1);
                SpriteToGrid(randomSprite, 38, y, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(75);
            }

            // Specified text buttons set up
            string[] t =
            [
                $"{text}\nDamager\nLife points : ***, Strength : ++\nRage : Return back received damages. Damager still takes damages.",
                $"{text}\nHealer\nLife points : ****, Strength : +\nHeal : Heal 2 life points.",
                $"{text}\nTank\nLife points : *****, Strength : +\nStrong Attack : Tank uses 1 life point to increase his strength by 1, then attacks. After his attack, his strength goes back to normal.",
                $"{text}\nChoose a character randomly."
            ];
            return Selector(["Damager", "Healer", "Tank","Random"], t); // Buttons set up
        }

        /// <summary>
        /// AI Level selection
        /// </summary>
        /// <param name="AIvsAI"></param>
        /// <returns></returns>
        public static string DisplayIALevelSelection(bool AIvsAI)
        {
            // Three levels sprite representation (inspired of greek mythological gods)
            string tyche = "   O   \n L/|\\  \n   |   \n  / \\  ";
            string athena = "\\ 8O   \n \\/|-0 \n  \\|   \n  / \\  ";
            string zeus = " / O   \n \\/|\\  \n / |   \n  / \\  ";
            string[] sprites = [tyche, athena, zeus];
            List<ConsoleColor> colors = [ConsoleColor.Cyan, ConsoleColor.DarkGreen, ConsoleColor.Yellow];

            // Specified text buttons set up
            string text = "Choose which IA you want to fight :";
            string[] buts = ["Tyche", "Athena", "Zeus"];
            List<string> texts = new List<string> {
                $"{text}\nGoddess of luck, she'll bet everything on fate to win.",
                $"{text}\nGoddess of strategy, she will provide a duel worthy of the name.",
                $"{text}\nAbsolute God of Gods, this omniscient entity is faster and smarter than you, by far."
            };

            Fade();

            // Remove the "Zeus" level if the gamemode is AI vs other AI
            if (AIvsAI) 
            {
                buts = ["Tyche", "Athena"];
                sprites = [tyche, athena];
                texts.RemoveAt(2);
            }

            // Animation of the levels sprite representation
            int a = AIvsAI ? 5 : 0; // Data for placement depending of the levels disponibility
            for (int y = yMax + 1; y >= 2; y--)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    SpriteToGrid(sprites[i], (i + 1) * 8 + a + (6 + a) * i, y, colors[i], 0, 1);
                    PrintGrid();
                    Thread.Sleep(50);
                }
            }

            return Selector(buts, texts.ToArray()); // Buttons set up
        }

        /// <summary>
        /// Fight start animation
        /// </summary>
        /// <param name="playerLeft"></param>
        /// <param name="playerRight"></param>
        public static void DisplayFight(Character playerLeft, Character playerRight)
        {
            // Fight text sprite
            string fight = "FFFFF I  GGGGG H   H TTTTT I"
                   +  "\nF        G     H   H   T   I"
                   +  "\nFFF   I  G  GG HHHHH   T   I"
                   +  "\nF     I  G   G H   H   T    "
                   +  "\nF     I  GGGGG H   H   T   O";

            Fade();
            SoundManager.Play("bg_fight.mp3", true);
            FloorAnim();

            // Players character arrival animation
            for (int i = 10; i >= 0; i--) 
            {
                SpriteToGrid(playerLeft.SpriteLeftInstance, charLeft - i, 2, playerLeft.SpriteColorInstance, 1, 0);
                SpriteToGrid(playerRight.SpriteRightInstance, charRight + i, 2, playerRight.SpriteColorInstance, -1, 0);
                SpriteToGrid(new string(lifePoint[0], playerLeft.Health), 2 - i, 1, lifeC, -1, 0);
                SpriteToGrid(new string(lifePoint[0], playerRight.Health), xMax - playerRight.Health - 1 + i, 1, lifeC, 1, 0);
                PrintGrid();
                Thread.Sleep(50);
            }

            // Fight appearance animation
            for (int i = 5; i >= 0; i--)
            {
                SpriteToGrid(fight, 12, 1-i, gridTextC, 0, -1);
                PrintGrid();
                Thread.Sleep(50);
            }
            SoundManager.Play("win.mp3");
            Thread.Sleep(500);

            // Fight text vanish animation
            for (int i = 5; i >= 0; i--)
            {
                SpriteToGrid(fight, 12, -5 + i, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// End game display
        /// </summary>
        /// <param name="winner">Winner character</param>
        /// <param name="loser">Loser character</param>
        public static void DisplayEndGame(Character winner, Character loser) // Sah je pense c'est guez on peut enlever
        {
            SoundManager.StopAllLoops();
            int w = winner.IsLeft ? 1 : 2;  // Get if Player 1 or 2 wn

            // Loser character vanish animation
            SoundManager.Play("win.mp3");
            string loserSprite = loser.IsLeft ? loser.SpriteLeftInstance : loser.SpriteRightInstance;
            string winnerSprite = winner.IsLeft ? winner.SpriteLeftInstance : winner.SpriteRightInstance;
            int startX = loser.IsLeft ? charLeft : charRight;
            for (int i = 0; i <= 10; i++)
            {
                int s = loser.IsLeft ? -1 : 1;
                SpriteToGrid(loserSprite, startX + i * s, 2, loser.SpriteColorInstance, -s, 0);
                PrintGrid();
                Thread.Sleep(50);
            }

            Fade(true, true, 50);
            string c1 = "` , ` , ` , ` , ` ,";
            string c2 = " ` , ` , ` , ` , ` ";
            string confets1 = $"{c1}\n{c2}\n{c1}\n{c2}\n{c1}\n{c2}";
            string confets2 = $"{c2}\n{c1}\n{c2}\n{c1}\n{c2}\n{c1}";

            // Center the winner character and tell the user who won
            for (int i = yMax; i >= 0; i--)
            {
                SpriteToGrid(winnerSprite, charLeft + 17, 2 + i, winner.SpriteColorInstance, 0, 1);
                PrintGrid();
                Thread.Sleep(300);
            }
            PrintText($"{winner.Name} (Player {w}) wins !");

            // Falling confets animation
            SoundManager.Play("win2.mp3", true);
            for (int i = 5; i >= 0; i--)
            {
                SpriteToGrid(confets1, 1, 1 - i, ConsoleColor.Yellow, -1, 0);
                SpriteToGrid(confets1, xMax - 20, 1 - i, ConsoleColor.Yellow, -1, 0);
                PrintGrid();
                Thread.Sleep(200);
            }

            // Confets loop animation
            for (int i = 0; i < 26; i++)
            {
                if (i % 2 == 1)
                {
                    SpriteToGrid(confets1, 1, 1, ConsoleColor.Yellow);
                    SpriteToGrid(confets1, xMax - 20, 1, ConsoleColor.Yellow);
                }
                else
                {
                    SpriteToGrid(confets2, 1, 1, ConsoleColor.Yellow);
                    SpriteToGrid(confets2, xMax - 20, 1, ConsoleColor.Yellow);
                }
                PrintGrid();
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Credits menu
        /// </summary>
        public static void DisplayCredits()
        {
            // Data initialization 
            string c = "\"MONOMACHIA\""
                + "-A P5INJ PRODUCTION"
                + "-Gameplay Programmer : Adam Adhar"
            + "-Graphics Implementation : Marius Boulandet"
            + "-Audio Implementation : Elliot Nedellec"
            + "-All sounds and musics are from Itch.io :"
            + "-TipTopTomCat and Swiss Arcade Game Entertainment"
            + "--Special thanks : Beber"
            + "-2024 ©"
            + "---Thanks for playing <3";
            string credits = "";

            // Center each line of the credits
            foreach (string s in c.Split('-'))
            {
                string space = new string(' ', (xMax - 1 - s.Length) / 2);
                credits += space + s + space + "\n\n";
            }

            Fade();
            SoundManager.Play("credits.mp3", true);

            // Scrolls through the credits
            for (int i = 0; i < yMax + credits.Split('\n').Count() - 5; i++)
            {
                SpriteToGrid(credits, 1, yMax - i, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(500);
            }
            Thread.Sleep(1000);
            Fade(true, false);
        }

        /// <summary>
        /// A little Easter Egg in tribute to our class and its mascot, Beber !
        /// </summary>
        private static void Beber(int colorId)
        {
            // Sprites for the easter egg
            string beberText = "oo  ooo oo  ooo ooo"
                + "\no o o   o o o   o o"
                + "\nooo oo  ooo oo  ooo"
                + "\no o o   o o o   oo "
                + "\noo  ooo oo  ooo o o";
            beberText = beberText.Replace('o', '#');

            string psinj = "ooo ooo o o  o   o"
                       + "\no o o     o  o   o"
                       + "\nooo ooo o oo o   o"
                       + "\no     o o o oo   o"
                       + "\no   ooo o o  o ooo";
            psinj = psinj.Replace('o', '#');

            string beber = "   .-\"-.   "
            + "\n _/.-.-.\\_ "
            + "\n( ( o o ) )"
            + "\n |/  \"  \\| "
            + "\n  \\ .-. /  "
            + "\n  /`\"\"\"`\\  ";

            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

            if (!SoundManager.IsSoundRunning("credits.mp3"))
                SoundManager.StopAllLoops();
            Fade(true, true);
            SoundManager.Play("credits.mp3", true);

            // Beber animation
            for (int y = yMax + 1; y > 0; y--)
            {
                SpriteToGrid(beberText, 1, y, gridTextC, 0, 1);
                SpriteToGrid(beber, 21, y, colors[colorId], 0, 1);
                SpriteToGrid(psinj, 33, y, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(300);
            }

            if (Selector(["Beber"], ["Beber."]) == "Beber") // Buttons set up
            {
                Random rand = new Random();
                Beber(rand.Next(0, colors.Count()));
            }
        }
        #endregion

        #region Animations
        /// <summary>
        /// Damager's special ability "Rage" animation
        /// </summary>
        /// <param name="isLeft">If it's left player or not</param>
        /// <param name="dam">Received damages</param>
        public static void DamagerSpecialAnim(bool isLeft, int dam)
        {
            string rageBullet = isLeft ? "=>" : "<="; // Create the rage bullet sprite

            // Fire as many times as received damages animation
            for (int i = 0; i < dam; i++)
            {
                BulletAnim(rageBullet, isLeft, Damager.ColorSpecial, "damager_ulti.mp3");
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Healer's special ability "Heal" animation
        /// </summary>
        /// <param name="isLeft">If it's left player or not</param>
        public static void HealerSpecialAnim(bool isLeft)
        {
            // Data initialization depending if it's the left or right animation
            int startX = charLeft + 6;
            int s = 1;
            if (!isLeft)
            {
                startX = charRight - 1;
                s = -1;
            }

            // Create the magic bubble and move it 
            SoundManager.Play("spell.mp3");
            for (int i = 0; i < 4; i++)
            {
                SpriteToGrid(Healer.BulletSprite, startX + i * s, 2, Healer.SpriteColor, -Math.Sign(i) * s, 0);
                PrintGrid();
                Thread.Sleep(80);
            }
            Thread.Sleep(300);

            // Lightning appears 
            string[] lightning = ["/", "\\", "/"]; string forErase = " \n \n ";
            for (int i = 0; i < 3; i++)
            {
                SpriteToGrid(lightning[i], startX + 3 * s, yMax - 2 - i, ConsoleColor.Yellow);
                PrintGrid();
                Thread.Sleep(2);
            }
            SoundManager.Play("spark.mp3");

            // Magic bubble transforms in life point
            SpriteToGrid(lifePoint, startX + 3 * s, 2, lifeC);
            PrintGrid();
            Thread.Sleep(100);

            // Lightning vanish
            SpriteToGrid(forErase, startX + 3 * s, 3, defaultColor, 0, 0);
            PrintGrid();
            Thread.Sleep(300);


            // Life points reach the life bar at the top corner
            int getHealth = 2; 
            int xMov = 11 - getHealth;
            if (!isLeft)
                xMov += 2;

            SpriteToGrid(lifePoint, startX + 3 * s, 1, lifeC);
            PrintGrid();
            Thread.Sleep(80);
            SpriteToGrid(lifePoint, startX + 3 * s, 1, lifeC, 0, 1);
            PrintGrid();

            for (int x = 1; x < xMov; x++)
            {
                SpriteToGrid(lifePoint, startX + (3 - x) * s, 1, lifeC, s, 0);
                SpriteToGrid(lifePoint, startX + (4 - x) * s, 1, lifeC, s, 0);
                PrintGrid();
                Thread.Sleep(80);
            }
        }

        /// <summary>
        /// Tank's special ability "Strong Attack" animation
        /// </summary>
        /// <param name="isLeft">If it's left player or not</param>
        /// <param name="health">Tank health</param>
        public static void TankSpecialAnim(bool isLeft, int health)
        {
            // Data initialization depending if it's the left or right animation
            string spr = Tank.SpriteLeft;
            int posX = charLeft;
            int startX = 1 + health;
            int tarX = charLeft + 4;
            int s = 1;

            if (!isLeft)
            {
                posX = charRight;
                spr = Tank.SpriteRight;
                startX = xMax - 1 - health;
                tarX = charRight + 2;
                s = -1;
            }

            // The last life point of the life bar move above the Tank head
            // Horizontal movement
            for (int i = 1; i < Math.Abs(startX - tarX); i++)
            {
                SpriteToGrid(lifePoint, startX + i * s, 1, lifeC, -s, 0);
                PrintGrid();
                Thread.Sleep(200);
            }
            // Vertical movement
            SpriteToGrid(".", startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC);
            PrintGrid();
            Thread.Sleep(100);

            // Life point metamorphose animation
            string[] trans = ["|", "/", "-", "\\", "+"];
            SoundManager.Play("spark.mp3");
            for (int i = 0; i < 15; i++)
            {
                SpriteToGrid(trans[i % trans.Length], startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC);
                PrintGrid();
                Thread.Sleep(40);
            }
            SpriteToGrid("|", startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC, 0, 0);

            // Tank goes red and attack
            SpriteToGrid(spr, posX, 2, lifeC);
            PrintGrid();
            Thread.Sleep(500);
            TankAttackAnim(isLeft, Tank.SpriteColorSpecial);
        }

        /// <summary>
        /// Tank attack animation
        /// </summary>
        /// <param name="isLeft">If it's left player or not</param>
        /// <param name="spriteColor">Tank color during attack</param>
        public static void TankAttackAnim(bool isLeft = true, ConsoleColor spriteColor = ConsoleColor.White)
        {
            // Data initialization depending if it's the left or right animation
            if (spriteColor == ConsoleColor.White)
                spriteColor = Tank.SpriteColor;

            string s1 = Tank.SpriteLeftBody; string s2 = Tank.SpriteRightBody; string attack = Tank.SpriteLeftAttack;
            int startX = charLeft;
            int tarX = charRight - 7;

            if (!isLeft)
            {
                s1 = Tank.SpriteRightBody; attack = Tank.SpriteRightAttack; s2 = Tank.SpriteLeftBody;
                startX = charRight;
                tarX = charLeft + 7;
            }

            int max = Math.Abs(tarX - startX);
            int s = Math.Sign(tarX - startX);

            int sprId = 0;
            if (isLeft)
                sprId = -1;
            else
                sprId = 4;


            // Tank walk to his opponent
            for (int x = 0; x <= max; x++)
            {
                if (isLeft)
                    sprId++;
                else
                    sprId--;
                if (sprId > 3)
                    sprId = 0;
                else if (sprId < 0)
                    sprId = 3;
                SpriteToGrid(s1 + Tank.SpriteLegs[sprId % 2], startX + x * s, 2, spriteColor, -s * Math.Sign(x), 0);

                PrintGrid();
                Thread.Sleep(20);
            }

            // Tank hit his opponent with his sword
            SoundManager.Play("hit.mp3");
            SpriteToGrid(attack + Tank.SpriteLegs[sprId], tarX, 2, spriteColor);
            PrintGrid();
            Thread.Sleep(300);

            // Tank goes back
            s *= -1;
            for (int x = 0; x <= max; x++)
            {
                if (isLeft)
                    sprId--;
                else
                    sprId++;
                if (sprId > 3)
                    sprId = 0;
                else if (sprId < 0)
                    sprId = 3;
                SpriteToGrid(s2 + Tank.SpriteLegs[sprId], tarX + x * s, 2, spriteColor, -s * Math.Sign(x), 0);

                PrintGrid();
                Thread.Sleep(20);
            }
            SpriteToGrid(s1 + Tank.SpriteLegs[0], startX, 2, Tank.SpriteColor);
            PrintGrid();
        }

        /// <summary>
        /// Bullet animation for Healer and Damager attack
        /// </summary>
        /// <param name="sprite">Bullet sprite</param>
        /// <param name="isLeft">If it's left player or not</param>
        /// <param name="color">Bullet color</param>
        /// <param name="sound">Bullet fire sound</param>
        public static void BulletAnim(string sprite, bool isLeft, ConsoleColor color, string sound)
        {
            // Data initialization depending if it's the left or right animation
            int startX = charLeft + 7;
            int tarX = charRight - 1 - sprite.Length;
            int max = Math.Abs(tarX - startX);
            if (!isLeft)
            {
                startX = tarX;
                tarX = 11;
            }
            int s = Math.Sign(tarX - startX);

            // Bullet animation
            SoundManager.Play(sound);
            for (int x = 0; x <= max; x++)
            {
                SpriteToGrid(sprite, startX + x * s, 2, color, -s * Math.Sign(x), 0);
                PrintGrid();
                Thread.Sleep(7);
            }
            SpriteToGrid(sprite, startX + max * s, 2, color, 0, 0);
            DefenseAnim(true, true); 
            PrintGrid();
        }

        /// <summary>
        /// Defend action animation
        /// </summary>
        /// <param name="isLeft">If it's left player or not</param>
        /// <param name="erase">If we have to erase or create the defense sprite</param>
        public static void DefenseAnim(bool isLeft, bool erase = false)
        {
            // Data initialization depending if it's the left or right animation
            const string defense = "|\n|\n|\n|";

            int x = 11;
            if (!isLeft)
                x = 40;

            if (erase) // Erase all defenses sprite
            {
                SpriteToGrid(defense, 11, 2, gridTextC, 0, 0);
                SpriteToGrid(defense, 40, 2, defaultColor, 0, 0);
                PrintGrid();
            }
            else // Create the left or right defense sprite
            {
                SoundManager.Play("defend.flac");
                for(int i=4; i >= 0; i--)
                {
                    SpriteToGrid(defense, x, 2-i, defaultColor, 0, -1);
                    PrintGrid();
                    Thread.Sleep(200);
                }
            }
        }

        /// <summary>
        /// Floor apparition animation
        /// </summary>
        private static void FloorAnim()
        {
            // Data
            string _floor = new string('=', xMax / 2);

            const ConsoleColor floorC = ConsoleColor.DarkGreen;
            const int floorTop = yMax - 1;

            // Apparition
            for (int i = 0; i <= xMax / 2 + 1; i++)
            {
                SpriteToGrid(_floor, -_floor.Length + i, floorTop, floorC);
                SpriteToGrid(_floor, xMax + 1 - i, floorTop, floorC);
                PrintGrid();
                Thread.Sleep(2);
            }
        }

        /// <summary>
        /// Fade in and out animation
        /// </summary>
        /// <param name="intro">If it plays fade in</param>
        /// <param name="outro">If it plays fade out</param>
        /// <param name="delay">Time delay between each animation frame</param>
        public static void Fade(bool intro = true, bool outro = true, int delay = 150) // Animation de transition de scène avec un fondu
        {
            // Data
            string _line = new string('-', xMax);
            ConsoleColor color = ConsoleColor.DarkGray;
            int max; int min;

            // Erase buttons and text 
            Console.SetCursorPosition(0, yMax + 2);
            for (int i = Console.CursorTop; i < Console.WindowHeight - 1; i++)
                Console.WriteLine(new string(' ', Console.WindowWidth));

            // Fade in animation
            if (intro)
            {
                max = yMax; min = 0;
                for (int c = 0; c <= yMax / 2; c++)
                {
                    for (int i = 1; i < yMax; i++)
                    {
                        if (i <= min || i >= max)
                            SpriteToGrid(_line, 1, i, color);
                    }
                    min++; max--;
                    PrintGrid();
                    Thread.Sleep(delay);
                }
                Thread.Sleep(delay * 2);
            }

            // Fade out animation
            if (outro)
            {
                max = (yMax + 1) / 2; min = max - 1;
                for (int c = 0; c <= yMax / 2; c++)
                {
                    for (int i = 1; i < yMax; i++)
                    {
                        if (i <= min || i >= max)
                        {
                            SpriteToGrid(_line, 1, i, color);
                        }
                        else
                        {
                            SpriteToGrid(_line, 1, i, color, 0, 0);
                        }
                    }
                    min--; max++;
                    PrintGrid();
                    Thread.Sleep(delay);
                }
                Thread.Sleep(delay * 2);
            }

        }
        #endregion
        
        #region Utilitary fonctions
        /// <summary>
        /// Button selection display
        /// </summary>
        /// <param name="butsText">Buttons name</param>
        /// <param name="texts">Specified texts with each button</param>
        /// <returns></returns>
        public static string Selector(string[] butsText, string[] texts)
        {
            _buttonIndex = (int)Math.Ceiling((decimal)butsText.Count() / 2) - 1; // Center preset selected button
            string easterEgg = ""; // EasterEgg...
            string nextScene = ""; // User choice

            ConsoleKey input; // User input
            do
            {
                ClearInputs();
                PrintButs(butsText);

                if (texts[0] == _defaultText)
                {
                    if(_actualText != _defaultText) // Print default text if we have to do it
                        PrintText(_defaultText, 0);
                }
                else                                // Else print specified button text
                    PrintText(texts[_buttonIndex], 0);

                // Read user input at the right bottom corner then erase it from the screen to keep a clean window
                Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
                input = Console.ReadKey().Key;
                Console.SetCursorPosition(Console.WindowWidth-2, Console.WindowHeight - 2);
                Console.Write(' ');

                // Change selected button 
                if (input == ConsoleKey.LeftArrow) 
                {
                    SoundManager.Play("arrow_key.mp3");
                    _buttonIndex--;
                }
                else if (input == ConsoleKey.RightArrow)
                {
                    SoundManager.Play("arrow_key.mp3");
                    _buttonIndex++;
                }

                // Return to the first button if user press right arrow while being on the last button
                if (_buttonIndex >= butsText.Length)
                    _buttonIndex = 0;
                // Return to the last button if user press left arrow while being on the first button
                else if (_buttonIndex < 0)
                    _buttonIndex = butsText.Length - 1;

                #region Beber Easter Egg
                string b = input.ToString().ToUpper();
                if (b == "B" || b == "E" || b == "R")
                {
                    SoundManager.Play("selection.mp3");
                    easterEgg += b;
                    if (easterEgg.ToUpper().Contains("BEBER"))
                        Beber(6);
                }
                else
                    easterEgg = "";
                #endregion
            } while (input != ConsoleKey.Enter && input != ConsoleKey.Spacebar && input != ConsoleKey.DownArrow);

            // User press Enter or Space
            SoundManager.Play("selection.mp3");
            nextScene = butsText[_buttonIndex];
            return nextScene;
        }

        /// <summary>
        /// Convert a sprite (string) into the game screen (grid)
        /// </summary>
        /// <param name="sprite">Sprite to convert</param>
        /// <param name="oX">X (column) origin position</param>
        /// <param name="oY">Y (line) origin position</param>
        /// <param name="color">Sprite color</param>
        /// <param name="eX">Where to erase sprite from the X origin position</param>
        /// <param name="eY">Where to erase sprite from the Y origin position</param>
        static void SpriteToGrid(string sprite, int oX, int oY, ConsoleColor color = defaultColor, int eX = int.MaxValue, int eY = int.MaxValue)
        {
            string[] slice = sprite.Split('\n'); // Cut the sprite line by line

            if (Math.Max(eX, eY) < int.MaxValue) // If we have to erase a sprite already printed
            {
                // Go throught each char / "pixel" of the sprite
                for (int y = 0; y < slice.Length; y++) 
                {

                    for (int x = 0; x < slice[y].Length; x++)
                    {
                        // If the char is inside the game screen, erase it by replace it by ' '
                        if (x + oX + eX > 0 && y + oY + eY > 0 && x + oX + eX < xMax && y + oY + eY < yMax)
                        {
                            _grid[x + oX + eX, y + oY + eY] = ' ';
                            _colorGrid[x + oX + eX, y + oY + eY] = color;
                        }
                    }

                }
            }

            // If we don't want to just erase a sprite (by putting eX=0 and eY=0)
            if (!(eX == 0 && eY == 0))
            {
                // Go throught each char / "pixel" of the sprite
                for (int y = 0; y < slice.Length; y++)
                {
                    for (int x = 0; x < slice[y].Length; x++)
                    {
                        // If the char position is inside the game screen
                        if ((x + oX > 0 && y + oY > 0 && x + oX < xMax && y + oY < yMax) || sprite == _gridBorders)
                        {
                            _grid[x + oX, y + oY] = slice[y][x]; // Set the "pixel" for actual position in grid
                            _colorGrid[x + oX, y + oY] = color; // Set the color for actual position in grid
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Choose which attack animation to play depending on the character
        /// </summary>
        /// <param name="source">Attacker character</param>
        public static void ChooseAttack(Character source)
        {
            switch(source)
            {
                case Damager: BulletAnim(Damager.BulletSprite, source.IsLeft, source.SpriteColorInstance, "fire.mp3"); break;
                case Healer: BulletAnim(Healer.BulletSprite, source.IsLeft, source.SpriteColorInstance, "spell.mp3"); break;
                case Tank: TankAttackAnim(source.IsLeft, Tank.SpriteColor); break;
            }
        }

        /// <summary>
        /// Update the life points on the top corners of the gamescreen
        /// </summary>
        /// <param name="left">Life points of the left player (1)</param>
        /// <param name="right">Life points of the right player (2)</param>
        public static void UpdateLifePoints(int left, int right)
        {
            // Erase actual life points sprites
            SpriteToGrid(new string(lifePoint[0], xMax / 2), 2, 1, lifeC, 0, 0);
            SpriteToGrid(new string(lifePoint[0], xMax / 2), xMax / 2, 1, lifeC, 0, 0);

            // Create new sprites of actual life points
            SpriteToGrid(new string(lifePoint[0], left), 2, 1, lifeC);
            SpriteToGrid(new string(lifePoint[0], right), xMax - 1 - right, 1, lifeC, 1, 0);
            PrintGrid();
        }

        /// <summary>
        /// Clear the user inputs pressed while an animation
        /// </summary>
        static void ClearInputs() 
        {
            while (Console.KeyAvailable)
            {
                Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
                Console.ReadKey();
                Console.SetCursorPosition(Console.WindowWidth - 2, Console.WindowHeight - 2);
                Console.Write(' ');
            }
        }
        #endregion

        #region Print fonctions

        /// <summary>
        /// Print the game screen (grid)
        /// </summary>
        private static void PrintGrid()
        {
            Console.SetCursorPosition(gridLeft, 0); // Set the cursor at the specified position 
            // Browse each element of the grid and print it with his specified color
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                for (int x = 0; x < _grid.GetLength(0); x++)
                {
                    Console.ForegroundColor = _colorGrid[x, y];

                    if (_grid[x, y] == 0)
                        Console.Write(" ");
                    else
                        Console.Write(_grid[x, y]);
                }
                Console.SetCursorPosition(10, Console.CursorTop + 1);
            }
        }

        /// <summary>
        /// Print the buttons 
        /// </summary>
        /// <param name="butsText">Buttons name</param>
        private static void PrintButs(string[] butsText)
        {
            Console.SetCursorPosition(0, butTop); // Set cursor at the specified position

            // Define each button position depending on the buttons count
            List<int> spaces = new List<int>();
            switch (butsText.Length)
            {
                case 1: spaces = [25]; break;
                case 2: spaces = [16, 34]; break;
                case 3: spaces = [11, 25, 39]; break;
                case 4: spaces = [8, 19, 29, 40]; break;
            }

            // Print each buttons, whose selected one, and its name
            for (int i = 0; i < butsText.Count(); i++)
            {
                string s = "--";
                if (i == _buttonIndex)
                    s = "■■";

                Console.SetCursorPosition(10 + spaces[i], butTop); // Button position
                Console.Write(s);
                Console.SetCursorPosition(Console.CursorLeft - (butsText[i].Count() - 2) / 2 - 2, Console.CursorTop + 1); // Text below the button centered position
                Console.Write(butsText[i]);
            }
        }

        /// <summary>
        /// Print actual text
        /// </summary>
        /// <param name="text">Text to print</param>
        /// <param name="delay">Delay to print each letter of the text</param>
        public static void PrintText(string text, int delay = 10)
        {
            if(text != _actualText) // Erase precedent text if it's not the same as the new one
                ClearScreen(false, false, true);

            Console.SetCursorPosition(0, textTop);

            _actualText = text;
            for (int i = 0; i < text.Length; i++)
            {
                // If we are on the last char of the window's line but it's isn't end of the text
                if (Console.CursorLeft == Console.WindowWidth - 1 && i < text.Length - 1)
                {
                    // If the last char is a space, we line break
                    if (text[i] == ' ') 
                        Console.WriteLine();
                    // Else if we are in the middle of a word, we put a dash then break line to print the char
                    else if (text[i + 1] != ' ') 
                        Console.Write($"-\n{text[i]}");
                    // Else if we are at the last letter of a word, we print it
                    else
                        Console.WriteLine(text[i]);
                }
                // Else if the char isn't a space to print on the first char of the line, we simply print the char
                else if(Console.CursorLeft != 0 || text[i] != ' ') 
                {
                    Console.Write(text[i]);
                }
                Thread.Sleep(delay);
            }
        }

        /// <summary>
        /// Clean the different sections of the application window
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="buttons"></param>
        /// <param name="text"></param>
        public static void ClearScreen(bool grid = true, bool buttons = true, bool text = true)
        {
            if (grid) // Clean the game screen except the grid borders
            {
                SpriteToGrid(_gridBorders, 0, 0, defaultColor);
                PrintGrid();
            }
            if (buttons) // Clean the buttons and their name
            {
                Console.SetCursorPosition(0, butTop);
                for (int i = butTop; i < textTop-1; i++)
                    Console.WriteLine(new string(' ', Console.WindowWidth));
            }
            if (text) // Clean the text
            {
                Console.SetCursorPosition(0, textTop);
                for (int i = textTop; i < Console.WindowHeight-1; i++)
                    Console.WriteLine(new string(' ', Console.WindowWidth));
            }
        }

        #endregion
    }
}


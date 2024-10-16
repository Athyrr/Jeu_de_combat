using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jeu_de_combat
{
    public class GameDisplay
    {

        //// Code trouvé sur internet pour enlever le mode d'édition rapide, qui fait que si l'utilisateur appuie sur la souris, le programme se met en pause
        //private const uint ENABLE_QUICK_EDIT = 0x0040; // Mode QuickEdit
        //private const uint ENABLE_EXTENDED_FLAGS = 0x0080; // Permet de changer les modes
        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern IntPtr GetStdHandle(int nStdHandle);
        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        // Empêche la souris d'intérragir 
        //IntPtr consoleHandle = GetStdHandle(-10);
        //uint consoleMode = 0;
        //consoleMode &= ~ENABLE_QUICK_EDIT;
        //consoleMode |= ENABLE_EXTENDED_FLAGS;
        //SetConsoleMode(consoleHandle, consoleMode);

        // Initialisation de la fenêtre de jeu


        // Data 
        const int xMax = 51; // Limite de la grille en longueur
        const int yMax = 7;  // Limite de la grille en hauteur

        char[,] grid = new char[xMax + 1, yMax + 1]; // Grille représentant l'écran de jeu, chaque élément est un "pixel" qui prend en valeur un caractère (char)
        ConsoleColor[,] colorGrid = new ConsoleColor[xMax + 1, yMax + 1]; // Grille représentant l'écran de jeu, chaque élément représente la couleur du "pixel"

        const int gridLeft = 10;      // Colonne assignée pour afficher l'écran de jeu (grille), la ligne étant la première (0)
        const int butTop = yMax + 2;  // Ligne assignée pour afficher les boutons, la colonne étant la première (0)
        const int textTop = yMax + 5; // Ligne assignée pour afficher le texte, la colonne étant la première (0)
        const int charLeft = 4; // Emplacement du joueur à gauche
        const int charRight = 41; // Emplacement du joueur à droite

        const string defaultText = "Choisissez grâce aux flèches et entrez l'action souhaitée.";

        // Sprites
        const ConsoleColor defaultColor = ConsoleColor.White;
        const ConsoleColor gridTextC = ConsoleColor.Gray;

        string line = new string('-', xMax);
        string underLine = new string('_', xMax);
        string floor = new string('=', xMax / 2);

        // Damager
        //ConsoleColor bulletC = gridTextC;

        const string defense = "|\n|\n|\n|"; // Action Défendre
        const string vs = "V   V  SSSSS"
                + "\nV   V  S    "
                + "\n V V   SSSSS"
                + "\n V V       S"
                + "\n  V    SSSSS";

        // Sol
        const ConsoleColor floorC = ConsoleColor.DarkGreen;
        const int floorTop = yMax - 1;

        // Vie
        const string lifePoint = "*";
        const ConsoleColor lifeC = ConsoleColor.Red;

        public GameDisplay()
        {
            // Initialisation de la fenêtre de la console
            Console.SetWindowSize(72, 20);
            Console.SetBufferSize(72, 20);
            Console.Title = "Monomachia";
            Console.CursorVisible = false;

            // Création du sprite de la bordure
            string emptyGridString = ""; // Sprit
            for (int y = 0; y <= yMax; y++)
            {
                for (int x = 0; x <= xMax; x++)
                {
                    if ((y == 0 || y == yMax) && (x == 0 || x == xMax))
                        emptyGridString += "+";
                    else if (y == 0 || y == yMax)
                        emptyGridString += '-';
                    else if (x == 0 || x == xMax)
                        emptyGridString += "|";
                    else if (x < xMax)
                        emptyGridString += ' ';

                    colorGrid[x, y] = defaultColor;
                }

                if (y < grid.GetLength(1) - 1)
                    emptyGridString += '\n';
            }
            StringToGrid(emptyGridString, 0, 0);
        }

        // Variables
        int selectBut = 1;
        string text = string.Empty;

        void GamemodeSelection()
        {
            // Afficher les sprites sur grid

            FloorAnim();
            for (int i = 10; i > 0; i--)
            {
                StringToGrid(damL, charLeft - i, 2, damC, 1, 0);
                StringToGrid(tanR, charRight + i, 2, tanC, -1, 0);
                StringToGrid(vs, 20, 2 - i, gridTextC, 0, -1);
                //StringToGrid(floor, 1, floorTop+i, floorC, 0, 1);
                PrintGrid();
                Thread.Sleep(100);
            }

            // Boutons et texte
            ButSelect(["Player vs AI", "AI vs AI"], [defaultText]);
        }

        void CharacterSelection()
        {
            // Afficher sprites sur grid
            for (int y = yMax + 1; y >= 2; y--)
            {
                StringToGrid(damL, 9, y, damC, 0, 1);
                StringToGrid(heaL, 22, y, heaC, 0, 1);
                StringToGrid(tanR, 36, y, tanC, 0, 1);
                PrintGrid();
                Thread.Sleep(100);
            }
            PrintGrid();
            // Afficher Bouttons et Texte associé 
            string[] t =
            [
                "Damager\nPoints de vie : ***, Points d'attaque : **\nRage : Inflige en retour les dégâts qui lui sont infligés durant ce tour. Les dégâts sont quand même subis par le Damager.",
                    "Healer\nPoints de vie : ****, Points d'attaque : *\nSoin : Récupère deux points de vie",
                    "Tank\nPoints de vie : *****, Points d'attaque : *\nAttaque puissante : Correspond à une attaque durant laquelle le Tank sacrifie un de ses points de vie pour augmenter sa force d’attaque de 1 et ce uniquement durant le tour en cours.",
                ];
            ButSelect(["Damager", "Healer", "Tank"], t);
        }

        void DisplayFight(Character playerLeft, Character playerRight)
        {
            FloorAnim();
            for (int i = 10; i >= 0; i--)
            {
                StringToGrid(playerLeft.SpriteLeft, charLeft - i, 2, playerLeft.SpriteColor, 1, 0);
                StringToGrid(playerRight.SpriteRight, charRight + i, 2, playerRight.SpriteColor, -1, 0);
                UpdateLifePoints(3 - i, 5 - i);
                PrintGrid();
                Thread.Sleep(50);
            }
            PrintGrid();
        }

        void ButClick(string nextScene)
        {
            if (nextScene == "Quit")
            {
                Fade(true, false);
                Environment.Exit(0);
            }
            else
            {
                Fade(true, true);
                switch (nextScene)
                {
                    case "Play": GamemodeSelection(); break;
                    case "Credits": Credits(); break;

                    case "Player vs AI": CharacterSelection(); break;
                    case "AI vs AI": CharacterSelection(); break;

                    case "Damager": Fight(); break; // Choisit la classe Damager
                    case "Tank": Fight(); break; // Choisit le classe Tank
                    case "Healer": Fight(); break; // Choisit la classe Healer

                    case "BEBER": Beber(); break;
                    default: Text("Commande invalide", 0, true); break;

                }
            }
        }

        void ButSelect(string[] butsText, string[] texts)
        {
            ClearInputs();
            string easterEgg = ""; // EasterEgg...
            string nextScene = "";
            ConsoleKey choice;
            do
            {
                PrintButs(butsText);
                if (texts.Length <= 1)
                {
                    if (text != defaultText)
                    {
                        text = texts[0];
                        PrintText(0);
                    }
                }
                else
                {
                    text = texts[selectBut];
                    PrintText(0);
                }

                Console.SetCursorPosition(0, Console.WindowTop);
                choice = Console.ReadKey().Key;
                Console.SetCursorPosition(0, Console.CursorTop); Console.Write(' ');

                if (choice == ConsoleKey.LeftArrow)
                    selectBut--;
                else if (choice == ConsoleKey.RightArrow)
                    selectBut++;

                if (selectBut >= butsText.Length)
                    selectBut = 0;
                else if (selectBut < 0)
                    selectBut = butsText.Length - 1;


                // Easter Egg..
                easterEgg += choice;
                if (easterEgg.ToUpper().Contains("BEBER") || easterEgg.ToUpper().Contains("P5INJ"))
                {
                    nextScene = "BEBER";
                    break;
                }
            } while (choice != ConsoleKey.Enter);

            text = "";
            if (nextScene != "BEBER")
                nextScene = butsText[selectBut];

            ButClick(nextScene);
        }

        void StringToGrid(string sprite, int oX, int oY, ConsoleColor color = defaultColor, int eX = int.MaxValue, int eY = int.MaxValue)
        {

            string[] slice = sprite.Split('\n');

            if (Math.Max(eX, eY) < int.MaxValue)
            {
                for (int y = 0; y < slice.Length; y++)
                {

                    for (int x = 0; x < slice[y].Length; x++)
                    {
                        if (x + oX + eX > 0 && y + oY + eY > 0 && x + oX + eX < xMax && y + oY + eY < yMax)
                        {
                            grid[x + oX + eX, y + oY + eY] = ' ';
                            colorGrid[x + oX + eX, y + oY + eY] = color;
                        }
                    }

                }
            }

            if (!(eX == 0 && eY == 0))
            {
                for (int y = 0; y < slice.Length; y++)
                {
                    for (int x = 0; x < slice[y].Length; x++)
                    {
                        if ((x + oX > 0 && y + oY > 0 && x + oX < xMax && y + oY < yMax) || sprite == emptyGridString)
                        {
                            grid[x + oX, y + oY] = slice[y][x];
                            colorGrid[x + oX, y + oY] = color;
                        }
                    }
                }
            }
        }

        void DamagerSpecialAnim(bool lookRight)
        {
            int startX = charLeft + 7;
            if (!lookRight)
                startX = charRight - 1;
            // Le pistolet s'agrandit d'autant de dégats reçus
            int damages = 7; // RECUP DEGATS RECUS
            string rageBullet = new string('=', damages);
            rageBullet += '>';

            // OU tire autant de balles que de dégâts reçus
            for (int i = 0; i < damages; i++)
            {
                BulletAnim("=>", true, damCspecial, 7 / (i + 1));
                Thread.Sleep(100);
            }
            //StringToGrid(rageBullet, startX, 2, damCspecial);
            //BulletAnim(rageBullet, true, damCspecial);
        }

        void HealerSpecialAnim(bool lookRight)
        {
            // Initialisation des données pour l'animation du joueur gauche
            int startX = charLeft + 5;
            int s = 1;

            if (!lookRight)
            {
                startX = charRight - 1;
                s = -1;
            }

            // Créer la bulle et la faire déplacer de 4 sur la droite
            for (int i = 0; i < 4; i++)
            {
                StringToGrid(magicBall, startX + i * s, 2, heaC, -Math.Sign(i) * s, 0);
                PrintGrid();
                Thread.Sleep(80);
            }
            Thread.Sleep(300);

            // Faire apparaître l'éclair qui le transforme en point de vie
            string[] lightning = ["/", "\\", "/"]; string forErase = " \n \n ";
            for (int i = 0; i < 3; i++)
            {
                StringToGrid(lightning[i], startX + 3 * s, yMax - 2 - i, ConsoleColor.Yellow);
                PrintGrid();
                Thread.Sleep(2);
            }

            // La bulle se transforme en point de vie 
            StringToGrid(lifePoint, startX + 3 * s, 2, lifeC);
            PrintGrid();
            Thread.Sleep(100);

            // L'éclair disparaît
            StringToGrid(forErase, startX + 3 * s, 3, defaultColor, 0, 0);
            PrintGrid();
            Thread.Sleep(300);


            // Les points de vie s'ajoute à notre barre (mov vertical puis horizontal)
            int getHealth = 2; // Récupérer les points de vie du joueur qui fait le spécial !!
            int xMov = 11 - getHealth; // Distance horizontale à parcourir
            if (!lookRight)
                xMov += 2;

            StringToGrid(lifePoint, startX + 3 * s, 1, lifeC);
            PrintGrid();
            Thread.Sleep(80);
            StringToGrid(lifePoint, startX + 3 * s, 1, lifeC, 0, 1);
            PrintGrid();

            for (int x = 1; x < xMov; x++)
            {
                StringToGrid(lifePoint, startX + (3 - x) * s, 1, lifeC, s, 0);
                StringToGrid(lifePoint, startX + (4 - x) * s, 1, lifeC, s, 0);
                PrintGrid();
                Thread.Sleep(80);
            }
            // Mettre à jour les points de vie
            PrintText();
        }

        void TankSpecialAnim(bool lookRight)
        {
            // Initialisation des données pour l'animation du joueur gauche
            int getHealth = 5; // RECUP VIE
            string spr = tanL;
            int posX = charLeft;
            int startX = 1 + getHealth;
            int tarX = charLeft + 4;
            int s = 1;

            if (!lookRight)
            {
                posX = charRight;
                spr = tanR;
                startX = xMax - 1 - getHealth;
                tarX = charRight + 2;
                s = -1;
            }

            // Le dernier point de vie se déplace horizontalement jusqu'au dessus de la tête du tank
            // Horizontal
            for (int i = 1; i < Math.Abs(startX - tarX); i++)
            {
                StringToGrid(lifePoint, startX + i * s, 1, lifeC, -s, 0);
                PrintGrid();
                Thread.Sleep(200);
            }
            // Vertical
            StringToGrid(".", startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC);
            PrintGrid();
            Thread.Sleep(100);

            // Le point de vie se consume et offre sa force au tank (animation)
            string[] trans = ["|", "/", "-", "\\", "+"];
            for (int i = 0; i < 15; i++)
            {
                StringToGrid(trans[i % trans.Length], startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC);
                PrintGrid();
                Thread.Sleep(40);
            }
            StringToGrid("|", startX + (Math.Abs(startX - tarX) - 1) * s, 1, lifeC, 0, 0);

            // Le tank devient rouge et prêt à attaquer
            StringToGrid(spr, posX, 2, lifeC);
            PrintGrid();
            tanC = tanCspecial;
            Thread.Sleep(500);
            AttackAnim(lookRight);
        }

        void BulletAnim(string bul, bool lookRight, ConsoleColor color, int delay = 7)
        {
            int startX = charLeft + 7;
            int tarX = charRight - 1 - bul.Length;
            int max = Math.Abs(tarX - startX);
            if (!lookRight)
            {
                startX = tarX;
                tarX = 11;
            }
            int s = Math.Sign(tarX - startX);


            for (int x = 0; x <= max; x++)
            {
                StringToGrid(bul, startX + x * s, 2, color, -s * Math.Sign(x), 0);
                PrintGrid();
                Thread.Sleep(delay);
            }
            StringToGrid(bul, startX + max * s, 2, color, 0, 0);
            DefenseAnim(true, true);
            PrintGrid();
        }

        void AttackAnim(bool lookRight = true)
        {
            // Par défaut, on rentre les données de l'animation pour le joueur de gauche
            string s1 = tanLbody; string s2 = tanRbody; string attack = tanLattack;
            int startX = charLeft;
            int tarX = charRight - 7;

            if (!lookRight) // Si finalement on veut faire l'animation pour le joueur de droite, on change les données
            {
                s1 = tanR; attack = tanRattack; s2 = tanL;
                startX = charRight;
                tarX = charLeft + 7;
            }

            int max = Math.Abs(tarX - startX);
            int s = Math.Sign(tarX - startX);

            int sprId = 0;
            if (lookRight)
                sprId = -1;
            else
                sprId = 4;

            // Le tank s'avance
            for (int x = 0; x <= max; x++)
            {
                if (lookRight)
                    sprId++;
                else
                    sprId--;
                if (sprId > 3)
                    sprId = 0;
                else if (sprId < 0)
                    sprId = 3;
                StringToGrid(s1 + leg[sprId % 2], startX + x * s, 2, tanC, -s * Math.Sign(x), 0);

                PrintGrid();
                Thread.Sleep(25);
            }

            // Le tank donne un coup d'épée
            StringToGrid(attack + leg[sprId], tarX, 2, tanC);
            PrintGrid();
            Thread.Sleep(300);

            s *= -1;
            // Le tank reprend sa place
            for (int x = 0; x <= max; x++)
            {
                if (lookRight)
                    sprId--;
                else
                    sprId++;
                if (sprId > 3)
                    sprId = 0;
                else if (sprId < 0)
                    sprId = 3;
                StringToGrid(s2 + leg[sprId], tarX + x * s, 2, tanC, -s * Math.Sign(x), 0);

                PrintGrid();
                Thread.Sleep(25);
            }
            tanC = tanCneutral;
            StringToGrid(s1 + leg[0], startX, 2, tanC);
            PrintGrid();
        }

        void DefenseAnim(bool isLeft, bool erase = false)
        {
            int x = 11;
            if (!isLeft)
                x = 40;

            if (erase)
            {
                StringToGrid(defense, 11, 2, defaultColor, 0, 0);
                StringToGrid(defense, 40, 2, defaultColor, 0, 0);
            }
            else
                StringToGrid(defense, x, 2);
        }

        void FloorAnim()
        {
            for (int i = 0; i <= xMax / 2 + 1; i++)
            {
                StringToGrid(floor, -floor.Length + i, floorTop, floorC);
                StringToGrid(floor, xMax + 1 - i, floorTop, floorC);
                PrintGrid();
                Thread.Sleep(2);
            }
        }

        public void OpenAnim()
        {
            //Fade(false, true, 500);
            Fade(false, true);

            string title = "MMMM OOOO N  N OOOO MMMM  AA  CCCC H  H  I   AA " +
                         "\nM  M O  O NN N O  O M  M A  A C    HHHH     A  A" +
                         "\nM  M O  O N NN O  O M  M AAAA C    HHHH  I  AAAA" +
                         "\nM  M OOOO N  N OOOO M  M A  A CCCC H  H  I  A  A";
            char t = '*';
            title = title.Replace('M', t); title = title.Replace('O', t); title = title.Replace('N', t);
            title = title.Replace('A', t); title = title.Replace('C', t); title = title.Replace('H', t); title = title.Replace('I', t);

            for (int y = yMax + 1; y > 1; y--)
            {
                StringToGrid(title, 2, y, gridTextC, 0, 1);
                PrintGrid();
                //Thread.Sleep(0);
                Thread.Sleep(300);
            }
        }

        void Fade(bool intro, bool outro, int delay = 150) // Animation de transition de scène avec un fondu
        {
            ConsoleColor color = ConsoleColor.DarkGray;
            // Effacer le texte et les boutons
            Console.SetCursorPosition(0, yMax + 2);
            for (int i = Console.CursorTop; i < Console.WindowHeight - 1; i++)
                Console.WriteLine(new string(' ', Console.WindowWidth));

            int max; int min;
            // Intro
            if (intro)
            {
                max = yMax; min = 0;
                for (int c = 0; c <= yMax / 2; c++)
                {
                    for (int i = 1; i < yMax; i++)
                    {
                        if (i <= min || i >= max)
                            StringToGrid(line, 1, i, color);
                    }
                    min++; max--;
                    PrintGrid();
                    Thread.Sleep(delay);
                }
                Thread.Sleep(delay * 2);
            }

            // Outro
            if (outro)
            {
                max = (yMax + 1) / 2; min = max - 1;
                for (int c = 0; c <= yMax / 2; c++)
                {
                    for (int i = 1; i < yMax; i++)
                    {
                        if (i <= min || i >= max)
                        {
                            StringToGrid(line, 1, i, color);
                        }
                        else
                        {
                            StringToGrid(line, 1, i, color, 0, 0);
                        }
                    }
                    min--; max++;
                    PrintGrid();
                    Thread.Sleep(delay);
                }
                Thread.Sleep(delay * 2);
            }

        }

        void Start() // Sah je pense c'est guez on peut enlever
        {
            string start = "SSSSS TTTTT    A   RRRRR TTTTT  |"
                       + "\nS       T     A A  R   R   T    |"
                       + "\nSSSSS   T    AAAAA RRRRR   T    |"
                       + "\n    S   T    A   A R  R    T    |"
                       + "\nSSSSS   T    A   A R   R   T    O"
                       + "\n_________________________________";

            for (int x = -32; x <= xMax + 1; x++)
            {
                StringToGrid(start, x, 1, gridTextC, -1, 0);
                PrintGrid();
            }
        }

        void UpdateLifePoints(int left, int right)
        {
            if (left >= 0 && right >= 0)
            {
                StringToGrid(new string(lifePoint[0], left), 2, 1, lifeC);
                StringToGrid(new string(lifePoint[0], right), xMax - right - 1, 1, lifeC);
                PrintGrid();
            }
        }

        void PrintGrid()
        {
            Console.SetCursorPosition(gridLeft, 0);
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    Console.ForegroundColor = colorGrid[x, y];

                    if (grid[x, y] == 0)
                        Console.Write(" ");
                    else
                        Console.Write(grid[x, y]);
                }
                Console.SetCursorPosition(10, Console.CursorTop + 1);
            }
        }

        void PrintButs(string[] butsText)
        {
            Console.SetCursorPosition(0, butTop); // Place le curseur à l'endroit assigné pour l'affichage des boutons

            // Définit l'emplacement de chaque bouton
            List<int> spaces = new List<int>();
            switch (butsText.Length)
            {
                case 2: spaces = [16, 34]; break;
                case 3: spaces = [11, 25, 39]; break;
            }

            // Affiche les boutons et leur texte correspondant
            for (int i = 0; i < butsText.Count(); i++)
            {
                string s = "--";
                if (i == selectBut)
                    s = "■■";

                Console.SetCursorPosition(10 + spaces[i], butTop);
                Console.Write(s);
                Console.SetCursorPosition(Console.CursorLeft - (butsText[i].Count() - 2) / 2 - 2, Console.CursorTop + 1);
                Text(butsText[i]);
            }
        }

        void PrintText(int delay = 10) // Affiche le texte en fonction de la situation
        {
            Console.SetCursorPosition(0, textTop); // Place le curseur à l'endroit assigné pour le texte
            for (int i = Console.CursorTop; i < Console.WindowHeight; i++) // Efface les potentielles textes précédents
            {
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, i);
            }

            Console.SetCursorPosition(0, textTop);
            Text(text, delay); // Écrit le texte avec un délai d'apparition des caractères pour créer un effet dialogue

            //Console.WriteLine();
        }

        void Text(string text, int delay = 10, bool debug = false)
        {
            if (debug)
                Console.SetCursorPosition(Console.CursorLeft, Console.WindowTop);

            //if(text.Length <= 72) IDEE de centrer le texte en bas 
            //    Console.SetCursorPosition((72 - text.Length) / 2, Console.CursorTop);

            for (int i = 0; i < text.Length; i++)
            {
                if (Console.CursorLeft == 0 && text[i] == ' ') // Si on est au début de la ligne et qu'on doit afficher un espace, on ne le fait pas
                {

                }
                else if (Console.CursorLeft == Console.WindowWidth - 1 && i < text.Length - 1) // Si on est au dernier caractère de la ligne de la fenètre mais que ce n'est pas la fin du texte
                {
                    if (text[i] == ' ') // Si le dernier caractère à afficher est un espace, on saute directement la ligne
                        Console.WriteLine();
                    else if (text[i + 1] != ' ') // Si on est en plein milieu d'un mot, on met un tiret puis on va à la ligne pour afficher le caractère
                        Console.Write($"-\n{text[i]}");
                    else // Si on est à la fin d'un mot, on peut donc afficher le dernier caractère et aller à la ligne
                        Console.WriteLine(text[i]);
                }
                else
                {
                    Console.Write(text[i]);
                }
                Thread.Sleep(delay);
            }
        }

        void ClearInputs() // Permet de supprimer les actions faites par le joueur pendant les animations
        {
            while (Console.KeyAvailable)
            {
                Console.SetCursorPosition(0, Console.WindowTop);
                Console.ReadKey();
                Console.SetCursorPosition(0, Console.CursorTop); Console.Write(' ');
            }
        }

        void Credits()
        {
            string c = "\"MONOMACHIA\""
                + "-A P5INJ PRODUCTION"
                + "-Gameplay Programmer : Adam Adhar"
            + "-Sound Designer : Elliot Nedellac"
            + "-Graphics Implementation : Marius Boulandet"
            + "-Special thanks : Beber"
            + "---Thanks for playing <3";
            string credits = "";
            foreach (string s in c.Split('-'))
            {
                string space = new string(' ', (xMax - 1 - s.Length) / 2);
                credits += space + s + space + "\n\n";
            }

            for (int i = 0; i < yMax + credits.Split('\n').Count() - 5; i++)
            {
                StringToGrid(credits, 1, yMax - i, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(500);
            }
            Thread.Sleep(1000);
            Fade(true, false);
            Game();
        }

        void Beber() // Un petit easter egg en l'hommage de notre promo et de sa mascotte !
        {
            ConsoleColor beberC = ConsoleColor.DarkYellow;
            string beberText = "oo  ooo oo  ooo ooo"
                + "\no o o   o o o   o o"
                + "\nooo oo  ooo oo  ooo"
                + "\no o o   o o o   oo "
                + "\noo  ooo oo  ooo o o";
            beberText = beberText.Replace('o', '*');

            string psinj = "ooo ooo o o  o   o"
                       + "\no o o     o  o   o"
                       + "\nooo ooo o oo o   o"
                       + "\no     o o o oo   o"
                       + "\no   ooo o o  o ooo";
            psinj = psinj.Replace('o', '*');

            string beber = "   .-\"-.   "
            + "\n _/.-.-.\\_ "
            + "\n( ( o o ) )"
            + "\n |/  \"  \\| "
            + "\n  \\ .-. /  "
            + "\n  /`\"\"\"`\\  ";

            for (int y = yMax + 1; y > 0; y--)
            {
                StringToGrid(beberText, 1, y, gridTextC, 0, 1);
                StringToGrid(beber, 21, y, beberC, 0, 1);
                StringToGrid(psinj, 33, y, gridTextC, 0, 1);
                PrintGrid();
                Thread.Sleep(300);
            }
        }
    }
}
}


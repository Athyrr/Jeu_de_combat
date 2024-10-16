namespace Jeu_de_combat
{
    internal class Program()
    {
        ///<inheritdoc cref="GameDisplay"/>
        public GameDisplay _displayModule = new GameDisplay();

        /// <summary>
        /// Tracks <see cref="AttackProcess"/> datas. 
        /// </summary>
        private static List<AttackProcess> _attackProcesses = new();

        /// <summary>
        /// Tracks <see cref="DefendProcess"/> datas. 
        /// </summary>
        private static List<DefendProcess> _defendProcesses = new();

        /// <summary>
        /// The player 1.
        /// </summary>
        private static Character _player1 = null;

        /// <summary>
        /// The player 2.
        /// </summary>
        private static Character _player2 = null;

        /// <summary>
        /// Is the game running ?
        /// </summary>
        private static bool _isRunning = false;

        /// <summary>
        /// Is the game stopped ?
        /// </summary>
        private static bool _stopFighting = false;

        /// <summary>
        /// Makes the player1 an AI.
        /// </summary>
        private static bool _wantAI = false;

        /// <summary>
        /// The difficulty of the game
        /// </summary>
        private static int difficulty;

        public static void Main(string[] args)
        {
            GameDisplay.Init();

            GameState state = GameState.Intro;

            _isRunning = true;

            while (_isRunning)
            {
                switch (state)
                {
                    case GameState.Intro:

                        string menu = GameDisplay.DisplayMenu();

                        switch (menu)
                        {
                            case "Quit":
                                GameDisplay.Fade(true, false);
                                Environment.Exit(0);
                                break;

                            case "Play":
                                state = GameState.GameModeSelection;
                                break;

                            case "Credits":
                                state = GameState.Credit;
                                break;

                            default:
                                Console.WriteLine("Error menu seletion !");
                                break;
                        }

                        break;

                    case GameState.GameModeSelection:
                        string mode = GameDisplay.DisplayGameModeSelection();
                        _wantAI = GameModeSelection(mode);

                        state = GameState.PlayerSelection;
                        break;

                    case GameState.PlayerSelection:

                        Console.WriteLine();
                        string characterString = "";

                        if (!_wantAI)
                            characterString = GameDisplay.DisplayCharacterSelection();

                        _player1 = PlayerSelection(_wantAI, characterString);
                        _player1.IsIA = _wantAI;
                        Console.WriteLine($"Player 1 picked : {_player1.Name}");

                        if (!_wantAI) _wantAI = true;

                        _player2 = PlayerSelection(_wantAI, characterString);
                        _player2.IsIA = _wantAI;
                        Console.WriteLine($"Player 2 picked : {_player2.Name}");

                        state = GameState.Game;
                        break;

                    case GameState.Game:
                        GameDisplay.DisplayFight(_player1, _player2);
                        Game();
                        _isRunning = false;

                        state = GameState.Credit;
                        break;

                    case GameState.Credit:
                        GameDisplay.DisplayCredits();

                        state = GameState.Intro;
                        break;

                    default:

                        Console.WriteLine("Error, Invalid state.");

                        state = GameState.None;
                        break;
                }
            }
        }

        /// <summary>
        /// Displays a menu which set the game mode.
        /// </summary>
        private static bool GameModeSelection(string mode)
        {
            return mode == "AI vs AI" ? true : false;
        }


        /// <summary>
        /// Allows a player to select a character.
        /// </summary>
        /// <param name="wantIA"></param>
        /// <returns>Returns the index of the selected character archetype.</returns>
        private static Character PlayerSelection(bool wantIA, string characterString)
        {
            Dictionary<int, Character> indexCharacters = new Dictionary<int, Character>
            {
                { 1, new Damager() },
                { 2, new Healer() },
                { 3, new Tank() }
            };

            if (wantIA)
            {
                Random rand = new Random();
                int choice = rand.Next(1, 4);

                return indexCharacters[choice];
            }

            switch (characterString)
            {
                case nameof(Damager):
                    return new Damager();


                case nameof(Healer):
                    return new Healer();


                case nameof(Tank):
                    return new Tank();

                //Case 4 : other character

                default:
                    Console.WriteLine("Failed select player! Player input is invalid.");
                    return null;
            }
        }

        /// <summary>
        /// The game loop.
        /// </summary>
        private static void Game()
        {
            Console.WriteLine();
            Console.WriteLine("|| FIGHTTTTTTTTTTT ||");
            Console.WriteLine();

            //FightDisplay(Charatcter player1, CHaracter player2)

            while (!_stopFighting)
            {
                Choice(_player1, _player2);
                Choice(_player2, _player1);

                ProcessDefends(_defendProcesses);
                ProcessAttacks(_attackProcesses);
                ProcessDamagersSpecial();

                ResetEffects();
                ClearProcesses();
            }
        }

        /// <summary>
        /// Asks a player how it will act.
        /// </summary>
        /// <param name="source">The player who will act</param>
        /// <param name="target">The target character who will receive the attack</param>
        /// <returns></returns>
        private static int Choice(Character source, Character target)
        {
            int choice = 0;

            if (!source.IsIA)
            {
                string question = $"(player 1) {source.Name} : What would you like to do ?\n1: attack | 2: defend | 3: special attack";
                choice = AskForInput(question, 1, 3);
            }
            else
            {
                Console.WriteLine("(AI) Je reflechis");
                choice = AIBehavior(source);

                Thread.Sleep(500);
            }

            if (choice == 1)
                _attackProcesses.Add(new AttackProcess(source, target, false, source.Strength));

            if (choice == 3)
                _attackProcesses.Add(new AttackProcess(source, target, true, source.Strength));

            if (choice == 2)
                _defendProcesses.Add(new DefendProcess(source));

            return choice;
        }

        /// <summary>
        /// Ends the game if a win condition is reached.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns>Returns true if a win condition is reached.</returns>
        private static bool EndGame(Character player1, Character player2)
        {
            Console.WriteLine();

            if (!player1.IsAlive && !player2.IsAlive)
            {
                Console.WriteLine($"(player1) {player1.Name} and (player 2) {player2.Name} are both dead.");
                Console.WriteLine("Draw !");
                _stopFighting = true;
                return true;
            }

            if (!player1.IsAlive && player2.IsAlive)
            {
                Console.WriteLine($"(player 2) {player2.Name} slayed (player 1) {player1.Name}.");
                Console.WriteLine("Player 2 won !");
                _stopFighting = true;
                return true;
            }

            if (player1.IsAlive && !player2.IsAlive)
            {
                Console.WriteLine($"(player 1) {player1.Name} slayed (player 2) {player2.Name}.");
                Console.WriteLine("Player 1 won !");
                _stopFighting = true;
                return true;
            }

            return false;
        }

        public static int AskForInput(string question, int min, int max)
        {
            int output = 0;
            Console.WriteLine(question);
            while (!int.TryParse(Console.ReadLine(), out output) || output > max || output < min)
            {
                Console.WriteLine("Invalid input");
                Console.WriteLine(question);
            }

            return output;
        }

        /// <summary>
        /// Resets all active effects.
        /// </summary>
        private static void ResetEffects()
        {
            _player1.ResetEffects();
            _player2.ResetEffects();
        }

        /// <summary>
        /// Clears all process lists.
        /// </summary>
        private static void ClearProcesses()
        {
            _attackProcesses.Clear();
            _defendProcesses.Clear();
        }

        /// <summary>
        /// Processes all tracked attacks. It makes attackers act.
        /// </summary>
        /// <param name="processes"></param>
        private static void ProcessAttacks(List<AttackProcess> processes)
        {
            if (processes.Count <= 0)
                return;

            foreach (AttackProcess process in processes)
            {
                string name = process.Source == _player1 ? "(Player 1)" : "(Player 2)";

                if (process.IsSpecial)
                {
                    Console.WriteLine($"{name} {process.Source.Name} use his special attack !");
                    process.Source.SpecialAttack(process.Target);
                }
                else
                {
                    Console.WriteLine($"{name} {process.Source.Name} attacks !");
                    process.Source.Attack(process.Target, process.DamageAmount);
                }
                if (EndGame(_player1, _player2))
                    return;
            }
        }

        /// <summary>
        /// Processes all tracked defenses. It makes defensers act.
        /// </summary>
        /// <param name="processes"></param>
        private static void ProcessDefends(List<DefendProcess> processes)
        {
            if (processes.Count <= 0)
                return;

            foreach (var process in processes)
            {
                Console.WriteLine($"{process.Defender.Name} defends !");
                process.Defender.Defend();
            }
        }

        /// <summary>
        /// Processes damagers special attack if any.
        /// </summary>
        private static void ProcessDamagersSpecial()
        {
            if (_player1.GetType() != typeof(Damager) && _player2.GetType() != typeof(Damager))
                return;

            if (_player1 is Damager damager1 && damager1.SpecialEffectEnabled)
            {
                int reveivedDamages = damager1.DamagesTaken;
                Console.WriteLine("Reflects " + damager1.DamagesTaken + " dmg.");
                _player1.Attack(_player2, reveivedDamages);

                if (EndGame(_player1, _player2))
                    return;
            }

            if (_player2 is Damager damager2 && damager2.SpecialEffectEnabled)
            {
                int reveivedDamages = damager2.DamagesTaken;
                Console.WriteLine("Reflects " + damager2.DamagesTaken + " dmg.");
                _player2.Attack(_player1, reveivedDamages);

                if (EndGame(_player1, _player2))
                    return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Returns an int that represent the class of the character</returns>
        public static int CharacterType(Character source)
        {
            if (source is Damager)
                return 1;
            else if (source is Healer)
                return 2;
            else return 3;
        }

        /// <summary>
        /// IA behaviour depending on current diffculty
        /// </summary>
        public static int AIBehavior(Character source)
        {
            Random rand = new Random();
            bool followBehaviour = true;

            switch (difficulty)
            {
                case 1:
                    followBehaviour = false;
                    break;
                case 2:
                    followBehaviour = rand.Next(0, 2) == 0; // 1 chance sur 2
                    break;

                case 3:
                    followBehaviour = !(rand.Next(0, 6) == 0); // 5 chances sur 6
                    break;

                default:
                    Console.WriteLine("Difficulty is not in valid int");
                    break;
            }

            int choice = rand.Next(1, 4);
            if (!followBehaviour || difficulty == 1)
                return choice;


            switch (source.CharacterClass)
            {
                case CharacterClasses.Damager:
                    if (source.Health > source.MaxHealth / 2)
                        choice = 1;

                    else if (source.Health <= 2)
                        choice = 2;

                    else
                    {
                        rand = new Random();
                        choice = rand.Next(0, 2);
                    }

                    break;

                case CharacterClasses.Healer:
                    if (source.Health <= source.MaxHealth - 2)
                        choice = 3;

                    else
                        choice = 1;

                    break;

                case CharacterClasses.Tank:
                    if (source.Health > source.MaxHealth / 2)
                        choice = 3;

                    else if (source.Health <= 2)
                        choice = 2;

                    else
                        choice = 1;

                    break;
            }
            return choice;
        }
    }
}
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
                        Random rand = new Random();
                        Console.WriteLine();
                        string characterString = "";
                        string[] characters = ["Damager", "Healer", "Tank"];

                        if (!_wantAI)
                            characterString = GameDisplay.DisplayCharacterSelection();

                        if (characterString == "Random")
                            characterString = characters[rand.Next(0, 3)];
                        _player1 = PlayerSelection(_wantAI, characterString);
                        _player1.IsIA = _wantAI;
                        _player1.IsLeft = true;

                        if (!_wantAI) _wantAI = true;

                        _player2 = PlayerSelection(_wantAI, characterString);
                        _player2.IsIA = _wantAI;
                        _player2.IsLeft = false;

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
            // a enlever plus tard maybe
            Random rand = new Random();
            difficulty = rand.Next(1, 4);

            while (!_stopFighting)
            {
                Choice(_player1, _player2);
                Choice(_player2, _player1);

                GameDisplay.ClearScreen(false);
                ProcessDefends(_defendProcesses);
                ProcessAttacks(_attackProcesses);
                ProcessDamagersSpecial();
                GameDisplay.UpdateLifePoints(_player1.Health, _player2.Health);
                GameDisplay.DefenseAnim(true, true);

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
        private static string Choice(Character source, Character target)
        {
            string choice = "";

            if (!source.IsIA)
            {
                string question = $"(Player 1) {source.Name} : What would you like to do ?\n\n";
                string[] texts =
                {
                    $"{question}Attack : Infliges {source.Strength} damages",
                    $"{question}Defend : Prevent from getting damages if the opponent attack",
                    $"{question}{source.SpecialDescription}"
                };

                choice = GameDisplay.Selector(["Attack", "Defend", "Special"], texts);
            }
            else
            {
                choice = AIBehavior(source);

                Thread.Sleep(500);
            }

            if (choice == "Attack")
                _attackProcesses.Add(new AttackProcess(source, target, false, source.Strength));

            if (choice == "Special")
                _attackProcesses.Add(new AttackProcess(source, target, true, source.Strength));

            if (choice == "Defend")
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
                //Console.WriteLine($"(player 2) {player2.Name} slayed (player 1) {player1.Name}.");
                //Console.WriteLine("Player 2 won !");
                _stopFighting = true;
                GameDisplay.DisplayEndGame(player2);
                return true;
            }

            if (player1.IsAlive && !player2.IsAlive)
            {
                //Console.WriteLine($"(player 1) {player1.Name} slayed (player 2) {player2.Name}.");
                //Console.WriteLine("Player 1 won !");
                _stopFighting = true;
                GameDisplay.DisplayEndGame(player1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets all active effects.
        /// </summary>
        private static void ResetEffects()
        {
            GameDisplay.DefenseAnim(true, true);
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
                if (process.IsSpecial)
                {
                    process.Source.SpecialAttack(process.Target, process.Source.IsLeft);
                }
                else
                {
                    GameDisplay.ChooseAttack(process.Source);
                    process.Source.Attack(process.Target, process.DamageAmount);
                }
                if (EndGame(_player1, _player2))
                    return;

                GameDisplay.UpdateLifePoints(_player1.Health, _player2.Health);
                Thread.Sleep(1000);
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
                process.Defender.Defend(process.Defender.IsLeft);
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
                GameDisplay.DamagerSpecialAnim(true, reveivedDamages);
                _player1.Attack(_player2, reveivedDamages);

                if (EndGame(_player1, _player2))
                    return;
            }

            if (_player2 is Damager damager2 && damager2.SpecialEffectEnabled)
            {
                int reveivedDamages = damager2.DamagesTaken;
                GameDisplay.DamagerSpecialAnim(false, reveivedDamages);
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
        public static string AIBehavior(Character source)
        {
            Random rand = new Random();
            bool followBehaviour = true;
            string[] choices = ["Attack", "Defend", "Special"];

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
            //if (!followBehaviour)
            //    return choices[choice-1];


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
            return choices[choice-1];
        }
    }
}
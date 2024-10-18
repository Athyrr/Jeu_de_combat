using System.Diagnostics;

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
            SoundManager.Init();



            GameState state = GameState.Intro;

            while (true)
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
                            characterString = characters[rand.Next(0, characters.Length)];

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
            //difficulty = rand.Next(1, 4);
            difficulty = 3;

            while (!_stopFighting)
            {
                AttackSelection(_player1, _player2);
                AttackSelection(_player2, _player1);

                GameDisplay.ClearScreen(false);
                if(difficulty==3)
                {
                    _defendProcesses.Reverse();
                    _attackProcesses.Reverse();
                }

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
        private static string AttackSelection(Character source, Character target)
        {
            string choice = "";
            List<string> choices = ["Attack", "Defend", "Special"];

            string question = $"(Player 1) {source.Name} : What would you like to do ?\n\n";
            List<string> choicesText = new List<string> {
                $"{question}Attack : Infliges {source.Strength} damages",
                $"{question}Defend : Prevent from getting damages if the opponent attack",
                $"{question}{source.SpecialDescription}"
            };

            // Si le personnage a déjà utilisé son spécial ou est en incapacité de l'utiliser, on lui retire la possibilité de le réutiliser
            if (source.previousChoices.Contains("Special") || source is Healer && source.Health >= source.MaxHealth - 1 || source is Tank && source.Health == 1)
            {
                choices.Remove("Special");
                choicesText.RemoveAt(2);
            }

            // On retire des possibilités la dernière action effectuée
            int id = -1;
            if(source.previousChoices.Count > 0)
                id = choices.IndexOf(source.previousChoices[source.previousChoices.Count-1]);
            if (id >= 0)
            {
                choices.RemoveAt(id);
                choicesText.RemoveAt(id);
            }

            if (!source.IsIA) // Choix du joueur
            {
                choice = GameDisplay.Selector(choices.ToArray(), choicesText.ToArray());
            }
            else // Choix de l'IA
            {
                choice = AIBehavior(source, choices.ToArray(), target);
            }

            source.previousChoices.Add(choice);

            if (choice == "Attack")
                _attackProcesses.Add(new AttackProcess(source, target, false, source.Strength));

            else if (choice == "Special")
                _attackProcesses.Add(new AttackProcess(source, target, true, source.Strength));

            else if (choice == "Defend")
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
            GameDisplay.UpdateLifePoints(_player1.Health, _player2.Health);

            if (!player1.IsAlive && !player2.IsAlive)
            {
                GameDisplay.PrintText("Draw !");
                _stopFighting = true;
                return true;
            }

            if (!player1.IsAlive && player2.IsAlive)
            {
                Console.WriteLine($"{player1.Name} (Player 1) wins !");
                _stopFighting = true;
                GameDisplay.DisplayEndGame(player2, player1);
                return true;
            }

            if (player1.IsAlive && !player2.IsAlive)
            {
                Console.WriteLine($"{player1.Name} (Player 2) wins !");
                _stopFighting = true;
                GameDisplay.DisplayEndGame(player1, player2);
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
                    string whichPlayer = process.Source.IsLeft ? "Player 1" : "Player 2";
                    GameDisplay.PrintText($"{process.Source.Name} ({whichPlayer}) uses his special !");

                    process.Source.SpecialAttack(process.Target);
                }
                else
                {
                    string whichPlayer = process.Source.IsLeft ? "Player 1" : "Player 2";
                    GameDisplay.PrintText($"{process.Source.Name} ({whichPlayer}) attacks !");

                    GameDisplay.ChooseAttack(process.Source);
                    process.Source.Attack(process.Target, process.DamageAmount);
                }

                if (EndGame(_player1, _player2))
                {
                    return;
                }

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
                string whichPlayer = process.Defender.IsLeft ? "Player 1" : "Player 2";
                GameDisplay.PrintText($"{process.Defender.Name} ({whichPlayer}) defends itself !");
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
                GameDisplay.DamagerSpecialAnim(true, reveivedDamages*2);
                _player1.Attack(_player2, reveivedDamages*2);

                if (EndGame(_player1, _player2))
                    return;
            }

            if (_player2 is Damager damager2 && damager2.SpecialEffectEnabled)
            {
                int reveivedDamages = damager2.DamagesTaken;
                GameDisplay.DamagerSpecialAnim(false, reveivedDamages*2);
                _player2.Attack(_player1, reveivedDamages*2);

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
        public static string AIBehavior(Character source, string[] choices, Character target)
        {
            Random rand = new Random();
            string choice = choices[rand.Next(0, choices.Count())]; // Choix aléatoire prédifini

            switch (difficulty)
            {
                case 1: // L'IA effectuera un choix aléatoire
                    return choice;

                case 2: // L'IA effectuera le meilleur choix possible selon sa situation actuelle 
                    switch (source.CharacterClass)
                    {
                        case CharacterClasses.Damager: // Si l'IA est un Damager
                            // Si la vie de l'IA est plus grande que 2 et que l'IA peut faire son spécial, elle a une chance sur deux de le faire
                            if (source.Health > source.MaxHealth / 2 && choices.Contains("Special") && rand.Next(0, 2) == 0)
                                choice = "Special";

                            // Sinon, si la vie de l'IA est plus grande que 2 et que l'IA peut attaquer, elle attaque
                            else if (source.Health > source.MaxHealth / 2 && choices.Contains("Attack"))
                                choice = "Attack";

                            // Sinon, si la vie est plus petite ou égale à deux et qu'elle peut se défendre, elle défend
                            else if (source.Health <= 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            break;

                        case CharacterClasses.Healer: // Si l'IA est un Healer
                            // Si la vie de l'IA est plus petite ou égale à 2 et que l'IA peut faire son spécial, elle le fait
                            if (source.Health <= source.MaxHealth - 2 && choices.Contains("Special"))
                                choice = "Special";

                            // Si la vie de l'IA est plus petite ou égale à 2 et qu'elle peut se défendre, elle le fait.
                            else if (source.Health <= source.MaxHealth - 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            // Sinon, si elle peut attaquer, elle le fait
                            else if (choices.Contains("Attack"))
                                choice = "Attack";

                            break;

                        case CharacterClasses.Tank: // Si l'IA est un Tank
                            // Si la vie de l'IA est supérieure à 3 et qu'il peut faire son spécial, il le fait
                            if (source.Health > source.MaxHealth / 2 && choices.Contains("Special"))
                                choice = "Special";

                            // Sinon, si sa vie est inférieure ou égale à 2 et qu'il peut se défendre, il le fait
                            else if (source.Health <= 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            // Sinon, si il peut attaquer, il le fait
                            else if (choices.Contains("Attack"))
                                choice = "Attack";

                            break;
                    }
                    break;

                case 3: // L'IA jouera avant nous et effectuera le meilleur choix possible en étudiant toutes les situations possibles grâce à une simulation sur 2 tours
                    string playerChoice = target.previousChoices[target.previousChoices.Count-1]; // L'IA Zeus, omnisciente, récupère le choix du joueur.
                    if(playerChoice=="Special")
                        playerChoice = target.Name;

                    switch(playerChoice)
                    {
                        case "Defend": // Le joueur va se défendre
                            if(choices.Contains("Special"))
                                // Si l'IA est un Tank et que son spécial qui va malgré tout tuer le joueur
                                // OU que l'IA est un Healer et qu'il a perdu 2 points de vie ou plus
                                if (source is Tank && source.Health > 1 && target.Health == 1
                                    || source is Healer && source.Health <= source.MaxHealth - 2)
                                    return "Special";

                            if(choices.Contains("Attack"))
                                // Si l'IA serait en danger au prochain tour si elle ne garde pas son action défense, elle attaque
                                if (source.Health <= target.Strength
                                    || target is Tank && !target.previousChoices.Contains("Special") && source.Health <= 2)
                                    return "Attack";

                            // Sah jsp
                            if(choices.Contains("Defend"))
                                return "Defend";
                            break;

                        case "Attack": // Le joueur va attaquer
                            if (choices.Contains("Attack"))
                                // Si l'attaque de l'IA va tuer le joueur
                                if (source.Strength >= target.Health)
                                    return "Attack";

                            if (choices.Contains("Special"))
                                // Si l'IA est un Damager/Tank et que son spécial va tuer le joueur
                                // OU que l'IA est un Healer qui ne peut pas se défendre mais doit se soigner pour éviter la mort
                                if (source is Damager && source.Health > target.Strength && target.Health <= target.Strength * 2
                                || source is Tank && source.Health > 1 && target.Health <= 2
                                || source is Healer && !choices.Contains("Defend") && source.Health <= target.Strength) 
                                    return "Special";


                            if (choices.Contains("Defend"))
                                // Si l'attaque qui arrive est mortelle
                                if (source.Health <= target.Strength)
                                    return "Defend";

                            break;

                        case "Damager": // Le joueur va utiliser la capacité Rage 
                            if (choices.Contains("Attack"))
                                // Si l'attaque va tuer le Damager avant qu'il puisse faire son spécial
                                if (source.Strength >= target.Health)
                                    return "Attack";

                            if (choices.Contains("Defend"))
                                // Si l'IA peut se défendre, elle le fait pour éviter le contrecoup 
                                    return "Defend";

                            if (choices.Contains("Special"))
                                // Si l'IA est un Damager ou un Healer, elle fait son spécial pour éviter le contrecoup
                                // OU si l'IA est un Tank dont le spécial tuerait le joueur
                                if(source is Damager || source is Healer || source is Tank && source.Health > 1 && target.Health <= 2)
                                    return "Special"; 

                            break;

                        case "Healer": // Le joueur va attaquer
                            if (choices.Contains("Attack"))
                                // Si l'attaque va tuer le Damager avant qu'il puisse faire son spécial
                                if (source.Strength >= target.Health)
                                    return "Attack";

                            if (choices.Contains("Special"))
                                // Si le spécial du Tank va tuer le joueur 
                                // OU que l'IA est un Healer qui doit se soigner
                                if(source is Tank && source.Health > 1 && target.Health <= 2
                                    || source is Healer && source.Health <= source.MaxHealth - 2)
                                return "Special";

                            if (choices.Contains("Attack"))
                                // Sinon, on attaque malgré tout
                                return "Attack";

                            break;

                        case "Tank": // Le joueur va attaquer
                            if (choices.Contains("Special"))
                                // Si le spécial du Damager va tuer le joueur et qu'on peut encaisser l'attaque du Tank
                                // OU Si le spécial du Tank va tuer le joueur et qu'on peut l'utiliser
                                // OU Si on doit se soigner
                                if(source is Damager && source.Health > 2 && target.Health <= 4
                                    || source is Tank && source.Health > 1 && target.Health <= 2
                                    || source is Healer && source.Health <= source.MaxHealth - 2)
                                    return "Special";

                            if (choices.Contains("Attack"))
                                // Si notre attaque va tuer le joueur
                                if (source.Strength >= target.Health)
                                    return "Attack";

                            if (choices.Contains("Defend"))
                                return "Defend";

                            break;
                    }

                    break;

                default:
                    Console.WriteLine("Difficulty is not in valid int");
                    break;
            }

            return choice;
        }
    }
}
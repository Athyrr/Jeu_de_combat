using System.Diagnostics;

namespace Jeu_de_combat
{
    internal class Program()
    {
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
        private static List<string> difficulty = new List<string>();

        public static void Main(string[] args)
        {
            GameDisplay.Init();
            SoundManager.Init();

            GameState state = GameState.Intro;
            int chooseCharacterId = 0; // Pour indiquer la bonne sélection lors de la sélection de personnage pour le joueur et les IA

            while (true)
            {
                switch (state)
                {
                    case GameState.Intro:
                        SoundManager.StopAllLoops();
                        chooseCharacterId = 0;
                        _stopFighting = false;
                        difficulty.Clear();
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

                        if (_wantAI)
                        {
                            chooseCharacterId = 1;
                            state = GameState.AISelection;
                        }
                        else
                            state = GameState.CharacterSelection;
                        break;

                    case GameState.CharacterSelection:
                        Console.WriteLine();
                        string characterString = "";
                        string[] characters = ["Damager", "Healer", "Tank"];
                        string[] texts =
                        {
                            "Choose your character :",
                            "Choose IA character :",
                            "Choose IA character :"
                        };
                        if (_wantAI && chooseCharacterId == 0)
                            chooseCharacterId = 1;

                        characterString = GameDisplay.DisplayCharacterSelection(texts[chooseCharacterId]);

                        if (chooseCharacterId <= 1) // Choix du joueur ou pour l'IA
                        {
                            _player1 = PlayerSelection(characterString);
                            _player1.IsIA = chooseCharacterId == 1;
                            _player1.IsLeft = true;
                        }
                        else // Choix pour l'IA
                        {
                            _player2 = PlayerSelection(characterString);
                            _player2.IsIA = true;
                            _player2.IsLeft = false;
                        }

                        if (chooseCharacterId == 0)
                        {
                            state = GameState.AISelection;
                            chooseCharacterId = 2;
                        }
                        else if (chooseCharacterId == 1)
                        {
                            state = GameState.AISelection;
                            chooseCharacterId = 2;
                        }
                        else
                            state = GameState.Game;

                        break;

                    case GameState.AISelection:
                        difficulty.Add(GameDisplay.DisplayIALevelSelection(_wantAI));

                        state = GameState.CharacterSelection;
                        break;

                    case GameState.Game:

                        SoundManager.StopAllLoops();

                        GameDisplay.DisplayFight(_player1, _player2);

                        Game();

                        state = GameState.Credit;
                        break;

                    case GameState.Credit:

                        SoundManager.StopAllLoops();

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
            return mode == "AI vs AI";
        }


        /// <summary>
        /// Allows a player to select a character.
        /// </summary>
        /// <param name="wantIA"></param>
        /// <returns>Returns the index of the selected character archetype.</returns>
        private static Character PlayerSelection(string characterString)
        {
            Random rand = new Random();
            Dictionary<int, Character> indexCharacters = new Dictionary<int, Character>
            {
                { 1, new Damager() },
                { 2, new Healer() },
                { 3, new Tank() }
            };

            switch (characterString)
            {
                case nameof(Damager):
                    return new Damager();


                case nameof(Healer):
                    return new Healer();


                case nameof(Tank):
                    return new Tank();

                case "Random":
                    return indexCharacters[rand.Next(1, 4)];

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
            Random rand = new Random();

            while (!_stopFighting)
            {
                AttackSelection(_player1, _player2);
                AttackSelection(_player2, _player1);

                GameDisplay.ClearScreen(false);
                if (difficulty[0] == "Zeus")
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

            // If player can't use his special
            if (source.previousChoices.Contains("Special") || source is Healer && source.Health >= source.MaxHealth - 1 || source is Tank && source.Health == 1)
            {
                choices.Remove("Special");
                choicesText.RemoveAt(2);
            }

            // If player defended last turn and so can't defend again
            int id = -1;
            if (source.previousChoices.Count > 0)
            {
                if (source.previousChoices[source.previousChoices.Count() - 1] == "Defend")
                {
                    choices.Remove("Defend");
                    choicesText.RemoveAt(1);
                }
            }

            if (!source.IsIA) // User choice
            {
                choice = GameDisplay.Selector(choices.ToArray(), choicesText.ToArray());
            }
            else // AI choice
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
                _stopFighting = true;
                GameDisplay.DisplayEndGame(player2, player1);
                return true;
            }

            if (player1.IsAlive && !player2.IsAlive)
            {
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
                int receivedDamages = damager1.DamagesTaken;
                if (receivedDamages > 0)
                {
                    GameDisplay.DamagerSpecialAnim(true, receivedDamages);
                    _player1.Attack(_player2, receivedDamages);
                }
                else
                {
                    GameDisplay.PrintText("Damager (Player 1) special missed !");
                    Thread.Sleep(500);
                }

                if (EndGame(_player1, _player2))
                    return;
            }

            if (_player2 is Damager damager2 && damager2.SpecialEffectEnabled)
            {
                int receivedDamages = damager2.DamagesTaken;
                if (receivedDamages > 0)
                {
                    GameDisplay.DamagerSpecialAnim(false, receivedDamages);
                    _player2.Attack(_player1, receivedDamages);
                }
                else
                {
                    GameDisplay.PrintText("Damager (Player 2) special missed !");
                    Thread.Sleep(500);
                }

                if (EndGame(_player1, _player2))
                    return;
            }
        }

        /// <summary>
        /// AI behaviour depending on current difficulty
        /// </summary>
        public static string AIBehavior(Character source, string[] choices, Character target)
        {
            Random rand = new Random();
            string choice = choices[rand.Next(0, choices.Count())]; // Random preset choice
            int which = source.IsLeft ? 1 : 0; // Which AI between left and right (in AI vs AI) to get his level

            switch (difficulty[which])
            {
                // AI will do a choice randomly, with a 10% chance doing its special ability.
                case "Tyche":
                    int r = rand.Next(0, 100);
                    if (r < 10 && !source.previousChoices.Contains("Special"))
                        choice = "Special";
                    else
                    {
                        List<string> newChoices = choices.ToList();
                        newChoices.Remove("Special");
                        choice = newChoices[rand.Next(0, newChoices.Count())];
                    }



                    break;

                // AI will do a coherent choice according to his situation (character, life points)
                case "Athena":
                    switch (source.CharacterClass)
                    {
                        case CharacterClasses.Damager: // AI is a Damager
                            // If AI's life is greater than 2, it has half a chance to do its special ability Rage
                            if (source.Health > source.MaxHealth / 2 && choices.Contains("Special") && rand.Next(0, 2) == 0)
                                choice = "Special";

                            // Else if AI's life's greater than 2, it attacks
                            else if (source.Health > source.MaxHealth / 2 && choices.Contains("Attack"))
                                choice = "Attack";

                            // Else if AI's life's smaller than 3, it defends
                            else if (source.Health <= 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            break;

                        case CharacterClasses.Healer: // AI is a Healer
                            // If AI's life is smaller than 3, it does its special ability Heal
                            if (source.Health <= source.MaxHealth - 2 && choices.Contains("Special"))
                                choice = "Special";

                            // Else if AI's life is smaller than 3 but it can't do its special, it defends
                            else if (source.Health <= source.MaxHealth - 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            // Else, it attacks
                            else if (choices.Contains("Attack"))
                                choice = "Attack";

                            break;

                        case CharacterClasses.Tank: // If AI is a Tank
                            // AI has half a chance to do its special if it has over 2 life points 
                            if (source.Health > 2 && rand.Next(0, 2) == 0 && choices.Contains("Special"))
                                choice = "Special";

                            // Else if its life is smaller than 3, it defends
                            else if (source.Health <= 2 && choices.Contains("Defend"))
                                choice = "Defend";

                            // Else it attacks
                            else if (choices.Contains("Attack"))
                                choice = "Attack";

                            break;
                    }
                    break;

                // AI will play before the player while already know his action and so will do the best possible choice
                case "Zeus":
                    string playerChoice = target.previousChoices[target.previousChoices.Count - 1];
                    if (playerChoice == "Special")
                        playerChoice = target.Name;

                    bool canHeal = (source is Healer && source.Health <= source.MaxHealth - 2 && choices.Contains("Special"));
                    bool canRage = (source is Damager && choices.Contains("Special"));
                    bool canStrong = (source is Tank && choices.Contains("Special") && source.Health > 1);

                    // Can AI kill the player ?
                    if (target.Health <= source.Strength + (playerChoice == "Defend" ? 0 : 1) && canStrong)
                        return "Special";
                    if (target.Health <= source.Strength && choices.Contains("Attack") && playerChoice != "Defend")
                        return "Attack";

                    // Else we check what the player will do and act in consequence
                    switch (playerChoice)
                    {
                        case "Defend":
                            if (canHeal)
                                return "Special";
                            if (choices.Contains("Defend") && source.Health > target.Strength)
                                return "Defend";
                            break;

                        case "Attack":
                            if (target.Strength >= source.Health) // Attack will kill the AI
                            {
                                if (canHeal)
                                    return "Special";
                                if (choices.Contains("Defend"))
                                    return "Defend";
                            }
                            if (canStrong)
                                return "Special";
                            break;

                        case "Damager": // Player is a Damager and use his special
                            if (canHeal)
                                return "Special";
                            if (choices.Contains("Defend"))
                                return "Defend";
                            if (canRage)
                                return "Special";
                            break;

                        case "Healer": // Player is a Healer and use his special
                            if (canHeal || canStrong)
                                return "Special";
                            break;

                        case "Tank": // Player is a Tank and use his special
                            if (target.Strength + 1 >= source.Health) // Strong Attack will kill the AI
                            {
                                if (canHeal)
                                    return "Special";
                                if (choices.Contains("Defend"))
                                    return "Defend";
                            }
                            if (canStrong || canRage)
                                return "Special";
                            if (choices.Contains("Defend"))
                                return "Defend";
                            break;

                    }
                    return "Attack";
                    break;

                default:
                    Console.WriteLine("Difficulty is not in valid int");
                    break;
            }

            return choice;
        }
    }
}
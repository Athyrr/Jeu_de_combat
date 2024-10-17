using System.Text;

namespace Jeu_de_combat
{
    /// <summary>
    /// Defines a Tank character.
    /// </summary>
    public class Tank : Character
    {
        #region Fields

        ///<inheritdoc cref="Character.Name"/>
        private string _name = nameof(Tank);

        ///<inheritdoc cref="Character.Strength"/>
        private int _strength = 1;

        ///<inheritdoc cref="Character.Health"/>
        private int _health = 5;

        ///<inheritdoc cref="Character.MaxHealth"/>
        private int _maxHealth = 5;

        ///<inheritdoc cref="Character.SpecialDescription"/>
        private string _specialDescription = "Strong Attack : Tank uses 1 life point to increase his strength by 1, then attacks. After his attack, his strength goes back to normal.";

        /// <summary>
        /// Is the damager special used?
        /// </summary>
        private bool _specialEffectEnabled = false;

        ///<inheritdoc cref="Character.SpriteLeft"/>
        private static string _spriteLeft = "   O | \n 0-|-| \n   |   \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteRight"/>
        private static string _spriteRight = " | O   \n |-|-0 \n   |   \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteColor"/>
        private static ConsoleColor _spriteColor = ConsoleColor.DarkBlue;

        /// <summary>
        /// SpriteColor of special attack
        /// </summary>
        private static ConsoleColor _spriteColorSpecial = ConsoleColor.Red;

        private static string _spriteLeftBody = "   O | \n 0-|-| \n   |   ";
        private static string _spriteRightBody = " | O   \n |-|-0 \n   |   ";
        private static string _spriteLeftAttack = "   O  /\n 0-|\\/ \n   |   ";
        private static string _spriteRightAttack = "\\  O   \n \\/|-0 \n   |   ";
        private static string[] _spriteLegs = ["\n  / \\  ", "\n   |\\  ", "\n  /|   ", "\n  / \\  "];

        #endregion

        /// <inheritdoc cref="Tank"/>
        public Tank()
        {
            InitCharacter();
        }

        #region Public API

        /////<inheritdoc cref="Character.IsAlive"/>
        //public new bool IsAlive => _health > 0 ? true : false;

        ///<inheritdoc cref=" _specialEffectEnabled"/>
        public bool SpecialEffectEnabled => _specialEffectEnabled;

        public new static string SpriteLeft => _spriteLeft;
        public new static string SpriteRight => _spriteRight;
        public static string SpriteRightBody => _spriteRightBody;
        public static string SpriteLeftBody => _spriteLeftBody;
        public static string SpriteLeftAttack => _spriteLeftAttack;
        public static string SpriteRightAttack => _spriteRightAttack;
        public static string[] SpriteLegs => _spriteLegs;


        ///<inheritdoc cref="_spriteColorNeutral"/>
        public new static ConsoleColor SpriteColor => _spriteColor;

        ///<inheritdoc cref="_spriteColorSpecial"/>
        public static ConsoleColor SpriteColorSpecial => _spriteColorSpecial;

        #endregion


        #region Public API

        /// <summary>
        /// <inheritdoc cref=" Character.SpecialAttack"/>
        /// Inflics 1 damage more but sacrifices 1 health.
        /// </summary>
        public override void SpecialAttack(Character target, bool lookRight)
        {
            GameDisplay.TankSpecialAnim(lookRight);
            if (Health < 2)
            {
                Console.WriteLine("Cannot use Tank special, You don't have enough health");
                return;
            }

            _specialEffectEnabled = true;

            Health--;
            Attack(target, Strength + 1);
        }

        ///<inheritdoc cref="Character.InitCharacter"/>
        protected override void InitCharacter()
        {
            CharacterClass = CharacterClasses.Damager;

            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
            SpecialDescription = _specialDescription;

            SpriteColorInstance = _spriteColor;
            SpriteLeftInstance = _spriteLeft;
            SpriteRightInstance = _spriteRight;
        }

        ///<inheritdoc cref="Character.ResetEffects"/>
        public override void ResetEffects()
        {
            base.ResetEffects();

            _specialEffectEnabled = false;
        }

        #endregion
    }
}

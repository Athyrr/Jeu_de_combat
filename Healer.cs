namespace Jeu_de_combat
{
    /// <summary>
    /// Defines a Healer character.
    /// </summary>
    public class Healer : Character
    {
        #region Fields

        ///<inheritdoc cref="Character.Name"/>
        private string _name = nameof(Healer);

        ///<inheritdoc cref="Character.Strength"/>
        private int _strength = 1;

        ///<inheritdoc cref="Character.Health"/>
        private int _health = 4;

        ///<inheritdoc cref="Character.MaxHealth"/>
        private int _maxHealth = 4;

        ///<inheritdoc cref="Character.SpecialDescription"/>
        private string _specialDescription = "Heal : Heal 2 life points.";


        ///<inheritdoc cref="Character.SpriteLeft"/>
        private static string _spriteLeft = "   A @ \n  /|-| \n   | | \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteRight"/>
        private static string _spriteRight = " @ A   \n |-|\\  \n | |   \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteColor"/>
        private static ConsoleColor _spriteColor = ConsoleColor.DarkMagenta;

        /// <summary>
        /// Bullet's Sprite
        /// </summary>
        private static string _bulletSprite = "o";

        #endregion


        #region Lifecycle

        ///<inheritdoc cref="Healer"/>
        public Healer()
        {
            InitCharacter();
        }

        #endregion


        #region Public API

        /////<inheritdoc cref="Character.IsAlive"/>
        //public new bool IsAlive => _health > 0 ? true : false;

        public new static string SpriteLeft => _spriteLeft;
        public new static string SpriteRight => _spriteRight;
        public new static ConsoleColor SpriteColor => _spriteColor;

        ///<inheritdoc cref="_bulletSprite"/>
        public static string BulletSprite => _bulletSprite;


        ///<inheritdoc cref="Character.SpecialAttack(Character)"/>
        public override void SpecialAttack(Character target, bool lookRight)
        {
            Health = Math.Min(Health + 2, MaxHealth);
            GameDisplay.HealerSpecialAnim(lookRight);
        }

        #endregion


        #region Protected API

        ///<summary>
        /// <inheritdoc cref=" Character.SpecialAttack"/>
        ///Heals 2 point of health.
        ///</summary>
        protected override void InitCharacter()
        {
            CharacterClass = CharacterClasses.Healer;

            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
            SpecialDescription = _specialDescription;

            SpriteLeftInstance = _spriteLeft;

            SpriteRightInstance = _spriteRight;
            SpriteColorInstance = _spriteColor;
        }

        #endregion
    }
}

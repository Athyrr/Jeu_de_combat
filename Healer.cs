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

        ///<inheritdoc cref="Character.SpriteLeft"/>
        private string _spriteLeft = "   A @ \n  /|-| \n   | | \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteRight"/>
        private string _spriteRight = " @ A   \n |-|\\  \n | |   \n  / \\  ";

        /// <inheritdoc cref="Character.SpriteColor"/>
        private ConsoleColor _spriteColor = ConsoleColor.DarkMagenta;

        /// <summary>
        /// Bullet's Sprite
        /// </summary>
        private string _bulletSprite = "o";

        #endregion

        ///<inheritdoc cref="Healer"/>
        public Healer()
        {
            InitCharacter();
        }

        #region Public API

        ///<inheritdoc cref="Character.IsAlive"/>
        public new bool IsAlive => _health > 0 ? true : false;

        ///<summary>
        /// <inheritdoc cref=" Character.SpecialAttack"/>
        ///Heals 2 point of health.
        ///</summary>
        ///
     
        ///<inheritdoc cref="_bulletSprite"/>
        public string BulletSprite => _bulletSprite;

        public override void SpecialAttack(Character target)
        {
            Health = Math.Min(Health + 2, MaxHealth);

            Console.WriteLine("Heal !");

            string playerIndexString = IsIA ? "(player 2)" : "(player 1)";
            Console.WriteLine($"{playerIndexString} Health : " + Health);
        }

        ///<inheritdoc cref="Character.InitCharacter"/>
        protected override void InitCharacter()
        {
            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
            SpriteLeft = _spriteLeft;
            SpriteRight = _spriteRight;
            SpriteColor = _spriteColor;
        }

        #endregion
    }
}

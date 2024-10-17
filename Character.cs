namespace Jeu_de_combat
{
    /// <summary>
    /// Defines a base character.
    /// </summary>
    public abstract class Character
    {
        #region Fields

        /// <summary>
        /// The name of the class.
        /// </summary>
        private string _name = "";

        /// <summary>
        /// Character attack power.
        /// </summary>
        private int _strength = 0;

        /// <summary>
        /// Character PV amount
        /// </summary>
        private int _health = 0;

        /// <summary>
        /// Max health value
        /// </summary>
        private int _maxHealth = 0;

        /// <summary>
        /// Is the character defending.
        /// </summary>
        private bool _isDefending = false;

        /// <summary>
        /// Is the player an AI.
        /// </summary>
        private bool _isIA = false;

        /// <summary>
        /// If the Character is on left or on right
        /// </summary>
        private bool _isLeft = true;

        /// <summary>
        /// Explication of character's special
        /// </summary>
        private string _specialDescription = "";

        //Sprites

        /// <summary>
        /// The character left sprite.
        /// </summary>
        private string _spriteLeft = "";

        /// <summary>
        /// The character right sprite.
        /// </summary>
        private string _spriteRight = "";

        /// <summary>
        /// The character sprite color.
        /// </summary>
        private ConsoleColor _spriteColor = ConsoleColor.White;

        #endregion


        #region Public API

        /// <inheritdoc cref="_name"/>
        public string Name { get => _name; protected set => _name = value; }

        /// <inheritdoc cref="_strength"/>
        public int Strength { get => _strength; protected set => _strength = value; }

        ///<inheritdoc cref="_health"/>
        public int Health { get => Math.Max(0, _health); protected set => _health = value; }

        ///<inheritdoc cref="_health"/>
        public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }

        /// <summary>
        /// Is the character alive ?
        /// </summary>
        public bool IsAlive => _health > 0 ? true : false;

        ///<inheritdoc cref="_isIA"/>
        public bool IsIA { get => _isIA; set => _isIA = value; }

        ///<inheritdoc cref="_isLeft"/>
        public bool IsLeft { get => _isLeft; set => _isLeft = value; }

        ///<inheritdoc cref="_specialDescription"/>
        public string SpecialDescription { get => _specialDescription; protected set => _specialDescription = value; }
        
        /// <summary>
        /// The class of the character
        /// </summary>
        public CharacterClasses CharacterClass = CharacterClasses.None;


        ///<inheritdoc cref="_spriteLeft"/>
        public static string SpriteLeft = "";

        ///<inheritdoc cref="_spriteRight"/>
        public static string SpriteRight = "";

        ///<inheritdoc cref="_spriteColor"/>
        public static ConsoleColor SpriteColor = ConsoleColor.White;


        /// <summary>
        /// Non-static version of <see cref="SpriteLeft"/>
        /// </summary>
        public string SpriteLeftInstance { get => _spriteLeft; protected set => _spriteLeft = value; }

        /// <summary>
        /// None-static version of <see cref="SpriteLeft"/>
        /// </summary>
        public string SpriteRightInstance { get => _spriteRight; protected set => _spriteRight = value; }

        /// <summary>
        /// None-static version of <see cref="SpriteColor"/>
        /// </summary>
        public ConsoleColor SpriteColorInstance { get => _spriteColor; protected set => _spriteColor = value; }


        /// <summary>
        /// Use a basic attack.
        /// </summary>
        /// <param name="target">The target aimed by the attack.</param>
        public virtual void Attack(Character target, int damageAmount)
        {
            target.TakeDamage(this, damageAmount);
        }

        /// <summary>
        /// Inflics damages to the character.
        /// </summary>
        /// <param name="damageSource">The source to which damages are inflicted.</param>
        /// <param name="damageSource">The source to which damages are inflicted.</param>
        protected virtual void TakeDamage(Character damageSource, int damageAmount)
        {
            if (_isDefending)
                if (damageSource is not Tank tank || !tank.SpecialEffectEnabled)
                    return;
                else if (tank.SpecialEffectEnabled)
                    damageAmount -= 1;


            _health = Math.Max(0, _health - damageAmount);
        }

        /// <summary>
        /// Use character special attack.
        /// </summary>
        public abstract void SpecialAttack(Character target, bool lookRight);

        /// <summary>
        /// Defends the character, and avoid taken damages.
        /// </summary>
        public void Defend(bool lookRight)
        {
            GameDisplay.DefenseAnim(lookRight);
            _isDefending = true;
        }

        /// <summary>
        /// Initializes character stats.
        /// </summary>
        protected abstract void InitCharacter();

        /// <summary>
        /// Resets all current active effects.
        /// </summary>
        public virtual void ResetEffects()
        {
            _isDefending = false;
        }

        #endregion
    }
}

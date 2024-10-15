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

        /// <summary>
        /// The class of the character
        /// </summary>
        public CharacterClasses CharacterClass = CharacterClasses.None;

        //Sprites Properties

        ///<inheritdoc cref="_spriteLeft"/>

        public string SpriteLeft => _spriteLeft; 
        ///<inheritdoc cref="_spriteRight"/>

        public string SpriteRight => _spriteRight;

        ///<inheritdoc cref="_spriteColor"/>
        public ConsoleColor SpriteColor => _spriteColor;


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

            string name = !IsIA ? "(Player 1)" : "(Player 2)";

            Console.WriteLine($"{name} {_name} damages received : {damageAmount}.");
            Console.WriteLine($"{name} {_name} remaning life : {_health}.");
        }

        /// <summary>
        /// Use character special attack.
        /// </summary>
        public abstract void SpecialAttack(Character target);

        public void Defend()
        {
            _isDefending = true;
        }

        /// <summary>
        /// Initializes character stats.
        /// </summary>
        protected abstract void InitStats();

        public virtual void ResetEffects()
        {
            _isDefending = false;
        }

        #endregion




    }
}

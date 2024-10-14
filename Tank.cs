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

        /// <summary>
        /// Is the damager special used?
        /// </summary>
        private bool _specialEffectEnabled = false;

        #endregion

        /// <inheritdoc cref="Tank"/>
        public Tank()
        {
            InitStats();
        }

        #region Public API

        ///<inheritdoc cref="Character.IsAlive"/>
        public new bool IsAlive => _health > 0 ? true : false;

        ///<inheritdoc cref=" _specialEffectEnabled"/>
        public bool SpecialEffectEnabled => _specialEffectEnabled;

        #endregion


        #region Public API

        /// <summary>
        /// <inheritdoc cref=" Character.SpecialAttack"/>
        /// Inflics 1 damage more but sacrifices 1 health.
        /// </summary>
        public override void SpecialAttack(Character target)
        {
            if (Health < 2)
            {
                Console.WriteLine("Cannot use Tank special, You don't have enough health");
                return;
            }

            _specialEffectEnabled = true;

            Health--;
            Attack(target, Strength + 1);
        }

        ///<inheritdoc cref="Character.InitStats"/>
        protected override void InitStats()
        {
            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
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

namespace Jeu_de_combat
{
    /// <summary>
    /// Defines a Damager character
    /// </summary>
    public class Damager : Character
    {
        #region Fields





        ///<inheritdoc cref="Character.Name"/>
        private string _name = nameof(Damager);

        ///<inheritdoc cref="Character.Strength"/>
        private int _strength = 2;

        ///<inheritdoc cref="Character.Health"/>
        private int _health = 3;

        ///<inheritdoc cref="Character.MaxHealth"/>
        private int _maxHealth = 3;

        /// <summary>
        /// Was the Damager special attack used?
        /// </summary>
        private bool _specialEffectEnabled = false;

        /// <summary>
        /// The amount of damages taken during this turn.
        /// </summary>
        private int _damagesTaken = 0;

        #endregion


        #region Lifecycle

        ///<inheritdoc cref="Damager"/>
        public Damager()
        {
            InitStats();
        }

        #endregion


        #region Public API

        ///<inheritdoc cref="Character.IsAlive"/>
        public new bool IsAlive => Health > 0 ? true : false;

        ///<inheritdoc cref=_damagesTaken"/>
        public int DamagesTaken => _damagesTaken;

        ///<inheritdoc cref="_specialEffectEnabled"/>
        public bool SpecialEffectEnabled => _specialEffectEnabled;


        ///<inheritdoc cref="Character.TakeDamage(Character, int)"/>
        protected override void TakeDamage(Character source, int damageAmount)
        {
            base.TakeDamage(source, damageAmount);

            _damagesTaken = damageAmount;
        }

        /// <summary>
        /// <inheritdoc cref="Character.SpecialAttack"/>
        /// Use the damager special attack. Inflics back damages to the source.
        /// </summary>
        public override void SpecialAttack(Character target)
        {
            _specialEffectEnabled = true;
        }

        ///<inheritdoc cref="Character.ResetEffects"/>
        public override void ResetEffects()
        {
            base.ResetEffects();

            _specialEffectEnabled = false;
            _damagesTaken = 0;
        }

        #endregion


        #region Protected API

        ///<inheritdoc cref="Character.InitStats"/>
        protected override void InitStats()
        {
            CharacterClass = CharacterClasses.Damager;

            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
        }

        #endregion
    }
}

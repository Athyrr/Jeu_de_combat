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

        #endregion

        ///<inheritdoc cref="Healer"/>
        public Healer()
        {
            InitStats();
        }

        #region Public API

        ///<inheritdoc cref="Character.IsAlive"/>
        public new bool IsAlive => _health > 0 ? true : false;


        ///<summary>
        /// <inheritdoc cref=" Character.SpecialAttack"/>
        ///Heals 2 point of health.
        ///</summary>
        public override void SpecialAttack(Character target)
        {
            Health = Math.Min(Health + 2, MaxHealth);

            Console.WriteLine("Heal !");

            string playerIndexString = IsIA ? "(player 2)" : "(player 1)";
            Console.WriteLine($"{playerIndexString} Health : " + Health);
        }

        ///<inheritdoc cref="Character.InitStats"/>
        protected override void InitStats()
        {
            CharacterClass = CharacterClasses.Healer;

            Name = _name;
            MaxHealth = _maxHealth;
            Health = _maxHealth;
            Strength = _strength;
        }

        #endregion
    }
}

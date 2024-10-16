namespace Jeu_de_combat
{
    /// <summary>
    /// Attack datas.
    /// </summary>
    public class AttackProcess
    {
        /// <summary>
        /// The attacker.
        /// </summary>
        public Character Source;
        
        /// <summary>
        /// The attacker's target
        /// </summary>
        public Character Target;

        /// <summary>
        /// Is the attack a special attack ?
        /// </summary>
        public bool IsSpecial;

        /// <summary>
        /// The amount of damages to deal.
        /// </summary>
        public int DamageAmount;


        ///<inheritdoc cref="AttackProcess"/>
        public AttackProcess(Character source, Character target, bool isSpecial, int damageAmount = 0)
        {
            Source = source;
            Target = target;
            IsSpecial = isSpecial;
            DamageAmount = damageAmount;
        }
    }
}

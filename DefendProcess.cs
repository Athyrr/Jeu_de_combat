namespace Jeu_de_combat
{
    /// <summary>
    /// Defense datas.
    /// </summary>
    public class DefendProcess
    {
        /// <summary>
        /// The character defending.
        /// </summary>
        public Character Defender;

        ///<inheritdoc cref="DefendProcess"/>
        public DefendProcess(Character defender)
        {
            Defender = defender;
        }

    }
}

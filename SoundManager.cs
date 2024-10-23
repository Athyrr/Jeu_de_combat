using WMPLib;

namespace Jeu_de_combat
{
    /// <summary>
    /// static class to manage sound and music in game
    /// </summary>
    public static class SoundManager
    {
        static string path = "Assets/";
        static string[] soundsPath = new string[]
            {
                //music
                "bg_menu.mp3",  //land with no dragons
                "bg_fight.mp3", //herding cats
                "credits.mp3",  //admin rights - full
                "win2.mp3",     //Claimed

                //sfx
                "arrow_key.mp3",    //Blip 7 - Sound effects Pack 2
                "selection.mp3",    //Blip 9 - Sound effects Pack 2
                "damager_ulti.mp3", //Explosion 5 - Sound effects Pack 2
                "fire.mp3",         //Laser-weapon 2 - Sound effects Pack 2
                "hit.mp3",          //Hit 4 - Sound effects Pack 2
                "spark.mp3",        //1up 8 - Sound effects Pack 2
                "spell.mp3",        //1up 3 - Sound effects Pack 2
                "win.mp3",          //1up 1 - Sound effects Pack 2
                "defend.flac",       //Teleport 2 - Sound effects Pack 2
                "defend_break.mp3"  //Teleport 8 - Sound effects Pack 2
            };

        private static Dictionary<string, WindowsMediaPlayer> sounds = new();
        private static List<string> loops = new();

        /// <summary>
        /// Load all sounds in a dictionnary. It must be called at the beginning of the game.
        /// </summary>
        public static void Init()
        {
            foreach (var file in soundsPath)
            {
                sounds[file] = new WindowsMediaPlayer();
                sounds[file].URL = path + file;
                sounds[file].controls.stop();
            }
            sounds["arrow_key.mp3"].settings.volume = 30;
            sounds["selection.mp3"].settings.volume = 30;

        }

        /// <summary>
        /// Play a sound
        /// </summary>
        /// <param name="sound">file name of the sound</param>
        /// <param name="loop">to make the sound loop. False by default</param>
        public static void Play(string sound, bool loop = false)
        {
            sounds[sound].controls.play();
            if (loop)
            {
                sounds[sound].settings.setMode("loop", true);
                loops.Add(sound);
            }
        }

        /// <summary>
        /// stop a sound
        /// </summary>
        /// <param name="sound">file name of the sound</param>
        public static void Stop(string sound)
        {
            sounds[sound].controls.stop();
        }

        /// <summary>
        /// return if the sound is playing or not
        /// </summary>
        /// <returns></returns>
        public static bool IsSoundRunning(string sound)
        {
            return sounds[sound].playState == WMPPlayState.wmppsPlaying;
        }

        /// <summary>
        /// Stop all looping sounds
        /// </summary>
        public static void StopAllLoops()
        {
            foreach(var sound in loops)
            {
                Stop(sound);
            }
            loops.Clear();
        }
    }
}

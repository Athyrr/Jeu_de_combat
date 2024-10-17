﻿using WMPLib;

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

                //sfx
                "arrow_key.mp3",    //Blip 7 - Sound effects Pack 2
                "selection.mp3",    //Blip 9 - Sound effects Pack 2
                "damager_ulti.mp3", //Explosion 5 - Sound effects Pack 2
                "fire.mp3",         //Laser-weapon 2 - Sound effects Pack 2
                "hit.mp3",          //Hit 4 - Sound effects Pack 2
                "lose.mp3",         //Lose 6 - Sound effects Pack 2
                "spark.mp3",        //1up 8 - Sound effects Pack 2
                "spell.mp3",        //1up 3 - Sound effects Pack 2
                "win.mp3"           //1up 1 - Sound effects Pack 2
            };

        static Dictionary<string, WindowsMediaPlayer> sounds = new();

        /// <summary>
        /// Load all sounds in a dictionnary. It must be called at the beginning of the game.
        /// </summary>
        public static void InitSounds()
        {
            foreach (var file in soundsPath)
            {
                sounds[file] = new WindowsMediaPlayer();
                sounds[file].URL = path + file;
                sounds[file].controls.stop();
            }
        }

        /// <summary>
        /// Play a sound
        /// </summary>
        /// <param name="sound">file name of the sound</param>
        /// <param name="loop">to make the sound loop. False by default</param>
        public static void Play(string sound, bool loop=false)
        {
            sounds[sound].controls.play();
            if (loop)
                sounds[sound].settings.setMode("loop", true);
        }

        /// <summary>
        /// stop a sound
        /// </summary>
        /// <param name="sound">file name of the sound</param>
        public static void Stop(string sound)
        {
            sounds[sound].controls.stop();
        }
    }
}
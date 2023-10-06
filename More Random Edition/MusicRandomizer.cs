using StardewValley;
using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Randomizes the music in the game
	/// </summary>
	public class MusicRandomizer
	{
		/// <summary>
		/// The dictionary of music replacements
		/// </summary>
		public static Dictionary<string, string> MusicReplacements { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// The list of songs
		/// </summary>
		public static List<string> MusicList = new List<string>
			{
				"50s",
				"AbigailFlute",
				"AbigailFluteDuet",
				"aerobics",
                "archaeo",
				"babblingBrook",
				"bigDrums",
				"breezy",
				"bugLevelLoop",
                "caldera",
				"Cavern",
				"christmasTheme",
				"Cloth",
				"CloudCountry",
				"clubloop",
				"communityCenter",
				"cowboy_boss",
				"cowboy_outlawsong",
				"Cowboy_OVERWORLD",
				"Cowboy_singing",
				"Cowboy_undead",
				"cracklingFire",
				"crane_game",
				"crane_game_fast",
				"Crystal Bells",
                "Cyclops",
				"darkCaveLoop",
				"desolate",
				"distantBanjo",
				"echos",
				"elliottPiano",
				"EmilyDance",
				"EmilyDream",
				"EmilyTheme",
				"end_credits",
				"event1",
				"event2",
				"fall_day_ambient",
				"fall1",
				"fall2",
				"fall3",
				"fallFest",
				"fieldofficeTentMusic",
				"FlowerDance",
                "FrogCave",
				"Frost_Ambient",
                "Ghost Synth",
				"grandpas_theme",
				"gusviolin",
				"harveys_theme_jazz",
				"heavy",
				"heavyEngine",
				"honkytonky",
				"Hospital_Ambient",
				"Icicles",
                "IslandMusic",
				"jaunty",
				"jojaOfficeSoundscape",
				"jungle_ambience",
				"junimoKart",
				"junimoKart_ghostMusic",
				"junimoKart_mushroomMusic",
				"junimoKart_slimeMusic",
				"junimoKart_whaleMusic",
				"junimoStarSong",
				"kindadumbautumn",
				"Lava_Ambient",
				"libraryTheme",
				"MainTheme",
				"Majestic",
				"MarlonsTheme",
				"marnieShop",
				"mermaidSong",
				"moonlightJellies",
				"movie_classic",
				"movie_nature",
				"movie_wumbus",
				"movieScreenAmbience",
				"movieTheater",
				"movieTheaterAfter",
				"musicboxsong",
				"Near The Planet Core",
				"New Snow",
				"night_market",
				"nightTime",
				"ocean",
				"Of Dwarves",
				"Orange",
				"Overcast",
				"Pink Petals",
				"PIRATE_THEME",
				"PIRATE_THEME(muffled)",
				"playful",
				"Plums",
				"pool_ambient",
				"poppy",
				"ragtime",
				"rain",
				"roadnoise",
				"sad_kid",
				"sadpiano",
				"Saloon1",
				"sam_acoustic1",
				"sam_acoustic2",
				"sampractice",
				"sappypiano",
				"Secret Gnomes",
				"SettlingIn",
				"shaneTheme",
				"shimmeringbastion",
				"spaceMusic",
				"spirits_eve",
				"spring_day_ambient",
				"spring_night_ambient",
				"spring1",
				"spring2",
				"spring3",
				"springtown",
				"Stadium_ambient",
				"starshoot",
				"submarine_song",
				"summer_day_ambient",
				"summer1",
				"summer2",
				"summer3",
				"SunRoom",
				"sweet",
				"tickTock",
				"tinymusicbox",
				"title_night",
				"tribal",
				"tropical_island_day_ambient",
				"Tropical Jam",
				"Upper_Ambient",
                "Volcano_Ambient",
                "VolcanoMines",
				"VolcanoMines1",
				"VolcanoMines2",
				"wavy",
				"wedding",
				"wind",
				"winter_day_ambient",
				"winter1",
				"winter2",
				"winter3",
				"WizardSong",
				"woodsTheme",
				"XOR",
			};

		/// <summary>
		/// The last song that played/is playing
		/// </summary>
		private static string _lastCurrentSong { get; set; }

		/// <summary>
		/// Randomizes all the music to another song
		/// </summary>
		/// <returns>A dictionary of song names to their alternatives</returns>
		public static void Randomize()
		{
			List<string> musicReplacementPool = new List<string>(MusicList);
			MusicReplacements = new Dictionary<string, string>();
			_lastCurrentSong = "";

			foreach (string song in MusicList)
			{
				string replacementSong = Globals.RNGGetAndRemoveRandomValueFromList(musicReplacementPool);
				MusicReplacements.Add(song, replacementSong);
			}

			WriteToSpoilerLog();
		}

		/// <summary>
		/// Attempts to replace the current song with a different one
		/// If the song was barely replaced, it doesn't do anything
		/// </summary>
		public static void TryReplaceSong()
		{
			string currentSong = Game1.currentSong?.Name;
			if (_lastCurrentSong == currentSong) { return; }

			string newSongToPlay = Globals.Config.Music.RandomSongEachTransition ? GetRandomSong() : GetMappedSong(currentSong);
			if (!string.IsNullOrWhiteSpace(newSongToPlay))
			{
				_lastCurrentSong = newSongToPlay;
				Game1.changeMusicTrack(newSongToPlay);

				if (Globals.Config.Music.HUD)
				{
					Game1.addHUDMessage(new HUDMessage($"Song: {currentSong} | Replaced with: {newSongToPlay}", null));
				}
			}
		}

		/// <summary>
		/// Gets the song that's mapped to the given song
		/// </summary>
		/// <param name="currentSong">The song to look up</param>
		/// <returns />
		private static string GetMappedSong(string currentSong)
		{
			if (MusicReplacements.TryGetValue(currentSong ?? "", out string value))
			{
				return value;
			}
			return string.Empty;
		}

		/// <summary>
		/// Gets a random song
		/// </summary>
		/// <returns />
		private static string GetRandomSong()
		{
			return Globals.RNGGetRandomValueFromList(MusicList, true);
		}

		/// <summary>
		/// Writes the music info to the spoiler log
		/// </summary>
		/// <param name="musicList">The music replacement list</param>
		private static void WriteToSpoilerLog()
		{
			if (!Globals.Config.Music.Randomize || Globals.Config.Music.RandomSongEachTransition) { return; }

			Globals.SpoilerWrite("==== MUSIC ====");
			foreach (string song in MusicReplacements.Keys)
			{
				Globals.SpoilerWrite($"{song} is now {MusicReplacements[song]}");
			}

			Globals.SpoilerWrite("---");
			Globals.SpoilerWrite("");
		}
	}
}
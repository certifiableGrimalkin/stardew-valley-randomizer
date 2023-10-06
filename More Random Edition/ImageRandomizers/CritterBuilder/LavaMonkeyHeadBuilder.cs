using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewModdingAPI.Events;

namespace Randomizer
{
    public class LavaMonkeyHeadBuilder : ImageBuilder
    {
        private const string LavaMonkeyHeadDirectory = "Lava Monkey Head";

        /// <summary>
        /// Critter names and the number of sprites they have.
        /// </summary>
        private static readonly Dictionary<string, int> BaseCritterSprites = new Dictionary<string, int>
        {
            {"Lava Monkey Head", 3},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterPaths = new Dictionary<string, string>
        {
            {"Lava Monkey Head", "Lava Monkey Head"},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterNames = new Dictionary<string, string>
        {
            {"Lava Monkey Head", "Lava Monkey Head"},
        };

        private List<string> LavaMonkeyHeadImages { get; set; }
        private Dictionary<string, string> _replacements { get; set; }

        /// <summary>
        /// A map of the critter position in the dictionary to the id it belongs to
        /// </summary>
        private Dictionary<Point, Critter> PointsToCritterMap;

        public LavaMonkeyHeadBuilder() : base()
        {
            BaseFileName = "BigSmallCrabMonkeyScream-temp.png";
            OutputFileName = "BigSmallCrabMonkeyScreamHead-temp.png";
            SubDirectory = "Critter";
            SetUpPointsToCritterMap();
            PositionsToOverlay = PointsToCritterMap.Keys.ToList();
            ///foreach (var point in PointsToCritterMap)
            ///{
            ///    Globals.ConsoleWarn($"{point.Key}: {point.Value.Name} {point.Value.SpriteNumber}");
            ///}

            ImageHeightInPx = 12;
            ImageWidthInPx = 14;
            OffsetHeightInPx = 12;
            OffsetWidthInPx = 14;
            InitialHeightOffsetInPx = 333;

            LavaMonkeyHeadImages = Directory.GetFiles(Path.Combine(ImageDirectory,LavaMonkeyHeadDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ValidateImages();

            _replacements = BuildReplacements();
            WriteToSpoilerLog(_replacements);
        }

        /// <summary>
        /// Sets up the map to link tilesheet points to their critter sprites.
        /// </summary>
        private void SetUpPointsToCritterMap()
		{
			PointsToCritterMap = new Dictionary<Point, Critter>();
            var ItemsInRow = 3;
            foreach (int y in Enumerable.Range(0,1))
            {
                foreach (int x in Enumerable.Range(0, ItemsInRow))
                {
                    if (y * ItemsInRow + x < 3)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Lava Monkey Head", y * ItemsInRow + x + 1));
                    }
                }
            }
		}


        /// <summary>
        /// Writes the critters to the spoiler log
        /// </summary>
        /// <param name="critterReplacements">Dictionary of vanilla critters, and what they're changed to.</param>
        private static void WriteToSpoilerLog(Dictionary<string,string> critterReplacements)
        {
            if (!Globals.Config.Critter.Randomize) { return; }

            Globals.SpoilerWrite("==== Lava Monkey Head ====");
            foreach (KeyValuePair<string,string> crit in critterReplacements)
            {
                Globals.SpoilerWrite($"{crit.Key} - {Path.GetFileName(crit.Value)}");
            }
            Globals.SpoilerWrite("");
        }

        private Dictionary<string, string> BuildReplacements()
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();
            if (!Globals.Config.Critter.Randomize)
            {
                foreach (KeyValuePair<string,int> crit in BaseCritterSprites)
                {
                    foreach (int count in Enumerable.Range(1, crit.Value))
                    {
                        string temp_file_with_ext = $"{BaseCritterNames[crit.Key]}_{count}.png";
                        replacements.Add($"{crit.Key} {count}", Path.Combine(ImageDirectory,BaseCritterPaths[crit.Key],temp_file_with_ext));
                    }
                }
            }
            else if (!Globals.Config.Critter.FrameMayhem)
            {
                foreach (KeyValuePair<string, int> crit in BaseCritterSprites)
                {
                    switch(BaseCritterPaths[crit.Key])
                    {
                        case "Lava Monkey Head":
                            string lavaMonkeyHeadrep = Globals.RNGGetAndRemoveRandomValueFromList(LavaMonkeyHeadImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{lavaMonkeyHeadrep}_{count}.png");
                            }
                            break;
                        default:
                            Globals.ConsoleError($"No critter type for: {crit.Key}.");
                            break;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> crit in BaseCritterSprites)
                {
                    switch (BaseCritterPaths[crit.Key])
                    {
                        case "Lava Monkey Head":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(LavaMonkeyHeadImages)}_{count}.png");
                            }
                            break;
                        default:
                            Globals.ConsoleError($"No critter type for: {crit.Key}.");
                            break;
                    }
                }

            }
            return replacements;
        }


		/// <summary>
		/// Gets a random file name that matches the critter name at the given position
		/// Will remove the name found from the list
		/// </summary>
		/// <param name="position">The position</param>
		/// <returns>The selected file name</returns>
		protected override string GetRandomFileName(Point position)
        {
            string fileName = "";
            fileName = _replacements[$"{GetCritterTypeFromPosition(position)} {GetCritterSpriteNumberFromPosition(position)}"];

            if (string.IsNullOrEmpty(fileName))
            {
                Globals.ConsoleWarn($"Using default image for critter at image position - you may not have enough critter images: {position.X}, {position.Y}");
                return null;
            }
            return fileName;
        }

        /// <summary>
        /// Gets the name of the critter at that position.
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The selected critter name</returns>
        private string GetCritterTypeFromPosition(Point position)
        {
            return PointsToCritterMap[position].Name;
        }

        /// <summary>
        /// Gets the number of the sprite of the critter at that position.
        /// </summary>
        /// <param name="position">The position</param>
        /// <returns>The selected critter's sprite number.</returns>
        private int GetCritterSpriteNumberFromPosition(Point position)
        {
            return PointsToCritterMap[position].SpriteNumber;
        }

        /// <summary>
        /// Whether the settings premit random critter images
        /// </summary>
        /// <returns>True if so, false otherwise</returns>
        public override bool ShouldSaveImage()
		{
            return Globals.Config.Critter.Randomize;
		}

		/// <summary>
		/// Validates that all the potentially needed bundle images exist
		/// </summary>
		private void ValidateImages()
		{
            // Check that all critter types have the correct number of sprites
            string LavaMonkeyHeadImageDirectory = $"{CustomImagesPath}/Critter/Lava Monkey Head";

            List<string> LavaMonkeyHeadImageNames = Directory.GetFiles(LavaMonkeyHeadImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();

            foreach (string LavaMonkeyHeadImageName in LavaMonkeyHeadImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Lava Monkey Head"]))
                    if (!LavaMonkeyHeadImageNames.Contains($"{Path.GetFileName(LavaMonkeyHeadImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(LavaMonkeyHeadImageName)}_{count}.png not found at: {LavaMonkeyHeadImageDirectory}");
                    }
            }
        }
	}
}

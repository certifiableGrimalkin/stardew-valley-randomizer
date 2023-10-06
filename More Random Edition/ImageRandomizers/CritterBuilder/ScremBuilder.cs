using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewModdingAPI.Events;

namespace Randomizer
{
    public class ScremBuilder : ImageBuilder
    {
        private const string ScremDirectory = "Island Scream Animal";

        /// <summary>
        /// Critter names and the number of sprites they have.
        /// </summary>
        private static readonly Dictionary<string, int> BaseCritterSprites = new Dictionary<string, int>
        {
            {"Island Scream Animal", 4},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterPaths = new Dictionary<string, string>
        {
            {"Island Scream Animal", "Island Scream Animal"},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterNames = new Dictionary<string, string>
        {
            {"Island Scream Animal", "Island Scream"},
        };

        private List<string> ScremImages { get; set; }
        private Dictionary<string, string> _replacements { get; set; }

        /// <summary>
        /// A map of the critter position in the dictionary to the id it belongs to
        /// </summary>
        private Dictionary<Point, Critter> PointsToCritterMap;

        public ScremBuilder() : base()
        {
            BaseFileName = "BigSmallCrabMonkey-temp.png";
            OutputFileName = "BigSmallCrabMonkeyScream-temp.png";
            SubDirectory = "Critter";
            SetUpPointsToCritterMap();
            PositionsToOverlay = PointsToCritterMap.Keys.ToList();
            ///foreach (var point in PointsToCritterMap)
            ///{
            ///    Globals.ConsoleWarn($"{point.Key}: {point.Value.Name} {point.Value.SpriteNumber}");
            ///}

            ImageHeightInPx = 25;
            ImageWidthInPx = 20;
            OffsetHeightInPx = 25;
            OffsetWidthInPx = 20;
            InitialHeightOffsetInPx = 308;
            InitialWidthOffsetInPx = 141;

            ScremImages = Directory.GetFiles(Path.Combine(ImageDirectory,ScremDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", ""))
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
            var ItemsInRow = 4;
            foreach (int y in Enumerable.Range(0,1))
            {
                foreach (int x in Enumerable.Range(0, ItemsInRow))
                {
                    if (y * ItemsInRow + x < 4)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Island Scream Animal", y * ItemsInRow + x + 1));
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

            Globals.SpoilerWrite("==== Island Scream Animal ====");
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
                        case "Island Scream Animal":
                            string lavaMonkeyrep = Globals.RNGGetAndRemoveRandomValueFromList(ScremImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{lavaMonkeyrep}_{count}.png");
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
                        case "Island Scream Animal":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(ScremImages)}_{count}.png");
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
            string ScremImageDirectory = $"{CustomImagesPath}/Critter/Island Scream Animal";

            List<string> ScremImageNames = Directory.GetFiles(ScremImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();

            foreach (string ScremImageName in ScremImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Island Scream Animal"]))
                    if (!ScremImageNames.Contains($"{Path.GetFileName(ScremImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(ScremImageName)}_{count}.png not found at: {ScremImageDirectory}");
                    }
            }
        }
	}
}

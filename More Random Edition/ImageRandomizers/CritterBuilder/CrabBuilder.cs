using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewModdingAPI.Events;

namespace Randomizer
{
    public class CrabBuilder : ImageBuilder
    {
        private const string CrabDirectory = "Crab";

        /// <summary>
        /// Critter names and the number of sprites they have.
        /// </summary>
        private static readonly Dictionary<string, int> BaseCritterSprites = new Dictionary<string, int>
        {
            {"Crab", 6},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterPaths = new Dictionary<string, string>
        {
            {"Crab", "Crab"},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterNames = new Dictionary<string, string>
        {
            {"Crab", "Crab"},
        };

        private List<string> CrabImages { get; set; }
        private Dictionary<string, string> _replacements { get; set; }

        /// <summary>
        /// A map of the critter position in the dictionary to the id it belongs to
        /// </summary>
        private Dictionary<Point, Critter> PointsToCritterMap;

        public CrabBuilder() : base()
        {
            BaseFileName = "BigSmall-temp.png";
            OutputFileName = "BigSmallCrab-temp.png";
            SubDirectory = "Critter";
            SetUpPointsToCritterMap();
            PositionsToOverlay = PointsToCritterMap.Keys.ToList();
            ///foreach (var point in PointsToCritterMap)
            ///{
            ///    Globals.ConsoleWarn($"{point.Key}: {point.Value.Name} {point.Value.SpriteNumber}");
            ///}

            ImageHeightInPx = 18;
            ImageWidthInPx = 18;
            OffsetHeightInPx = 18;
            OffsetWidthInPx = 18;
            InitialHeightOffsetInPx = 272;

            CrabImages = Directory.GetFiles(Path.Combine(ImageDirectory,CrabDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", ""))
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
            foreach (int y in Enumerable.Range(0,2))
            {
                foreach (int x in Enumerable.Range(0, ItemsInRow))
                {
                    if (y * ItemsInRow + x < 6)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Crab", y * ItemsInRow + x + 1));
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

            Globals.SpoilerWrite("==== Crab ====");
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
                        case "Crab":
                            string crabrep = Globals.RNGGetAndRemoveRandomValueFromList(CrabImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{crabrep}_{count}.png");
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
                        case "Crab":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(CrabImages)}_{count}.png");
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
            string CrabImageDirectory = $"{CustomImagesPath}/Critter/Crab";

            List<string> CrabImageNames = Directory.GetFiles(CrabImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();

            foreach (string CrabImageName in CrabImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Crab"]))
                    if (!CrabImageNames.Contains($"{Path.GetFileName(CrabImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(CrabImageName)}_{count}.png not found at: {CrabImageDirectory}");
                    }
            }
        }
	}
}

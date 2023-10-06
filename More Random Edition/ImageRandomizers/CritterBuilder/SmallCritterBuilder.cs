using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using StardewModdingAPI.Events;

namespace Randomizer
{
    public class SmallCritterBuilder : ImageBuilder
    {
        private const string ButterflyDirectory = "Butterfly";
        private const string SmallButterflyDirectory = "Small Butterfly";
        private const string FrogDirectory = "Frog";
        private const string WoodpeckerDirectory = "Woodpecker";

        /// <summary>
        /// Critter names and the number of sprites they have.
        /// </summary>
        private static readonly Dictionary<string, int> BaseCritterSprites = new Dictionary<string, int>
        {
            {"Blue Butterfly", 4},
            {"Red Butterfly", 4},
            {"Yellow Butterfly", 4},
            {"Green Butterfly", 4},
            {"Pink Butterfly", 4},
            {"Orange Butterfly", 4},
            {"White Small Butterfly", 3},
            {"Beige Tiny Butterfly", 3},
            {"Lavender X Butterfly", 3},
            {"Pink Small Butterfly", 3},
            {"Yellow Tiny Butterfly", 3},
            {"Pink X Butterfly", 3},
            {"Grass Frog", 7},
            {"Pond Frog", 7},
            {"Woodpecker", 5},
            {"Island Yellow Butterfly", 4},
            {"Island Pink Butterfly", 4},
            {"Island Lime Butterfly", 4},
            {"Island Red Butterfly", 4},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterPaths = new Dictionary<string, string>
        {
            {"Blue Butterfly", "Butterfly"},
            {"Red Butterfly", "Butterfly"},
            {"Yellow Butterfly", "Butterfly"},
            {"Green Butterfly", "Butterfly"},
            {"Pink Butterfly", "Butterfly"},
            {"Orange Butterfly", "Butterfly"},
            {"White Small Butterfly", "Small Butterfly"},
            {"Beige Tiny Butterfly", "Small Butterfly"},
            {"Lavender X Butterfly", "Small Butterfly"},
            {"Pink Small Butterfly", "Small Butterfly"},
            {"Yellow Tiny Butterfly", "Small Butterfly"},
            {"Pink X Butterfly", "Small Butterfly"},
            {"Grass Frog", "Frog"},
            {"Pond Frog", "Frog"},
            {"Woodpecker", "Woodpecker"},
            {"Island Yellow Butterfly", "Butterfly"},
            {"Island Pink Butterfly", "Butterfly"},
            {"Island Lime Butterfly", "Butterfly"},
            {"Island Red Butterfly", "Butterfly"},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterNames = new Dictionary<string, string>
        {
            {"Blue Butterfly", "Blue"},
            {"Red Butterfly", "Red"},
            {"Yellow Butterfly", "Yellow"},
            {"Green Butterfly", "Green"},
            {"Pink Butterfly", "Pink"},
            {"Orange Butterfly", "Orange"},
            {"White Small Butterfly", "Small White"},
            {"Beige Tiny Butterfly", "Tiny Beige"},
            {"Lavender X Butterfly", "X Lavender"},
            {"Pink Small Butterfly", "Small Pink"},
            {"Yellow Tiny Butterfly", "Tiny Yellow"},
            {"Pink X Butterfly", "X Pink"},
            {"Grass Frog", "Grass Frog"},
            {"Pond Frog", "Pond Frog"},
            {"Woodpecker", "Woodpecker"},
            {"Island Yellow Butterfly", "Island Yellow"},
            {"Island Pink Butterfly", "Island Pink"},
            {"Island Lime Butterfly", "Island Lime"},
            {"Island Red Butterfly", "Island Red"},
        };

        private List<string> ButterflyImages { get; set; }
        private List<string> SmallButterflyImages { get; set; }
        private List<string> FrogImages { get; set; }
        private List<string> WoodpeckerImages { get; set; }
        private Dictionary<string, string> _replacements { get; set; }

        /// <summary>
        /// A map of the critter position in the dictionary to the id it belongs to
        /// </summary>
        private Dictionary<Point, Critter> PointsToCritterMap;

        public SmallCritterBuilder() : base()
        {
            BaseFileName = "Big-temp.png";
            OutputFileName = "BigSmall-temp.png";
            SubDirectory = "Critter";
            SetUpPointsToCritterMap();
            PositionsToOverlay = PointsToCritterMap.Keys.ToList();
            ///foreach (var point in PointsToCritterMap)
            ///{
            ///    Globals.ConsoleWarn($"{point.Key}: {point.Value.Name} {point.Value.SpriteNumber}");
            ///}

            ImageHeightInPx = 16;
            ImageWidthInPx = 16;
            OffsetHeightInPx = 16;
            OffsetWidthInPx = 16;

            ButterflyImages = Directory.GetFiles(Path.Combine(ImageDirectory,ButterflyDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            SmallButterflyImages = Directory.GetFiles(Path.Combine(ImageDirectory,SmallButterflyDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            FrogImages = Directory.GetFiles(Path.Combine(ImageDirectory,FrogDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png")
                         || x.EndsWith("_7.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", "").Replace("_7.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            WoodpeckerImages = Directory.GetFiles(Path.Combine(ImageDirectory,WoodpeckerDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", ""))
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
            var ItemsInRow = 20;
            foreach (int y in Enumerable.Range(0,32))
            {
                foreach (int x in Enumerable.Range(0, ItemsInRow))
                {
                    if (y * ItemsInRow + x < 128) { } // Big Critters.
                    else if (y * ItemsInRow + x < 132)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Blue Butterfly", y * ItemsInRow + x + 1 - 128));
                    }
                    else if (y * ItemsInRow + x < 136)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Red Butterfly", y * ItemsInRow + x + 1 - 132));
                    }
                    else if (y * ItemsInRow + x < 140)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Yellow Butterfly", y * ItemsInRow + x + 1 - 136));
                    }
                    else if (y * ItemsInRow + x < 148){ } // Brown Bird inbetween.
                    else if (y * ItemsInRow + x < 152)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Green Butterfly", y * ItemsInRow + x + 1 - 148));
                    }
                    else if (y * ItemsInRow + x < 156)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Pink Butterfly", y * ItemsInRow + x + 1 - 152));
                    }
                    else if (y * ItemsInRow + x < 160)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Orange Butterfly", y * ItemsInRow + x + 1 - 156));
                    }
                    else if (y * ItemsInRow + x < 163)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("White Small Butterfly", y * ItemsInRow + x + 1 - 160));
                    }
                    else if (y * ItemsInRow + x < 166)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Beige Tiny Butterfly", y * ItemsInRow + x + 1 - 163));
                    }
                    else if (y * ItemsInRow + x < 169)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Lavender X Butterfly", y * ItemsInRow + x + 1 - 166));
                    }
                    else if (y * ItemsInRow + x < 180) { } // Blue Bird inbetween.
                    else if (y * ItemsInRow + x < 183)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Pink Small Butterfly", y * ItemsInRow + x + 1 - 180));
                    }
                    else if (y * ItemsInRow + x < 186)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Yellow Tiny Butterfly", y * ItemsInRow + x + 1 - 183));
                    }
                    else if (y * ItemsInRow + x < 189)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Pink X Butterfly", y * ItemsInRow + x + 1 - 186));
                    }
                    else if (y * ItemsInRow + x < 280) { } // Big Critters inbetween.
                    else if (y * ItemsInRow + x < 287)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Grass Frog", y * ItemsInRow + x + 1 - 280));
                    }
                    else if (y * ItemsInRow + x < 300) { } // White Bunny inbetween.
                    else if (y * ItemsInRow + x < 307)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Pond Frog", y * ItemsInRow + x + 1 - 300));
                    }
                    else if (y * ItemsInRow + x < 320) { } // White Bunny inbetween.
                    else if (y * ItemsInRow + x < 325)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Woodpecker", y * ItemsInRow + x + 1 - 320));
                    }
                    else if (y * ItemsInRow + x < 364) { } // Owl and Crab inbetween.
                    else if (y * ItemsInRow + x < 368)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Island Yellow Butterfly", y * ItemsInRow + x + 1 - 364));
                    }
                    else if (y * ItemsInRow + x < 372)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Island Pink Butterfly", y * ItemsInRow + x + 1 - 368));
                    }
                    else if (y * ItemsInRow + x < 376)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Island Lime Butterfly", y * ItemsInRow + x + 1 - 372));
                    }
                    else if (y * ItemsInRow + x < 380)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Island Red Butterfly", y * ItemsInRow + x + 1 - 376));
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

            Globals.SpoilerWrite("==== Small Critter ====");
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
                        case "Butterfly":
                            string butterflyrep = Globals.RNGGetAndRemoveRandomValueFromList(ButterflyImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{butterflyrep}_{count}.png");
                            }
                            break;
                        case "Small Butterfly":
                            string smallbutterflyrep = Globals.RNGGetAndRemoveRandomValueFromList(SmallButterflyImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{smallbutterflyrep}_{count}.png");
                            }
                            break;
                        case "Frog":
                            string frogrep = Globals.RNGGetAndRemoveRandomValueFromList(FrogImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{frogrep}_{count}.png");
                            }
                            break;
                        case "Woodpecker":
                            string woodpeckerrep = Globals.RNGGetAndRemoveRandomValueFromList(WoodpeckerImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{woodpeckerrep}_{count}.png");
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
                        case "Butterfly":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(ButterflyImages)}_{count}.png");
                            }
                            break;
                        case "Small Butterfly":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(SmallButterflyImages)}_{count}.png");
                            }
                            break;
                        case "Frog":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(FrogImages)}_{count}.png");
                            }
                            break;
                        case "Woodpecker":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(WoodpeckerImages)}_{count}.png");
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
            string ButterflyImageDirectory = $"{CustomImagesPath}/Critter/Butterfly";
            string SmallButterflyImageDirectory = $"{CustomImagesPath}/Critter/Small Butterfly";
            string FrogImageDirectory = $"{CustomImagesPath}/Critter/Frog";
            string WoodpeckerImageDirectory = $"{CustomImagesPath}/Critter/Woodpecker";

            List<string> ButterflyImageNames = Directory.GetFiles(ButterflyImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> SmallButterflyImageNames = Directory.GetFiles(SmallButterflyImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> FrogImageNames = Directory.GetFiles(FrogImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> WoodpeckerImageNames = Directory.GetFiles(WoodpeckerImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();

            foreach (string ButterflyImageName in ButterflyImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Blue Butterfly"]))
                    if (!ButterflyImageNames.Contains($"{Path.GetFileName(ButterflyImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(ButterflyImageName)}_{count}.png not found at: {ButterflyImageDirectory}");
                    }
            }
            foreach (string SmallButterflyImageName in SmallButterflyImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["White Small Butterfly"]))
                    if (!SmallButterflyImageNames.Contains($"{Path.GetFileName(SmallButterflyImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(SmallButterflyImageName)}_{count}.png not found at: {SmallButterflyImageDirectory}");
                    }
            }
            foreach (string FrogImageName in FrogImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Grass Frog"]))
                    if (!FrogImageNames.Contains($"{Path.GetFileName(FrogImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(FrogImageName)}_{count}.png not found at: {FrogImageDirectory}");
                    }
            }
            foreach (string WoodpeckerImageName in WoodpeckerImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Woodpecker"]))
                    if (!WoodpeckerImageNames.Contains($"{Path.GetFileName(WoodpeckerImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(WoodpeckerImageName)}_{count}.png not found at: {WoodpeckerImageDirectory}");
                    }
            }
        }
	}
}

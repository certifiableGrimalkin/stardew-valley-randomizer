using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;

namespace Randomizer
{
    public class BigCritterBuilder : ImageBuilder
    {
        private const string SeagullDirectory = "Seagull";
        private const string CrowDirectory = "Crow";
        private const string BirdDirectory = "Bird";
        private const string RabbitDirectory = "Rabbit";
        private const string SquirrelDirectory = "Squirrel";
        private const string OwlDirectory = "Owl";
        private const string GorillaDirectory = "Gorilla";
        /// <summary>
        /// Critter names and the number of sprites they have.
        /// </summary>
        private static readonly Dictionary<string, int> BaseCritterSprites = new Dictionary<string, int>
        {
            {"Seagull", 14},
            {"Crow", 11},
            {"Brown Bird", 9},
            {"Blue Bird", 9},
            {"Grey Rabbit", 7},
            {"Squirrel", 8},
            {"White Rabbit", 7},
            {"Owl", 4},
            {"Gorilla", 7},
            {"Purple Bird", 9},
            {"Red Bird", 9},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterPaths = new Dictionary<string, string>
        {
            {"Seagull", "Seagull"},
            {"Crow", "Crow"},
            {"Brown Bird", "Bird"},
            {"Blue Bird", "Bird"},
            {"Grey Rabbit", "Rabbit"},
            {"Squirrel", "Squirrel"},
            {"White Rabbit", "Rabbit"},
            {"Owl", "Owl"},
            {"Gorilla", "Gorilla"},
            {"Purple Bird", "Bird"},
            {"Red Bird", "Bird"},
        };
        /// <summary>
        /// Default paths.
        /// </summary>
        private static readonly Dictionary<string, string> BaseCritterNames = new Dictionary<string, string>
        {
            {"Seagull", "Seagull"},
            {"Crow", "Crow"},
            {"Brown Bird", "Brown"},
            {"Blue Bird", "Blue"},
            {"Grey Rabbit", "Grey"},
            {"Squirrel", "Squirrel"},
            {"White Rabbit", "White"},
            {"Owl", "Owl"},
            {"Gorilla", "Gorilla"},
            {"Purple Bird", "Purple"},
            {"Red Bird", "Red"},
        };

        private List<string> SeagullImages { get; set; }
        private List<string> CrowImages { get; set; }
        private List<string> BirdImages { get; set; }
        private List<string> RabbitImages { get; set; }
        private List<string> SquirrelImages { get; set; }
        private List<string> OwlImages { get; set; }
        private List<string> GorillaImages { get; set; }
        private Dictionary<string, string> _replacements { get; set; }

        /// <summary>
        /// A map of the critter position in the dictionary to the id it belongs to
        /// </summary>
        private Dictionary<Point, Critter> PointsToCritterMap;

        public BigCritterBuilder() : base()
        {
            BaseFileName = "critters.png";
            OutputFileName = "Big-temp.png";
            SubDirectory = "Critter";
            SetUpPointsToCritterMap();
            PositionsToOverlay = PointsToCritterMap.Keys.ToList();

            ImageHeightInPx = 32;
            ImageWidthInPx = 32;
            OffsetHeightInPx = 32;
            OffsetWidthInPx = 32;

            SeagullImages = Directory.GetFiles(Path.Combine(ImageDirectory,SeagullDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png")
                         || x.EndsWith("_7.png")
                         || x.EndsWith("_8.png")
                         || x.EndsWith("_9.png")
                         || x.EndsWith("_10.png")
                         || x.EndsWith("_11.png")
                         || x.EndsWith("_12.png")
                         || x.EndsWith("_13.png")
                         || x.EndsWith("_14.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", "").Replace("_7.png", "").Replace("_8.png", "").Replace("_9.png", "").Replace("_10.png", "").Replace("_11.png", "").Replace("_12.png", "").Replace("_13.png", "").Replace("_14.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            CrowImages = Directory.GetFiles(Path.Combine(ImageDirectory,CrowDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png")
                         || x.EndsWith("_7.png")
                         || x.EndsWith("_8.png")
                         || x.EndsWith("_9.png")
                         || x.EndsWith("_10.png")
                         || x.EndsWith("_11.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", "").Replace("_7.png", "").Replace("_8.png", "").Replace("_9.png", "").Replace("_10.png", "").Replace("_11.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            BirdImages = Directory.GetFiles(Path.Combine(ImageDirectory,BirdDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png")
                         || x.EndsWith("_7.png")
                         || x.EndsWith("_8.png")
                         || x.EndsWith("_9.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", "").Replace("_7.png", "").Replace("_8.png", "").Replace("_9.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            RabbitImages = Directory.GetFiles(Path.Combine(ImageDirectory,RabbitDirectory))
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

            SquirrelImages = Directory.GetFiles(Path.Combine(ImageDirectory,SquirrelDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png")
                         || x.EndsWith("_5.png")
                         || x.EndsWith("_6.png")
                         || x.EndsWith("_7.png")
                         || x.EndsWith("_8.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", "").Replace("_5.png", "").Replace("_6.png", "").Replace("_7.png", "").Replace("_8.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            OwlImages = Directory.GetFiles(Path.Combine(ImageDirectory,OwlDirectory))
                .Where(x => x.EndsWith("_1.png")
                         || x.EndsWith("_2.png")
                         || x.EndsWith("_3.png")
                         || x.EndsWith("_4.png"))
                .Select(x => x.Replace("_1.png", "").Replace("_2.png", "").Replace("_3.png", "").Replace("_4.png", ""))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            GorillaImages = Directory.GetFiles(Path.Combine(ImageDirectory,GorillaDirectory))
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
            var ItemsInRow = 10;
            foreach (int y in Enumerable.Range(0,16))
            {
                foreach (int x in Enumerable.Range(0, ItemsInRow))
                {
                    if (y * ItemsInRow + x < 14)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Seagull", y * ItemsInRow + x + 1));
                    }
                    else if (y * ItemsInRow + x < 25)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Crow", y * ItemsInRow + x + 1 - 14));
                    }
                    else if (y * ItemsInRow + x < 34)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Brown Bird", y * ItemsInRow + x + 1 - 25));
                    }
                    else if (y * ItemsInRow + x < 45){ } // Butterflies inbetween.
                    else if (y * ItemsInRow + x < 54)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Blue Bird", y * ItemsInRow + x + 1 - 45));
                    }
                    else if (y * ItemsInRow + x < 60)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Grey Rabbit", y * ItemsInRow + x + 1 - 54));
                    }
                    else if (y * ItemsInRow + x < 68)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Squirrel", y * ItemsInRow + x + 1 - 60));
                    }
                    else if (y * ItemsInRow + x < 69) // Grey Rabbit Last Sprite.
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Grey Rabbit", 7));
                    }
                    else if (y * ItemsInRow + x < 70) // White Rabbit Last Sprite
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("White Rabbit", 7));
                    }
                    else if (y * ItemsInRow + x < 74) { } // Frogs inbetween.
                    else if (y * ItemsInRow + x < 80)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("White Rabbit", y * ItemsInRow + x + 1 - 74));
                    }
                    else if (y * ItemsInRow + x < 83) { } // Woodpecker and Crab inbetween.
                    else if (y * ItemsInRow + x < 87)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Owl", y * ItemsInRow + x + 1 - 83));
                    }
                    else if (y * ItemsInRow + x < 110) { } // Island stuff inbetween.
                    else if (y * ItemsInRow + x < 117)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Gorilla", y * ItemsInRow + x + 1 - 110));
                    }
                    else if (y * ItemsInRow + x < 125) { } // Empty inbetween.
                    else if (y * ItemsInRow + x < 134)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Purple Bird", y * ItemsInRow + x + 1 - 125));
                    }
                    else if (y * ItemsInRow + x < 135) { } // Empty inbetween.
                    else if (y * ItemsInRow + x < 144)
                    {
                        PointsToCritterMap.Add(new Point(x, y), new Critter("Red Bird", y * ItemsInRow + x + 1 - 135));
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

            Globals.SpoilerWrite("==== Big Critter ====");
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
                        case "Seagull":
                            string seagullrep = Globals.RNGGetAndRemoveRandomValueFromList(SeagullImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{seagullrep}_{count}.png");
                            }
                            break;
                        case "Crow":
                            string crowrep = Globals.RNGGetAndRemoveRandomValueFromList(CrowImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{crowrep}_{count}.png");
                            }
                            break;
                        case "Bird":
                            string birdrep = Globals.RNGGetAndRemoveRandomValueFromList(BirdImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{birdrep}_{count}.png");
                            }
                            break;
                        case "Rabbit":
                            string rabbitrep = Globals.RNGGetAndRemoveRandomValueFromList(RabbitImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{rabbitrep}_{count}.png");
                            }
                            break;
                        case "Squirrel":
                            string squirrelrep = Globals.RNGGetAndRemoveRandomValueFromList(SquirrelImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{squirrelrep}_{count}.png");
                            }
                            break;
                        case "Owl":
                            string owlrep = Globals.RNGGetAndRemoveRandomValueFromList(OwlImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{owlrep}_{count}.png");
                            }
                            break;
                        case "Gorilla":
                            string gorillarep = Globals.RNGGetAndRemoveRandomValueFromList(GorillaImages);
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{gorillarep}_{count}.png");
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
                        case "Seagull":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(SeagullImages)}_{count}.png");
                            }
                            break;
                        case "Crow":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(CrowImages)}_{count}.png");
                            }
                            break;
                        case "Bird":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(BirdImages)}_{count}.png");
                            }
                            break;
                        case "Rabbit":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(RabbitImages)}_{count}.png");
                            }
                            break;
                        case "Squirrel":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(SquirrelImages)}_{count}.png");
                            }
                            break;
                        case "Owl":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(OwlImages)}_{count}.png");
                            }
                            break;
                        case "Gorilla":
                            foreach (int count in Enumerable.Range(1, crit.Value))
                            {
                                replacements.Add($"{crit.Key} {count}", $"{Globals.RNGGetRandomValueFromList(GorillaImages)}_{count}.png");
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
            string SeagullImageDirectory = $"{CustomImagesPath}/Critter/Seagull";
            string CrowImageDirectory = $"{CustomImagesPath}/Critter/Crow";
            string BirdImageDirectory = $"{CustomImagesPath}/Critter/Bird";
            string RabbitImageDirectory = $"{CustomImagesPath}/Critter/Rabbit";
            string SquirrelImageDirectory = $"{CustomImagesPath}/Critter/Squirrel";
            string OwlImageDirectory = $"{CustomImagesPath}/Critter/Owl";
            string GorillaImageDirectory = $"{CustomImagesPath}/Critter/Gorilla";
            List<string> SeagullImageNames = Directory.GetFiles(SeagullImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> CrowImageNames = Directory.GetFiles(CrowImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> BirdImageNames = Directory.GetFiles(BirdImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> RabbitImageNames = Directory.GetFiles(RabbitImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> SquirrelImageNames = Directory.GetFiles(SquirrelImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> OwlImageNames = Directory.GetFiles(OwlImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();
            List<string> GorillaImageNames = Directory.GetFiles(GorillaImageDirectory)
                .Where(x => x.EndsWith(".png"))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();


            foreach (string SeagullImageName in SeagullImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Seagull"]))
                    if (!SeagullImageNames.Contains($"{Path.GetFileName(SeagullImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(SeagullImageName)}_{count}.png not found at: {SeagullImageDirectory}");
                    }
            }
            foreach (string CrowImageName in CrowImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Crow"]))
                    if (!CrowImageNames.Contains($"{Path.GetFileName(CrowImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(CrowImageName)}_{count}.png not found at: {CrowImageDirectory}");
                    }
            }
            foreach (string BirdImageName in BirdImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Brown Bird"]))
                    if (!BirdImageNames.Contains($"{Path.GetFileName(BirdImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(BirdImageName)}_{count}.png not found at: {BirdImageDirectory}");
                    }
            }
            foreach (string RabbitImageName in RabbitImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Grey Rabbit"]))
                    if (!RabbitImageNames.Contains($"{Path.GetFileName(RabbitImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(RabbitImageName)}_{count}.png not found at: {RabbitImageDirectory}");
                    }
            }
            foreach (string SquirrelImageName in SquirrelImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Squirrel"]))
                    if (!SquirrelImageNames.Contains($"{Path.GetFileName(SquirrelImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(SquirrelImageName)}_{count}.png not found at: {SquirrelImageDirectory}");
                    }
            }
            foreach (string OwlImageName in OwlImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Owl"]))
                    if (!OwlImageNames.Contains($"{Path.GetFileName(OwlImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(OwlImageName)}_{count}.png not found at: {OwlImageDirectory}");
                    }
            }
            foreach (string GorillaImageName in GorillaImages)
            {
                foreach (int count in Enumerable.Range(1, BaseCritterSprites["Gorilla"]))
                    if (!GorillaImageNames.Contains($"{Path.GetFileName(GorillaImageName)}_{count}"))
                    {
                        Globals.ConsoleWarn($"{Path.GetFileName(GorillaImageName)}_{count}.png not found at: {GorillaImageDirectory}");
                    }
            }
        }
	}
}

using HarmonyLib;
using Netcode;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Randomizer
{
	/// <summary>The mod entry point</summary>
	public class ModEntry : Mod
	{
		private AssetLoader _modAssetLoader;
		private AssetEditor _modAssetEditor;

		private IModHelper _helper;

		static IGenericModConfigMenuAPI api;

		/// <summary>The mod entry point, called after the mod is first loaded</summary>
		/// <param name="helper">Provides simplified APIs for writing mods</param>
		[Obsolete]
		public override void Entry(IModHelper helper)
		{
			_helper = helper;
			Globals.ModRef = this;
			Globals.Config = Helper.ReadConfig<ModConfig>();

			var harmony = new Harmony(this.ModManifest.UniqueID);

			// Patches Rings to be Donatable to the CC.
			harmony.Patch(
			   original: AccessTools.Method(typeof(StardewValley.Menus.Bundle), nameof(StardewValley.Menus.Bundle.canAcceptThisItem), new Type[] {typeof(StardewValley.Item), typeof(ClickableTextureComponent), typeof(bool)}),
			   prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.canAcceptThisItem_Prefix))
			);
			harmony.Patch(
			   original: AccessTools.Method(typeof(StardewValley.Menus.Bundle), nameof(StardewValley.Menus.Bundle.tryToDepositThisItem)),
			   prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.TryToDepositThisItem_Prefix))
			);

			ImageBuilder.CleanUpReplacementFiles();

			this._modAssetLoader = new AssetLoader(this);
			this._modAssetEditor = new AssetEditor(this);
			helper.Content.AssetLoaders.Add(this._modAssetLoader);
			helper.Content.AssetEditors.Add(this._modAssetEditor);

			this.PreLoadReplacments();
			helper.Events.GameLoop.GameLaunched += (sender, args) => this.TryLoadModConfigMenu();
			helper.Events.GameLoop.SaveLoaded += (sender, args) => this.CalculateAllReplacements();
			helper.Events.Display.RenderingActiveMenu += (sender, args) => _modAssetLoader.TryReplaceTitleScreen();
			helper.Events.GameLoop.ReturnedToTitle += (sender, args) => _modAssetLoader.ReplaceTitleScreenAfterReturning();

			if (Globals.Config.Music.Randomize) { helper.Events.GameLoop.UpdateTicked += (sender, args) => MusicRandomizer.TryReplaceSong(); }
			if (Globals.Config.RandomizeRain) { helper.Events.GameLoop.DayEnding += _modAssetLoader.ReplaceRain; }

			if (Globals.Config.Crops.Randomize)
			{
				helper.Events.Multiplayer.PeerContextReceived += (sender, args) => FixParsnipSeedBox();
			}

			if (Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize)
			{
				helper.Events.Display.RenderingActiveMenu += (sender, args) => CraftingRecipeAdjustments.HandleCraftingMenus();

				// Fix for the Special Orders causing crashes
				// Re-instate the object info when the save is first loaded for the session, and when saving so that the
				// items have the correct names on the items sold summary screen
				helper.Events.GameLoop.DayEnding += (sender, args) => _modAssetEditor.UndoObjectInformationReplacements();
				helper.Events.GameLoop.SaveLoaded += (sender, args) => _modAssetEditor.RedoObjectInformationReplacements();
				helper.Events.GameLoop.Saving += (sender, args) => _modAssetEditor.RedoObjectInformationReplacements();
			}

			if (Globals.Config.RandomizeForagables)
			{
				helper.Events.GameLoop.GameLaunched += (sender, args) => WildSeedAdjustments.ReplaceGetRandomWildCropForSeason();
			}

			if (Globals.Config.Fish.Randomize)
			{
				helper.Events.GameLoop.DayStarted += (sender, args) => OverriddenSubmarine.UseOverriddenSubmarine();
				helper.Events.GameLoop.DayEnding += (sender, args) => OverriddenSubmarine.RestoreSubmarineLocation();
			}

			if (Globals.Config.Bundles.Randomize)
			{
				helper.Events.Display.MenuChanged += BundleMenuAdjustments.FixRingSelection;

				if (Globals.Config.Bundles.ShowDescriptionsInBundleTooltips)
				{
					helper.Events.Display.RenderedActiveMenu += (sender, args) => BundleMenuAdjustments.AddDescriptionsToBundleTooltips();
				}
			}
		}

		internal class MenuPatches
		{
			private static IMonitor Monitor;

			// call this method from your Entry class
			internal static void Initialize(IMonitor monitor)
			{
				Monitor = monitor;
			}

			/// <summary>
			/// The new method to replace Stardew Valley's Bundle.cs's canAcceptThisItem
			/// </summary>
			/// <param name="item">The item you are trying to deposit</param>
			/// <param name="slot">The slot you're trying to deposit to</param>
			/// <param name="ignore_stack_count">Whether we don't care about stack count</param>
			/// <returns>What item the player should get back after trying to depositing</returns>
			internal static bool canAcceptThisItem_Prefix(StardewValley.Item item, ClickableTextureComponent slot, ref bool __result, StardewValley.Menus.Bundle __instance, bool ignore_stack_count = false)
			{
				try
				{
					{

						bool isRing = item is Ring;
						__result = false;
						if (!__instance.depositsAllowed || (!isRing && !(item is StardewValley.Object)))
							__result = false;
						StardewValley.Object @object = item as StardewValley.Object;
						for (int index = 0; index < __instance.ingredients.Count; ++index)
						{
							if (isRing || __instance.IsValidItemForThisIngredientDescription(@object, __instance.ingredients[index]) &&
								(ignore_stack_count || __instance.ingredients[index].stack <= item.Stack) &&
								(slot == null || slot.item == null))
								__result = true;
						}
                    }
					return false; // don't run original logic
				}
				catch (Exception ex)
				{
					Monitor.Log($"Failed in {nameof(canAcceptThisItem_Prefix)}:\n{ex}", LogLevel.Error);
					return true; // run original logic
				}
            }

            /// <summary>
            /// The new method to replace Stardew Valley's Bundle.cs's tryToDepositThisItem
            /// </summary>
            /// <param name="item">The item you are trying to deposit</param>
            /// <param name="slot">The slot you're trying to deposit to</param>
            /// <param name="noteTextureName">Unsure what this is</param>
            /// <returns>What item the player should get back after trying to depositing</returns>
            internal static bool TryToDepositThisItem_Prefix(StardewValley.Item item, ClickableTextureComponent slot, string noteTextureName, ref StardewValley.Item __result, StardewValley.Menus.Bundle __instance)
			{
				try
				{
					{

						if (!__instance.depositsAllowed)
						{
							if (Game1.player.hasCompletedCommunityCenter())
								Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtAJM"));
							else
								Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:JunimoNote_MustBeAtCC"));
							__result = item;
							return false;
						}

						bool isRing = item is Ring;
						if (!(item is StardewValley.Object || isRing) || item is Furniture)
						{
							__result = item;
							return false;
						}

						StardewValley.Object @object = item as StardewValley.Object;
						bool ringDeposited = false;
						for (int index = 0; index < __instance.ingredients.Count; ++index)
						{
							if (!__instance.ingredients[index].completed &&
								__instance.ingredients[index].index == (int)((NetFieldBase<int, NetInt>)item.parentSheetIndex) &&
								(
									item.Stack >= __instance.ingredients[index].stack &&
									(isRing || (int)((NetFieldBase<int, NetInt>)@object.quality) >= __instance.ingredients[index].quality)
								) &&
								slot.item == null)
							{
								if (isRing)
								{
									ringDeposited = true;
								}
								item.Stack -= __instance.ingredients[index].stack;
								__instance.ingredients[index] = new BundleIngredientDescription(__instance.ingredients[index].index, __instance.ingredients[index].stack, __instance.ingredients[index].quality, true);
								__instance.ingredientDepositAnimation(slot, noteTextureName, false);
								slot.item = new StardewValley.Object(__instance.ingredients[index].index, __instance.ingredients[index].stack, false, -1, __instance.ingredients[index].quality);
								Game1.playSound("newArtifact");
								(Game1.getLocationFromName("CommunityCenter") as CommunityCenter).bundles.FieldDict[__instance.bundleIndex][index] = true;
								slot.sourceRect.X = 512;
								slot.sourceRect.Y = 244;

								Multiplayer multiplayer = Globals.ModRef.Helper.Reflection
									.GetField<Multiplayer>(typeof(Game1), "multiplayer", true)
									.GetValue();
								multiplayer.globalChatInfoMessage("BundleDonate", Game1.player.displayName, slot.item.DisplayName);
							}
						}
						if (!ringDeposited && item.Stack > 0)
						{
							__result = item;
							return false;
						}
                        __result = null;
                    }
					return false; // don't run original logic
				}
				catch (Exception ex)
				{
					Monitor.Log($"Failed in {nameof(TryToDepositThisItem_Prefix)}:\n{ex}", LogLevel.Error);
					return true; // run original logic
				}
			}
		}

		private void TryLoadModConfigMenu()
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				{
					Globals.ConsoleTrace("GenericModConfigMenu not present");
					return;
				}

				api = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
				api.RegisterModConfig(ModManifest, () => Globals.Config = new ModConfig(), () => Helper.WriteConfig(Globals.Config));

				ModConfigMenuHelper menuHelper = new ModConfigMenuHelper(api, ModManifest);
				menuHelper.RegisterModOptions();

			}

			/// <summary>
			/// Loads the replacements that can be loaded before a game is selected
			/// </summary>
			public void PreLoadReplacments()
			{
				_modAssetLoader.CalculateReplacementsBeforeLoad();
				_modAssetEditor.CalculateEditsBeforeLoad();
			}

			/// <summary>
			/// Does all the randomizer replacements that take place after a game is loaded
			/// </summary>
			public void CalculateAllReplacements()
			{
				//Seed is pulled from farm name
				byte[] seedvar = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(Game1.player.farmName.Value));
				int seed = BitConverter.ToInt32(seedvar, 0);

				this.Monitor.Log($"Seed Set: {seed}");

				Globals.RNG = new Random(seed);
				Globals.SpoilerLog = new SpoilerLogger(Game1.player.farmName.Value);

				// Make replacements and edits
				_modAssetLoader.CalculateReplacements();
				_modAssetEditor.CalculateEdits();
				_modAssetLoader.RandomizeImages();
				Globals.SpoilerLog.WriteFile();

				// Invalidate all replaced and edited assets so they are reloaded
				_modAssetLoader.InvalidateCache();
				_modAssetEditor.InvalidateCache();

				// Ensure that the bundles get changed if they're meant to
				Game1.GenerateBundles(Game1.bundleType, true);

				if (Globals.Config.RandomizeForagables)
				{
					ChangeDayOneForagables();
				}
				FixParsnipSeedBox();
				OverriddenSeedShop.ReplaceShopStockMethod();
				OverriddenAdventureShop.FixAdventureShopBuyAndSellPrices();
			}

			/// <summary>
			/// A passthrough to calculate adn invalidate UI edits
			/// Used when the lanauage is changed
			/// </summary>
			public void CalculateAndInvalidateUIEdits()
			{
				_modAssetEditor.CalculateAndInvalidateUIEdits();
			}

			/// <summary>
			/// Fixes the foragables on day 1 - the save file is created too quickly for it to be
			/// randomized right away, so we'll change them on the spot on the first day
			/// </summary>
			public void ChangeDayOneForagables()
			{
				SDate currentDate = SDate.Now();
				if (currentDate.DaysSinceStart < 2)
				{
					List<GameLocation> locations = Game1.locations
						.Concat(
							from location in Game1.locations.OfType<BuildableGameLocation>()
							from building in location.buildings
							where building.indoors.Value != null
							select building.indoors.Value
						).ToList();

					List<Item> newForagables =
						ItemList.GetForagables(Seasons.Spring)
							.Where(x => x.ShouldBeForagable) // Removes the 1/1000 items
							.Cast<Item>().ToList();

					foreach (GameLocation location in locations)
					{
						List<int> foragableIds = ItemList.GetForagables().Select(x => x.Id).ToList();
						List<Vector2> tiles = location.Objects.Pairs
							.Where(x => foragableIds.Contains(x.Value.ParentSheetIndex))
							.Select(x => x.Key)
							.ToList();

						foreach (Vector2 oldForagableKey in tiles)
						{
							Item newForagable = Globals.RNGGetRandomValueFromList(newForagables, true);
							location.Objects[oldForagableKey].ParentSheetIndex = newForagable.Id;
							location.Objects[oldForagableKey].Name = newForagable.Name;
						}
					}
				}
			}

			/// <summary>
			/// Fixes the item name that you get at the start of the game
			/// </summary>
			public void FixParsnipSeedBox()
			{
				GameLocation farmHouse = Game1.locations.Where(x => x.Name == "FarmHouse").First();

				List<StardewValley.Objects.Chest> chestsInRoom =
					farmHouse.Objects.Values.Where(x =>
						x.DisplayName == "Chest")
						.Cast<StardewValley.Objects.Chest>()
						.Where(x => x.giftbox.Value)
					.ToList();

				if (chestsInRoom.Count > 0)
				{
					string parsnipSeedsName = ItemList.GetItemName((int)ObjectIndexes.ParsnipSeeds);
					StardewValley.Item itemInChest = chestsInRoom[0].items[0];
					if (itemInChest.Name == "Parsnip Seeds")
					{
						itemInChest.Name = parsnipSeedsName;
						itemInChest.DisplayName = parsnipSeedsName;
					}
				}
			}
	}
}
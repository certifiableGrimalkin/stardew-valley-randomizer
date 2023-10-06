using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using SVBundle = StardewValley.Menus.Bundle;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer
{
	public class BundleMenuAdjustments
	{
		private static JunimoNoteMenu _currentActiveBundleMenu { get; set; }


		/// <summary>
		/// Fixes the ability to highlight rings in the bundle menu
		/// </summary>
		public static void FixRingSelection(object sender, MenuChangedEventArgs e)
		{
			if (!Globals.Config.Bundles.Randomize || !(e.NewMenu is JunimoNoteMenu))
			{
				_currentActiveBundleMenu = null;
				return;
			}

			_currentActiveBundleMenu = (JunimoNoteMenu)e.NewMenu;
			_currentActiveBundleMenu.inventory.highlightMethod = HighlightBundleCompatibleItems;
		}

		/// <summary>
		/// A copy of the Utlity.cs code for highlightSmallObjects, but with rings included
		/// </summary>
		/// <param name="item">The Stardew Valley item</param>
		/// <returns>True if the item should be draggable, false otherwise</returns>
		private static bool HighlightBundleCompatibleItems(SVItem item)
		{
			if (item is Ring)
			{
				return true;
			}
			else if (item is SVObject)
			{
				return !(bool)((NetFieldBase<bool, NetBool>)(item as SVObject).bigCraftable);
			}
			return false;
		}

		/// <summary>
		/// Adds tooltips for the bundle items so that you can see where to get fish
		/// </summary>
		public static void AddDescriptionsToBundleTooltips()
		{
			bool settingEnabled = Globals.Config.Bundles.Randomize && Globals.Config.Bundles.ShowDescriptionsInBundleTooltips;
			if (!settingEnabled || _currentActiveBundleMenu == null) { return; }

			foreach (ClickableTextureComponent ingredient in _currentActiveBundleMenu.ingredientList)
			{
				ingredient.hoverText = $"{ingredient.item.DisplayName}:\n{ingredient.item.getDescription()}";
			}
		}
	}
}

using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Represents a crop
	/// </summary>
	public class CropItem : Item
	{
		public int Price { get; set; }
		public string CategoryString { get; set; }
		public string Description { get; set; }

		public override bool IsFlower
		{
			get
			{
				return CategoryString == "18/Basic -80";
			}
		}

		public SeedItem MatchingSeedItem
		{
			get
			{
				return ItemList.GetSeedFromCrop(this);
			}
		}

		public CropItem(int id, string categoryString) : base(id)
		{
			IsCrop = true;
			DifficultyToObtain = ObtainingDifficulties.LargeTimeRequirements;
			CategoryString = categoryString;
		}

		public override string ToString()
		{
			return $"{Name}/{Price}/{CategoryString}/{Name}/{Description}";
		}

		/// <summary>
		/// Gets all the crop items
		/// </summary>
		/// <param name="includeUnchangedCrops">Include unchanged crop items (ancient fruit)</param>
		/// <returns />
		public static List<CropItem> Get(bool includeUnchangedCrops = false)
		{
			return ItemList.Items.Values.Where(x =>
				x.IsCrop &&
				(includeUnchangedCrops || x.Id != (int)ObjectIndexes.AncientFruit))
			.Cast<CropItem>()
			.ToList();
		}
	}
}

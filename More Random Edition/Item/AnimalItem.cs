namespace Randomizer
{
	/// <summary>
	/// Represents items that you get from raising animals
	/// </summary>
	public class AnimalItem : Item
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The id of the item</param>
		public AnimalItem(int id, ObtainingDifficulties difficultyToObtain = ObtainingDifficulties.MediumTimeRequirements) : base(id)
		{
			IsAnimalProduct = true;
			DifficultyToObtain = difficultyToObtain;
		}
	}
}

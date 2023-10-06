namespace Randomizer
{
	/// <summary>
	/// A list of categories of crafting items
	/// This helps determine what the randomized recipes can possibly be
	/// </summary>
	public enum CraftableCategories
	{
		/// <summary>
		/// Use this for things like chests, torches, etc. - things you need early game
		/// </summary>
		Easy,

		/// <summary>
		/// Use this for things like paths
		/// </summary>
		EasyAndNeedMany,

		/// <summary>
		/// Use this for more useful things like scarecrows, sprinklers, etc.
		/// </summary>
		Moderate,

		/// <summary>
		/// Use this for things like fertilizer
		/// </summary>
		ModerateAndNeedMany,

		/// <summary>
		/// Use this for not quite endgame items such as quality sprinklers
		/// </summary>
		Difficult,

		/// <summary>
		/// Use this for things like quality fertilizer
		/// </summary>
		DifficultAndNeedMany,

		/// <summary>
		/// Use this for all endgame things, such as Iridium sprinklers
		/// </summary>
		Endgame,

		/// <summary>
		/// A special setting used by wild seeds - this will result in foragables from the current season
		/// being needed to craft the item
		/// </summary>
		Foragables,

        /// <summary>
        /// Use this for recipes from Ginger Island.
        /// </summary>
        Postgame,

        /// <summary>
        /// Use this for recipes from Ginger Island's Wallnut Room.
        /// </summary>
		Qi

    }
}

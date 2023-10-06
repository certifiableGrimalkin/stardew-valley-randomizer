namespace Randomizer
{
    /// <summary>
    /// Represents an critter sprite in the tilesheet
    /// </summary>
    public class Critter
    {
        public string Name { get; }
        public int SpriteNumber { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The Critter's name.</param>
        /// <param name="spritecount">Amount of Sprites a Critter has.</param>
        /// <param name="spritenumber">Which, in order, Sprite of this Critter it is.</param>
        public Critter(string name, int spritenumber)
        {
            Name = name;
            SpriteNumber = spritenumber;
        }
    }
}

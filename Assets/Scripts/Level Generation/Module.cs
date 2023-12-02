using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Scriptable Object asset for one specific module.
    /// </summary>
    [CreateAssetMenu(fileName = "New Module", menuName = "Map Generation/Module")]
    public class Module : ScriptableObject
    {
        /// <summary>
        /// Different edge connection types.
        /// </summary>
        public enum EdgeConnectionTypes
        {
            NONE,
            Road = 1,
            TrotoarOpen,
            TrotoarLeft,
            TrotoarRight,
            TrotoarTop,
            TrotoarBottom,
            EMPTY = 10,
            Building = 11,
            BuildingLeft = 12,
            BuildingRight = 13,
            BuildingTop = 14,
            BuildingBottom = 15,
            Block = 4,
            Open = 5
        }

        /// <summary>
        /// The module`s game object.
        /// </summary>
        public GameObject moduleGO;

        /// <summary>
        /// The module`s edge connections starting with the bottom one going counter clockwise.
        ///
        /// [bottom, right, top, left]
        /// </summary>
        public EdgeConnectionTypes[] edgeConnections = new EdgeConnectionTypes[4];
    }
}
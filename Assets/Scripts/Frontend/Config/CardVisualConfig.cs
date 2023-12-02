using GameState;
using SerializedDict;
using System;
using UnityEngine;

namespace Frontend.Config
{
    [CreateAssetMenu(fileName = "New Visual Config", menuName = "SGJ/Baba/VisualConfig")]
    [Serializable]
    public class CardVisualConfig : ScriptableObject
    {
        [field: SerializeField]
        public CardVisualDataDictionary CardData { get; set; }
    }
    
    [Serializable]
    public class CardVisualDataDictionary : SerializedDictionary<int, CardVisualData> {}

    [Serializable]
    public class CardVisualData
    {
        public Sprite image;
        public string name;
        public CardType cardType;
        public Vector2[] area;
    }
}
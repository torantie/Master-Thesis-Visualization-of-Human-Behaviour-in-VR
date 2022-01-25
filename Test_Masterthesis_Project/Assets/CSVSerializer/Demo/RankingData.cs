using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingData : ScriptableObject
{
    public enum Country
    {
        gb=1,
        de=2,
        fi,
        be
    }
    [System.Serializable]
    public class Item
    {
        public int ranking;
        public string driver;
        public string constructor;
        public int score;
        public int podium;

        public Sprite icon;
        public Country country;
        public string[] win;
    }

    public Item[] m_Items;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageStringData : ScriptableObject
{
    [System.Serializable]
    public class Item
    {
        public string id;
        public string en, jp, ru, ch, fr, ko;
    }

    public Item[] m_Items;
}

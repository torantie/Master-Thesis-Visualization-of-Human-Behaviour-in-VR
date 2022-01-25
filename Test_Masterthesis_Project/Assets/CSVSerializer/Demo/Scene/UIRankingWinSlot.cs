using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingWinSlot : MonoBehaviour
{
    public Text m_Text;

    public void Load(string text)
    {
        m_Text.text = text;
    }
}

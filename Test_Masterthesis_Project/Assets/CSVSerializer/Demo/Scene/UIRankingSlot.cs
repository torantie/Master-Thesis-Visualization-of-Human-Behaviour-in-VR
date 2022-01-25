using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingSlot : MonoBehaviour 
{
    public Text m_TextRanking;
    public Text m_TextConstructor;
    public Text m_TextDriver;
    public Text m_TextScore;
    public Text m_TextPodium;

    public Image m_Flag;
    public GameObject m_Win;

    public Transform m_WinListRoot;

    public Image m_ImageCar;

    public void Load(RankingData.Item item, Sprite flag, bool winlist)
    {
        m_TextRanking.text = item.ranking.ToString();
        m_TextConstructor.text = item.constructor;
        m_TextDriver.text = item.driver;
        m_TextScore.text = item.score.ToString();
        m_TextPodium.text = item.podium.ToString();
        m_Flag.sprite = flag;
        m_ImageCar.overrideSprite = item.icon;

        m_Win.SetActive(winlist);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRanking : MonoBehaviour 
{
    public Transform m_ListRoot;
    public GameObject m_SlotPrefab;
    public GameObject m_WinSlotPrefab;

    public RankingData m_RankingData;
    public Sprite[] m_Country;

    private void Awake()
    {
        m_SlotPrefab.gameObject.SetActive(false);
        m_WinSlotPrefab.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        foreach (Transform trans in m_ListRoot.GetComponentsInChildren<Transform>())
        {
            if (trans == m_ListRoot)
                continue;
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < m_RankingData.m_Items.Length; i++)
        {
            UIRankingSlot slot = GameObject.Instantiate(m_SlotPrefab).GetComponent<UIRankingSlot>();
            slot.transform.SetParent(m_ListRoot);
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.gameObject.SetActive(true);

            RankingData.Item item = m_RankingData.m_Items[i];

            for (int n = 0; n < m_RankingData.m_Items[i].win.Length; n++)
            {
                UIRankingWinSlot winslot = GameObject.Instantiate(m_WinSlotPrefab).GetComponent<UIRankingWinSlot>();
                winslot.transform.SetParent(slot.m_WinListRoot);
                winslot.transform.localScale = new Vector3(1, 1, 1);
                winslot.gameObject.SetActive(true);
                winslot.Load(m_RankingData.m_Items[i].win[n]);
            }

            Sprite country = GetCountrySprite(item.country.ToString());
            slot.Load(item, country, item.win.Length > 0 ? true : false);
        }
    }

    Sprite GetCountrySprite(string id)
    {
        for (int i = 0; i < m_Country.Length; i++)
        {
            if (m_Country[i].name == id)
                return m_Country[i];
        }
        return null;
    }
}
using TMPro;
using UnityEngine;

public class UIScrollViewCollectionContainer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_headline;

    [SerializeField]
    private GameObject m_scrollViewCollectionContainer;

    public GameObject ScrollViewCollectionContainer { get => m_scrollViewCollectionContainer; private set => m_scrollViewCollectionContainer = value; }

    public UIFilterCategory[] UIFilterCategories { get { return ScrollViewCollectionContainer.GetComponentsInChildren<UIFilterCategory>(true); } }

    public UIFilterCategory[] ActiveUIFilterCategories { get { return ScrollViewCollectionContainer.GetComponentsInChildren<UIFilterCategory>(); } }

    public string Title { get { return m_headline.text; } }

    public void SetHeader(string a_text)
    {
        m_headline.text = a_text;
    }
}

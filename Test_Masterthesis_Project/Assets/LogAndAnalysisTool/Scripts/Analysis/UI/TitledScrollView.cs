using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitledScrollView<T, U> : MonoBehaviour where U : ScrollViewContentHandler
{
    [SerializeField]
    protected TMP_Text m_headline;

    [SerializeField]
    protected U m_contentFiller;

    public T Title { get; protected set; }

    public U ContentFiller { get => m_contentFiller; protected set => m_contentFiller = value; }

    public virtual void SetContent(T a_title, List<string> a_itemNames)
    {
        Title = a_title;
        ContentFiller.Fill(a_itemNames);
        SetHeader(Title);
    }

    public virtual void SetContent(T a_title, Dictionary<string, Color> a_colorMappings)
    {
        Title = a_title;
        ContentFiller.Fill(a_colorMappings);
        SetHeader(Title);
    }

    protected virtual void SetHeader(T a_title)
    {
        m_headline.text = a_title.ToString();
    }
}

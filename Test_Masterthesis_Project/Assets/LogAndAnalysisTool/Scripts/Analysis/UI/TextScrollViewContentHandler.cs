using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextScrollViewContentHandler : ScrollViewContentHandler
{

    public override void Fill(List<string> a_namesToShow)
    {
        foreach (var name in a_namesToShow)
        {
            TryCreateTextPrefab(name, out _);
        }
    }


    public override void Fill(Dictionary<string, Color> a_colorMappings)
    {
        foreach (var colorMapping in a_colorMappings)
        {
            TryCreateTextPrefab(colorMapping.Key, colorMapping.Value, out _);
        }
    }

    protected virtual bool TryCreateTextPrefab(string a_name, out GameObject a_gameObject)
    {
        a_gameObject = m_instantiatedContentElements.FirstOrDefault(instantiatedContentElement => instantiatedContentElement.GetComponent<TMP_Text>().text == a_name);

        if (a_gameObject == default)
        {
            var textObject = Instantiate(m_contentElementPrefab, transform);
            var textComponent = textObject.GetComponent<TMP_Text>();
            textComponent.text = a_name;
            m_instantiatedContentElements.Add(textObject);
            a_gameObject = textObject;
            return true;
        }
        else
        {
            Debug.LogFormat("TextPrefab with name {0} already exists.", a_name);
        }

        return false;
    }

    protected virtual bool TryCreateTextPrefab(string a_name, Color a_color, out GameObject a_gameObject)
    {
        if (TryCreateTextPrefab(a_name, out a_gameObject))
        {
            a_gameObject.GetComponent<TMP_Text>().color = a_color;
            return true;
        }
        return false;
    }



    /// <summary>
    /// Destroy all text game objects.
    /// </summary>
    public void Clear()
    {
        foreach (var text in m_instantiatedContentElements)
        {
            Destroy(text.gameObject);
        }
        m_instantiatedContentElements.Clear();
    }
}

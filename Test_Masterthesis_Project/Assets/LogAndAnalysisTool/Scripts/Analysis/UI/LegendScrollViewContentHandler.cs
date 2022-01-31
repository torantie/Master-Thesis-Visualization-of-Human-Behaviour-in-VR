using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LegendScrollViewContentHandler : TextScrollViewContentHandler
{
    public override void Fill(Dictionary<string, Color> a_colorMappings)
    {
        foreach (var colorMapping in a_colorMappings)
        {
            TryCreateTextPrefab(colorMapping.Key, colorMapping.Value, out _);
        }
    }

    protected override bool TryCreateTextPrefab(string a_name, Color a_color, out GameObject a_gameObject)
    {
        if (TryCreateTextPrefab(a_name, out a_gameObject))
        {
            var textComponent = a_gameObject.GetComponentInChildren<TMP_Text>();
            if (a_color.a != 0)
                textComponent.color = a_color;

            foreach (var image in a_gameObject.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name == "Color Indicator")
                {
                    image.color = a_color;
                    break;
                }
            }
            return true;
        }
        return false;
    }

    protected override bool TryCreateTextPrefab(string a_name, out GameObject a_gameObject)
    {
        a_gameObject = m_instantiatedContentElements.FirstOrDefault(instantiatedContentElement => instantiatedContentElement.GetComponentInChildren<TMP_Text>().text == a_name);

        if (a_gameObject == default)
        {
            var textObject = Instantiate(m_contentElementPrefab, transform);
            var textComponent = textObject.GetComponentInChildren<TMP_Text>();
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
}

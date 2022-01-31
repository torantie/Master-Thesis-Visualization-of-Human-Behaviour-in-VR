using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollViewContentHandler : MonoBehaviour
{
    /// <summary>
    /// Prefab to instantiate to fill content.
    /// </summary>
    [SerializeField]
    protected GameObject m_contentElementPrefab;

    /// <summary>
    /// Instantiated content elements.
    /// </summary>
    protected readonly List<GameObject> m_instantiatedContentElements = new List<GameObject>();

    /// <summary>
    /// Fill scroll view with a list of strings.
    /// </summary>
    /// <param name="a_stringsToShow">Strings to show in the scroll view.</param>
    public abstract void Fill(List<string> a_stringsToShow);

    /// <summary>
    /// Fill scroll view with strings and colors.
    /// </summary>
    /// <param name="a_colorMappings">Strings and colors to show in the scroll view.</param>
    public abstract void Fill(Dictionary<string, Color> a_colorMappings);
}

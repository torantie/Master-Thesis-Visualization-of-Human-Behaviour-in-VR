using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Handling collection of checkboxes inside a scrollview. 
/// Not using ToggleGroups because of reset issues.
/// (see https://forum.unity.com/threads/why-does-the-toggle-group-always-reset-my-default-value.681454/)
/// </summary>
public class CheckboxScrollViewContentHandler : ScrollViewContentHandler
{
    /// <summary>
    /// Only one toggle is active at all times.
    /// </summary>
    [SerializeField]
    private bool onlyOneToggle;

    /// <summary>
    /// Only one toggle is active at all times.
    /// </summary>
    public bool OnlyOneToggle { get => onlyOneToggle; set => onlyOneToggle = value; }

    /// <summary>
    /// Event to signal that a toggle state was changed.
    /// </summary>
    public UnityEvent<GameObject, string, bool> OnToggleChange { get; private set; } = new UnityEvent<GameObject, string, bool>();

    /// <summary>
    /// Add names to checkbox container if they do not already exist.
    /// </summary>
    /// <param name="a_namesToShow">Names to fill into checkbox container.</param>
    public override void Fill(List<string> a_namesToShow)
    {
        foreach (var name in a_namesToShow)
        {
            TryCreateTogglePrefab(name, out _);
        }
        if (OnlyOneToggle)
        {
            DisableAllExceptOne();
        }
    }

    /// <summary>
    /// Add names and colors to checkbox container if they do not already exist.
    /// </summary>
    /// <param name="a_colorMappings">Colors and names to fill into checkbox container.</param>
    public override void Fill(Dictionary<string, Color> a_colorMappings)
    {
        foreach (var colorMapping in a_colorMappings)
        {
            TryCreateTogglePrefab(colorMapping.Key, colorMapping.Value, out _);
        }
        if (OnlyOneToggle)
        {
            DisableAllExceptOne();
        }
    }

    /// <summary>
    /// Create toggle prefab.
    /// </summary>
    /// <param name="a_name">Name of toggle text.</param>
    /// <param name="a_gameObject">Created GameObject.</param>
    /// <returns>True if created. False if toggle already exists.</returns>
    protected virtual bool TryCreateTogglePrefab(string a_name, out GameObject a_gameObject)
    {
        a_gameObject = m_instantiatedContentElements.FirstOrDefault(instantiatedContentElement => instantiatedContentElement.GetComponentInChildren<Text>().text == a_name);

        if (a_gameObject == default)
        {
            var checkBox = Instantiate(m_contentElementPrefab, transform);
            var textComponent = checkBox.GetComponentInChildren<Text>();
            textComponent.text = a_name;
            var toggle = checkBox.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(on => OnToggleValueChanged(toggle, on));
            m_instantiatedContentElements.Add(checkBox);
            a_gameObject = checkBox;
            return true;
        }
        else
        {
            Debug.LogFormat("TogglePrefab with name {0} already exists.", a_name);
        }

        return false;
    }

    /// <summary>
    /// Create toggle prefab with color.
    /// </summary>
    /// <param name="a_name">Name of toggle text.</param>
    /// <param name="a_color">Color of toggle text.</param>
    /// <param name="a_gameObject">Created GameObject.</param>
    /// <returns>True if created. False if toggle already exists.</returns>
    protected virtual bool TryCreateTogglePrefab(string a_name, Color a_color, out GameObject a_gameObject)
    {
        if (TryCreateTogglePrefab(a_name, out a_gameObject))
        {
            a_gameObject.GetComponent<Text>().color = a_color;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Destroy all toggle game objects.
    /// </summary>
    public void Clear()
    {
        foreach (var checkbox in m_instantiatedContentElements)
        {
            Destroy(checkbox);
        }
        m_instantiatedContentElements.Clear();
    }

    /// <summary>
    /// Get Text value of selected toggle items.
    /// </summary>
    /// <returns>List of selected toggle items text.</returns>
    public List<string> GetSelectedItems()
    {
        List<string> selectedItems = new List<string>();
        foreach (var instantiatedCheckBox in m_instantiatedContentElements)
        {
            var textComponent = instantiatedCheckBox.GetComponentInChildren<Text>();
            if (instantiatedCheckBox.GetComponent<Toggle>().isOn)
                selectedItems.Add(textComponent.text);
        }

        return selectedItems;
    }

    /// <summary>
    /// Get one selected toggles text from the checkbox collection.
    /// </summary>
    /// <returns>Selected toggle text.</returns>
    public string GetSelectedItem()
    {
        var firstOnToggle = m_instantiatedContentElements.FirstOrDefault(instantiatedCheckbox => instantiatedCheckbox.GetComponent<Toggle>().isOn);
        if (firstOnToggle.Equals(default))
        {
            Debug.LogError("No item selected.");
            return "";
        }

        return firstOnToggle.GetComponentInChildren<Text>().text;
    }

    /// <summary>
    /// If OneToggle Mode is active:
    /// - Disable other checkboxes if value changed to on. 
    /// - Re-enable toggle if set to false and no other toggle is set to on.
    /// </summary>
    /// <param name="a_toggle">Toggle throwing event.</param>
    /// <param name="a_on">New toggle State.</param>
    private void OnToggleValueChanged(Toggle a_toggle, bool a_on)
    {
        try
        {
            if (OnlyOneToggle)
            {
                if (a_on)
                {
                    foreach (var instantiatedCheckBox in m_instantiatedContentElements)
                    {
                        if (!instantiatedCheckBox.Equals(a_toggle.gameObject))
                        {
                            instantiatedCheckBox.GetComponent<Toggle>().isOn = false;
                        }
                    }
                }
                else
                {
                    var checkboxSelected = m_instantiatedContentElements.Exists(toggle => toggle.GetComponent<Toggle>().isOn);
                    if (!checkboxSelected)
                        a_toggle.isOn = true;
                }
            }

            OnToggleChange.Invoke(gameObject, a_toggle.GetComponentInChildren<Text>().text, a_on);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// Disable all checkboxes except one.
    /// </summary>
    private void DisableAllExceptOne()
    {
        var oneOn = false;
        foreach (var instantiatedCheckbox in m_instantiatedContentElements)
        {
            var toggle = instantiatedCheckbox.GetComponent<Toggle>();
            if (toggle.isOn)
            {
                if (oneOn)
                    toggle.isOn = false;

                oneOn = true;
            }
        }
    }

}

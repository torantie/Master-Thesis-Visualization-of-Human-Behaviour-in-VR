using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Todo
/// </summary>
public class ReplayPanel : MenuPanel
{
    [SerializeField]
    private TMP_Dropdown m_animationEntitySelect;

    public TMP_Dropdown AnimationEntitySelect { get => m_animationEntitySelect; private set => m_animationEntitySelect = value; }

}

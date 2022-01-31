using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private MenuContent m_uiContent;

    [SerializeField]
    private UIAppearanceManager m_uiAppearanceManager;

    public void ShowSelection(VisualDataPoint a_visualDataPoint)
    {
        m_uiContent.SetSelectionPanel(a_visualDataPoint);
        m_uiAppearanceManager.ActivateAndMoveMenu();
    }
}

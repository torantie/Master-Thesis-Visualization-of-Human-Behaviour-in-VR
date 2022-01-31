using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class UIAppearanceManager : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty m_menuPress;

    [SerializeField]
    private MenuManager m_menuManager;

    [SerializeField]
    private Camera m_analysisXRCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_menuPress.action.performed += OnMenuPress;
    }

    private void OnMenuPress(InputAction.CallbackContext obj)
    {
        if (m_menuManager.gameObject.activeSelf)
        {
            m_menuManager.gameObject.SetActive(false);
        }
        else
        {
            ActivateAndMoveMenu();
        }
    }

    public void ActivateAndMoveMenu()
    {
        m_menuManager.gameObject.SetActive(true);
        MoveMenuInfrontOfPlayer();
    }

    private void MoveMenuInfrontOfPlayer()
    {
        //https://answers.unity.com/questions/772331/spawn-object-in-front-of-player-and-the-way-he-is.html
        Vector3 playerPos = m_analysisXRCamera.transform.position;
        Vector3 playerDirection = m_analysisXRCamera.transform.forward;
        Quaternion playerRotation = m_analysisXRCamera.transform.rotation;
        float spawnDistance = 1.5f;

        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;


        m_menuManager.transform.position = spawnPos;
        m_menuManager.transform.rotation = playerRotation;
    }
}

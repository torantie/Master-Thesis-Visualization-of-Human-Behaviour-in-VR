using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// https://www.youtube.com/watch?v=wGvh7Suo1h4
/// </summary>
public class TeleportInteractionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_baseControllerGameObject;

    [SerializeField]
    private GameObject m_teleportationGameObject;

    [SerializeField]
    private InputActionReference m_teleportActivationReference;

    [Space]

    [SerializeField]
    private UnityEvent OnTeleportActivate;

    [SerializeField]
    private UnityEvent OnTeleportCancel;


    private void Start()
    {
        m_teleportActivationReference.action.performed += TeleportActivate;
        m_teleportActivationReference.action.canceled += TeleportCancel;
    }

    private void TeleportCancel(InputAction.CallbackContext obj) => Invoke("DeactivateTeleport", .1f);

    private void DeactivateTeleport() => OnTeleportCancel.Invoke();

    private void TeleportActivate(InputAction.CallbackContext obj) => OnTeleportActivate.Invoke();

    /*
     * [SerializeField]
    private TeleportationProvider m_teleportationProvider;

    [SerializeField]
    private XRRayInteractor m_xrRayInteractor;
        
    [SerializeField]
    private InputActionProperty m_teleport;

    [SerializeField]
    private InputActionProperty m_teleportCancel;

    [SerializeField]
    private InputActionProperty m_move;

    private bool m_isActive;

    // Start is called before the first frame update
    void Start()
    {
        m_teleport.action.performed += OnTeleportPerformed;
        m_teleport.action.Enable();
        m_teleportCancel.action.performed += OnTeleportCancelPerformed;
        m_teleportCancel.action.Enable();
        m_move.action.Enable();
    }

    private void OnTeleportCancelPerformed(InputAction.CallbackContext obj)
    {
        m_xrRayInteractor.enabled = true;
        m_isActive = true;
    }

    private void OnTeleportPerformed(InputAction.CallbackContext obj)
    {
        m_xrRayInteractor.enabled = false;
        m_isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
            return;

        if (m_move.action.triggered)
            return;

        if(!m_xrRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            m_xrRayInteractor.enabled = false;
            m_isActive = false;
        }
        else
        {
            TeleportRequest request = new TeleportRequest()
            {
                destinationPosition = hit.point,
            };
            m_teleportationProvider.QueueTeleportRequest(request);
        }
    }
    */
}

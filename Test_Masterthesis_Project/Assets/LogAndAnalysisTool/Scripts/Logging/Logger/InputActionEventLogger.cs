using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionEventLogger : EventLogger
{
    [SerializeField]
    protected List<InputActionSubscription> m_inputActionSubscriptions = new List<InputActionSubscription>();

    [SerializeField]
    protected LoggingManager m_loggingManager;

    [SerializeField]
    private HMDLogger m_hmdLogger;

    public override void StartListening()
    {
        foreach (var inputActionSubscription in m_inputActionSubscriptions)
        {
            var inputActionProperty = inputActionSubscription.InputActionProperty;
            switch (inputActionSubscription.InputActionPhase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    inputActionProperty.action.started += InputActionTriggered;
                    break;
                case InputActionPhase.Performed:
                    inputActionProperty.action.performed += InputActionTriggered;
                    break;
                case InputActionPhase.Canceled:
                    inputActionProperty.action.canceled += InputActionTriggered;
                    break;
                default:
                    break;
            }
        }
    }

    public override void StopListening()
    {
        foreach (var inputActionSubscription in m_inputActionSubscriptions)
        {
            var inputActionProperty = inputActionSubscription.InputActionProperty;
            switch (inputActionSubscription.InputActionPhase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    inputActionProperty.action.started -= InputActionTriggered;
                    break;
                case InputActionPhase.Performed:
                    inputActionProperty.action.performed -= InputActionTriggered;
                    break;
                case InputActionPhase.Canceled:
                    inputActionProperty.action.canceled -= InputActionTriggered;
                    break;
                default:
                    break;
            }
        }
    }

    private void InputActionTriggered(InputAction.CallbackContext a_callbackContext)
    {
        string value = ReadValueToString(ref a_callbackContext);
        var position = GetPosition(a_callbackContext);

        var inputActionEvent = new InputActionEventDataPoint(
            m_loggingManager.StudyManager,
            Guid.NewGuid(),
            DateTime.UtcNow,
            position)
        {
            duration = a_callbackContext.duration,
            inputActionName = a_callbackContext.action.name,
            inputActionControlPath = a_callbackContext.control.path,
            phase = a_callbackContext.phase,
            startTime = a_callbackContext.startTime,
            time = a_callbackContext.time,
            value = value,
            valueType = a_callbackContext.valueType.ToString(),
        };
        m_loggingManager.OnEventTriggered(inputActionEvent);
    }

    private Vector3 GetPosition(InputAction.CallbackContext a_callbackContext)
    {
        try
        {
            foreach (var inputActionSubscription in m_inputActionSubscriptions)
            {
                if (inputActionSubscription.InputActionProperty.reference.action.id.Equals(a_callbackContext.action.id))
                {
                    return inputActionSubscription.ActionTransform.position;
                    //var binding = a_callbackContext.action.GetBindingForControl(a_callbackContext.control);
                    //if (binding.HasValue)
                    //{
                    //    foreach (var mapping in inputActionSubscription.PathToTransformMappings)
                    //    {
                    //        if (mapping.Path.Equals(binding.Value.path))
                    //        {
                    //            position = mapping.Transform.position.ToString();
                    //        }
                    //    }
                    //}
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        if (m_hmdLogger != null)
            return m_hmdLogger.GetTransfromPosition();
        else
            return Vector3.zero;
    }

    private static string ReadValueToString(ref InputAction.CallbackContext a_callbackContext)
    {
        string value;
        //Todo
        if (a_callbackContext.valueType == typeof(Quaternion))
            value = a_callbackContext.ReadValue<Quaternion>().ToString();
        else if (a_callbackContext.valueType == typeof(Vector3))
            value = a_callbackContext.ReadValue<Vector3>().ToString();
        else if (a_callbackContext.valueType == typeof(Vector2))
            value = a_callbackContext.ReadValue<Vector2>().ToString();
        else if (a_callbackContext.valueType == typeof(float))
            value = a_callbackContext.ReadValue<float>().ToString();
        else if (a_callbackContext.valueType == typeof(double))
            value = a_callbackContext.ReadValue<double>().ToString();
        else if (a_callbackContext.valueType == typeof(int))
            value = a_callbackContext.ReadValue<int>().ToString();
        else
            value = a_callbackContext.ReadValueAsObject().ToString();
        return value;
    }


    void OnValidate()
    {
        //foreach (var inputActionSubscription in m_inputActionSubscriptions)
        //{
        //    var inputActionProperty = inputActionSubscription.InputActionProperty;
        //    if (inputActionProperty.reference != null)
        //    {
        //        var pathToTransformMappings = inputActionSubscription.PathToTransformMappings;
        //        var bindings = inputActionProperty.action.bindings;
        //        if (pathToTransformMappings.Count == 0 || pathToTransformMappings.Count != bindings.Count)
        //        {
        //            pathToTransformMappings.Clear();
        //            foreach (var control in bindings)
        //            {
        //                pathToTransformMappings.Add(new PathToTransformMapping(control.path, null));
        //            }
        //        }
        //    }
        //}
    }
}

[Serializable]
public class InputActionSubscription
{
    [SerializeField]
    private InputActionProperty m_inputActionProperty;

    [SerializeField]
    private InputActionPhase m_inputActionPhase;

    [SerializeField]
    private Transform m_actionTransform;

    public Transform ActionTransform { get => m_actionTransform; set => m_actionTransform = value; }

    //[SerializeField]
    //private List<PathToTransformMapping> m_pathToTransformMappings = new List<PathToTransformMapping>();
    //public List<PathToTransformMapping> PathToTransformMappings { get => m_pathToTransformMappings; set => m_pathToTransformMappings = value; }

    public InputActionPhase InputActionPhase { get => m_inputActionPhase; set => m_inputActionPhase = value; }

    public InputActionProperty InputActionProperty { get => m_inputActionProperty; set => m_inputActionProperty = value; }
}

[Serializable]
public class PathToTransformMapping
{
    [SerializeField, ReadOnly]
    private string m_path;

    [SerializeField]
    private Transform m_transform;

    public PathToTransformMapping(string a_path, Transform a_transform)
    {
        m_path = a_path;
        m_transform = a_transform;
    }

    public string Path { get => m_path; set => m_path = value; }

    public Transform Transform { get => m_transform; set => m_transform = value; }
}

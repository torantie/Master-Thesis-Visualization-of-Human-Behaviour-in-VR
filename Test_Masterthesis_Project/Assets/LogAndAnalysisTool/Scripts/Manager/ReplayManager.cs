using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    [SerializeField]
    private ReplayPanel m_replayPanel;

    [SerializeField]
    private Slider m_timeline;

    [SerializeField]
    private DataQueryManager m_dataQueryManager;

    [SerializeField]
    private GameObject m_cameraRigPrefab;

    [SerializeField]
    private GameObject m_leftControllerPrefab;

    [SerializeField]
    private GameObject m_rightControllerPrefab;

    /// <summary>
    /// Currently only holds instantiated objects for participant.
    /// </summary>
    private List<GameObject> m_instantiatedObjects = new List<GameObject>();

    private (string id, List<Animation> animations) m_selectedAnimations;

    private const string AnimationName = "Replay";

    private List<string> m_transformIds = new List<string>();

    private readonly Dictionary<string, List<Animation>> m_animationsCollection = new Dictionary<string, List<Animation>>();


    void Start()
    {
        if (m_animationsCollection.Count == 0)
        {
            CreateParticipantAnimations();
            CreateTransformsAnimations();
        }
    }

    void Update()
    {
        if (m_selectedAnimations != default && m_selectedAnimations.animations != default)
        {
            foreach (var animation in m_selectedAnimations.animations)
            {
                if (m_selectedAnimations != default && animation.isPlaying)
                    m_timeline.value = animation[AnimationName].normalizedTime;
            }
        }
    }

    public void Play()
    {
        try
        {
            var selectedEntity = m_replayPanel.AnimationEntitySelect.options[m_replayPanel.AnimationEntitySelect.value].text;

            if (m_selectedAnimations != default && m_selectedAnimations.id != selectedEntity)
            {
                m_selectedAnimations.animations.ForEach(animation => animation.Stop());
            }

            foreach (var animationKvp in m_animationsCollection)
            {
                if (TryFindAndPlayEntity(animationKvp, selectedEntity, m_dataQueryManager.GetStudyParticipants()))
                {
                    m_instantiatedObjects.ForEach(instantiatedObject => instantiatedObject?.SetActive(true));
                    return;
                }
                if (TryFindAndPlayEntity(animationKvp, selectedEntity, m_transformIds))
                    return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void Pause()
    {
        try
        {
            if (m_selectedAnimations != default)
            {
                foreach (var animation in m_selectedAnimations.animations)
                {
                    if (animation.isPlaying)
                    {
                        //save time and set after stop since Stop() resets animation time
                        var time = animation[AnimationName].time;
                        animation[AnimationName].speed = 0;
                        animation.Stop();
                        animation[AnimationName].time = time;
                    }
                    else
                    {
                        m_instantiatedObjects.ForEach(instantiatedObject => instantiatedObject?.SetActive(false));
                        animation[AnimationName].time = 0f;
                        m_timeline.value = 0f;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void TimeLineValueChanged(float a_changedValue)
    {
        try
        {
            if (m_selectedAnimations != default)
            {
                foreach (var animation in m_selectedAnimations.animations)
                {
                    if (!animation.isPlaying)
                    {
                        var newPlayTime = animation[AnimationName].length * a_changedValue;
                        animation[AnimationName].time = newPlayTime;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }


    /// <summary>
    /// Find the selected entity in a proviede list of entities. Set and play the selected animations if the selected entity was found.
    /// </summary>
    /// <param name="a_animationCollectionKvp">Animation information to play if entity was found. Key is a entity id and value are the animations of the entity.</param>
    /// <param name="a_selectedEntityId">Selected entity id to play.</param>
    /// <param name="a_entityIdList">List of entity names to search.</param>
    /// <returns>True if entity was found and played. False if not.</returns>
    private bool TryFindAndPlayEntity(KeyValuePair<string, List<Animation>> a_animationCollectionKvp, string a_selectedEntityId, List<string> a_entityIdList)
    {
        foreach (var entityId in a_entityIdList)
        {
            if (a_selectedEntityId == entityId && a_animationCollectionKvp.Key == entityId)
            {
                m_selectedAnimations = (a_animationCollectionKvp.Key, a_animationCollectionKvp.Value);
                foreach (var animation in m_selectedAnimations.animations)
                {
                    animation[AnimationName].speed = 1;
                    animation.Play();
                }
                return true;
            }
        }
        return false;
    }

    private void CreateTransformsAnimations()
    {
        try
        {
            if (m_dataQueryManager.TryGetTransformsData(k => k.transformId, out var groupedTransforms))
            {
                foreach (var trackableTransform in FindObjectsOfType<TrackableTransform>())
                {
                    if (!groupedTransforms.ContainsKey(trackableTransform.TransformId))
                    {
                        Debug.LogWarningFormat("No data for key: {0}", trackableTransform.TransformId);
                        continue;
                    }
                    var animationClip = new AnimationClip();
                    var transformDataPoints = groupedTransforms[trackableTransform.TransformId];
                    var start = transformDataPoints[0].pointInTimeUtc;
                    var locRotScaleCurves = new LocRotScaleCurves(true, true, true);

                    foreach (var transformDataPoint in transformDataPoints)
                    {
                        float time = GetDataPointTime(start, transformDataPoint);

                        locRotScaleCurves.GetLocationCurves().AddKeyFrame(time, transformDataPoint.position.StringToVector3());
                        locRotScaleCurves.GetRotationCurves().AddKeyFrame(time, transformDataPoint.rotation.StringToQuaternion());
                        locRotScaleCurves.GetScaleCurves().AddKeyFrame(time, transformDataPoint.scale.StringToVector3());
                    }
                    locRotScaleCurves.SetCurvesToAnimationClip(animationClip);
                    // Todo parent transform removed to set world position,rotation
                    trackableTransform.transform.parent = null;

                    AddAnimationClip(trackableTransform.TransformId, trackableTransform.gameObject, animationClip);
                    m_transformIds.Add(trackableTransform.TransformId);
                    //var animator = trackableTransform.gameObject.AddComponent<Animator>();
                    //var animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                    //if (animatorOverrideController == null)
                    //{
                    //    animatorOverrideController = new AnimatorOverrideController();
                    //    animator.runtimeAnimatorController = animatorOverrideController;
                    //}
                    //animatorOverrideController[animationClip.name] = animationClip;
                    //animator.Play(animationClip.name);
                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void CreateParticipantAnimations()
    {
        try
        {
            if (m_dataQueryManager.TryGetParticipantData(k => k.participantId, out var groupedParticipants))
            {
                var instantiatedCameraRig = Instantiate(m_cameraRigPrefab);
                var instantiatedLeft = Instantiate(m_leftControllerPrefab);
                var instantiatedRight = Instantiate(m_rightControllerPrefab);
                m_instantiatedObjects.Add(instantiatedCameraRig);
                m_instantiatedObjects.Add(instantiatedLeft);
                m_instantiatedObjects.Add(instantiatedRight);
                foreach (var participantDataKvp in groupedParticipants)
                {
                    var animationClipCamera = new AnimationClip();
                    var animationClipLeft = new AnimationClip();
                    var animationClipRight = new AnimationClip();
                    var participantDataPoints = participantDataKvp.Value;
                    var start = participantDataPoints[0].pointInTimeUtc;
                    var headCurves = new LocRotScaleCurves(true, true, false);
                    var leftCurves = new LocRotScaleCurves(true, true, false);
                    var rightCurves = new LocRotScaleCurves(true, true, false);

                    foreach (var participantDataPoint in participantDataPoints)
                    {
                        float time = GetDataPointTime(start, participantDataPoint);
                        if (participantDataPoint.position != null && participantDataPoint.hmdTransformRotation != null)
                        {
                            headCurves.GetLocationCurves().AddKeyFrame(time, participantDataPoint.position.StringToVector3());
                            headCurves.GetRotationCurves().AddKeyFrame(time, participantDataPoint.hmdTransformRotation.StringToQuaternion());
                        }
                        if (participantDataPoint.leftControllerTransformPosition != null && participantDataPoint.leftControllerTransformRotation != null)
                        {
                            leftCurves.GetLocationCurves().AddKeyFrame(time, participantDataPoint.leftControllerTransformPosition.StringToVector3());
                            leftCurves.GetRotationCurves().AddKeyFrame(time, participantDataPoint.leftControllerTransformRotation.StringToQuaternion());
                        }
                        if (participantDataPoint.rightControllerTransformPosition != null && participantDataPoint.rightControllerTransformRotation != null)
                        {
                            rightCurves.GetLocationCurves().AddKeyFrame(time, participantDataPoint.rightControllerTransformPosition.StringToVector3());
                            rightCurves.GetRotationCurves().AddKeyFrame(time, participantDataPoint.rightControllerTransformRotation.StringToQuaternion());
                        }
                    }
                    if (headCurves.HasKeyframes())
                    {
                        headCurves.SetCurvesToAnimationClip(animationClipCamera);
                        AddAnimationClip(participantDataKvp.Key, instantiatedCameraRig, animationClipCamera); 
                    }
                    else
                    {
                        m_instantiatedObjects.Remove(instantiatedCameraRig);
                        Destroy(instantiatedCameraRig);
                    }
                    if (leftCurves.HasKeyframes())
                    {
                        leftCurves.SetCurvesToAnimationClip(animationClipLeft);
                        AddAnimationClip(participantDataKvp.Key, instantiatedLeft, animationClipLeft); 
                    }
                    else
                    {
                        m_instantiatedObjects.Remove(instantiatedLeft);
                        Destroy(instantiatedLeft);
                    }
                    if (rightCurves.HasKeyframes())
                    {
                        rightCurves.SetCurvesToAnimationClip(animationClipRight);
                        AddAnimationClip(participantDataKvp.Key, instantiatedRight, animationClipRight); 
                    }
                    else
                    {
                        m_instantiatedObjects.Remove(instantiatedRight);
                        Destroy(instantiatedRight);
                    }


                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// Add or get an animation component to/from the given game object. Stop running animations on the gameobject. 
    /// Add the animation component to the created animations and as a selection.
    /// </summary>
    /// <param name="a_animationId">Id of the animation.</param>
    /// <param name="a_gameObject">Object to animate.</param>
    /// <param name="a_animationClip">AnimationClip for the animation.</param>
    private void AddAnimationClip(string a_animationId, GameObject a_gameObject, AnimationClip a_animationClip)
    {
        a_animationClip.name = AnimationName;
        a_animationClip.legacy = true;
        if (a_gameObject.TryGetComponent(out Animation animation))
        {
            animation.Stop();
            animation.enabled = true;
        }
        else
        {
            animation = a_gameObject.AddComponent<Animation>();
        }
        animation.clip = a_animationClip;
        animation.AddClip(a_animationClip, a_animationClip.name);
        animation[AnimationName].time = 0f;

        if (m_animationsCollection.ContainsKey(a_animationId))
            m_animationsCollection[a_animationId].Add(animation);
        else
            m_animationsCollection[a_animationId] = new List<Animation>() { animation };

        if (m_replayPanel.AnimationEntitySelect.options.TrueForAll(option => option.text != a_animationId))
            m_replayPanel.AnimationEntitySelect.options.Add(new TMP_Dropdown.OptionData(a_animationId));
    }

    private static float GetDataPointTime<T>(DateTime a_start, T a_spatialLoggingDataPoint) where T : SpatialLoggingDataPoint
    {
        var pointInTime = a_spatialLoggingDataPoint.pointInTimeUtc;
        var timeFromStart = pointInTime - a_start;
        var time = (float)timeFromStart.TotalSeconds;
        return time;
    }
}

/// <summary>
/// Todo
/// https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
/// </summary>
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}





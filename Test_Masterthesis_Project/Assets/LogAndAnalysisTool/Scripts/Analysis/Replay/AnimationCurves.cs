using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collection of animation curves vor vector3 and quaternion.
/// </summary>
public class AnimationCurves
{
    private Dictionary<char, AnimationCurve> m_curves = new Dictionary<char, AnimationCurve>();

    private bool m_isQuaternion;

    public string PropertyName { get; }

    public AnimationCurves(bool a_isQuaternion, string a_propertyName)
    {
        PropertyName = a_propertyName;
        m_isQuaternion = a_isQuaternion;
        m_curves['x'] = new AnimationCurve();
        m_curves['y'] = new AnimationCurve();
        m_curves['z'] = new AnimationCurve();
        if (m_isQuaternion)
            m_curves['w'] = new AnimationCurve();
    }

    public void AddKeyFrame(float a_time, Vector3 a_vector)
    {
        if (!m_isQuaternion)
        {
            //in tangent 0 out tangent float.PositiveInfinity to achieve tangent mode constant (no interpolation between frames)
            m_curves['x'].AddKey(new Keyframe(a_time, a_vector.x, 0, float.PositiveInfinity));
            m_curves['y'].AddKey(new Keyframe(a_time, a_vector.y, 0, float.PositiveInfinity));
            m_curves['z'].AddKey(new Keyframe(a_time, a_vector.z, 0, float.PositiveInfinity));
        }
        else
        {
            Debug.LogError("Cannot add keyframe for vector3 to a quaternion AnimationCurves.");
        }
    }

    public void AddKeyFrame(float a_time, Quaternion a_quaternion)
    {
        if (m_isQuaternion)
        {
            //in tangent 0 out tangent float.PositiveInfinity to achieve tangent mode constant (no interpolation between frames)
            m_curves['x'].AddKey(new Keyframe(a_time, a_quaternion.x, 0, float.PositiveInfinity));
            m_curves['y'].AddKey(new Keyframe(a_time, a_quaternion.y, 0, float.PositiveInfinity));
            m_curves['z'].AddKey(new Keyframe(a_time, a_quaternion.z, 0, float.PositiveInfinity));
            m_curves['w'].AddKey(new Keyframe(a_time, a_quaternion.w, 0, float.PositiveInfinity));
        }
        else
        {
            Debug.LogError("Cannot add keyframe for quaternion to a vector3 AnimationCurves.");
        }
    }

    public void AddKeyFrames(char a_dimensionName, params Keyframe[] a_keyframes)
    {
        if (m_curves.TryGetValue(a_dimensionName, out var curve))
        {
            curve.keys = a_keyframes;
        }
        else
        {
            Debug.LogErrorFormat("Dimension name {0} does not exist.", a_dimensionName);
        }
    }

    public void SetCurvesToAnimationClip(AnimationClip a_animationClip)
    {
        SetCurvesToAnimationClip(PropertyName, a_animationClip, "", typeof(Transform));
    }

    public int GetKeyframesCount() 
    {
        var count = 0;

        foreach (var curveKvp in m_curves)
        {
            count += curveKvp.Value.keys.Length;
        }

        return count;
    }

    private void SetCurvesToAnimationClip(string a_propertyName, AnimationClip a_animationClip, string a_relativePath, Type a_type)
    {
        foreach (var curveKvp in m_curves)
        {
            SetAnimationClipCurve(curveKvp.Key, a_propertyName, a_animationClip, a_relativePath, a_type);
        }
    }

    private void SetAnimationClipCurve(char a_dimensionName, string a_propertyName, AnimationClip a_animationClip, string a_relativePath, Type a_type)
    {
        if (m_curves.TryGetValue(a_dimensionName, out var curve))
        {
            a_animationClip.SetCurve(a_relativePath, a_type, string.Format("{0}.{1}", a_propertyName, a_dimensionName), curve);
        }
        else
        {
            Debug.LogErrorFormat("Dimension name {0} does not exist.", a_dimensionName);
        }
    }
}
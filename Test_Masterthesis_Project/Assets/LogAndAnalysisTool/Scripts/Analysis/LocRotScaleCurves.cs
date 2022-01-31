using System.Collections.Generic;
using UnityEngine;

public class LocRotScaleCurves
{
    private Dictionary<string, AnimationCurves> m_locRotScaleCurves = new Dictionary<string, AnimationCurves>();

    private const string PositionKey = "localPosition";

    private const string RotationKey = "localRotation";

    private const string ScaleKey = "localScale";

    public LocRotScaleCurves(bool a_location = false, bool a_rotation = false, bool a_scale = false)
    {
        if (a_location)
            m_locRotScaleCurves[PositionKey] = new AnimationCurves(false, PositionKey);
        if (a_rotation)
            m_locRotScaleCurves[RotationKey] = new AnimationCurves(true, RotationKey);
        if (a_scale)
            m_locRotScaleCurves[ScaleKey] = new AnimationCurves(false, ScaleKey);
    }

    public void SetCurvesToAnimationClip(AnimationClip a_animationClip)
    {
        foreach (var locRotScaleCurves in m_locRotScaleCurves)
        {
            locRotScaleCurves.Value.SetCurvesToAnimationClip(a_animationClip);
        }
    }

    public AnimationCurves GetLocationCurves()
    {
        if (TryGetCurves(PositionKey, out var a_animationCurves))
        {
            return a_animationCurves;
        }
        return null;
    }

    public AnimationCurves GetRotationCurves()
    {
        if (TryGetCurves(RotationKey, out var a_animationCurves))
        {
            return a_animationCurves;
        }
        return null;
    }

    public AnimationCurves GetScaleCurves()
    {
        if (TryGetCurves(ScaleKey, out var a_animationCurves))
        {
            return a_animationCurves;
        }
        return null;
    }

    public bool HasKeyframes()
    {
        return GetKeyframesCount() != 0;
    }

    private int GetKeyframesCount()
    {
        var count = 0;

        foreach (var locRotScaleCurveKvp in m_locRotScaleCurves)
        {
            count += locRotScaleCurveKvp.Value.GetKeyframesCount();
        }

        return count;
    }

    private bool TryGetCurves(string a_propertyName, out AnimationCurves a_animationCurves)
    {
        if (!m_locRotScaleCurves.TryGetValue(a_propertyName, out a_animationCurves))
        {
            Debug.LogErrorFormat("No curves for {0}", a_propertyName);
            return false;
        }

        return true;
    }

}

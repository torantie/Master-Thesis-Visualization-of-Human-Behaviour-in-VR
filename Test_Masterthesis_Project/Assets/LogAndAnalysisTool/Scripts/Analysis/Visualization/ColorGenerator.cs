using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    private const float MaxHueDegree = 1;

    private float CurrentHueDegree { get; set; } = 0;

    private float HueDegreeStepSize { get; set; } = 0.1f;

    private int NumberOfObjectGroups { get; set; }

    public ColorGenerator(int a_numberOfObjectGroups)
    {
        NumberOfObjectGroups = a_numberOfObjectGroups;
        HueDegreeStepSize = MaxHueDegree / NumberOfObjectGroups;
    }

    public static ColorMapping GetColorMappings(List<string> a_ids)
    {
        var colorMappings = new ColorMapping();
        var numberOfObjectGroups = a_ids.Count;
        var hueDegreeStepSize = MaxHueDegree / numberOfObjectGroups;
        var currentHueDegree = 0f;

        foreach (var id in a_ids)
        {
            if (currentHueDegree != MaxHueDegree)
                currentHueDegree += hueDegreeStepSize;
            else
                currentHueDegree = MaxHueDegree;

            colorMappings[id] = Color.HSVToRGB(currentHueDegree, 1, 1);
        }

        return colorMappings;
    }

    public Color GetColor()
    {
        var hueDegree = GetCurrentHueDegree();
        return Color.HSVToRGB(hueDegree, 1, 1);
    }

    public void Reset(int a_numberOfObjectGroups)
    {
        NumberOfObjectGroups = a_numberOfObjectGroups;
        HueDegreeStepSize = MaxHueDegree / NumberOfObjectGroups;
        ResetCurrentHueDegree();
    }

    private void ResetCurrentHueDegree()
    {
        CurrentHueDegree = 0;
    }

    private float GetCurrentHueDegree()
    {
        var currentHueDegree = CurrentHueDegree;

        if (CurrentHueDegree != MaxHueDegree)
            CurrentHueDegree += HueDegreeStepSize;
        else
            CurrentHueDegree = MaxHueDegree;

        Debug.Log(string.Format("currentHueDegree: {0} HueDegreeStepSize: {1}", currentHueDegree, HueDegreeStepSize));

        return currentHueDegree;
    }

}

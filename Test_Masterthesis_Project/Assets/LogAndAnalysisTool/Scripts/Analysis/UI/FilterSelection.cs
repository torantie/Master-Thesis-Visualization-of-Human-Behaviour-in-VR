using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterSelection
{
    public FilterSelection(FilterCategory a_category, List<string> a_selectedItems)
    {
        Category = a_category;
        SelectedItems = a_selectedItems;
    }

    public FilterCategory Category { get; private set; }

    public List<string> SelectedItems { get; private set; }
}

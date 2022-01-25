using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitledCheckBoxScrollView<T> : TitledScrollView<T, CheckboxScrollViewContentHandler>
{
    public (T title, List<string> selectedItems) GetSelection()
    {
        return (Title, ContentFiller.GetSelectedItems());
    }

    public (T title, string selectedItem) GetSingleSelection()
    {
        return (Title, ContentFiller.GetSelectedItem());
    }
}

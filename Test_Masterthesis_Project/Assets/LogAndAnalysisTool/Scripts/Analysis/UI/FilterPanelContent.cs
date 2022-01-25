using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilterPanelContent : MonoBehaviour
{
    [SerializeField]
    private GameObject m_scrollViewCollectionContainerPrefab;

    [SerializeField]
    private GameObject m_filterCategoryPrefab;

    public Dictionary<VisualizationType, UIScrollViewCollectionContainer> FilterCategoriesCollection { get; private set; } = new Dictionary<VisualizationType, UIScrollViewCollectionContainer>();

    public void AddFilterCategories(VisualizationType a_visualization, Dictionary<FilterCategory, List<string>> a_filterSelections)
    {
        if (!FilterCategoriesCollection.ContainsKey(a_visualization))
        {
            UIScrollViewCollectionContainer uiFilterCategories = CreateUIFilterCategories(a_visualization);
            foreach (var filterSelectionKvp in a_filterSelections)
            {
                AddFilterCategory(filterSelectionKvp, uiFilterCategories.ScrollViewCollectionContainer.transform);
            }
            Debug.LogFormat("Filter categories for visualization {0} does not already exist. Create new filter categories object.", a_visualization);
        }
        else
        {
            FilterCategoriesCollection[a_visualization].gameObject.SetActive(true);
            Debug.LogFormat("Filter categories for visualization {0} already exist. Set active.", a_visualization);
        }
    }

    public void RemoveFilterCategories(VisualizationType a_visualization)
    {
        if (FilterCategoriesCollection.TryGetValue(a_visualization, out var filterCategories))
        {
            filterCategories.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogFormat("Filter categories for visualization {0} already not visible.", a_visualization);
        }
    }

    public void HideAllFilterCategories()
    {
        foreach (var filterCategoriesCollectionKvp in FilterCategoriesCollection)
        {
            filterCategoriesCollectionKvp.Value.gameObject.SetActive(false);
        }
    }

    public List<VisualizationFilterSelection> GetVisualizationFilterSelection()
    {
        var visualizationFilterSelections = new List<VisualizationFilterSelection>();
        foreach (var filterCategoriesCollectionKvp in FilterCategoriesCollection)
        {
            if (filterCategoriesCollectionKvp.Value.gameObject.activeSelf)
            {
                Debug.LogFormat("Include {0} in selection.", filterCategoriesCollectionKvp.Key);
                var uiFilterCategories = filterCategoriesCollectionKvp.Value.ActiveUIFilterCategories;
                if (uiFilterCategories != null)
                {
                    var filterSelections = new Dictionary<FilterCategory, List<string>>();
                    foreach (var uiFilterCategory in uiFilterCategories)
                    {
                        var (title, selectedItems) = uiFilterCategory.GetSelection();
                        filterSelections[title] = selectedItems;
                    }
                    foreach (var loggingDataPointType in filterSelections[FilterCategory.EventType].ParseToEnumCollection<LoggingDataPointType>())
                    {
                        var filterSelections2 = filterSelections;
                        //Only add the selected sentiment filters for sentimentevents
                        if (!loggingDataPointType.Equals(LoggingDataPointType.SentimentEvent)
                            && filterSelections.ContainsKey(FilterCategory.Sentiment))
                        {
                            filterSelections2 = new Dictionary<FilterCategory, List<string>>(filterSelections);
                            filterSelections2.Remove(FilterCategory.Sentiment);
                        }

                        visualizationFilterSelections.Add(new VisualizationFilterSelection(filterCategoriesCollectionKvp.Key, filterSelections2, loggingDataPointType));
                    }

                }
                else
                {
                    Debug.LogError("No UIFilterCategory scripts attached to children.");
                }
            }
        }
        return visualizationFilterSelections;
    }

    private UIScrollViewCollectionContainer CreateUIFilterCategories(VisualizationType a_visualization)
    {
        var uiFilterCategoriesObject = Instantiate(m_scrollViewCollectionContainerPrefab, transform);
        var uiFilterCategories = uiFilterCategoriesObject.GetComponent<UIScrollViewCollectionContainer>();
        uiFilterCategories.SetHeader(a_visualization.ToFriendlyString());
        FilterCategoriesCollection[a_visualization] = uiFilterCategories;
        return uiFilterCategories;
    }

    private void AddFilterCategory(KeyValuePair<FilterCategory, List<string>> a_filterSelection, Transform a_parent)
    {
        if (a_filterSelection.Value.Count == 0)
        {
            Debug.LogFormat("No entries to display for FilterCategory {0}.", a_filterSelection.Key);
            return;
        }

        var uiFilterCategory = Instantiate(m_filterCategoryPrefab, a_parent).GetComponent<UIFilterCategory>();
        uiFilterCategory.SetContent(a_filterSelection.Key, a_filterSelection.Value);

        if (a_filterSelection.Key.Equals(FilterCategory.EventType))
        {
            uiFilterCategory.ContentFiller.OnToggleChange.AddListener(OnEventTypeSelectionChanged);
        }
    }

    private void OnEventTypeSelectionChanged(GameObject a_gameObject, string a_text, bool a_on)
    {
        try
        {
            if (a_text.TryParseToEnum<LoggingDataPointType>(out var loggingDataPointType))
            {
                var container = FindFilterCategoryContentsContainer(a_gameObject);
                var containerVisualizationType = container.Title.ParseToVisualizationType();
                switch (loggingDataPointType)
                {
                    case LoggingDataPointType.SentimentEvent:
                        if (containerVisualizationType.Equals(VisualizationType.HeatMap))
                        {
                            var filterCategory = container.UIFilterCategories.FirstOrDefault(filterCategory => filterCategory.Title.Equals(FilterCategory.Sentiment));
                            if (filterCategory != default)
                                filterCategory.gameObject.SetActive(a_on);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private UIScrollViewCollectionContainer FindFilterCategoryContentsContainer(GameObject a_filterCategoryContentObject)
    {
        foreach (var filterCategoriesCollectionKvp in FilterCategoriesCollection)
        {
            var uiFilterCategories = filterCategoriesCollectionKvp.Value.UIFilterCategories;
            if (uiFilterCategories != null)
            {
                var filterSelections = new Dictionary<FilterCategory, List<string>>();
                foreach (var uiFilterCategory in uiFilterCategories)
                {
                    if (uiFilterCategory.ContentFiller.gameObject.Equals(a_filterCategoryContentObject))
                    {
                        return filterCategoriesCollectionKvp.Value;
                    }
                }
            }
        }

        return null;
    }

}

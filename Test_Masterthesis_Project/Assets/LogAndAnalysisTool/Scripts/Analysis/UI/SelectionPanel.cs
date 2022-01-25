using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class SelectionPanel : MenuPanel
{
    [SerializeField]
    private TextMeshProUGUI m_selectionTextContent;

    [SerializeField]
    private GameObject m_audioPlayButtonPrefab;

    [SerializeField]
    private GameObject m_additionalContent;


    public void SetContent(VisualDataPoint a_visualDataPoint)
    {
        var loggingDataPoint = a_visualDataPoint.LoggingDataPoint;
        var fieldInformation = SerializationHelper.FieldsToString(loggingDataPoint);
        m_selectionTextContent.text = fieldInformation;

        ClearAdditionalContent();

        Debug.LogFormat("Set SelectionPanel text content to {0}.", fieldInformation);
        try
        {
            switch (loggingDataPoint.Type)
            {
                case LoggingDataPointType.ParticipantTrackingData:
                    var participantTrackingData = loggingDataPoint as ParticipantTrackingDataPoint;
                    break;
                case LoggingDataPointType.SpatialAudioEvent:
                    var audioEvent = loggingDataPoint as SpatialAudioEventDataPoint;
                    SetAudioEventContent(a_visualDataPoint, audioEvent);
                    break;
                case LoggingDataPointType.OfflineSpatialAudioEvent:
                    var offlineAudioEvent = loggingDataPoint as OfflineSpatialAudioEventDataPoint;
                    SetAudioEventContent(a_visualDataPoint, offlineAudioEvent);
                    break;
                case LoggingDataPointType.InputActionEvent:
                    var inputActionEvent = loggingDataPoint as SpatialAudioEventDataPoint;
                    break;
                case LoggingDataPointType.CustomSpatialEvent:
                    var customEvent = loggingDataPoint as CustomSpatialEventDataPoint;
                    break;
                case LoggingDataPointType.TransformTrackingData:
                    var transformTrackingData = loggingDataPoint as TransformTrackingDataPoint;
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private void SetAudioEventContent(VisualDataPoint a_visualDataPoint, SpatialAudioEventDataPoint a_audioEvent)
    {
        if (a_visualDataPoint.TryGetComponent<AudioPlayer>(out var audioPlayer))
        {
            var playButton = Instantiate(m_audioPlayButtonPrefab, m_additionalContent.transform);
            if (playButton.TryGetComponent<AudioPlayButton>(out var audioPlayButton))
            {
                audioPlayButton.AudioPlayer = audioPlayer;
                audioPlayButton.AudioPlayer.AudioEventDataPoint = a_audioEvent;
            }
        }
    }

    private void ClearAdditionalContent()
    {
        int childs = m_additionalContent.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            Destroy(m_additionalContent.transform.GetChild(i).gameObject);
        }
    }

}

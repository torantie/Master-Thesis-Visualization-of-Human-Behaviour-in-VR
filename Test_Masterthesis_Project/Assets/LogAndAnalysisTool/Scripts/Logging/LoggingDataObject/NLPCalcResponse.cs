using System;

[Serializable]
public class NLPCalcResponse : SpatialAudioEventDataPoint
{
    public SentenceSentiment[] sentenceSentiments;

    public WordOccurrence[] wordOccurrences;


}


/// <summary>
/// Class copied in python nlp tool.
/// </summary>
[Serializable]
public class SentenceSentiment
{
    public string sentence;

    public Sentiment sentiment;
}

/// <summary>
/// Class copied in python nlp tool.
/// </summary>
[Serializable]
public class WordOccurrence
{
    public string word;

    public int occurrence;
}

/// <summary>
/// Enum copied in python nlp tool.
/// </summary>
[Serializable]
public enum Sentiment
{
    None, Positive, Negative, Neutral
}

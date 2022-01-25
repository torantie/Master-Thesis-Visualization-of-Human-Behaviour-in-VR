using CsvHelper.Configuration;
using System;

namespace SpeechToText
{
    public class AudioEventDataPoint
    {
        public Guid DataPointGuid { get; set; } = Guid.NewGuid();

        public string SessionId { get; set; }

        public string ParticipantId { get; set; }

        public string TaskId { get; set; }

        public DateTime PointInTimeUtc { get; set; }

        public DateTime EndPointInTimeUtc { get; set; }

        public string SpokenText { get; set; }

        public float ConfidenceLevel { get; set; }

        public long AudioPositionTicks { get; set; }

        public long AudioDurationTicks { get; set; }

        public string AudioFileName { get; set; }
    }

    /// <summary>
    /// Enum copied in python nlp tool.
    /// </summary>
    [Serializable]
    public enum Sentiment
    {
        None, Positive, Negative, Neutral
    }

    public class AudioEventDataPointMap : ClassMap<AudioEventDataPoint>
    {
        public AudioEventDataPointMap()
        {
            Map(m => m.DataPointGuid).Index(0).Name("dataPointGuid");
            Map(m => m.SessionId).Index(1).Name("sessionId");
            Map(m => m.ParticipantId).Index(2).Name("participantId");
            Map(m => m.TaskId).Index(3).Name("taskId");
            Map(m => m.PointInTimeUtc).Index(4).Name("pointInTimeUtc").TypeConverterOption.Format("o");
            Map(m => m.EndPointInTimeUtc).Index(5).Name("endPointInTimeUtc").TypeConverterOption.Format("o");
            Map(m => m.SpokenText).Index(6).Name("spokenText");
            Map(m => m.ConfidenceLevel).Index(7).Name("confidenceLevel");
            Map(m => m.AudioPositionTicks).Index(8).Name("audioPositionTicks");
            Map(m => m.AudioDurationTicks).Index(9).Name("audioDurationTicks");
            Map(m => m.AudioFileName).Index(10).Name("audioFileName");
        }
    }
}

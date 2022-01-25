using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Threading;

namespace SpeechToText
{
    class SpeechRecognizer
    {
        private bool m_completed;

        private List<AudioEventDataPoint> m_audioEventDataPoints = new List<AudioEventDataPoint>();

        private RecordingInfo m_recordingInfo;

        private RecordingSession m_currentRecordingSession;

        private readonly FileInfo m_audioFile;

        private readonly DirectoryInfo m_participantDirectory;


        public SpeechRecognizer(FileInfo a_audioFile, DirectoryInfo a_participantDirectory)
        {
            m_audioFile = a_audioFile;
            m_participantDirectory = a_participantDirectory;
        }

        public void StartSpeechRecognition()
        {

            using (var streamReader = new StreamReader(Path.Combine(m_audioFile.Directory.FullName, "RecordingInfo.json")))
            {
                var fileContent = streamReader.ReadToEnd();
                m_recordingInfo = JsonConvert.DeserializeObject<RecordingInfo>(fileContent);
            }
            bool foundClip = false;
            foreach (var recordingSession in m_recordingInfo.recordingSessions)
            {
                var fileNameClipIdString = m_audioFile.Name.Split('_')[1].Split('.')[0];
                if (int.TryParse(fileNameClipIdString, out var fileNameClipId))
                {
                    foreach (var clipId in recordingSession.clipIds)
                    {
                        if (clipId == fileNameClipId)
                        {
                            m_currentRecordingSession = recordingSession;
                            foundClip = true;
                            break;
                        }
                    }
                }

                if (foundClip)
                    break;
            }

            using (var recognizer = new SpeechRecognitionEngine(new CultureInfo("de-de")))
            {
                // Create and load a grammar.  
                Grammar dictation = new DictationGrammar();
                dictation.Name = "Dictation Grammar";

                recognizer.LoadGrammar(dictation);

                // Configure the input to the recognizer.  
                recognizer.SetInputToWaveFile(m_audioFile.FullName);

                // Attach event handlers for the results of recognition.  
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
                recognizer.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(OnRecognizeCompleted);

                // Perform recognition on the entire file.  
                Console.WriteLine("Starting asynchronous recognition of file {0}.", m_audioFile.FullName);
                m_completed = false;
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                // Keep the console window open.  
                while (!m_completed)
                {
                }
                Console.WriteLine("Done.");
            }
        }

        // Handle the SpeechRecognized event.  
        void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var result = e.Result;

            if (result != null && result.Text != null)
            {
                var confidence = result.Confidence;
                var text = result.Text;
                var audioPosition = result.Audio.AudioPosition.Ticks;
                var audioDuration = result.Audio.Duration.Ticks;
                var startDateTime = m_currentRecordingSession.RecordingStart.Add(result.Audio.AudioPosition);
                var audioEventDataPoint = new AudioEventDataPoint()
                {
                    AudioDurationTicks = audioDuration,
                    AudioPositionTicks = audioPosition,
                    SpokenText = text,
                    ConfidenceLevel = confidence,
                    AudioFileName = m_audioFile.Name,
                    SessionId = m_currentRecordingSession.sessionId,
                    ParticipantId = m_currentRecordingSession.participantId,
                    TaskId = m_currentRecordingSession.taskId,
                    PointInTimeUtc = startDateTime/*.ToString("MM/dd/yyyy HH:mm:ss.fff")*/,
                    EndPointInTimeUtc = startDateTime.Add(result.Audio.Duration)/*.ToString("MM/dd/yyyy HH:mm:ss.fff")*/,
                };

                m_audioEventDataPoints.Add(audioEventDataPoint);
                Console.WriteLine("  Recognized text =  {0}", text);
            }
            else
            {
                Console.WriteLine("  Recognized text not available.");
            }
        }

        // Handle the RecognizeCompleted event.  
        void OnRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("  Error encountered, {0}: {1}",
                e.Error.GetType().Name, e.Error.Message);
            }
            if (e.Cancelled)
            {
                Console.WriteLine("  Operation cancelled.");
            }
            if (e.InputStreamEnded)
            {
                Console.WriteLine("  End of stream encountered.");
            }

            var csvConfig = new CsvConfiguration(Thread.CurrentThread.CurrentCulture);
            var path = Path.Combine(m_participantDirectory.FullName, "CSV", "OfflineAudioData.csv");

            if (File.Exists(path))
            {
                csvConfig.HasHeaderRecord = false;

                Console.WriteLine("File already exists append audio event datapoints.");
                using (var csvWriter = new CsvWriter(new StreamWriter(path, true), csvConfig))
                {
                    csvWriter.Context.RegisterClassMap<AudioEventDataPointMap>();
                    csvWriter.WriteRecords(m_audioEventDataPoints);
                }
            }
            else
            {
                using (var csvWriter = new CsvWriter(new StreamWriter(path), csvConfig))
                {
                    csvWriter.Context.RegisterClassMap<AudioEventDataPointMap>();
                    csvWriter.WriteHeader<AudioEventDataPoint>();
                    csvWriter.NextRecord();
                    csvWriter.WriteRecords(m_audioEventDataPoints);
                }
            }

            m_completed = true;
        }
    }
}

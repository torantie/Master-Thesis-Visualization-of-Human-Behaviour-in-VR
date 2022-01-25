using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Threading;

namespace SpeechToText
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss.fff";
            Thread.CurrentThread.CurrentCulture = culture;
            Console.WriteLine(DateTime.UtcNow); 
            Console.WriteLine(DateTime.Now);

            Console.WriteLine("Enter study directory path.");
            var readLine = Console.ReadLine();
            DirectoryInfo directoryInfo = new DirectoryInfo(readLine);
            var studyDirectories = directoryInfo.GetDirectories();

            foreach (var sessionDir in studyDirectories)
            {
                foreach (var participantDirectory in sessionDir.GetDirectories())
                {
                    foreach (var audioDirectories in participantDirectory.GetDirectories("Audio"))
                    {
                        foreach (var audioFile in audioDirectories.GetFiles("*.wav"))
                        {
                            var speechRecognizer = new SpeechRecognizer(audioFile, participantDirectory);
                            speechRecognizer.StartSpeechRecognition();
                        }
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        
    }
}

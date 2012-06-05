using System;
using System.IO;
using System.Threading;
using LumenWorks.Framework.IO.Csv;
using NLog;

namespace Gelf4NLog.ConsoleRunner
{
    class Program
    {
        private static readonly Random Random = new Random();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            while (true)
            {
                var comic = GetNextComic();

                var eventInfo = new LogEventInfo
                                    {
                                        Message = comic.Title,
                                        Level = LogLevel.Info,
                                    };
                eventInfo.Properties.Add("Publisher", comic.Publisher);
                eventInfo.Properties.Add("ReleaseDate", comic.ReleaseDate);

                Logger.Log(eventInfo);

                Thread.Sleep(1000);
            }
        }

        private static Comic GetNextComic()
        {
            var nextComicIndex = Random.Next(1, 400);

            using (var csv = new CsvReader(new StreamReader("comics.csv"), false))
            {
                csv.MoveTo(nextComicIndex);
                return new Comic
                {
                    Title = csv[2],
                    Publisher = csv[1],
                    ReleaseDate = csv[0]
                };
            }
        }
    }

    class Comic
    {
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Publisher { get; set; }
    }
}

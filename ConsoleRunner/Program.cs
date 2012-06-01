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
                var nextComic = GetNextComic();

                var eventInfo = new LogEventInfo
                                    {
                                        Message = nextComic,
                                        Level = LogLevel.Info,
                                    };
                Logger.Log(eventInfo);

                Thread.Sleep(1000);
            }
        }

        private static string GetNextComic()
        {
            var nextComicIndex = Random.Next(1, 400);

            using (var csv = new CsvReader(new StreamReader("comics.csv"), false))
            {
                csv.MoveTo(nextComicIndex);
                return string.Format("{0} from {1} was released on {2}", csv[2], csv[1], csv[0]);
            }
        }
    }
}

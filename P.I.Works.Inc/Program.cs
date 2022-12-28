using P.I.Works.Inc;
using System.Diagnostics;
using System.Globalization;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Start working...");

        var FilePath = "exhibitA-input.csv";
        var Header = true;
        DateTime? FilterDate = DateTime.Parse("2016-08-10", CultureInfo.InvariantCulture);

        Console.WriteLine("Defalut values are:");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(@"file=exhibitA-input.csv header=True filterDate=2016-08-10");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine("You can use diffrent parameters using this pattern:");
        Console.WriteLine(@"[Sample:: P.I.Works.Inc.exe file=c:\data.csv header=true filterDate=2016-08-10|All]");
        Console.WriteLine("--------------------------------------------------------");
        Console.WriteLine();

        if (args.Any(x => x.StartsWith("file")))
        {
            FilePath = args.First(x => x.StartsWith("file")).Split('=')[1].Split('=')[1];
        }

        if (!File.Exists(FilePath))
        {
            Console.WriteLine(@"Missing file path argument or file does not exists[Sample:: P.I.Works.Inc.exe file=c:\data.csv header=false filterDate=2016-08-10|All]");

            return;
        }

        if (args.Any(x => x.StartsWith("header")))
        {
            try
            {
                Header = Convert.ToBoolean(args.First(x => x.StartsWith("header")).Split('=')[1]);
            }
            catch
            {
                Console.WriteLine(@"Wrong format of header argument [Sample:: P.I.Works.Inc.exe file=c:\data.csv header=false filterDate=2016-08-10|All]");

                return;
            }
        }

        if (args.Any(x => x.ToLower().StartsWith("filterdate")))
        {

            try
            {
                if (args.First(x => x.ToLower().StartsWith("filterdate")).Split('=')[1].ToLower() == "all")
                {
                    FilterDate = null;
                }
                else
                    FilterDate = Convert.ToDateTime(args.First(x => x.ToLower().StartsWith("filterdate")).Split('=')[1]);
            }
            catch
            {
                Console.WriteLine(@"Wrong format of filterDate argument [Sample:: P.I.Works.Inc.exe file:c:\data.csv header:false filterDate:2016-08-10|All]");

                return;
            }
        }


        ExhibitEntityCollection exhibitEntities = new ExhibitEntityCollection();

        int counter = 0;

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        using (StreamReader reader = new StreamReader(System.IO.File.OpenRead(FilePath)))
        {
            Console.WriteLine("Strat reading data from flat file...");

            if (Header)
                reader.ReadLine();

            while (!reader.EndOfStream)
            {
                exhibitEntities.ReadFromDelimitedString('\t', row: reader.ReadLine()).Wait();

                counter++;

#if DEBUG
                if (counter == 1000)
                    break;
#endif
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Finish reading {counter} rows from flat file in [{stopwatch.Elapsed}].");
            Console.ForegroundColor = ConsoleColor.White;

            stopwatch.Reset();

            stopwatch.Start();

            Console.WriteLine();

            Console.WriteLine("Strat processing data...");

            var Result = exhibitEntities.DistributionDistinctSongPlay(FilterDate);

            var DisplayDate = FilterDate.HasValue ? " for date " + FilterDate.Value.ToLongDateString() : "";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Finish processing {exhibitEntities.FiltredDataCount} rows in [{stopwatch.Elapsed}]{DisplayDate}.");
            Console.ForegroundColor = ConsoleColor.White;

            var FileName = $"result-{DateTime.Now.Ticks}.csv";

            using (StreamWriter writer = new StreamWriter(FileName))
            {
                writer.WriteLine(Result);
            }

            Console.WriteLine();

            Console.WriteLine($"Result is saving as {FileName}.");
        }
    }
}
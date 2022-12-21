using P.I.Works.Inc;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Start working...");

        var FilePath = "exhibitA-input.csv";
        var Header = true;
        DateTime? FilterDate = null;


        if ( args.Length == 0)
        {
            Console.WriteLine("Defalut values are:");
            Console.WriteLine(@"file=exhibitA-input.csv header=false filterDate=All");
            Console.WriteLine();
            Console.WriteLine("You can use diffrent parameters using this pattern:");
            Console.WriteLine(@"[Sample:: P.I.Works.Inc.exe file=c:\data.csv header=true filterDate=2016-08-08]");
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine();
        }
        else
        {
            if (args.Any(x=>x.StartsWith("file")))
            {
                FilePath = args[0].Split('=')[1];

                if (!File.Exists(FilePath))
                {
                    Console.WriteLine(@"Missing file path argument or file does not exists[Sample:: P.I.Works.Inc.exe file=c:\data.csv header=false filterDate=2016-08-8]");

                    return;
                }
            }
            else
            {
                Console.WriteLine(@"Missing file path argument [Sample:: P.I.Works.Inc.exe file=c:\data.csv header=false filterDate=2016-08-8]""");

                return;
            }

            if (args.Any(x => x.StartsWith("header")))
            {
                try
                {
                    Header = Convert.ToBoolean(args[1].Split('=')[1]);
                }
                catch
                {
                    Console.WriteLine(@"Wrong format of header argument [Sample:: P.I.Works.Inc.exe file=c:\data.csv header=false filterDate=2016-08-8]");

                    return;
                }
            }
            if (args.Any(x => x.StartsWith("filterDate")))
            {
                try
                {
                    FilterDate = Convert.ToDateTime(args[2].Split('=')[1]);
                }
                catch
                {
                    Console.WriteLine(@"Wrong format of filterDate argument [Sample:: P.I.Works.Inc.exe file:c:\data.csv header:false filterDate:2016-08-8]");

                    return;
                }
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
                if (counter == 10000)
                    break;
#endif
            }

            Console.WriteLine($"Finish reading {counter} rows from flat file in {stopwatch.Elapsed}.");

            stopwatch.Reset();

            stopwatch.Start();

            Console.WriteLine("Strat processing data...");

            var Result = exhibitEntities.DistributionDistinctSongPlay(FilterDate);

            Console.WriteLine($"Finish processing rows in {stopwatch.Elapsed}.");

            using (StreamWriter writer = new StreamWriter("result.csv"))
            {
                writer.WriteLine(Result);
            }

            Console.WriteLine($"Result is saving as result.csv.");
        }
    }
}
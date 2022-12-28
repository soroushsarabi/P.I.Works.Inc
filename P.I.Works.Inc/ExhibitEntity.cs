using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.I.Works.Inc
{
    internal class ExhibitEntity
    {
        public ExhibitEntity()
        {

        }
        public ExhibitEntity(string pLAY_ID, int sONG_ID, int cLIENT_ID, DateTime pLAY_TS)
        {
            PLAY_ID = pLAY_ID;
            SONG_ID = sONG_ID;
            CLIENT_ID = cLIENT_ID;
            PLAY_TS = pLAY_TS;
        }

        public string PLAY_ID { get; set; }
        public int SONG_ID { get; set; }

        public int CLIENT_ID { get; set; }

        public DateTime PLAY_TS { get; set; }
    }

    internal class ExhibitEntityCollection : List<ExhibitEntity>
    {
        public int FiltredDataCount { get; set; }
        public async Task<bool> ReadFromDelimitedString(char seperator, string row)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Temp = row.Split(seperator);

                    if (Temp.Length < 4)
                    {
                        throw new Exception($"Parameter count failed. {row}");
                    }

                    this.Add(new ExhibitEntity { PLAY_ID = Temp[0], SONG_ID = int.Parse(Temp[1]), CLIENT_ID = int.Parse(Temp[2]), PLAY_TS = DateTime.ParseExact(Temp[3].Split(' ')[0], "dd/MM/yyyy", CultureInfo.InvariantCulture) });

                    return true;
                }
                catch (Exception ex)
                {
                    //call log function
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(row);
                    Console.ForegroundColor = ConsoleColor.White;

                    return false;
                }

            });
        }

        public DistributionDistinctCollection DistributionDistinctSongPlay(DateTime? filterDate)
        {
            var FiltredData = this.ToList();

            if (filterDate != null)
            {
                FiltredData = this.Where(x => x.PLAY_TS.Date == filterDate.Value.Date).ToList();
            }

            FiltredDataCount = FiltredData.Count;

            var ClientsSongsDate = FiltredData.GroupBy(x => new { x.CLIENT_ID , x.SONG_ID , x.PLAY_TS.Date }).Select(x=>new {A=x.Key.CLIENT_ID , B=x.Key.SONG_ID , C=x.Key.Date , D=x.Count()}).ToList();

            var ClientDate = ClientsSongsDate.GroupBy(x => new { x.A, x.C.Date }).Select(x => new { Ra = x.Key.A, Rb = x.Count() }).ToList();

            var Result = ClientDate.GroupBy(x => x.Rb).Select(x => new { UCli = x.Key, UC = x.Count() }).OrderBy(x => x.UCli).ToList();

            DistributionDistinctCollection distributions = new DistributionDistinctCollection();

            foreach (var item in Result)
            {
                distributions.Add(new DistributionDistinct { DISTINCT_PLAY_COUNT = item.UCli , CLIENT_COUNT= item.UC });
            }

// var t = FiltredData.Where(x => x.CLIENT_ID == 249).ToList();

            return distributions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.I.Works.Inc
{
    internal class DistributionDistinct
    {
        public DistributionDistinct()
        {

        }
        public DistributionDistinct(int dISTINCT_PLAY_COUNT, int cLIENT_COUNT)
        {
            DISTINCT_PLAY_COUNT = dISTINCT_PLAY_COUNT;
            CLIENT_COUNT = cLIENT_COUNT;
        }

        public int DISTINCT_PLAY_COUNT { get; set; }

        public int CLIENT_COUNT { get; set; }

        public override string ToString()
        {
            return $"{DISTINCT_PLAY_COUNT},{CLIENT_COUNT}";
        }
    }

    internal class DistributionDistinctCollection:List<DistributionDistinct>
    {
        public override string ToString()
        {
            string Output = "DISTINCT_PLAY_COUNT,CLIENT_COUNT \r\n";

            foreach (var item in this)
            {
                Output += item + Environment.NewLine;
            }

            return Output;
        }
    }
}

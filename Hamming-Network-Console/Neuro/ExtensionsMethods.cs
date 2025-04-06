using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuro
{
    public static class ExtensionsMethods
    {
        /// <returns></returns>
        public static double NextDoubleRange(this System.Random random, double minNumber, double maxNumber, int round_num = 5)
        {
            return Math.Round(random.NextDouble() * (maxNumber - minNumber) + minNumber, round_num);
        }
    }
}

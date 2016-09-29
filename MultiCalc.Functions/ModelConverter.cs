using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiCalc.Models;

namespace MultiCalc.Functions
{
    public static class ModelConverter
    {
        public static CalculateModel ConvertStringsToCalculateModel(string factors, string particularNumberMax)
        {
            var calcModel = new CalculateModel();

            var factorList = new List<int>();
            var factorStringArray = factors.Split(',');
            foreach (var factor in factorStringArray)
            {
                factorList.Add(Convert.ToInt32(factor.Trim()));
            }

            calcModel.Factors = factorList.ToArray();

            calcModel.ParticularNumberMax = Convert.ToInt64(particularNumberMax);

            return calcModel;
        }
    }
}

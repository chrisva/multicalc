using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiCalc.Functions
{
    /// <summary>
    /// A set of methods to sum multiplies.
    /// </summary>
    public static class SumOfMultiples
    {

        /// <summary>
        /// Calculate the sum of all the multiples of particular numbers up to but not including that number. This is done in a serial approach and is better
        /// for smaller ranges and factor arrays.
        /// </summary>
        /// <param name="factors">An array of factors that will be factor in the multiplication in the range of numbers.</param>
        /// <param name="particularNumberMax">The max number in the range of particular numbers to be multiplied with the given factors.</param>
        /// <returns></returns>
        public static float To(int[] factors, long particularNumberMax)
        {
            if (particularNumberMax > 100000000)
                throw new ArgumentException("The ParticularNumberMax cannot be over 100000000. The solution is not scaled for that calculation. Contact support and order a dedicated machine for your calculations.");

            long sum = 0;

            for (long i = 0; i < particularNumberMax; i++)
            {
                if (factors.Any(factor => i % factor == 0))
                {
                    sum += i;
                }
            }

            return sum;
        }

        /// <summary>
        /// Parallell calculation of the sum of all the multiples of particular numbers up to but not including that number. This is done in a parallel approach and is better
        /// for larger ranges and factor arrays and if the multiplication is repeated on the CPU. The approach is better optimized for CPU cache.
        /// </summary>
        /// <param name="factors">An array of factors that will be factor in the multiplication in the range of numbers.</param>
        /// <param name="particularNumberMax">The max number in the range of particular numbers to be multiplied with the given factors.</param>
        /// <returns></returns>
        public static long ParallelTo(int[] factors, long particularNumberMax)
        {
            long sum = 0;

            Parallel.For(0, particularNumberMax, (i) =>
            {
                    if (factors.Any(factor => i % factor == 0))
                    {
                        sum += i;
                    }
            });

            return sum;
        }
    }
}
using System.Diagnostics;

namespace Task3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int size = 20000;
            const double e = 0.001;

            double[][] Matrix;
            double[] FreeElements;
            double[] Solutions;

            (Matrix, FreeElements, Solutions) = GenerateRandomLinearEquation(size);

            double[] WantedValues = new double[size];

            MeasureTime(() => {
                Jacobi(size, Matrix, FreeElements, ref WantedValues, e);
            });

            int[] threadNum = new int[] { 2, 4, 8, 16 };

            foreach (var NumberOfThreads in threadNum)
            {
                double[] WantedValuesParalel = new double[size];
                MeasureTime(() => {
                    JacobiParalel(size, Matrix, FreeElements, ref WantedValuesParalel, e, NumberOfThreads);
                    Console.Write(NumberOfThreads.ToString() + " Threads:   ");
                });
            }
        }

        public static int[,] CreateArrayFromTo(int size, int threadNum)
        {
            int[,] FromToArray = new int[threadNum, 2];

            int redundant = size % threadNum;
            int redundantToAdd = 0;

            for (int q = 0; q < threadNum; q++)
            {
                int from = size / threadNum * q + redundantToAdd;
                int to = size / threadNum * (q + 1) + redundantToAdd;

                if (redundant > 0)
                {
                    redundant--;
                    redundantToAdd++;
                    to++;
                }
                FromToArray[q, 0] = from;
                FromToArray[q, 1] = to;
            }
            return FromToArray;
        }

        public static void JacobiParalel(int size, double[][] coefficients, double[] values, ref double[] X, double eps, int threadNum)
        {
            int[,] parameters = CreateArrayFromTo(size, threadNum);
            double[] previousX = new double[size];
            double err;
            Thread[] threads = new Thread[threadNum];

            do
            {
                err = 0;
                double[] newValues = new double[size];

                for (int q = 0; q < threadNum; q++)
                {
                    int FromToVariable = q;
                    threads[q] = new Thread(v => {
                        for (int i = parameters[FromToVariable, 0]; i < parameters[FromToVariable, 1]; i++)
                        {
                            newValues[i] = values[i];
                            for (int j = 0; j < size; j++)
                                if (i != j) newValues[i] -= coefficients[i][j] * previousX[j];

                            newValues[i] = newValues[i] / coefficients[i][i];
                            if (Math.Abs(previousX[i] - newValues[i]) > err) err = Math.Abs(previousX[i] - newValues[i]);
                        }
                    });
                }

                foreach (var item in threads)
                    item.Start();
                foreach (var item in threads)
                    item.Join();

                previousX = newValues;
            } while (err > eps);

            X = previousX;
        }

        public static void Jacobi(int size, double[][] coefficients, double[] values, ref double[] X, double eps)
        {
            double[] previousX = new double[size];
            double err;

            do
            {
                err = 0;
                double[] newValues = new double[size];

                for (int i = 0; i < size; i++)
                {
                    newValues[i] = values[i];
                    for (int j = 0; j < size; j++)
                        if (i != j) newValues[i] -= coefficients[i][j] * previousX[j];

                    newValues[i] = newValues[i] / coefficients[i][i];
                    if (Math.Abs(previousX[i] - newValues[i]) > err) err = Math.Abs(previousX[i] - newValues[i]);
                }

                previousX = newValues;
            } while (err > eps);

            X = previousX;
        }

        public static (double[][], double[], double[]) GenerateRandomLinearEquation(int size)
        {
            Random random = new();

            List<double[]> matrix = new();
            var freeElements = new double[size];
            var solutions = new double[size];

            for (int i = 0; i < size; i++)
                solutions[i] = random.Next(1, 10);

            for (int i = 0; i < size; i++)
            {
                var toAdd = new double[size];

                for (int j = 0; j < size; j++)
                {
                    if (j == i) toAdd[j] = random.Next(100 * size, 200 * size);
                    else toAdd[j] = random.Next(1, 10);

                    freeElements[i] += toAdd[j] * solutions[j];
                }
                matrix.Add(toAdd);
            }
            return (matrix.ToArray(), freeElements, solutions);
        }

        public static void MeasureTime(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();
            long elapsedTimeMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Elapsed Time: {elapsedTimeMilliseconds} ms");
        }
    }
}
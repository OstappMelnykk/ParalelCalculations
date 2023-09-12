using System.Diagnostics;

namespace Task2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int m = 1000;
            const int n = 1000;
            const int q = 1000;

            int[,] m1 = new int[m, n];
            int[,] m2 = new int[n, q];
            int[,] m3 = new int[m, q];

            matrixFilling(m1);
            matrixFilling(m2);

            if (m1.GetLength(1) != m2.GetLength(0))
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");

            int rows1 = m1.GetLength(0);
            int cols1 = m1.GetLength(1);
            int cols2 = m2.GetLength(1);

            MeasureTime(() => {
                for (int i = 0; i < rows1; i++)
                {
                    for (int j = 0; j < cols2; j++)
                    {
                        int sum = 0;
                        for (int k = 0; k < cols1; k++)
                            sum += m1[i, k] * m2[k, j];
                        m3[i, j] = sum;
                    }
                }
            });

            Array.Clear(m3, 0, m3.Length);

            foreach (int numberOfThreads in new[] { 2, 4, 8, 16, 32, 64 })
                CalculateTimeInDefinedNumberOfThreads(numberOfThreads, m1, m2, m3);
        }


        public static void CalculateTimeInDefinedNumberOfThreads(
            int NumberOfThreads, int[,] m1, int[,] m2, int[,] m3)
        {
            Thread[] threads = new Thread[NumberOfThreads];

            int m = m1.GetLength(0);
            int n = m1.GetLength(1);
            int q = m2.GetLength(1);


            for (int i = 0; i < NumberOfThreads; i++)
            {
                int rStart = (m * i) / NumberOfThreads;
                int rEnd = (m * (i + 1)) / NumberOfThreads;
                threads[i] = (new Thread(() => BlockOfCalculation(m1, m2, m3, rStart, rEnd)));
                threads[i].Start();
            }

            MeasureTime(() => {
                foreach (var thread in threads)
                    thread.Join();
                Console.Write(NumberOfThreads.ToString() + " Threads:   ");

            });

        }

        public static void BlockOfCalculation(int[,] m1, int[,] m2, int[,] m3, int rStartM1, int rEndM1)
        {

            int m = m1.GetLength(0);
            int n = m1.GetLength(1);
            int q = m2.GetLength(1);

            for (int i = rStartM1; i < rEndM1; i++)
            {
                for (int j = 0; j < q; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < n; k++)
                        sum += m1[i, k] * m2[k, j];
                    m3[i, j] = sum;
                }
            }
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

        public static void matrixFilling(int[,] m)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            Random random = new Random();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++) m[i, j] = random.Next(0, 5);
        }

    }
}
using System.Diagnostics;

namespace Task1
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Random random = new Random();

			const int a = 25000;
			const int b = 25000;

			int[,] m1 = new int[a, b];
			int[,] m2 = new int[a, b];
			int[,] m3 = new int[a, b];


			for (int i = 0; i < a; i++)
			{
				for (int j = 0; j < b; j++)
				{
					m1[i, j] = random.Next(0, 5);
					m2[i, j] = random.Next(0, 5);
				}
			}

			MeasureTime(() => {
				for (int i = 0; i < a; i++)
				{
					for (int j = 0; j < b; j++)
						m3[i, j] = m1[i, j] + m2[i, j];
				}
			});

			m3 = new int[a, b];

			foreach (int numberOfThreads in new[] { 2, 4, 8, 16, 32 })
				CalculateTimeInDefinedNumberOfThreads(numberOfThreads, m1, m2, m3, a, b);
		}

		public static void BlockOfCalculation(int[,] m1, int[,] m2, int[,] m3, int rStart, int rEnd, int cStart, int cEnd)
		{
			for (int i = rStart; i < rEnd; i++)
				for (int j = cStart; j < cEnd; j++) m3[i, j] = m1[i, j] + m2[i, j];
		}


		public static void CalculateTimeInDefinedNumberOfThreads(
			int NumberOfThreads, int[,] m1, int[,] m2, int[,] m3, int a, int b)
		{
			Thread[] threads = new Thread[NumberOfThreads];

			for (int i = 0; i < NumberOfThreads; i++)
			{
				int rStart = (a * i) / NumberOfThreads;
				int rEnd = (a * (i + 1)) / NumberOfThreads;
				threads[i] = (new Thread(() => BlockOfCalculation(m1, m2, m3, rStart, rEnd, 0, b)));
				threads[i].Start();
			}

			MeasureTime(() =>{				
				foreach (var thread in threads)
					thread.Join();
				Console.Write(NumberOfThreads.ToString() + " Threads:   ");
			});
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
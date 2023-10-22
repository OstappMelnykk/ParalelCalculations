using System.Diagnostics;

namespace Task5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int INF = int.MaxValue; // Нескінченність
            int V = 1000;
            int[,] graph = GenerateRandomGraph(V, INF);

            
            MeasureTime(() => { FloydAlgorithm(graph, V, INF);});

            foreach(var num in new[] { 2, 4, 8, 16, 32, 64 })
                MeasureTime(() => { ParallelFloydAlgorithm(graph, V, INF, num); Console.Write(num.ToString() + " Threads:   "); });         
        }
        

        static void FloydAlgorithm(int[,] graph, int V, int INF)
        {
            int[,] dist = new int[V, V];
            for (int i = 0; i < V; i++){
                for (int j = 0; j < V; j++){
                    dist[i, j] = graph[i, j];
                }
            }

            for (int k = 0; k < V; k++){
                for (int i = 0; i < V; i++){
                    for (int j = 0; j < V; j++){
                        if (dist[i, k] != INF && dist[k, j] != INF && dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            }
        }

        static void ParallelFloydAlgorithm(int[,] graph, int V, int INF, int numThreads)
        {
            int[,] dist = new int[V, V];
            for (int i = 0; i < V; i++){
                for (int j = 0; j < V; j++){
                    dist[i, j] = graph[i, j];
                }
            }

            Parallel.For(0, V, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, k =>{
                for (int i = 0; i < V; i++){
                    for (int j = 0; j < V; j++){
                        if (dist[i, k] != INF && dist[k, j] != INF && dist[i, k] + dist[k, j] < dist[i, j])
                            dist[i, j] = dist[i, k] + dist[k, j];
                    }
                }
            });
        }

        static int[,] GenerateRandomGraph(int V, int INF){
            Random random = new Random();
            int[,] graph = new int[V, V];

            for (int i = 0; i < V; i++){
                for (int j = 0; j < V; j++){
                    if (i == j) graph[i, j] = 0;
                    else graph[i, j] = (random.Next(100) < 75) ? random.Next(1, 1000001) : INF;                
                }
            }

            return graph;
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

        #region Alternalive
        /*static void ParallelFloydAlgorithm(int[,] dist, int V, int numThreads)
        {
            int INF = int.MaxValue; // Нескінченність

            for (int k = 0; k < V; k++)
            {
                var tasks = new Task[numThreads];
                int range = V / numThreads;

                for (int t = 0; t < numThreads; t++)
                {
                    int start = t * range;
                    int end = (t == numThreads - 1) ? V : start + range;

                    tasks[t] = Task.Run(() =>
                    {
                        for (int i = start; i < end; i++)
                        {
                            for (int j = 0; j < V; j++)
                            {
                                if (dist[i, k] != INF && dist[k, j] != INF && dist[i, k] + dist[k, j] < dist[i, j])
                                {
                                    dist[i, j] = dist[i, k] + dist[k, j];
                                }
                            }
                        }
                    });
                }

                Task.WaitAll(tasks);
            }
        }*/
        #endregion
        
    }
}






/*int[,] graph = {
    {0, 10, INF, 30, 100},
    {INF, 0, 50, INF, INF},
    {INF, INF, 0, INF, 10},
    {INF, INF, 20, 0, 60},
    {INF, INF, INF, INF, 0}
};*/
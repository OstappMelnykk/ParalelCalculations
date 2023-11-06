using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Task7
{
    class Program
    {
        const int INF = int.MaxValue;

        struct PrimsResult
        {
            public List<int> parent;
            public List<int> key;
        }

        static PrimsResult Prims(List<List<int>> graph, int start, int numThreads)
        {
            int n = graph.Count;
            List<int> key = new List<int>(new int[n]);
            List<int> parent = new List<int>(new int[n]);
            for (int i = 0; i < n; i++)
            {
                key[i] = INF;
                parent[i] = -1;
            }
            key[start] = 0;
            List<bool> visited = new List<bool>(new bool[n]);

            object lockObject = new object();

            Action<int> parallelPrims = threadId =>
            {
                for (int k = 0; k < n - 1; k++)
                {
                    int u = -1;
                    int localMin = INF;
                    for (int i = 0; i < n; i++)
                    {
                        if (!visited[i] && key[i] < localMin)
                        {
                            localMin = key[i];
                            u = i;
                        }
                    }

                    if (u == -1) break;

                    visited[u] = true;

                    for (int v = 0; v < n; v++)
                    {
                        if (!visited[v] && graph[u][v] != INF && graph[u][v] < key[v])
                        {
                            lock (lockObject)
                            {
                                parent[v] = u;
                                key[v] = graph[u][v];
                            }
                        }
                    }
                }
            };

            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(() => parallelPrims(i)));
            }

            threads.ForEach(thread => thread.Start());
            threads.ForEach(thread => thread.Join());

            return new PrimsResult { parent = parent, key = key };
        }

        static List<List<int>> GenerateRandomGraph(int n)
        {
            List<List<int>> graph = new List<List<int>>();
            Random rand = new Random();

            for (int i = 0; i < n; i++){
                graph.Add(new List<int>());
                for (int j = 0; j < n; j++){
                    if (i == j) graph[i].Add(0);
                    else graph[i].Add(1 + rand.Next(10));
                }
            }
            return graph;
        }

        static void Main()
        {
            int n = 5;
            int size = 10000;
            List<List<int>> graph = GenerateRandomGraph(size);

            int start = 0;

            int numThreads = 1;

            foreach (var num in new[] { 2, 4, 8 })
                MeasureTime(() =>
                {
                    Prims(graph, start, num);
                    Console.Write(num.ToString() + " Threads:   ");
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


    /*class Program
    {
        static void Main()
        {
            const int size = 1000;        
            Graph test = new(size);

            MeasureTime(() => { Prim(test, 0, out int resultNonPar); });
           
            foreach (int x in new[] { 2, 4 }){
                MeasureTime(() =>{
                    ParallelPrim(test, 0, x, out int result);
                    Console.Write(x.ToString() + " Threads:   ");
                });
            }

        }

        public static void Prim(Graph graph, int start, out int result)
        {
            foreach (var x in graph.Points)
            {
                x.Passed = false;
                x.Weight = int.MaxValue;
            }

            graph.Points[start].Weight = 0;
            var current = graph.Points[start];

            while (current is not null)
            {
                var cons = current.Edges;
                foreach (var con in cons)
                {
                    if (!con.SecondPoint.Passed)
                    {
                        if (con.SecondPoint.Weight > con.Weight) con.SecondPoint.Weight = con.Weight;
                    }
                }
                current.Passed = true;
                current = graph.Points
                    .Where(p => p.Passed)
                    .SelectMany(p => p.Edges
                                    .Where(con => !con.SecondPoint.Passed)
                                    .OrderBy(con => con.SecondPoint.Weight)
                                    .Select(con => con.SecondPoint))
                    .OrderBy(p => p.Weight)
                    .FirstOrDefault();
            }

            result = 0;

            for (int i = 0; i < graph.Points.Count; i++)
                result += graph.Points[i].Weight;
        }

        public static void ParallelPrim(Graph graph, int start, int threadNum, out int result)
        {
            var flag = true;
            ConcurrentQueue<Job> jobs = new();

            foreach (var point in graph.Points)
            {
                point.Passed = false;
                point.Weight = int.MaxValue;
            }

            Thread[] threads = new Thread[threadNum];
            CountdownEvent countdownEvent = new(threadNum);

            graph.Points[start].Weight = 0;
            GraphPoint current = graph.Points[start];
            List<GraphPoint> passed = new();





            for (int i = 0; i < threadNum; i++)
            {
                threads[i] = new Thread(() =>
                {
                    while (flag)
                    {
                        if (jobs.TryDequeue(out Job job))
                        {
                            job.Data
                                .GetRange(job.From, job.Take)
                                .ForEach(con =>{
                                    if (!con.SecondPoint.Passed && con.SecondPoint.Weight > con.Weight){
                                        con.SecondPoint.Weight = con.Weight;
                                    }
                                });
                            countdownEvent.Signal();
                        }
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();

            while (current != null)
            {
                int toEach = current.Edges.Count / threadNum;
                int remainder = current.Edges.Count % threadNum;

                for (int i = 0; i < threadNum; i++)
                {
                    int startCon = i * toEach;
                    int take = i + 1 != threadNum ? toEach : toEach + remainder;
                    Job job = new() { Data = current.Edges, From = startCon, Take = take };
                    jobs.Enqueue(job);
                }

                while (countdownEvent.CurrentCount != 0){}

                countdownEvent.Reset();
                current.Passed = true;
                passed.Add(current);

                current = graph.Points
                    .Where(p => p.Passed)
                    .SelectMany(p => p.Edges)
                    .Where(con => !con.SecondPoint.Passed)
                    .OrderBy(p => p.Weight)
                    .Select(p => p.SecondPoint)
                    .FirstOrDefault();
            }

            flag = false;
            foreach (var thread in threads)
                thread.Join();

            result = 0;

            for (int i = 0; i < graph.Points.Count; i++)
                result += graph.Points[i].Weight;
        }



        public class Job
        {
            public List<Edge> Data { get; set; }
            public int From { get; set; }
            public int Take { get; set; }
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

    public class GraphPoint
    {
        public string Name { get; set; }
        public bool Passed { get; set; }
        public List<Edge> Edges { get; set; }
        public int Weight { get; set; }


        public GraphPoint(string name)
        {
            Passed = false;
            Weight = int.MaxValue;
            Name = name;
            Edges = new();
        }
    }
    public class Edge
    { 
        public GraphPoint FirstPoint { get; set; }
        public GraphPoint SecondPoint { get; set; }
        public int Weight { get; set; }
    }
    public class Graph
    {
        public List<GraphPoint> Points { get; set; }
        public List<Edge> Connections { get; set; }

        public Graph(int pointNum)
        {
            Points = new();
            Random random = new();
            for (int i = 0; i < pointNum; i++)
            {
                GraphPoint newPoint = new("Point" + i);
                int connection = 0;

                if (Points.Count != 0)
                {
                    connection = random.Next(0, Points.Count);
                    int weight = random.Next(1, 1000);
                    Edge connectionFrom = new() { FirstPoint = Points[connection], SecondPoint = newPoint, Weight = weight };
                    Edge connectionTo = new() { FirstPoint = newPoint, SecondPoint = Points[connection], Weight = weight };
                    newPoint.Edges.Add(connectionTo);
                    Points[connection].Edges.Add(connectionFrom);
                }

                foreach (var x in Points)
                {
                    if (random.NextDouble() < 0.5 && Points[connection].Name != x.Name)
                    {
                        int weight = random.Next(1, 1000);
                        Edge connectionFrom = new() { FirstPoint = x, SecondPoint = newPoint, Weight = weight };
                        Edge connectionTo = new() { FirstPoint = newPoint, SecondPoint = x, Weight = weight };
                        newPoint.Edges.Add(connectionTo);
                        x.Edges.Add(connectionFrom);
                    }
                }

                Points.Add(newPoint);
            }
        }         
    }*/
}
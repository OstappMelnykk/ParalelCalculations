using System.Collections.Concurrent;
using System.Diagnostics;

namespace Task6
{


    internal class Program
    {
        public static Dictionary<string, Dictionary<string, int>> GenerateRandomGraph(int numVertices, int numEdges)
        {
            if (numVertices < 2 || numEdges < numVertices - 1 || numEdges > numVertices * (numVertices - 1) / 2)
                throw new ArgumentException("Неприпустимі параметри графу");

            var random = new Random();
            var graph = new Dictionary<string, Dictionary<string, int>>();

            var vertices = Enumerable.Range(0, numVertices)
                .Select(i => $"a{i}")
                .ToList();

            foreach (var vertex in vertices)
                graph[vertex] = new Dictionary<string, int>();

            for (int i = 0; i < numEdges; i++){
                string vertex1, vertex2;
                do{
                    vertex1 = vertices[random.Next(numVertices)];
                    vertex2 = vertices[random.Next(numVertices)];
                } while (vertex1 == vertex2 || graph[vertex1].ContainsKey(vertex2) || graph[vertex2].ContainsKey(vertex1));

                int weight = random.Next(1, 100001); // Випадкова вага ребра від 1 до 20 (можна змінити за потребою)

                graph[vertex1][vertex2] = weight;
                graph[vertex2][vertex1] = weight;
            }
            return graph;
        }

        static void Main(string[] args)
        {
            int numVertices = 10000; // кількість вершин
            int numEdges = 39995000;  // кількість ребер
            var graph = GenerateRandomGraph(numVertices, numEdges);

            string startNode = "a0";

            MeasureTime(() => { FindShortestPaths(graph, startNode); });



            foreach (var num in new[] { 2, 4, 8})
                MeasureTime(() => 
                { 
                    FindShortestPathsParallel(graph, startNode, num);
                    Console.Write(num.ToString() + " Threads:   "); 
                });
        }

        public static Dictionary<string, int> FindShortestPaths(Dictionary<string, Dictionary<string, int>> graph, string startNode)
        {
            Dictionary<string, int> distances = new Dictionary<string, int>();

            foreach (string node in graph.Keys)
                distances[node] = int.MaxValue;
            distances[startNode] = 0;

            HashSet<string> unvisitedNodes = new HashSet<string>(graph.Keys);

            while (unvisitedNodes.Count > 0){
                string currentNode = null;

                //Цей цикл визначає вузол з найменшою відстанню серед неперевірених вузлів у контексті алгоритму Дейкстри,
                //щоб обрати його як поточний вузол і розпочати розгляд сусідніх вузлів для оновлення їхніх відстаней.
                foreach (string node in unvisitedNodes)
                    if (currentNode == null || distances[node] < distances[currentNode]) currentNode = node;

                unvisitedNodes.Remove(currentNode);

                //Цей цикл використовується для перевірки можливих коротших відстаней до сусідніх вузлів від поточного вузла і,
                //у випадку знаходження коротшого шляху, оновлює відстані до цих вузлів у відомостях.
                foreach (var neighbor in graph[currentNode]){
                    int potentialDistance = distances[currentNode] + neighbor.Value;
                    if (potentialDistance < distances[neighbor.Key]) distances[neighbor.Key] = potentialDistance;
                }
            }
            return distances;
        }


        public static Dictionary<string, int> FindShortestPathsParallel(Dictionary<string, Dictionary<string, int>> graph, string startNode, int numThreads)
        {
            Dictionary<string, int> distances = new Dictionary<string, int>();
            foreach (string node in graph.Keys)
                distances[node] = int.MaxValue;
            distances[startNode] = 0;

            HashSet<string> unvisitedNodes = new HashSet<string>(graph.Keys);
            object unvisitedNodesLock = new object(); // Блокування для unvisitedNodes

            while (unvisitedNodes.Count > 0)
            {
                var concurrentDistances = new ConcurrentDictionary<string, int>(distances);
                Parallel.ForEach(unvisitedNodes, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, (currentNode) =>
                {
                    string closestNode = null;
                    int closestDistance = int.MaxValue;

                    // Знаходження вузла з найменшою відстанню
                    foreach (var node in unvisitedNodes)
                    {
                        if (concurrentDistances[node] < closestDistance)
                        {
                            closestDistance = concurrentDistances[node];
                            closestNode = node;
                        }
                    }

                    if (closestNode == null)
                        return;

                    // Видалення вузла зі списку неперевірених
                    lock (unvisitedNodesLock)
                    {
                        unvisitedNodes.Remove(closestNode);
                    }

                    // Оновлення відстаней до сусідніх вузлів
                    foreach (var neighbor in graph[closestNode])
                    {
                        int potentialDistance = closestDistance + neighbor.Value;
                        if (potentialDistance < concurrentDistances[neighbor.Key])
                        {
                            concurrentDistances[neighbor.Key] = potentialDistance;
                        }
                    }
                });

                // Копіюємо дані назад з ConcurrentDictionary до Dictionary distances
                foreach (var kvp in concurrentDistances)
                {
                    distances[kvp.Key] = kvp.Value;
                }
            }

            return distances;
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






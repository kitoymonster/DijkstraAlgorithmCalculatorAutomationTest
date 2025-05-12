namespace DijkstraShortestPathCalculator.Utils;

public class DijkstraPathFinder
{
    private readonly Dictionary<string, List<(string, int)>> _graph = new()
    {
        {"A", [("B", 4), ("C", 6)] },
        {"B", [("A", 4), ("F", 2)] },
        {"C", [("A", 6), ("D", 8)] },
        {"D", [("C", 8), ("E", 4), ("G", 1)] },
        {"E", [("B", 2), ("D", 4), ("F", 3), ("I", 8)] },
        {"F", [("B", 2), ("E", 3), ("G", 4), ("H", 6)] },
        {"G", [("D", 1), ("F", 4), ("H", 5), ("I", 5)] },
        {"H", [("F", 6), ("G", 5)] },
        {"I", [("E", 8), ("G", 5)] },
    };


    public (List<string> path, int cost) FindShortestPath(string start, string end)
    {
        var distances = new Dictionary<string, int>();
        var previous = new Dictionary<string, string>();
        var priorityQueue = new SortedSet<(int distance, string node)>();

        // Initialize distances
        foreach (var node in _graph.Keys)
        {
            distances[node] = int.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;
        priorityQueue.Add((0, start));

        if (start == end) return (new List<string>(), 0);
        
        while (priorityQueue.Count > 0)
        {
            var (currDist, currNode) = GetMin(priorityQueue);
            priorityQueue.Remove((currDist, currNode));

            if (currNode == end) break;

            foreach (var (neighbor, weight) in _graph[currNode])
            {
                int newDist = currDist + weight;
                if (newDist < distances[neighbor])
                {
                    priorityQueue.Remove((distances[neighbor], neighbor));
                    distances[neighbor] = newDist;
                    previous[neighbor] = currNode;
                    priorityQueue.Add((newDist, neighbor));
                }
            }
        }

        var path = ReconstructPath(previous, start, end);
        int totalCost = distances[end];

        return (path, totalCost);
    }

    private static (int, string) GetMin(SortedSet<(int, string)> set)
    {
        foreach (var item in set)
            return item;
        return (0, null)!; // fallback
    }

    private static List<string> ReconstructPath(Dictionary<string, string> previous, string start, string end)
    {
        var path = new List<string>();
        string current = end;

        while (current != null)
        {
            path.Insert(0, current);
            current = previous[current];
        }

        if (path.Count == 0 || path[0] != start)
            return new List<string>(); // No path found

        return path;
    }
}
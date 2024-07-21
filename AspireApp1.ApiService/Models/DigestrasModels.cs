using System.Reflection.PortableExecutable;

namespace AspireApp1.ApiService.Models;

public record Node (string Name)
{
    public Node? Parent { get; set; } 
    public bool Visited { get; set; }
    public List<Edge> Edges { get; private set; } = new List<Edge>();
}

public record Edge (Node Destination, int Distance);

public class Graph
{
    public List<Node> Nodes { get; private set; } = new List<Node>();

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void AddEdge(Node source, Node dest, int distance)
    {
        source.Edges.Add(new Edge(dest, distance));
    }

    public List<Node> ShortestPath(Node source, Node dest)
    {
        var queue = new Queue<Node>();
        queue.Enqueue(source);
        source.Visited = true;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == dest)
            {
                return GetPath(current);
            }

            foreach (var edge in current.Edges)
            {
                if (!edge.Destination.Visited)
                {
                    edge.Destination.Parent = current;
                    edge.Destination.Visited = true;
                    queue.Enqueue(edge.Destination);
                }
            }
        }

        return new List<Node>();
    }

   

    private List<Node> GetPath(Node node)
    {
        var path = new List<Node>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        path.Reverse();
        return path;
    }
}
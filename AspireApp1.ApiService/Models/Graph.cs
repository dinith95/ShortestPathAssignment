namespace AspireApp1.ApiService.Models;


public record Node (string Name)
{
    public Node? Parent { get; set; } 
    public bool Visited { get; set; }
    public List<Edge> Edges { get;  set; } = new List<Edge>();
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
}
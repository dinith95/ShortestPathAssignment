namespace AspireApp1.ApiService.Models;

public record Node (string Name)
{
    public Node? Parent { get; set; } 
    public bool Visited { get; set; }
    public List<Edge> Edges { get;  set; } = new List<Edge>();
}

public record Edge (Node Destination, int Distance);

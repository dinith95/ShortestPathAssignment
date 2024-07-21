using AspireApp1.ApiService.Dto;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface IDistanceCalculatorService
{
    Task<DistanceDto> FindShortestPath(string source, string dest);
}

public class DistanceCalculatorService : IDistanceCalculatorService
{
    private readonly Graph _graph;
    
    private readonly IDocumentDbService _documentDbService;

    public DistanceCalculatorService(IDocumentDbService documentDbService)
    {
        _graph = PopulateGraph();
        _documentDbService = documentDbService;
    }

    public async Task<DistanceDto> FindShortestPath(string source, string dest)
    {
        try
        {
            var nodes = await _documentDbService.GetNodesInGraph(Constants.Graph01);
            var sourceNode = nodes.FirstOrDefault(n => n.Name == source);
            var destNode = nodes.FirstOrDefault(n => n.Name == dest);
            Dictionary<string, int> distanceTable = nodes.ToDictionary(n => n.Name, n => int.MaxValue);

            // set source distance to 0
            distanceTable[source] = 0;

            await VisitNode(sourceNode, distanceTable);

            // get minimum distance node
            var minNode = GetMinimumDistanceNode(nodes, distanceTable);

            while (minNode != null)
            {
                await VisitNode(minNode, distanceTable);
                minNode = GetMinimumDistanceNode(nodes, distanceTable);
            }
            var shortestPath = GetPathDestination(dest);
            return new DistanceDto(shortestPath, distanceTable[dest]);
        }
        catch (Exception ex)
        {

            throw;
        }
       

    }

    private List<string> GetPathDestination(string destNodeStr)
    {
        var paths = new List<string>();
        var node = _graph.Nodes.FirstOrDefault(n => n.Name == destNodeStr);
        while (node != null)
        {
            paths.Add(node.Name);
            node = node.Parent;
        }

        return paths.Reverse<string>().ToList();
    }

    private async Task VisitNode(Node node, Dictionary<string, int> distanceTable)
    {
        node.Edges = await _documentDbService.GetEdgesInGraphForNode(Constants.Graph01, node.Name);
        foreach (var edge in node.Edges)
        {
            var nextNode = edge.Destination;
            if (!nextNode.Visited)
            {
                // only update if the calculated distance is less than the previous min  distance
                int distance = distanceTable[node.Name] + edge.Distance;
                if (distance < distanceTable[nextNode.Name])
                {
                    distanceTable[nextNode.Name] = distance;
                    nextNode.Parent = node;
                }

            }
        }
        node.Visited = true;
    }

    private Node GetMinimumDistanceNode(List<Node> nodes, Dictionary<string, int> distanceTable)
    {
        var minDistance = int.MaxValue;
        Node minNode = null;
        foreach (var node in nodes)
        {
            if (distanceTable[node.Name] < minDistance && !node.Visited)
            {
                minDistance = distanceTable[node.Name];
                minNode = node;
            }
        }
        return minNode;
    }



    private Graph PopulateGraph()
    {
        var graph = new Graph();
        var a = new Node("A");
        var b = new Node("B");
        var c = new Node("C");
        var d = new Node("D");
        var e = new Node("E");

        graph.AddNode(a);
        graph.AddNode(b);
        graph.AddNode(c);
        graph.AddNode(d);
        graph.AddNode(e);

        graph.AddEdge(a, b, 1);
        graph.AddEdge(a, c, 4);

        graph.AddEdge(b, a, 1);
        graph.AddEdge(b, c, 4);
        graph.AddEdge(b, e, 7);
        graph.AddEdge(b, d, 2);

        graph.AddEdge(c, a, 4);
        graph.AddEdge(c, b, 4);
        graph.AddEdge(c, e, 5);
        graph.AddEdge(c, d, 3);

        graph.AddEdge(d, b, 2);
        graph.AddEdge(d, c, 3);
        graph.AddEdge(d, e, 4);

        graph.AddEdge(e, b, 7);
        graph.AddEdge(e, c, 5);
        graph.AddEdge(e, d, 4);
        return graph;
    }


}

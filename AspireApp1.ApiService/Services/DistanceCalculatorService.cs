using AspireApp1.ApiService.Dto;
using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService.Services;

public interface IDistanceCalculatorService
{
    Task<DistanceDto> FindShortestPath(string source, string dest);
}

public class DistanceCalculatorService : IDistanceCalculatorService
{
    private readonly IDocumentDbService _documentDbService;
    private readonly List<Node> _nodes;

    public DistanceCalculatorService(IDocumentDbService documentDbService)
    {
        _documentDbService = documentDbService;
        _nodes = new List<Node>();
    }

    public async Task<DistanceDto> FindShortestPath(string source, string dest)
    {

        _nodes.AddRange( await _documentDbService.GetNodesInGraph(Constants.Graph01));

        var sourceNode = _nodes.FirstOrDefault(n => n.Name == source);
        var destNode = _nodes.FirstOrDefault(n => n.Name == dest);
        Dictionary<string, int> distanceTable = _nodes.ToDictionary(n => n.Name, n => int.MaxValue);

        // set source distance to 0
        distanceTable[source] = 0;

        await VisitNode(sourceNode, distanceTable);

        // get minimum distance node
        var minNode = GetMinimumDistanceNode(distanceTable);

        while (minNode != null)
        {
            await VisitNode(minNode, distanceTable);
            minNode = GetMinimumDistanceNode(distanceTable);
        }
        var shortestPath = GetPathDestination(dest);
        return new DistanceDto(shortestPath, distanceTable[dest]);
    }

    private List<string> GetPathDestination(string destNodeStr)
    {
        var paths = new List<string>();
        var node = _nodes.FirstOrDefault(n => n.Name == destNodeStr);
        while (node != null)
        {
            paths.Add(node.Name);
            node = node.Parent;
        }

        return paths.Reverse<string>().ToList();
    }

    private async Task VisitNode(Node node, Dictionary<string, int> distanceTable)
    {
        var edges = await _documentDbService.GetEdgesInGraphForNode(Constants.Graph01, node.Name);

        foreach (var edge in edges)
        {
            var nextNode = _nodes.FirstOrDefault(n => n.Name == edge.Destination);
            node.Edges.Add(new Edge(nextNode, edge.Distance));
        }

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

    private Node GetMinimumDistanceNode( Dictionary<string, int> distanceTable)
    {
        var minDistance = int.MaxValue;
        Node minNode = null;
        foreach (var node in _nodes)
        {
            if (distanceTable[node.Name] < minDistance && !node.Visited)
            {
                minDistance = distanceTable[node.Name];
                minNode = node;
            }
        }
        return minNode;
    }
}

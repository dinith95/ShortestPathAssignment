using AspireApp1.ApiService.Dto;
using AspireApp1.ApiService.Models;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspireApp1.ApiService.Services;

public interface IDistanceCalculatorService
{
    Task<DistanceDto> FindShortestPath(string source, string dest);
}

public class DistanceCalculatorService : IDistanceCalculatorService
{
    private readonly IDocumentDbService _documentDbService;
    private readonly IRediCachingService _rediCachingService;
    private readonly List<Node> _nodes;

    public DistanceCalculatorService(IDocumentDbService documentDbService,  IRediCachingService rediCachingService)
    {
        _documentDbService = documentDbService;
        _nodes = new List<Node>();
        _rediCachingService = rediCachingService;
    }

    public async Task<DistanceDto> FindShortestPath(string source, string dest)
    {
        DistanceDto distanceDto;
        var cacheVal = await _rediCachingService.CheckInRedisCache(source, dest);
        if (cacheVal != null)
        {
            distanceDto = cacheVal;
        }  
        else
        {
            await PopulateNodes();
            distanceDto = await GetFromDocumentDb(source, dest);
        }
        return distanceDto;
    }

    private async Task<DistanceDto> GetFromDocumentDb(string source, string dest)
    {
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
        await AddShortestPathsToRedis(source, distanceTable);
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

    private async Task AddShortestPathsToRedis(string source, Dictionary<string,int> distanceTable)
    {
        foreach (var node in _nodes)
        {
            if(node.Name == source)
                continue;
            var shortestPath = GetPathDestination(node.Name);
            var distance = distanceTable[node.Name];
            var distanceDto = new DistanceDto(shortestPath, distance);
            await _rediCachingService.AddToRedisCache(source, node.Name, distanceDto);
        }
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

    private async  Task PopulateNodes() =>
            _nodes.AddRange(await _documentDbService.GetNodesInGraph(Constants.Graph01));
}

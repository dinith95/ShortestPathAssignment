using AspireApp1.ApiService.Models;

namespace AspireApp1.ApiService;

public class DistanceCalculatorService
{
    private readonly Graph _graph;

    public DistanceCalculatorService()
    {
        _graph = PopulateGraph();
    }

    public int  FindShortestPath(string source, string dest)
    {
        var sourceNode = _graph.Nodes.FirstOrDefault(n => n.Name == source);
        var destNode = _graph.Nodes.FirstOrDefault(n => n.Name == dest);

        Dictionary<string, int> distanceTable = _graph.Nodes.ToDictionary(n => n.Name, n => int.MaxValue);

        // set source distance to 0
        distanceTable[source] = 0;

        VisitNode(sourceNode, distanceTable);

        // get minimum distance node
        var minNode = GetMinimumDistanceNode(distanceTable);

        while (minNode != null)
        {
            VisitNode(minNode, distanceTable);
            minNode = GetMinimumDistanceNode(distanceTable);
        }
        return distanceTable[dest]; 
       
    }

    private void VisitNode(Node node, Dictionary<string, int> distanceTable)
    {
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
                    //nextNode.Parent = node;
                }
               
            }
        }
        node.Visited = true;
    }

    private Node GetMinimumDistanceNode(Dictionary<string, int> distanceTable)
    {
        var minDistance = int.MaxValue;
        Node minNode = null;
        foreach (var node in _graph.Nodes)         
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

using AspireApp1.ApiService.DataAccess;
using System.Linq.Expressions;

namespace AspireApp1.ApiService.Services;

public interface IDocumentDbService
{
    Task<List<Edge>> GetEdgesInGraphForNode(string graph, string node);
    Task<List<Models.Node>> GetNodesInGraph(string graph);

}
public class DocumentDbService: IDocumentDbService
{
    private IDocumentDbRepo<Node> _nodeRepo;
    private IDocumentDbRepo<Edge> _edgeRepo;

    public DocumentDbService(IDocumentDbRepo<Node> nodeRepo, IDocumentDbRepo<Edge> edgeRepo)
    {
        _nodeRepo = nodeRepo;
        _edgeRepo = edgeRepo;
    }

    public async Task<List<Models.Node>> GetNodesInGraph(string graph)
    {
        Expression<Func<Node, bool>> filter = node => node.Type == graph;
        Expression<Func<Node, Node>> select = node => node;

        var nodes = await _nodeRepo.QueryItemsAsync(filter, select);
        
        return nodes
                .Select(n => new Models.Node(n.Name))
                .ToList();
    }

    public async Task<List<Edge>> GetEdgesInGraphForNode(string graph, string node)
    {
        Expression<Func<Edge, bool>> filter = edge => edge.Type == graph && edge.Source == node ;
        Expression<Func<Edge, Edge>> select = edge => edge;

        var edges = await _edgeRepo.QueryItemsAsync(filter, select);

        return edges;
    }

    private Models.Edge GetEdgeNode(Edge edge)
    {
        var destNode = new Models.Node(edge.Destination);
        return new Models.Edge(destNode, edge.Distance);
    }
}


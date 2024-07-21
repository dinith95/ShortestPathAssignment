namespace AspireApp1.ApiService.DataAccess;

public interface IEntity
{ 
    string id { get; set; }
    // Type is the partition key
    string Type { get; set; }
}

// graphName is the partition key 
[DocumentDbEntity(CollectionName = "Nodes")]
public class Node : IEntity
{
    public string id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

}

// graphName is the partition key
[DocumentDbEntity(CollectionName = "Edges")]
public class Edge : IEntity
{
    public string id { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public int Distance { get; set; }
    public string Type { get; set; }
}

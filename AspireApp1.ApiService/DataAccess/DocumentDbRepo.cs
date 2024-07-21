using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq.Expressions;

namespace AspireApp1.ApiService.DataAccess;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DocumentDbEntity : Attribute
{
    public string CollectionName { get; set; } = string.Empty;
}

public interface IDocumentDbRepo<T> where T : IEntity
{
    Task CreateItemAsync(T item);
    Task DeleteItemAsync(T item);
    Task<List<T>> QueryItemsAsync(Expression<Func<T, bool>> filterFunc, Expression<Func<T, T>> selectFunc);
    Task UpdateItemAsync(T item);
}

public class DocumentDbRepo<T> : IDocumentDbRepo<T> where T : IEntity
{

    private readonly string _databaseId;
    private CosmosClient _client;
    private readonly IConfiguration _config;

    public DocumentDbRepo( IConfiguration config, CosmosClient cosmosClient)
    {
        _config = config;
        _client = cosmosClient;
        _databaseId = _config.GetValue<string>("CosmosDb:DatabaseId");
    }

    public async Task<List<T>> QueryItemsAsync(Expression<Func<T, bool>> filterFunc, Expression<Func<T, T>> selectFunc)
    {
        var collectionName = GetCollectionName();
       
        var container = _client.GetContainer(_databaseId, collectionName);

        var results = new List<T>();
        var query = container.GetItemLinqQueryable<T>(true)
                                            .Where(filterFunc)
                                            .Select(selectFunc)
                                            .AsQueryable();

        var iterator = query.ToFeedIterator();

        while (iterator.HasMoreResults)
            results.AddRange(await iterator.ReadNextAsync());

        return results;
    }

    public async Task CreateItemAsync(T item)
    {
        var collectionName = GetCollectionName();
        var partitionKey = new PartitionKey(item.Type);
        var container = _client.GetContainer(_databaseId, collectionName);
        await container.CreateItemAsync(item, partitionKey: partitionKey);
    }

    public async Task UpdateItemAsync(T item)
    {
        var collectionName = GetCollectionName();
        var partitionKey = new PartitionKey(item.Type);
        var container = _client.GetContainer(_databaseId, collectionName);
        await container.ReplaceItemAsync(item, item.id, partitionKey: partitionKey);
    }

    public async Task DeleteItemAsync(T item)
    {
        var collectionName = GetCollectionName();
        var partitionKey = new PartitionKey(item.Type);
        var container = _client.GetContainer(_databaseId, collectionName);
        await container.DeleteItemAsync<T>(item.id, partitionKey: partitionKey);
    }

    private string GetCollectionName()
    {
        var attribute = (DocumentDbEntity)Attribute.GetCustomAttribute(typeof(T), typeof(DocumentDbEntity));
        if (attribute == null)
        {
            throw new InvalidOperationException("DocumentDbEntity attribute not found");
        }
        return attribute.CollectionName;
    }
}

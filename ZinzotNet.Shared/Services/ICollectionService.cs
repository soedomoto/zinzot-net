using ZinzotNet.Models;

namespace ZinzotNet.Services
{
    public interface ICollectionService
    {
        List<CollectionModel> AllCollections { get; }
    }
}
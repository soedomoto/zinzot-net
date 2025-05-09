using System.ComponentModel;
using System.Runtime.InteropServices;
using ZinzotNet.Models;

namespace ZinzotNet.Services
{
    public class CollectionService(ISupabaseService SupabaseService) : INotifyPropertyChanged
    {
        public bool Loading = false;
        public List<CollectionModel> AllCollections = [];
        public List<CollectionModel> NestedCollections = [];
        public List<string> SelectedCollectionPaths = [];
        private string? _selectedCollection = null;
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                if (_selectedCollection != value)
                {
                    _selectedCollection = value;

                    SelectedCollectionPaths = [];
                    if (_selectedCollection != null)
                    {
                        BuildSelectedCollectionPaths(_selectedCollection);
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCollection)));
                }
            }
        }

        private void BuildSelectedCollectionPaths(string col)
        {
            SelectedCollectionPaths = SelectedCollectionPaths.Prepend(col).ToList();
            var collection = AllCollections.FirstOrDefault(c => c.Key == col);
            if (collection != null && collection.ParentId != null)
            {
                BuildSelectedCollectionPaths(collection.ParentId);
            }
        }

        public async Task RefetchCollections()
        {
            Loading = true;

            var query = SupabaseService.Client
                .From<CollectionModel>();
            var response = await query.Get();

            Loading = false;
            AllCollections = response?.Models ?? [];
            NestedCollections = BuildHierarchy(AllCollections);
        }

        private List<CollectionModel> BuildHierarchy(List<CollectionModel> collections, string? parentId = null)
        {
            // Find collections where parent_id matches the given parentId (or null for root)
            var result = collections
                .Where(c => (parentId == null && c.ParentId == null) || c.ParentId == parentId)
                .Select(c =>
                {
                    // Recursively fetch children for each collection
                    c.Children = BuildHierarchy(collections, c.Key);
                    return c;
                })
                .ToList();

            return result;
        }
    }
}
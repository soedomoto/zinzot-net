using System.Reflection;
using System.Text.Json;
using Supabase.Postgrest.Interfaces;
using Radzen;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ZinzotNet.Models;

namespace ZinzotNet.Services
{

    public class TableReferenceState(ISupabaseService SupabaseService, CollectionService CollectionService)
    {
        public DataGridSettings Settings = new() { CurrentPage = 0, PageSize = 10 };
        public string? Collection = null;

        public bool Loading = false;
        public int Total = 0;
        public List<TableReferenceModel> DataSource = [];

        public async Task LoadData(LoadDataArgs _args)
        {
            await OnTableChange();
        }

        private HashSet<string> GetCollectionRecursive(string? parentId, HashSet<string> collections)
        {
            collections ??= [];

            foreach (var collection in CollectionService.AllCollections.FindAll(c => c.ParentId == parentId))
            {
                collections.Add(collection.Key!);
                foreach (var c in GetCollectionRecursive(collection.Key, collections))
                {
                    collections.Add(c);
                }
                // collections(GetCollectionRecursive(collection.Key, collections));
            }

            return collections;
        }

        public async Task OnTableChange()
        {
            try
            {
                Loading = true;

                IPostgrestTable<TableReferenceModel> cQuery = SupabaseService.Client
                    .From<TableReferenceModel>();

                var rQueryCols = string.Join(",", SupabaseModelExtensions.GetColumnNames(typeof(TableReferenceModel)));
                IPostgrestTable<TableReferenceModel> rQuery = SupabaseService.Client
                    .From<TableReferenceModel>()
                    .Select(rQueryCols)
                    .Range((Settings.CurrentPage ?? 0) * (Settings.PageSize ?? 10), ((Settings.CurrentPage ?? 0) + 1) * (Settings.PageSize ?? 10) - 1);

                if (Collection != null)
                {
                    var collections = GetCollectionRecursive(Collection, [Collection]);

                    cQuery = cQuery.Filter("collection_id", Supabase.Postgrest.Constants.Operator.In, collections.ToArray());
                    rQuery = rQuery.Filter("collection_id", Supabase.Postgrest.Constants.Operator.In, collections.ToArray());
                }

                foreach (var sort in Settings.Columns ?? [])
                {
                    var colName = SupabaseModelExtensions.GetColumnName<TableReferenceModel>(sort.Property);
                    if (sort.SortOrder != null)
                    {
                        var ordering = Supabase.Postgrest.Constants.Ordering.Ascending;
                        if (sort.SortOrder == SortOrder.Descending)
                        {
                            ordering = Supabase.Postgrest.Constants.Ordering.Descending;
                        }

                        rQuery = rQuery.Order(colName, ordering);
                    }
                }

                var count = await cQuery.Count(Supabase.Postgrest.Constants.CountType.Exact);
                var response = await rQuery.Get();

                Loading = false;
                Total = count;
                DataSource = response?.Models ?? [];

                foreach (var d in DataSource)
                {
                    // Console.WriteLine(d.CollectionItems);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.WhenAll();
        }
    }

    public class DetailReferenceState(ISupabaseService SupabaseService, IS3Service S3Service)
    {
        private string _id = string.Empty;
        public bool Loading;
        public ReferenceModel DataSource = new();

        public async Task OnParamChange(string id)
        {
            DataSource ??= new ReferenceModel();

            _id = id;
            await _OnParamChange();
        }

        public async Task _OnParamChange()
        {
            try
            {
                Loading = true;

                var ItemCreatorCols = SupabaseModelExtensions.GetColumnNames(typeof(ItemCreator));
                var itemCreatorsSel = "item_creators(" + string.Join(",", ItemCreatorCols) + ")";

                var ReferenceModelCols = SupabaseModelExtensions.GetColumnNames(typeof(ReferenceModel));
                // ReferenceModelCols = ReferenceModelCols.Append(itemCreatorsSel);
                var referenceModelSel = string.Join(",", ReferenceModelCols);

                var query = SupabaseService.Client
                    .From<ReferenceModel>()
                    .Select(referenceModelSel)
                    .Filter("itemId", Supabase.Postgrest.Constants.Operator.Equals, _id);

                var reference = (await query.Get())?.Models?.FirstOrDefault() ?? new ReferenceModel();
                for (int i = 0; i < reference.Attachments.Length; i++)
                {
                    reference.Attachments[i] = await S3Service.GetPresignedUrl(reference.Attachments[i]);
                }

                Loading = false;
                DataSource = reference ?? new ReferenceModel();

                await Task.WhenAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

public static class SupabaseModelExtensions
{
    public static string[] GetColumnNames(Type modelType)
    {
        return [.. modelType.GetProperties()
                .Where(prop => prop.GetCustomAttribute<ColumnAttribute>() != null || prop.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                .Select(prop =>
                {
                    var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                    var primaryKeyAttr = prop.GetCustomAttribute<PrimaryKeyAttribute>();
                    return columnAttr != null ? columnAttr.ColumnName : primaryKeyAttr?.ColumnName ?? prop.Name;
                })];
    }

    public static string GetColumnName<T>(string propertyName) where T : BaseModel
    {
        // Get the type of the model
        Type type = typeof(T);

        // Get the PropertyInfo for the property name
        PropertyInfo property = type.GetProperty(propertyName)
            ?? throw new ArgumentException($"Property '{propertyName}' not found in type {type.Name}");

        // Check for Column attribute
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        if (columnAttribute != null)
        {
            return columnAttribute.ColumnName;
        }

        // Check for PrimaryKey attribute
        var primaryKeyAttribute = property.GetCustomAttribute<PrimaryKeyAttribute>();
        if (primaryKeyAttribute != null)
        {
            return primaryKeyAttribute.ColumnName;
        }

        // If no attribute is found, throw an exception or return a default (e.g., property name)
        throw new ArgumentException($"Property '{propertyName}' has no Column or PrimaryKey attribute");
    }
}
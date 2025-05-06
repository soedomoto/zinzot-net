using System.Reflection;
using AntDesign.TableModels;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using ZinzotNet.Models;
using static Supabase.Postgrest.Constants;

namespace ZinzotNet.Services
{

    public class TableReferenceState(ISupabaseService SupabaseService)
    {
        public QueryModel<TableReferenceModel> _query = new(1, 10, 0, [], []);

        public bool _loading = false;
        public int _total = 0;
        public List<TableReferenceModel> _dataSource = [];

        public async Task OnTableChange(QueryModel<TableReferenceModel> query)
        {
            _query = query;
            await _OnTableChange();
        }

        public async Task _OnTableChange()
        {
            try
            {
                _loading = true;

                var cresponse = await SupabaseService.Client
                    .From<TableReferenceModel>()
                    .Count(Constants.CountType.Exact);

                var query = SupabaseService.Client
                    .From<TableReferenceModel>()
                    .Select(string.Join(",", SupabaseModelExtensions.GetColumnNames(typeof(TableReferenceModel))))
                    .Range((_query.PageIndex - 1) * _query.PageSize, _query.PageIndex * _query.PageSize - 1);

                foreach (var sort in _query.SortModel)
                {
                    var colName = SupabaseModelExtensions.GetColumnName<TableReferenceModel>(sort.FieldName);
                    if (sort.SortDirection != AntDesign.SortDirection.None)
                    {
                        var ordering = Constants.Ordering.Ascending;
                        if (sort.SortDirection == AntDesign.SortDirection.Descending)
                        {
                            ordering = Constants.Ordering.Descending;
                        }

                        query = query.Order(colName, ordering);
                    }
                }

                var response = await query.Get();

                _loading = false;
                _total = cresponse;
                _dataSource = response?.Models ?? [];
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
        private string _id;
        public bool _loading;
        public ReferenceModel _dataSource = new();

        public async Task OnParamChange(string id)
        {
            _id = id;
            await _OnParamChange();
        }

        public async Task _OnParamChange()
        {
            try
            {
                _loading = true;

                var ItemCreatorCols = SupabaseModelExtensions.GetColumnNames(typeof(ItemCreator));
                var itemCreatorsSel = "item_creators(" + string.Join(",", ItemCreatorCols) + ")";

                var ReferenceModelCols = SupabaseModelExtensions.GetColumnNames(typeof(ReferenceModel));
                // ReferenceModelCols = ReferenceModelCols.Append(itemCreatorsSel);
                var referenceModelSel = string.Join(",", ReferenceModelCols);

                var query = SupabaseService.Client
                    .From<ReferenceModel>()
                    .Select(referenceModelSel)
                    .Filter("itemId", Operator.Equals, _id);

                var reference = (await query.Get())?.Models?.FirstOrDefault() ?? new ReferenceModel();
                for (int i = 0; i < reference.Attachments.Length; i++)
                {
                    reference.Attachments[i] = await S3Service.GetPresignedUrl(reference.Attachments[i]);
                }

                _loading = false;
                _dataSource = reference ?? new ReferenceModel();

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
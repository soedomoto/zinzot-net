using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ZinzotNet.Models
{
    [Table("collections")]
    public class CollectionModel : BaseModel
    {
        [PrimaryKey("id")]
        public string? Key { get; set; }
        [Column("label")]
        public string? Label { get; set; }
        [Column("icon")]
        public string? Icon { get; set; }
        [Column("parent_id")]
        public string? ParentId { get; set; }
        [Reference(typeof(CollectionModel), true, false)]
        public List<CollectionModel> Children { get; set; } = [];
    }
}
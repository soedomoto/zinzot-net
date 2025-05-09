using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ZinzotNet.Models
{
    [Table("collections")]
    public class CollectionModel : BaseModel
    {
        [PrimaryKey("id")]
        public string Key { get; set; }
        [Column("label")]
        public string Label { get; set; }
        [Column("icon")]
        public string Icon { get; set; }
        [Column("parent_id")]
        public string ParentId { get; set; }
        [Reference(typeof(CollectionModel), true, false)]
        public List<CollectionModel> Children { get; set; } = [];
    }

    [Table("items")]
    public class TableReferenceModel : BaseModel
    {
        [PrimaryKey("itemId")]
        public string? ItemID { get; set; }
        [Column("title")]
        public string? Title { get; set; }
    }

    public class ReferenceModel : TableReferenceModel
    {
        [Column("DOI")]
        public string? DOI { get; set; }
        [Column("ISBN")]
        public string? ISBN { get; set; }
        [Column("ISSN")]
        public string? ISSN { get; set; }
        [Column("abstractNote")]
        public string? AbstractNote { get; set; }
        [Column("accessDate")]
        public string? AccessDate { get; set; }
        [Column("date")]
        public string? Date { get; set; }
        [Column("itemType")]
        public string? ItemType { get; set; }
        [Column("libraryCatalog")]
        public string? LibraryCatalog { get; set; }
        [Column("pages")]
        public string? Pages { get; set; }
        [Column("place")]
        public string? Place { get; set; }
        [Column("publicationTitle")]
        public string? PublicationTitle { get; set; }
        [Column("publisher")]
        public string? Publisher { get; set; }
        [Column("series")]
        public string? Series { get; set; }
        [Column("title")]
        public string? Title { get; set; }
        [Column("url")]
        public string? Url { get; set; }
        [Column("attachments")]
        public string[] Attachments { get; set; } = [];
        [Column("issue")]
        public string? Issue { get; set; }
        [Column("volume")]
        public string? Volume { get; set; }
        [Column("shortTitle")]
        public string? ShortTitle { get; set; }
        [Column("extra")]
        public string? Extra { get; set; }
        [Column("conferenceName")]
        public string? ConferenceName { get; set; }
        [Column("language")]
        public string? Language { get; set; }
        [Column("journalAbbreviation")]
        public string? JournalAbbreviation { get; set; }
        [Column("edition")]
        public string? Edition { get; set; }
        [Reference(typeof(ItemCreator), true, false)]
        public List<ItemCreator> ItemCreators { get; set; } = [];
    }

    [Table("item_creators")]
    public class ItemCreator : BaseModel
    {
        [PrimaryKey("id")]
        public string? ID { get; set; }
        [Column("createdAt")]
        public string? CreatedAt { get; set; }
        [Column("itemId")]
        public string? ItemID { get; set; }
        [Column("creatorId")]
        public string? CreatorID { get; set; }
        [Column("creatorType")]
        public string? CreatorType { get; set; }
        [Column("creatorTypeID")]
        public string? CreatorTypeID { get; set; }
        [Reference(typeof(Creator), true, true)]
        public required Creator Creator { get; set; }
    }

    [Table("creators")]
    public class Creator : BaseModel
    {
        [PrimaryKey("creatorId")]
        public string? CreatorID { get; set; }
        [Column("createdAt")]
        public string? CreatedAt { get; set; }
        [Column("firstName")]
        public required string FirstName { get; set; }
        [Column("lastName")]
        public required string LastName { get; set; }
    }
}
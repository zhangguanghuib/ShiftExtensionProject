namespace WKEA.Commerce.Runtime.DataModel
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.ComponentModel.DataAnnotations;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using SystemDataAnnotations = System.ComponentModel.DataAnnotations;

    public class WKEAProductExtended: CommerceEntity
    {
        private const string ItemIdColumn = "ItemId";
        private const string ProductIdColumn = "Product";
        private const string MTItemIdColumn = "MTItemId";
        private const string MTProductIdColumn = "MTProduct";
        private const string DataAreaIdColumn = "DataAreaId";

        [DataMember]
        [SystemDataAnnotations.Key]
        [Key]
        [Column(ItemIdColumn)]
        public string ItemId
        {
            get { return (string)this[ItemIdColumn]; }
            set { this[ItemIdColumn] = value; }
        }

        [DataMember]
        [Column(ProductIdColumn)]
        public long Product
        {
            get { return (long)this[ProductIdColumn]; }
            set { this[ProductIdColumn] = value; }
        }

        [DataMember]
        [Column(MTItemIdColumn)]
        public string MTItemId
        {
            get { return (string)this[MTItemIdColumn]; }
            set { this[MTItemIdColumn] = value; }
        }

        [DataMember]
        [Column(MTProductIdColumn)]
        public long MTProduct
        {
            get { return (long)this[MTProductIdColumn]; }
            set { this[MTProductIdColumn] = value; }
        }

        [DataMember]
        [Column(DataAreaIdColumn)]
        public string DataAreaId
        {
            get { return (string)this[DataAreaIdColumn]; }
            set { this[DataAreaIdColumn] = value; }
        }

        public WKEAProductExtended()
            : base("WKEAProductExtended")
        {
        }
    }
}

namespace WKEA.Commerce.Runtime.Messages
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class WKEAProductExtendedDataRequest : Request
    {
        public WKEAProductExtendedDataRequest(string itemId, long? productId)
        {
            this.ItemdId = itemId;
            this.ProductId = productId;
        }

        [DataMember]
        public string ItemdId { private set; get; }

        [DataMember]
        public long? ProductId { private set; get; }
    }
}

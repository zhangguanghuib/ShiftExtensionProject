namespace WKEA.Commerce.Runtime.Messages
{
    using System.Runtime.Serialization;
    using Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;


    [DataContract]
    public sealed class WKEAProductExtendedDataReponse: Response
    {
        public WKEAProductExtendedDataReponse(PagedResult<WKEAProductExtended> wKEAProductsExtended)
        {
            this.WKEAProductsExtended = wKEAProductsExtended;
        }

        [DataMember]
        public PagedResult<WKEAProductExtended> WKEAProductsExtended { get; private set; }
    }
}

namespace Contoso.Commerce.Runtime.WeChatPay
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;

    [DataContract]
    public class WKEAProductResponse : Response
    {
        public WKEAProductResponse(PagedResult<SimpleProduct> i_WKEAProducts)
        {
            this.WKEAProducts = i_WKEAProducts;
        }
        public WKEAProductResponse(PagedResult<ProductSearchResult> i_WKEAProductSearchResult)
        {
            this.WKEAProductSearchResult = i_WKEAProductSearchResult;
        }

        [DataMember]
        public PagedResult<SimpleProduct> WKEAProducts { get; private set; }

        [DataMember]
        public PagedResult<ProductSearchResult> WKEAProductSearchResult { get; private set; }
    }
}

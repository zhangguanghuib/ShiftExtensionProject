namespace Contoso.Commerce.Runtime.WeChatPay
{
    using System.Runtime.Serialization;
    using Microsoft.Dynamics.Commerce.Runtime.Messages;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using System.Collections.Generic;
    using System.Net.Http;

    [DataContract]
    public class WKEAProductRequest : Request
    {

        public WKEAProductRequest(HttpRequestMessage httpContent)
        {
            this.httpContent = httpContent;
        }
        public WKEAProductRequest()
        {

        }

        [DataMember]
        public HttpRequestMessage httpContent { get; private set; }

        [DataMember]
        public List<SimpleProduct> ListSimpleProduct { get; set; }

        public List<ProductSearchResult> ListProductSearchResult { get; set; }

    }
}

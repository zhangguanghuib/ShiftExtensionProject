namespace WKEA.Dynamics.Retail.RetailServerLibrary.ODataControllers
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.Commerce.Runtime;
    using Microsoft.Dynamics.Commerce.Runtime.DataModel;
    using Microsoft.Dynamics.Commerce.Runtime.Hosting.Contracts;
    using WKEA.Commerce.Runtime.DataModel;
    using WKEA.Commerce.Runtime.Messages;

    [RoutePrefix("WKEAProducts")]
    [BindEntity(typeof(WKEAProductExtended))]
    public class WKEAProductsController : IController
    {
        [HttpGet]
        [Authorization(CommerceRoles.Anonymous, CommerceRoles.Customer, CommerceRoles.Device, CommerceRoles.Employee)]
        public async Task<PagedResult<WKEAProductExtended>> GetWKEAProductsExtended(IEndpointContext context, long productId, string itemId, QueryResultSettings queryResultSettings)
        {
            var request = new WKEAProductExtendedDataRequest(itemId, productId)
            {
                QueryResultSettings = queryResultSettings
            };

            var reponse = await context.ExecuteAsync<WKEAProductExtendedDataReponse>(request).ConfigureAwait(false);
            return  reponse.WKEAProductsExtended;
        }
    }
}

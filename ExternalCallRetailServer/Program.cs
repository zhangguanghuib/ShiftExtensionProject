using Microsoft.Dynamics.Commerce.RetailProxy;
using Microsoft.Dynamics.Commerce.RetailProxy.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtAppToRS
{
    class Program
    {
        private static string clientId;
        private static string clientSecret;
        private static Uri retailServerUrl;
        private static string resource;
        private static string operatingUnitNumber;
        private static Uri authority;

        private static void GetConfiguration()
        {
            clientId = ConfigurationManager.AppSettings["aadClientId"];
            clientSecret = ConfigurationManager.AppSettings["aadClientSecret"];
            authority = new Uri(ConfigurationManager.AppSettings["aadAuthority"]);
            retailServerUrl = new Uri(ConfigurationManager.AppSettings["retailServerUrl"]);
            operatingUnitNumber = ConfigurationManager.AppSettings["operatingUnitNumber"];
            resource = ConfigurationManager.AppSettings["resource"];
        }


        private static async Task<ManagerFactory> CreateManagerFactory()
        {
            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority.ToString(), false);
            AuthenticationResult authResult = null;
            authResult = await authenticationContext.AcquireTokenAsync(resource, new ClientCredential(clientId, clientSecret));

            ClientCredentialsToken clientCredentialsToken = new ClientCredentialsToken(authResult.AccessToken);
            RetailServerContext retailServerContext = RetailServerContext.Create(retailServerUrl, operatingUnitNumber, clientCredentialsToken);
            ManagerFactory factory = ManagerFactory.Create(retailServerContext);
            return factory;
        }

        private static async Task<Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<SalesOrder>> GetOrderHistory(string customerId)
        {
            QueryResultSettings querySettings = new QueryResultSettings
            {
                Paging = new PagingInfo() { Top = 10, Skip = 10 }
            };

            ManagerFactory managerFactory = await CreateManagerFactory();
            ICustomerManager customerManage = managerFactory.GetManager<ICustomerManager>();
            return await customerManage.GetOrderHistory(customerId, querySettings);
        }

        private static async Task<Cart> CreateCart(string customerId)
        {
            ManagerFactory managerFactory = await CreateManagerFactory();
            ICartManager cartManager = managerFactory.GetManager<ICartManager>();
            Cart cart = new Cart()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerId
            };

            return await cartManager.Create(cart);
        }


        static void Main(string[] args)
        {

            try
            {
                GetConfiguration();
                Cart cart = Task.Run(async () => await CreateCart("2001")).Result;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //try
            //{
            //    GetConfiguration();
            //    Microsoft.Dynamics.Commerce.RetailProxy.PagedResult<SalesOrder> orderHistory = Task.Run(async () => await GetOrderHistory("2001")).Result;

            //    Console.WriteLine("Order Number | Customer Account | Date Time                     | Total Amount | Currency Code");
            //    foreach (SalesOrder salesOrder in orderHistory.Results)
            //    {
            //        Console.WriteLine(salesOrder.SalesId + "       |     " + salesOrder.CustomerId + "         |  " + salesOrder.CreatedDateTime + " |  " + salesOrder.TotalAmount + "     | " + salesOrder.CurrencyCode);
            //    }
            //    Console.WriteLine(orderHistory.Results.GetEnumerator().Current.Id);
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
    }
}

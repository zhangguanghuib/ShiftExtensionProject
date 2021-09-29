
  // tslint:disable
  import * as EntityClasses from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceModels.g';
  import * as Entities from '@msdyn365-commerce/retail-proxy/dist/Entities/CommerceTypes.g';  
  import { jsonLightReadStringPropertyValue } from '@msdyn365-commerce/retail-proxy/dist/externals';
  
  
    /**
    * WKEAProductExtended entity interface.
    */
    export interface IWKEAProductExtended {
    ItemId: string;
    Product: number;
    DataAreaId?: string;
    MTItemId?: string;
    MTProduct?: number;
    Images?: Entities.MediaLocation[];
    ExtensionProperties?: Entities.CommerceProperty[];
    }
  
    /**
    * WeChatPayQrCodeInfo entity interface.
    */
    export interface IWeChatPayQrCodeInfo {
    QrCodeText?: string;
    WeChatPayOrderId?: string;
    CartId?: string;
    ExpirationTimeUtc: Date;
    }
  
    /**
    * WeChatPayResult entity interface.
    */
    export interface IWeChatPayResult {
    CardPaymentAcceptResult?: Entities.CardPaymentAcceptResult;
    StatusInfo?: IWeChatPayStatusInfo;
    }
  
    /**
    * WeChatPayStatusInfo entity interface.
    */
    export interface IWeChatPayStatusInfo {
    Status: string;
    StatusValue: number;
    Message?: string;
    }
  
  /**
   * WKEAProductExtended entity class.
   */
  export class WKEAProductExtendedExtensionClass implements IWKEAProductExtended {
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public ItemId: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public Product: number;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public DataAreaId: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public MTItemId: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public MTProduct: number;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public Images: Entities.MediaLocation[];
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public ExtensionProperties: Entities.CommerceProperty[];
      
      // Navigation properties names
      
      /**
       * Construct an object from odata response.
       * @param {any} odataObject The odata result object.
       */
      constructor(odataObject?: any) {
      odataObject = odataObject || {};
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.ItemId = odataObject.ItemId;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.Product = (odataObject.Product) ? parseInt(odataObject.Product, 10) : 0;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.DataAreaId = odataObject.DataAreaId;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.MTItemId = odataObject.MTItemId;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.MTProduct = (odataObject.MTProduct) ? parseInt(odataObject.MTProduct, 10) : 0;
              
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        this.Images = undefined;
        if (odataObject.Images) {
          this.Images = [];
          for (var i = 0; i < odataObject.Images.length; i++) {
            if (odataObject.Images[i]) {
        if (odataObject.Images[i]['@odata.type']) {
          var className: string = odataObject.Images[i]['@odata.type'];
          className = className.substr(className.lastIndexOf('.') + 1).concat("Class");
          if (EntityClasses[className]) {
            this.Images[i] = new EntityClasses[className](odataObject.Images[i])
          }
        } else {
          this.Images[i] = new EntityClasses.MediaLocationClass(odataObject.Images[i]);
        }
      
            } else {
              // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
              this.Images[i] = undefined;
            }
          }
        }
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        this.ExtensionProperties = undefined;
        if (odataObject.ExtensionProperties) {
          this.ExtensionProperties = [];
          for (var i = 0; i < odataObject.ExtensionProperties.length; i++) {
            if (odataObject.ExtensionProperties[i]) {
        if (odataObject.ExtensionProperties[i]['@odata.type']) {
          var className: string = odataObject.ExtensionProperties[i]['@odata.type'];
          className = className.substr(className.lastIndexOf('.') + 1).concat("Class");
          if (EntityClasses[className]) {
            this.ExtensionProperties[i] = new EntityClasses[className](odataObject.ExtensionProperties[i])
          }
        } else {
          this.ExtensionProperties[i] = new EntityClasses.CommercePropertyClass(odataObject.ExtensionProperties[i]);
        }
      
            } else {
              // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
              this.ExtensionProperties[i] = undefined;
            }
          }
        }
      
      }
  }

  /**
   * WeChatPayQrCodeInfo entity class.
   */
  export class WeChatPayQrCodeInfoExtensionClass implements IWeChatPayQrCodeInfo {
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public QrCodeText: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public WeChatPayOrderId: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public CartId: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public ExpirationTimeUtc: Date;
      
      // Navigation properties names
      
      /**
       * Construct an object from odata response.
       * @param {any} odataObject The odata result object.
       */
      constructor(odataObject?: any) {
      odataObject = odataObject || {};
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.QrCodeText = odataObject.QrCodeText;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.WeChatPayOrderId = odataObject.WeChatPayOrderId;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.CartId = odataObject.CartId;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.ExpirationTimeUtc = (odataObject.ExpirationTimeUtc instanceof Date) ? odataObject.ExpirationTimeUtc
                : (odataObject.ExpirationTimeUtc) ? jsonLightReadStringPropertyValue(odataObject.ExpirationTimeUtc, 'Edm.DateTimeOffset', false)  : undefined;
              
      }
  }

  /**
   * WeChatPayResult entity class.
   */
  export class WeChatPayResultExtensionClass implements IWeChatPayResult {
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public CardPaymentAcceptResult: Entities.CardPaymentAcceptResult;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public StatusInfo: 
            IWeChatPayStatusInfo;
      
      // Navigation properties names
      
      /**
       * Construct an object from odata response.
       * @param {any} odataObject The odata result object.
       */
      constructor(odataObject?: any) {
      odataObject = odataObject || {};
        if (odataObject.CardPaymentAcceptResult === null) {
          // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
          this.CardPaymentAcceptResult = undefined;
        } else if (odataObject.CardPaymentAcceptResult['@odata.type'] == null) {
          this.CardPaymentAcceptResult = new EntityClasses.CardPaymentAcceptResultClass(odataObject.CardPaymentAcceptResult);
        } else {
          var className: string = odataObject.CardPaymentAcceptResult['@odata.type'];
          className = className.substr(className.lastIndexOf('.') + 1).concat("Class");
          if (EntityClasses[className]) {
            this.CardPaymentAcceptResult = new EntityClasses[className](odataObject.CardPaymentAcceptResult)
          }
        }

      
        if (odataObject.StatusInfo === null) {
          // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
          this.StatusInfo = undefined;
        } else if (odataObject.StatusInfo['@odata.type'] == null) {
          this.StatusInfo = new WeChatPayStatusInfoExtensionClass(odataObject.StatusInfo);
        } else {
          var className: string = odataObject.StatusInfo['@odata.type'];
          className = className.substr(className.lastIndexOf('.') + 1).concat("Class");
          if (EntityClasses[className]) {
            this.StatusInfo = new EntityClasses[className](odataObject.StatusInfo)
          }
        }

      
      }
  }

  /**
   * WeChatPayStatusInfo entity class.
   */
  export class WeChatPayStatusInfoExtensionClass implements IWeChatPayStatusInfo {
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public Status: string;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public StatusValue: number;
      
        // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
        public Message: string;
      
      // Navigation properties names
      
      /**
       * Construct an object from odata response.
       * @param {any} odataObject The odata result object.
       */
      constructor(odataObject?: any) {
      odataObject = odataObject || {};
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.Status = odataObject.Status;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.StatusValue = odataObject.StatusValue;
              
            // @ts-ignore - TODO bug fix #23427261 - remove ts-ignore - https://microsoft.visualstudio.com/DefaultCollection/OSGS/_workitems/edit/23427261/
            this.Message = odataObject.Message;
              
      }
  }

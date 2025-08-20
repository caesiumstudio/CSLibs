using System;
using System.Collections.Generic;
using CS;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPAppleStore : IAPStore
{
    private Logger logger = new Logger("IAPAppleStore");

    public event OnPurchaseSuccess OnPurchaseSuccessCB;
    public event OnPurchaseFailure OnPurchaseFailureCB;
    public event OnProductsLoaded OnProductsLoadedCB;

    public void SetOnPurchaseSuccess(OnPurchaseSuccess fun)
    {
        this.OnPurchaseSuccessCB += fun;
    }
    public void SetOnPurchaseFailure(OnPurchaseFailure fun)
    {
        this.OnPurchaseFailureCB += fun;
    }
    public void SetOnProductsLoaded(OnProductsLoaded fun)
    {
        this.OnProductsLoadedCB += fun;
    }

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;
    private string[] productList;

    /*** singleton***/
    private static IAPAppleStore instance = new IAPAppleStore();

    public static IAPAppleStore getInstance()
    {
        if (IAPAppleStore.instance == null)
            IAPAppleStore.instance = new IAPAppleStore();

        return IAPAppleStore.instance;
    }
    /*** singleton***/

    public async void LoadIAPProductCatalog()
    {
        logger.Log("Loading catalog");
        InitializationOptions options = new InitializationOptions().SetEnvironmentName("production");
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += OnIAPCatalogLoaded;
    }

    private void OnIAPCatalogLoaded(AsyncOperation operation)
    {
        ResourceRequest request = operation as ResourceRequest;
        logger.Log((request.asset as TextAsset).text);
        logger.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>(
            (request.asset as TextAsset).text
        );

        logger.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(this.GetPlatformAppStore())
        );

        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            logger.Log("Adding Prod Type: " + item.type + " Prod Id: " + item.id);
            builder.AddProduct(item.id, item.type);
        }

        // logger.Log($"Initializing Unity IAP with {builder.products.Count} products");
        // This will trigger onInitialized;
        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                logger.Log(string.Format("Purchasing product: '{0}'", product.definition.id));
                storeController.InitiatePurchase(product);
            }
            else
            {
                logger.Error("Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            logger.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    // START - Implement interface IStoreListener
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        logger.Log("OnInitialized: Init Success");
        storeController = controller;
        storeExtensionProvider = extensions;
        List<Product> availableProducts = new List<Product>();
        for (int i = 0; i < storeController.products.all.Length; i++)
        {
            Product product = storeController.products.all[i];
            if (product.availableToPurchase)
            {
                logger.Log("AVAILABLE TO PURCHASE - " + product.definition.id);
                availableProducts.Add(product);
            } else {
                logger.Log("NOT AVAILABLE TO PURCHASE - " + product.definition.id);
            }

        }
        OnProductsLoadedCB(availableProducts);
    }


    public bool IsSubscribedTo(Product product)
    {
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (product.receipt == null)
        {
            logger.Log("No product receipt found for " + product.definition.id);
            return false;

        }

        SubscriptionManager subscriptionManager = new SubscriptionManager(product, null);
        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
        SubscriptionInfo info = subscriptionManager.getSubscriptionInfo();
        logger.Log("SubInfo: " + info.ToString());
        return info.isSubscribed() == Result.True;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        logger.Log(
            string.Format(
                "OnPurchaseFailed: Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId,
                failureDescription
            )
        );
        OnPurchaseFailureCB(failureDescription.message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        logger.Log(
            string.Format(
                "OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId,
                failureReason
            )
        );
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;
        logger.Log("Purchased Item: " + productId);
        OnPurchaseSuccessCB(productId);

        return PurchaseProcessingResult.Complete;
    }

    // END - Implement interface IStoreListener

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ...
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((bool decision, string result) =>
            {
                Debug.Log("RestorePurchases continuing: " + result +
                          ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        logger.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string errorString)
    {
        logger.Error("OnInitializeFailed InitializationFailureReason:" + error);
    }

    private AppStore GetPlatformAppStore()
    {
        if (Utils.IsAndroid())
            return AppStore.GooglePlay;
        if (Utils.IsIPhone())
            return AppStore.AppleAppStore;

        return AppStore.NotSpecified;
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && storeExtensionProvider != null;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using Unity.Services.Core.Environments;


public delegate void OnPurchaseSuccess(string productId);
public delegate void OnPurchaseFailure(string purchaseFailureDescription);
public delegate void OnProductsLoaded(List<Product> products);

public interface IAPStore : IDetailedStoreListener
{
    public void RestorePurchases();
    public void LoadIAPProductCatalog();
    public bool IsSubscribedTo(Product product);
    public void BuyProductID(string productId);

    public void SetOnPurchaseSuccess(OnPurchaseSuccess fun);
    public void SetOnPurchaseFailure(OnPurchaseFailure fun);
    public void SetOnProductsLoaded(OnProductsLoaded fun);
}

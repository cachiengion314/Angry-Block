using System;
using System.Collections;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.SceneManagement;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
  public static IAPManager Instance { get; private set; }

  // Tạo một cấu trúc để nhập dữ liệu từ Inspector
  [Serializable]
  public struct IAPProduct
  {
    public string productId;
    public ProductType productType; // Consumable, NonConsumable, Subscription
  }

  // Danh sách các sản phẩm IAP nhập từ Inspector
  public IAPProduct[] products;

  public static string Prefix;

  private IStoreController storeController;
  private IExtensionProvider extensionProvider;

  // Callback trả về kết quả mua hàng
  private Action<bool, string> onPurchaseCallback;

  bool isRestoreComplete = false;
  public bool IsRestoreComplete { get { return isRestoreComplete; } }

  private void Awake()
  {
    // Đảm bảo đây là Singleton
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
#if UNITY_IOS
    Prefix = "com.cmz.coffeepack.";
#elif UNITY_ANDROID
    Prefix = Application.identifier + ".";
#endif

    InitializePurchasing();

    // CheckSubscriptionStatus(KeyString.KEY_IAP_REMOVEADS_SUBSCRIPTION, 
    // (isVIP, productId) =>
    // {
    //   GameManager.Instance.IsRemoveAds7d = isVIP;
    // });
  }

  public void InitializePurchasing()
  {
    if (IsInitialized())
    {
      return;
    }

    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

    // Tự động thêm các sản phẩm được khai báo từ Inspector vào cấu hình IAP
    foreach (var product in products)
    {
      builder.AddProduct(Prefix + product.productId, product.productType);
    }

    UnityPurchasing.Initialize(this, builder); // Sử dụng IDetailedStoreListener
  }

  private bool IsInitialized()
  {
    return storeController != null && extensionProvider != null;
  }

  public string GetLocalizedPrice(string productId)
  {
    if (IsInitialized())
    {
      // Lấy sản phẩm sau khi khởi tạo thành công
      Product product = storeController.products.WithID(Prefix + productId);

      if (product != null)
      {
        // Lấy giá của sản phẩm
        string localizedPrice = product.metadata.localizedPriceString;
        return localizedPrice;
      }
      else
      {
        // Debug.LogWarning("Không tìm thấy sản phẩm hoặc sản phẩm chưa có giá.");
        return null;
      }
    }
    else
    {
      // Debug.LogWarning("Unity IAP chưa được khởi tạo. Không thể lấy giá.");
      return null;
    }
  }

  // Hàm mua sản phẩm, truyền vào productId và callback trả về bool
  public void PurchaseProduct(string productId, Action<bool, string> callback)
  {
    ShowLoading();
    if (IsInitialized())
    {
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_INAPP_EVENT);
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_INAPP_PROCESS);
      }

      Product product = storeController.products.WithID(Prefix + productId);

      if (product != null && product.availableToPurchase)
      {
        Debug.Log($"Purchasing product asynchronously: '{product.definition.id}'");
        onPurchaseCallback = callback; // Lưu callback
        storeController.InitiatePurchase(product);
      }
      else
      {
        Debug.Log("BuyProductID: FAIL. Product not found or not available for purchase");
        callback?.Invoke(false, Prefix + productId);
        HideLoading();
      }
    }
    else
    {
      Debug.Log("BuyProductID FAIL. Not initialized.");
      callback?.Invoke(false, Prefix + productId);
      HideLoading();
    }
  }

  // Hàm khôi phục giao dịch
  public void RestorePurchases(Action<bool, string> callback)
  {
    if (!IsInitialized())
    {
      Debug.LogWarning("Unity IAP chưa được khởi tạo. Không thể khôi phục giao dịch.");
      return;
    }

    onPurchaseCallback = callback;
    isRestoreComplete = false;
    ShowLoading();

    // Phân biệt nền tảng đang chạy
#if UNITY_EDITOR
    Debug.Log("Đang chạy trên Unity Editor. Không có khôi phục giao dịch thực tế.");
    RestorePurchasesEditor();
#elif UNITY_IOS
        Debug.Log("Đang khôi phục giao dịch trên iOS...");
        extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((result, message) =>
        {
            if (result)
            {
                // Các sản phẩm sẽ được xử lý trong ProcessPurchase
                if(!isRestoreComplete){
                  // callback?.Invoke(false, message);
                  // HideLoading();
                  StartCoroutine(RestorePurchasesFailed());
                  Debug.LogWarning($"Không có giao dịch nào để khôi phục trên iOS. Thông báo: {message}");
                }
            }
            else
            {
                // callback?.Invoke(false, message);
                // HideLoading();
                StartCoroutine(RestorePurchasesFailed());
                Debug.LogWarning($"Không có giao dịch nào để khôi phục trên iOS. Thông báo: {message}");
            }
        });
#elif UNITY_ANDROID
        Debug.Log("Đang khôi phục giao dịch trên Android...");
        extensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions((result, message) =>
        {
            if (result)
            {
                // Các sản phẩm sẽ được xử lý trong ProcessPurchase
                if(!isRestoreComplete){
                  // callback?.Invoke(false, message);
                  // HideLoading();
                  StartCoroutine(RestorePurchasesFailed());
                  Debug.LogWarning($"Không có giao dịch nào để khôi phục trên Android. Thông báo: {message}");
                }
            }
            else
            {
                // callback?.Invoke(false, message);
                // HideLoading();
                StartCoroutine(RestorePurchasesFailed());
                Debug.LogWarning($"Không có giao dịch nào để khôi phục trên Android. Thông báo: {message}");
            }
        });
#else
        Debug.LogWarning("Nền tảng này không hỗ trợ khôi phục giao dịch.");
#endif
  }

  IEnumerator RestorePurchasesFailed()
  {
    yield return new WaitForSeconds(1.5f);
    onPurchaseCallback?.Invoke(false, "Failed");
    HideLoading();
  }

  // Mở trang Quản lý Subscription
  public void OpenSubscriptionSettings()
  {
#if UNITY_ANDROID
    Application.OpenURL("https://play.google.com/store/account/subscriptions");
#elif UNITY_IOS
    Application.OpenURL("https://apps.apple.com/account/subscriptions");
#endif
  }

  public void CheckSubscriptionStatus(string productId, Action<bool, string> callback)
  {
    if (IsInitialized())
    {
      Product product = storeController.products.WithID(Prefix + productId);
      if (product != null && product.hasReceipt)
      {
        SubscriptionManager subscriptionManager = new SubscriptionManager(product, null);
        SubscriptionInfo info = subscriptionManager.getSubscriptionInfo();

        if (info.isSubscribed() == Result.True)
        {
          Debug.Log("Subscription is active");
          callback?.Invoke(true, Prefix + productId);
        }
        else
        {
          Debug.Log("Subscription is inactive");
          callback?.Invoke(false, Prefix + productId);
        }
      }
      else
      {
        callback?.Invoke(false, Prefix + productId);
      }
    }
  }

  // Xử lý khi khởi tạo IAP thành công
  public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
  {
    storeController = controller;
    extensionProvider = extensions;
    Debug.Log("Unity IAP đã khởi tạo thành công.");
  }

  // Xử lý khi khởi tạo IAP thất bại
  public void OnInitializeFailed(InitializationFailureReason error)
  {
    Debug.LogError("Khởi tạo IAP thất bại: " + error);
  }
  public void OnInitializeFailed(InitializationFailureReason error, string message)
  {
    var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

    if (message != null)
    {
      errorMessage += $" More details: {message}";
    }

    Debug.Log(errorMessage);
  }

  // Xử lý giao dịch mua thành công và khôi phục sản phẩm
  public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
  {
    if (args.purchasedProduct.hasReceipt)
    {
      Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
      // Trả về kết quả thành công thông qua callback nếu có
      onPurchaseCallback?.Invoke(true, args.purchasedProduct.definition.id);
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_INAPP_SUCCESS);
      }
      HideLoading();
      isRestoreComplete = true;
      return PurchaseProcessingResult.Complete;
    }
    return PurchaseProcessingResult.Pending;
  }

  // Xử lý khi mua hàng thất bại
  public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
  {
    onPurchaseCallback?.Invoke(false, product.definition.id);
    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_INAPP_FAIL);
    }

    HideLoading();
    Debug.LogError($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
  }

  public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
  {
    onPurchaseCallback?.Invoke(false, product.definition.id);
    HideLoading();
    Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
  }

  private void RestorePurchasesEditor()
  {
    onPurchaseCallback?.Invoke(true, KeyString.KEY_IAP_REMOVEADS);
  }

  void ShowLoading()
  {

  }

  void HideLoading()
  {

  }
}

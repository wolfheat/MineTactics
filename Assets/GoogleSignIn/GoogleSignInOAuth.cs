using i5.Toolkit.Core.DeepLinkAPI;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System;
using UnityEngine;

public class GoogleSignInOAuth : BaseServiceBootstrapper
{
    [SerializeField] private ClientDataObject googleClientDataObject;
    [SerializeField] private ClientDataObject googleClientDataObjectEditorOnly;

    [DeepLink("returnLink")]
    public void DeepLinkReturn()
    {
        Debug.Log("returnLink deep link returned");
    }


    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();

        
        // Deep Linking
        DeepLinkingService service = new DeepLinkingService();
        service.AddDeepLinkListener(this);
        ServiceManager.RegisterService(service);
        
        
        oidc.OidcProvider = new GoogleOidcProvider();

        // Unity Activate of Deep Link?
        Debug.Log("Deep Link URL (Application.absoluteURL) = "+ Application.absoluteURL);
        

#if !UNITY_EDITOR
        oidc.OidcProvider.ClientData = googleClientDataObject.clientData;
        oidc.RedirectURI = "com.WolfheatProduction.MineTactics:/";        
#else
        oidc.OidcProvider.ClientData = googleClientDataObjectEditorOnly.clientData;
        oidc.RedirectURI = "";
        //oidc.RedirectURI = "https://wolfheat.github.io/privacyPolicy.html";
#endif
        oidc.ServerListener.ListeningUri = "http://127.0.0.1:65192/";
        ServiceManager.RegisterService(oidc);

    }

    private void OnDeepLinkActivated(string url)
    {
        Debug.Log(" ***** On Deep Link Activated *****");
        Debug.Log(" ***** URL = "+url);
    }

    protected override void UnRegisterServices()
    {

    }
}

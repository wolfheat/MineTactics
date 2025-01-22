using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System;
using UnityEngine;

public class GoogleSignInOAuth : BaseServiceBootstrapper
{
    [SerializeField] private ClientDataObject googleClientDataObject;
    [SerializeField] private ClientDataObject googleClientDataObjectEditorOnly;


    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new GoogleOidcProvider();

#if !UNITY_EDITOR
        oidc.OidcProvider.ClientData = googleClientDataObject.clientData;
        // Deep linking
        // If App is installed Take to specific app content - else - Take to store to download app
        oidc.RedirectURI = "com.WolfheatProduction.MineTactics:/";

#else
        oidc.OidcProvider.ClientData = googleClientDataObjectEditorOnly.clientData;
        oidc.RedirectURI = "";
        //oidc.RedirectURI = "https://wolfheat.github.io/privacyPolicy.html";
        oidc.ServerListener.ListeningUri = "http://127.0.0.1:65192/";
#endif
        ServiceManager.RegisterService(oidc);

    }

    protected override void UnRegisterServices()
    {

    }
}

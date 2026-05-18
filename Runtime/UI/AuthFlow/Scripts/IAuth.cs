using FTRShared.UI.AuthMenu;
using UnityEngine;

public interface IAuth
{
    void Initialize(
        API.AuthService authService,
        Session.Session session,
        Logging.Logger logger,
        AuthFlowManager flowManager
    );
}

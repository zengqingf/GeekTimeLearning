package com.tm.sdk.platform;

import androidx.annotation.CallSuper;

import com.tm.sdk.open.src.inter.ICallbackHandler;

public class PlatformService implements IPlatformService {
    @CallSuper
    public void init(Boolean isDebug) {}

    @CallSuper
    public void login(ICallbackHandler callbackHandler) {}

    @CallSuper
    public void logout() {}

    @CallSuper
    public void exit() {}

    @CallSuper
    public void pay(String requestId,
                    String price,
                    String mallType,
                    String productId,
                    String productName,
                    String productShortName,
                    String productDesc,
                    String productChId,
                    String appName,
                    String serverId,
                    String serverName,
                    String notifyUrl,
                    String accountId,
                    String accountName,
                    String roleId,
                    String roleName,
                    String openuid,
                    String extra,
                    ICallbackHandler callbackHandler) {

    }

    @Override
    public boolean testCallResult() {
        return false;
    }
}

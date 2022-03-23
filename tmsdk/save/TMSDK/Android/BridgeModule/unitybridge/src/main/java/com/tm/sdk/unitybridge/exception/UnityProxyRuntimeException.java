package com.tm.sdk.unitybridge.exception;

public class UnityProxyRuntimeException extends RuntimeException {
    public UnityProxyRuntimeException() {
        super();
    }

    public UnityProxyRuntimeException(String message, Throwable cause) {
        super(message, cause);
    }

    public UnityProxyRuntimeException(String message) {
        super(message);
    }

    public UnityProxyRuntimeException(Throwable cause) {
        super(cause);
    }
}

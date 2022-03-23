package com.tm.sdk.bridge.exception;

public class BridgeRuntimeException extends RuntimeException {
    public BridgeRuntimeException() {
        super();
    }

    public BridgeRuntimeException(String message, Throwable cause) {
        super(message, cause);
    }

    public BridgeRuntimeException(String message) {
        super(message);
    }

    public BridgeRuntimeException(Throwable cause) {
        super(cause);
    }
}
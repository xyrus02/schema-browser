package org.xyrusworx;

public interface ModelErrorHandler {
    void handle(String error);
    void handle(Throwable error);
}

package org.xyrusworx;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

@SuppressWarnings("unused")
public class DefaultModelErrorHandler implements ModelErrorHandler {

    private static final Logger LOGGER = LogManager.getLogger(DefaultModelErrorHandler.class);
    private static final DefaultModelErrorHandler INSTANCE = new DefaultModelErrorHandler();

    private DefaultModelErrorHandler(){}
    public static DefaultModelErrorHandler getInstance() {
        return INSTANCE;
    }

    @Override
    public void handle(String error) {
        LOGGER.error(error);
    }

    @Override
    public void handle(Throwable error) {
        LOGGER.error(error);
    }
}

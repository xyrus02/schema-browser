package org.xyrusworx;

import com.fasterxml.jackson.core.JsonProcessingException;
import org.xyrusworx.model.Model;
import org.xyrusworx.model.ModelFactory;

public class SchemaModelTestApp {

    public static void main(String[] argv) throws JsonProcessingException {
        ModelFactory factory = ModelFactory.getInstance();
        Model model = factory.createModel(argv[0], DefaultModelErrorHandler.getInstance());

        System.out.println(model.serialize());
    }

}

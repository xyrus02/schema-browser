package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.xmlet.xsdparser.xsdelements.XsdAbstractElement;
import org.xmlet.xsdparser.xsdelements.XsdElement;
import org.xmlet.xsdparser.xsdelements.XsdSchema;
import org.xyrusworx.ModelErrorHandler;

import java.util.ArrayList;
import java.util.List;

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
@JsonPropertyOrder({"name", "namespace", "objects"})
public class Model {

    private final ObjectMapper serializer = new ObjectMapper();
    private final List<ModelObject> rootElements;
    private ModelObject[] rootElementsArray;
    private final String name;
    private final String namespace;
    private final ModelErrorHandler errorHandler;

    Model(String name, XsdSchema xsRootSchema, ModelErrorHandler errorHandler) {

        this.name = name;
        this.namespace = xsRootSchema.getTargetNamespace();
        this.errorHandler = errorHandler;

        rootElements = new ArrayList<>();
        rootElementsArray = null;
    }

    public String getName() {
        return this.name;
    }

    public String getNamespace() {
        return this.namespace;
    }

    public ModelObject[] getObjects() {
        if (this.rootElementsArray == null) {
            this.rootElementsArray = new ModelObject[this.rootElements.size()];
            this.rootElements.toArray(this.rootElementsArray);
        }

        return rootElementsArray;
    }

    public String serialize() throws JsonProcessingException {
        return this.serializer.writerWithDefaultPrettyPrinter().writeValueAsString(this);
    }

    @Override
    public String toString() {
        return serialize(this);
    }

    void addRootElement(XsdElement xsElement) {

        if (xsElement == null) {
            return;
        }

        XsdAbstractElement xsParent = xsElement.getParent();

        if (!(xsParent instanceof XsdSchema)) {
            if (this.errorHandler != null) {
                this.errorHandler.handle(String.format("The element \"%s\" is not a root element.", xsElement.getName()));
            }

            return;
        }

        this.rootElements.add(new ModelObject(this, (XsdSchema)xsParent, xsElement));
        this.rootElementsArray = null;
    }

    String serialize(Object obj) {
        try {
            return this.serializer.writeValueAsString(obj);
        } catch (JsonProcessingException e) {
            this.errorHandler.handle(e);
            return "null";
        }
    }

    ModelErrorHandler getErrorHandler() {
        return this.errorHandler;
    }
}


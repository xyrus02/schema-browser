package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import org.xmlet.xsdparser.xsdelements.*;
import org.xyrusworx.DefaultModelErrorHandler;
import org.xyrusworx.ModelErrorHandler;

import java.util.ArrayList;
import java.util.List;

@SuppressWarnings({"WeakerAccess", "unused"})
@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
@JsonPropertyOrder({"name", "namespace", "tokens", "attributes"})
@JsonInclude(JsonInclude.Include.NON_NULL)
public abstract class ModelElement {

    @JsonIgnore
    private final ModelElement parent;

    private final Model owner;
    private final ModelErrorHandler errorHandler;
    private final String ownNamespace;
    private ModelProperty[] attributes;
    private ModelToken[] tokens;
    private String namespace;
    private String name;

    ModelElement(Model owner, XsdSchema xsSchema) {
        this.owner = owner;
        this.namespace = xsSchema.getTargetNamespace();
        this.parent = null;
        this.errorHandler = owner.getErrorHandler();
        this.ownNamespace = null;
    }
    ModelElement(Model owner, ModelElement parent, XsdElement xsElement, String ownNamespace) {
        this.owner = owner;
        this.parent = parent;
        this.errorHandler = null;
        this.ownNamespace = ownNamespace;
        this.loadElement(xsElement);
    }

    public String getName() {
        return this.name;
    }

    public String getNamespace() {
        if (this.parent == null) {
            return this.namespace;
        }
        else {
            return this.ownNamespace != null ? this.ownNamespace : (this.parent.getNamespace());
        }
    }

    @JsonIgnore
    public ModelElement getParent() {
        return this.parent;
    }

    public ModelProperty[] getAttributes() {
        return this.attributes;
    }

    public ModelToken[] getTokens() { return this.tokens; }

    @Override
    public String toString() {
        return this.owner.serialize(this);
    }

    ModelErrorHandler getErrorHandler() {
        if (this.parent == null) {
            return this.errorHandler == null ? DefaultModelErrorHandler.getInstance() : this.errorHandler;
        }
        else {
            return (this.parent.getErrorHandler());
        }
    }

    void loadElement(XsdElement xsElement) {
        this.name = xsElement.getName();

        XsdComplexType complexType = xsElement.getXsdComplexType();
        List<ModelSimpleProperty> attributes = new ArrayList<>();
        List<ModelToken> tokens = new ArrayList<>();

        if (complexType != null) {
            complexType.getXsdAttributes().map(x -> new ModelSimpleProperty(this.owner, this, x)).forEach(attributes::add);
            complexType.getXsdAttributeGroup().flatMap(XsdAttributeGroup::getAllAttributes).map(x -> new ModelSimpleProperty(this.owner, this, x)).forEach(attributes::add);

            processComplexType(tokens, complexType);
        }

        this.attributes = new ModelSimpleProperty[attributes.size()];
        attributes.toArray(this.attributes);
        if (this.attributes.length == 0) {
            this.attributes = null;
        }

        this.tokens = new ModelToken[tokens.size()];
        tokens.toArray(this.tokens);
        if (this.tokens.length == 0) {
            this.tokens = null;
        }
    }

    private void processComplexType(List<ModelToken> tokens, XsdComplexType xsComplexType) {

        if (xsComplexType == null) {
            return;
        }

        XsdAll xsAll = xsComplexType.getElements() != null ? xsComplexType.getChildAsAll() : null;
        XsdChoice xsChoice = xsComplexType.getElements() != null ? xsComplexType.getChildAsChoice() : null;
        XsdSequence xsSequence = xsComplexType.getElements() != null ? xsComplexType.getChildAsSequence() : null;
        XsdGroup xsGroup = xsComplexType.getElements() != null ? xsComplexType.getChildAsGroup() : null;

        processTokens(tokens, xsAll, xsChoice, xsSequence, xsGroup);

        XsdComplexContent complexContent = xsComplexType.getComplexContent();
        XsdSimpleContent simpleContent = xsComplexType.getSimpleContent();

        processContent(tokens, complexContent, simpleContent);
    }

    private void processTokens(List<ModelToken> tokens, XsdAll xsAll, XsdChoice xsChoice, XsdSequence xsSequence, XsdGroup xsGroup) {
        if (xsAll != null) {
            ModelTokenContainer mtc = ModelTokenContainer.create(this.owner, this, xsAll);
            if (mtc == null || mtc.getChildren() == null || mtc.getChildren().length > 0) {
                tokens.add(mtc);
            }
        }
        if (xsChoice != null) {
            ModelTokenContainer mtc = ModelTokenContainer.create(this.owner, this, xsChoice);
            if (mtc == null || mtc.getChildren() == null || mtc.getChildren().length > 0) {
                tokens.add(mtc);
            }
        }
        if (xsSequence != null) {
            ModelTokenContainer mtc = ModelTokenContainer.create(this.owner, this, xsSequence);
            if (mtc == null || mtc.getChildren() == null || mtc.getChildren().length > 0) {
                tokens.add(mtc);
            }
        }

        if (xsGroup != null) {
            xsAll = xsGroup.getElements() != null ? xsGroup.getChildAsAll() : null;
            xsChoice = xsGroup.getElements() != null ? xsGroup.getChildAsChoice() : null;
            xsSequence = xsGroup.getElements() != null ? xsGroup.getChildAsSequence() : null;

            processTokens(tokens, xsAll, xsChoice, xsSequence, null);
        }
    }

    private void processContent(List<ModelToken> tokens, XsdComplexContent xsComplexContent, XsdSimpleContent xsSimpleContent) {

        if (xsComplexContent != null) {
            XsdExtension xsExtension = xsComplexContent.getXsdExtension();

            if (xsExtension != null) {

                @SuppressWarnings("Duplicates")
                XsdAll xsAll = xsExtension.getElements() != null ? xsExtension.getChildAsAll() : null;
                XsdChoice xsChoice = xsExtension.getElements() != null ? xsExtension.getChildAsChoice() : null;
                XsdSequence xsSequence = xsExtension.getElements() != null ? xsExtension.getChildAsSequence() : null;
                XsdGroup xsGroup = xsExtension.getElements() != null ? xsExtension.getChildAsGroup() : null;

                processComplexType(tokens, xsExtension.getBaseAsComplexType());
                processTokens(tokens, xsAll, xsChoice, xsSequence, xsGroup);
            }
        }

    }
}


package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import org.xmlet.xsdparser.xsdelements.XsdAttribute;
import org.xmlet.xsdparser.xsdelements.XsdElement;

public interface ModelProperty extends ModelToken {

    String getNamespace();
    boolean isAttribute();

}

@SuppressWarnings("unused")
@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
@JsonPropertyOrder({"name", "namespace", "cardinality"})
class ModelSimpleProperty implements ModelProperty {

    private final Model owner;
    private final ModelElement parent;
    private final String name;
    private final int minOccurs;
    private final int maxOccurs;
    private final boolean isMaxUnbounded;
    private final boolean isAttribute;

    ModelSimpleProperty(Model owner, ModelElement parent, XsdElement xsElement) {
        this.owner = owner;
        this.parent = parent;
        this.name = xsElement.getName();
        this.isAttribute = false;
        this.minOccurs = xsElement.getMinOccurs();

        String maxOccurs = xsElement.getMaxOccurs();
        if (null != maxOccurs && !"".equals(maxOccurs) && maxOccurs.matches("^\\d+")) {
            this.maxOccurs = Integer.parseInt(maxOccurs);
            this.isMaxUnbounded = false;
        }
        else {
            this.maxOccurs = Integer.MAX_VALUE;
            this.isMaxUnbounded = true;
        }
    }

    ModelSimpleProperty(Model owner, ModelElement parent, XsdAttribute xsAttribute) {
        this.owner = owner;
        this.parent = parent;
        this.name = xsAttribute.getName();
        this.isAttribute = true;
        this.minOccurs = "required".equals(xsAttribute.getUse()) ? 1 : 0;
        this.maxOccurs = 1;
        this.isMaxUnbounded = false;
    }

    @Override
    @JsonIgnore
    public String getLabel() {
        return this.name;
    }

    public String getName() { return this.name; }

    @Override
    public String getNamespace() {
        return this.parent.getNamespace();
    }

    public String getCardinality() {
        return ModelToken.getCardinality(this);
    }

    @Override
    @JsonIgnore
    public int getMinOccurs() {
        return this.minOccurs;
    }

    @Override
    @JsonIgnore
    public int getMaxOccurs() {
        return this.maxOccurs;
    }

    @Override
    @JsonIgnore
    public boolean isMaxUnbounded() {
        return this.isMaxUnbounded;
    }

    @Override
    @JsonIgnore
    public boolean isAttribute() {
        return this.isAttribute;
    }

    @Override
    @JsonIgnore
    public boolean isContainer() {
        return false;
    }

    @Override
    public String toString() {
        return this.owner.serialize(this);
    }

}

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
@JsonPropertyOrder({"name", "namespace", "cardinality", "tokens", "attributes"})
class ModelChildObject extends ModelElement implements ModelProperty {

    private final int minOccurs;
    private final int maxOccurs;
    private final boolean isMaxUnbounded;

    ModelChildObject(Model owner, ModelElement parent, XsdElement xsElement, String ownNamespace) {

        super(owner, parent, xsElement, ownNamespace);

        this.minOccurs = xsElement.getMinOccurs();

        String maxOccurs = xsElement.getMaxOccurs();
        if (null != maxOccurs && !"".equals(maxOccurs) && maxOccurs.matches("^\\d+")) {
            this.maxOccurs = Integer.parseInt(maxOccurs);
            this.isMaxUnbounded = false;
        }
        else {
            this.maxOccurs = Integer.MAX_VALUE;
            this.isMaxUnbounded = true;
        }
    }

    @Override
    @JsonIgnore
    public String getLabel() {
        return super.getName();
    }

    public String getCardinality() {
        return ModelToken.getCardinality(this);
    }

    @Override
    @JsonIgnore
    public int getMinOccurs() {
        return this.minOccurs;
    }

    @Override
    @JsonIgnore
    public int getMaxOccurs() {
        return this.maxOccurs;
    }

    @Override
    @JsonIgnore
    public boolean isMaxUnbounded() {
        return this.isMaxUnbounded;
    }

    @Override
    @JsonIgnore
    public boolean isAttribute() {
        return false;
    }

    @Override
    @JsonIgnore
    public boolean isContainer() {
        return false;
    }
}


package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import org.xmlet.xsdparser.xsdelements.XsdAttribute;
import org.xmlet.xsdparser.xsdelements.XsdElement;

import java.beans.BeanProperty;

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
    @BeanProperty
    @JsonIgnore
    public String getLabel() {
        return this.name;
    }

    @BeanProperty
    public String getName() { return this.name; }

    @Override
    @BeanProperty
    public String getNamespace() {
        return this.parent.getNamespace();
    }

    @BeanProperty
    public String getCardinality() {
        return ModelToken.getCardinality(this);
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public int getMinOccurs() {
        return this.minOccurs;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public int getMaxOccurs() {
        return this.maxOccurs;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public boolean isMaxUnbounded() {
        return this.isMaxUnbounded;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public boolean isAttribute() {
        return this.isAttribute;
    }

    @Override
    @BeanProperty
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
    @BeanProperty
    @JsonIgnore
    public String getLabel() {
        return super.getName();
    }

    @BeanProperty
    public String getCardinality() {
        return ModelToken.getCardinality(this);
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public int getMinOccurs() {
        return this.minOccurs;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public int getMaxOccurs() {
        return this.maxOccurs;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public boolean isMaxUnbounded() {
        return this.isMaxUnbounded;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public boolean isAttribute() {
        return false;
    }

    @Override
    @BeanProperty
    @JsonIgnore
    public boolean isContainer() {
        return false;
    }
}


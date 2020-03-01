package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;
import org.xmlet.xsdparser.xsdelements.*;
import org.xmlet.xsdparser.xsdelements.elementswrapper.ReferenceBase;

import java.util.ArrayList;
import java.util.List;

public interface ModelTokenContainer extends ModelToken {

    ModelPropertyContainerType getContainerType();
    ModelToken[] getChildren();

    static ModelTokenContainer create(Model owner, ModelElement parent, XsdMultipleElements xsMultipleElements) {

        ModelTokenContainerImpl impl;

        if (xsMultipleElements instanceof XsdAll) {
            impl = new ModelTokenAllImpl(owner, (XsdAll)xsMultipleElements);
        }
        else if (xsMultipleElements instanceof XsdChoice) {
            impl = new ModelTokenChoiceImpl(owner, (XsdChoice)xsMultipleElements);
        }
        else if (xsMultipleElements instanceof XsdSequence) {
            impl = new ModelTokenSequenceImpl(owner, (XsdSequence)xsMultipleElements);
        }
        else {
            parent.getErrorHandler().handle(String.format("Unsupported token container implementor: %s", xsMultipleElements.getClass().getName()));
            return null;
        }

        xsMultipleElements.getElements().stream().map(ReferenceBase::getElement).forEach(e -> {
            if (e instanceof XsdElement) {
                impl.addChild(new ModelChildObject(owner, parent, (XsdElement)e, /*todo*/ null));
            }
            else if (e instanceof XsdMultipleElements) {
                ModelTokenContainer nc = create(owner, parent, (XsdMultipleElements)e);
                if (nc != null) impl.addChild(nc);
            }
            else {
                parent.getErrorHandler().handle(String.format("Unsupported token implementor: %s", e.getClass().getName()));
            }
        });

        return impl;
    }
}

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
@JsonPropertyOrder({"label", "container", "cardinality"})
abstract class ModelTokenContainerImpl implements ModelTokenContainer {

    private final List<ModelToken> children = new ArrayList<>();
    private final Model owner;
    private ModelToken[] childrenArray;

    private int minOccurs;
    private int maxOccurs;
    private boolean isMaxUnbounded;

    ModelTokenContainerImpl(Model owner) {

        this.owner = owner;
    }

    @Override
    public abstract String getLabel();

    @Override
    @JsonIgnore
    public abstract ModelPropertyContainerType getContainerType();

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

    public String getCardinality() {
        return ModelToken.getCardinality(this);
    }

    @Override
    public ModelToken[] getChildren() {
        if (this.childrenArray == null) {
            this.childrenArray = new ModelToken[this.children.size()];
            this.children.toArray(this.childrenArray);
        }

        return this.childrenArray;
    }

    void addChild(ModelToken token) {
        this.children.add(token);
        this.childrenArray = null;
    }

    @Override
    public boolean isContainer() {
        return true;
    }

    @Override
    public String toString() {
        return this.owner.serialize(this);
    }

    void setBounds(int minOccurs, String maxOccurs) {
        this.minOccurs = minOccurs;

        if (null != maxOccurs && !"".equals(maxOccurs) && maxOccurs.matches("^\\d+")) {
            this.maxOccurs = Integer.parseInt(maxOccurs);
            this.isMaxUnbounded = false;
        }
        else {
            this.maxOccurs = Integer.MAX_VALUE;
            this.isMaxUnbounded = true;
        }
    }
}

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
class ModelTokenSequenceImpl extends ModelTokenContainerImpl {

    ModelTokenSequenceImpl(Model owner, XsdSequence xsSequence) {
        super(owner);
        setBounds(xsSequence.getMinOccurs(), xsSequence.getMaxOccurs());
    }

    @Override
    public String getLabel() {
        return "Sequence";
    }

    @Override
    public ModelPropertyContainerType getContainerType() {
        return ModelPropertyContainerType.SEQUENCE;
    }

}

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
class ModelTokenChoiceImpl extends ModelTokenContainerImpl {

    ModelTokenChoiceImpl(Model owner, XsdChoice xsChoice) {
        super(owner);
        setBounds(xsChoice.getMinOccurs(), xsChoice.getMaxOccurs());
    }

    @Override
    public String getLabel() {
        return "Sequence";
    }

    @Override
    public ModelPropertyContainerType getContainerType() {
        return ModelPropertyContainerType.SEQUENCE;
    }

}

@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
class ModelTokenAllImpl extends ModelTokenContainerImpl {

    ModelTokenAllImpl(Model owner, XsdAll xsAll) {
        super(owner);
        setBounds(xsAll.getMinOccurs(), "1");
    }

    @Override
    public String getLabel() {
        return "Sequence";
    }

    @Override
    public ModelPropertyContainerType getContainerType() {
        return ModelPropertyContainerType.SEQUENCE;
    }

}

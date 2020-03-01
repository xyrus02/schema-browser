package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import org.xmlet.xsdparser.xsdelements.XsdElement;
import org.xmlet.xsdparser.xsdelements.XsdSchema;

@SuppressWarnings("WeakerAccess")
@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
public final class ModelObject extends ModelElement {

    ModelObject(Model owner, XsdSchema xsSchema, XsdElement xsElement) {
        super(owner, xsSchema);
        this.loadElement(xsElement);
    }
}

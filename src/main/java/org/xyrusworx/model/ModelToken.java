package org.xyrusworx.model;

public interface ModelToken {

    String getLabel();
    boolean isContainer();

    int getMinOccurs();
    int getMaxOccurs();
    boolean isMaxUnbounded();

    static String getCardinality(ModelToken mt) {
        String min = Integer.toString(mt.getMinOccurs());
        String max = mt.isMaxUnbounded() ? "*" : Integer.toString(mt.getMaxOccurs());

        return mt.getMinOccurs() == mt.getMaxOccurs() ? min : min + ".." + max;
    }

}

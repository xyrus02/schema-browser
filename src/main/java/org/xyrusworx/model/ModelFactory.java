package org.xyrusworx.model;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import org.xmlet.xsdparser.core.XsdParser;
import org.xmlet.xsdparser.core.utils.NamespaceInfo;
import org.xmlet.xsdparser.xsdelements.XsdSchema;
import org.xyrusworx.ModelErrorHandler;

import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@SuppressWarnings({"unused"})
@JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, getterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY, isGetterVisibility = JsonAutoDetect.Visibility.PUBLIC_ONLY)
public class ModelFactory {

    private static final ModelFactory INSTANCE = new ModelFactory();

    public static ModelFactory getInstance() {
        return INSTANCE;
    }

    public Model createModel(String xsdPath) {
        return this.createModel(xsdPath, null);
    }

    public Model createModel(String xmlSchemaFilePath, ModelErrorHandler errorHandler) {

        Path xsFilePath = Paths.get(xmlSchemaFilePath).toAbsolutePath();
        String xsName = xsFilePath.getFileName().toString().replaceFirst("\\.[^.]+$", "");

        XsdParser xsParser = new XsdParser(xsFilePath.toString());
        XsdSchema xsRootSchema = findRootSchema(xsParser.getResultXsdSchemas());

        if (xsRootSchema == null) {
            errorHandler.handle("Unable to locate root schema in schema tree due to a possible circular reference.");
            return null;
        }

        Model resultModel = new Model(xsName, xsRootSchema, errorHandler);
        xsParser.getResultXsdElements().forEach(resultModel::addRootElement);
        return resultModel;
    }

    private XsdSchema findRootSchema(Stream<XsdSchema> xsSchemas) {

        Map<String, NamespaceInfo> nsi = new HashMap<>();
        List<XsdSchema> xsSchemaList = xsSchemas
            .peek(xsSchema -> {
                Map<String, NamespaceInfo> localNsi = xsSchema.getNamespaces();
                localNsi.keySet().forEach(namespace -> nsi.putIfAbsent(namespace, localNsi.get(namespace)));
            })
            .collect(Collectors.toList());

        List<XsdSchema> dependencies = new ArrayList<>();

        for (XsdSchema xsSchema : xsSchemaList) {

            String tns = xsSchema.getTargetNamespace();
            if (null == tns || "".equals(tns)) {
                dependencies.add(xsSchema);
                continue;
            }

            String currentSchemaLocation = "";
            NamespaceInfo tnsi = nsi.get(tns);
            if (null != tnsi) {
                currentSchemaLocation = tnsi.getFile();
                if (currentSchemaLocation == null) {
                    currentSchemaLocation = "";
                }
            }

            for (XsdSchema xsSchemaComp : xsSchemaList) {
                if (xsSchemaComp.getChildrenImports().anyMatch(e -> tns.equals(e.getNamespace()))) {
                    dependencies.add(xsSchema);
                    break;
                }

                final String csl = currentSchemaLocation;
                if (xsSchemaComp.getChildrenIncludes().anyMatch(e -> csl.equals(e.getSchemaLocation()))) {
                    dependencies.add(xsSchema);
                    break;
                }
            }
        }

        xsSchemaList.removeAll(dependencies);
        return xsSchemaList.size() > 0 ? xsSchemaList.get(0) : null;
    }


}


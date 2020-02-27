"use strict";

const fs = require("fs");
const java = require("java");
const baseDir = "./dependency";
const dependencies = fs.readdirSync(baseDir);

dependencies.forEach(function(dependency){
    java.classpath.push(baseDir + "/" + dependency);
});

java.classpath.push("./classes");

exports.getJavaInstance = function() {
    return java;
};

"use strict";

const java = require('java');

java.classpath.push("./classes");
exports.getJavaInstance = function() {
    return java;
};

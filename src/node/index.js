const java = require('./system/java').getJavaInstance();

const test = java.newInstanceSync('org.xyrusworx.Test');

console.log(test.methodSync());
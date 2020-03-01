const java = require('./system/java').getJavaInstance();
const modelFactory = java.newInstanceSync('org.xyrusworx.SchemaModelFactory');

const arg = process.argv[2];

console.log(modelFactory.createModelSync(arg));
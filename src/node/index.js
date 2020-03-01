const java = require('./system/java').getJavaInstance();
const modelFactory = java.newInstanceSync('org.xyrusworx.model.ModelFactory');

const arg = process.argv[2];

let model = modelFactory.createModelSync(arg);
console.log(model.serializeSync());
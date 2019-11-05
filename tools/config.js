const fs = require('fs');
const path = require('path');
var os = require("os");
const dotenv = require('dotenv');

// find project root
const projectPath = path.normalize(__dirname + "/..");

// find and parse .env
const sepesFile = fs.readFileSync(projectPath + "/.env");
const sepesEnv = dotenv.parse(sepesFile);

// find and parse FrontEnd/.env-default
const frontEndFile = fs.readFileSync(projectPath + "/FrontEnd/.env-default");
const frontEndEnv = dotenv.parse(frontEndFile);

// merge create new object.
const env = {
    "REACT_APP_INSTRUMENTATION_KEY": sepesEnv.SEPES_INSTRUMENTATION_KEY,
    "REACT_APP_AUTH_CLIENT_ID": sepesEnv.SEPES_CLIENT_ID,
    "REACT_APP_SEPES_LOGIN_URL" : frontEndEnv.REACT_APP_SEPES_LOGIN_URL,
    "REACT_APP_SEPES_TEST_URL" : frontEndEnv.REACT_APP_SEPES_TEST_URL,
    "REACT_APP_SEPES_BASE_URL" : frontEndEnv.REACT_APP_SEPES_BASE_URL,
}

// create new .env file from object
var list = [];
for (var key in env) {
    if (env.hasOwnProperty(key)) {
        list.push(key + '=' + env[key]);
    }
};
const envFile = list.join(os.EOL);

// write new file back to disk.
const outPath = path.normalize(projectPath + "/FrontEnd/.env");
fs.writeFileSync(outPath , envFile);
console.log("Successfully written new .env to: " + outPath);
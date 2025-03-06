"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Path = require("path");
const MSConfig = require("../guartinel/admin/configuration/MSConfig");
const encryptor = require("../guartinel/security/encryptor");
console.log("-------------------------------DECRYPTOR-------------------------------");
console.log("With this tool you can decrypt encrypted texts based on the encryption keys of the ./config.json");
const DEFAULT_CONFIG_PATH = Path.join("./", 'config.json');
let configuration = new MSConfig.MSConfig(DEFAULT_CONFIG_PATH);
console.log("\n");
console.log("To exit: bye");
const readline = require("readline");
var rl = readline.createInterface(process.stdin, process.stdout);
rl.setPrompt('Enter encrypted text> ');
rl.prompt();
rl.on('line', function (line) {
    if (line === "bye")
        rl.close();
    try {
        console.log("Decrypted text is: " + encryptor.decryptText(line, configuration.security));
    }
    catch (err) {
        console.log("Cannot decrypt text..");
    }
    rl.prompt();
}).on('close', function () {
    console.log("Bye!");
    process.exit(0);
});
//# sourceMappingURL=decryptor.js.map
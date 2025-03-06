import * as Path from "path";
import * as MSConfig from "../guartinel/admin/configuration/MSConfig";
import * as encryptor from "../guartinel/security/encryptor";


console.log("-------------------------------ENCRYPTOR-------------------------------");
console.log("With this tool you can encrypt encrypted texts based on the encryption keys of the ./config.json");
const DEFAULT_CONFIG_PATH = Path.join("./", 'config.json');
let configuration = new MSConfig.MSConfig(DEFAULT_CONFIG_PATH);
console.log("\n");

console.log("To exit: bye");


import * as  readline from 'readline';
var rl = readline.createInterface(process.stdin, process.stdout);
rl.setPrompt('Enter plain text> ');
rl.prompt();
rl.on('line', function (line) {
   if (line === "bye") rl.close();
   try {
      console.log("Encrypted text is: " + encryptor.encryptText(line.toString(), configuration.security.encryptions[0])); // this will use the first encryption
   } catch (err) {
      console.log(JSON.stringify(err));
      console.log("Cannot encrypt text..");
   }
   rl.prompt();
}).on('close', function () {
   console.log("Bye!");
   process.exit(0);
});
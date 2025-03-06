import * as hasher from "../guartinel/security/hasher";

console.log("-------------------------------HASHER-------------------------------");
console.log("With this tool you can hash strings as the ms do with credentials");
console.log("\n");
console.log("To exit: bye");

import * as  readline from 'readline';
var rl = readline.createInterface(process.stdin, process.stdout);
rl.setPrompt('Salt>');

let salt;

rl.prompt();
rl.on('line', function (line) {
   if (line === "bye") rl.close();

   if (salt == null) {
      salt = line;
      rl.setPrompt('Plain>');
      rl.prompt();
      return;
   }

   try {
      let result = hasher.createHash(salt, line);
      console.log("Hashed text is: " + result);
      salt = null;
   } catch (err) {
      console.log("Cannot hash text..");
   }
   rl.setPrompt('Salt>');
   rl.prompt();
}).on('close', function () {
   console.log("Bye!");
   process.exit(0);
});
import * as utility from "../../guartinel/utils/commonUtils.js"
utility.registerGlobals();
var testObjet = {
   password: "tevePata"
};
global.utils.object.sanitizePropertyValuesByKey(["password"], testObjet);
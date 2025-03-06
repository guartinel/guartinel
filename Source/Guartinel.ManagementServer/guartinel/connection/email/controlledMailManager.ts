
var database = global.include('guartinel/database/public/databaseConnector.js');
import { MSError } from "../../../error/Errors";
import Errors = require("../../../error/Errors");
import * as securityTool from "../../security/tool";

import MSInternalServerError = Errors.MSInternalServerError;

let TAG = "ConrolledMailManager: ";


export function isEmailBlackListed(email: string, onFinished: (err: MSError, isBlackListed?: boolean, blackListToken?: string) => void) {

   database.getControlledEmailByProperty('email', email, afterControlledEmailRetrieved);

   function afterControlledEmailRetrieved(err, controlledEmail) {
      if (err) {
         return onFinished(new MSInternalServerError().innerError(err).logMessage(TAG + "Cannot retrieve controlled mail from db: " + email).logNow());
      }

      if (!global.utils.object.isNull(controlledEmail)) {
         return onFinished(null, controlledEmail.isBlackListed, controlledEmail.blackListToken);
      }
      controlledEmail = new database.getControlledEmailModel()({
         email: email,
         blackListToken: securityTool.generateRandomString()
      });
      controlledEmail.save(function (err) {
         if (err) {
            return onFinished(
               new MSInternalServerError()
                  .innerError(err)
                  .logMessage(TAG + "Cannot create new controlled email")
                  .logNow());
         }
        // return onFinished(null);
         return onFinished(null, controlledEmail.isBlackListed, controlledEmail.blackListToken);
      });
      //return onFinished(null, controlledEmail.isBlackListed, controlledEmail.blackListToken);
   }
}


console.log("#################### GUARTINEL MANAGEMENT SERVER #####################");
//#PRIORITY 0
import * as helper from "./helper";
helper.registerGlobals();

import * as Constants from "./common/constants";
import * as MSError from "./error/Errors";
import * as Response from "./guartinel/response/Response";
import * as cluster from "cluster";
Constants.registerGlobals();// TODO remove after the whole project migrated to TS
MSError.registerGlobals();// TODO remove after the whole project migrated to TS
Response.registerGlobals();;// TODO remove after the whole project migrated to TS

//#PRIORITY 1

import * as commonUtils from "./guartinel/utils/commonUtils";
commonUtils.registerGlobals();

import * as appEventHandler from "./appEventHandlers";
appEventHandler.register();
//#PRIORITY 2
import * as configController from "./guartinel/admin/configuration/configurationController";
import * as LogFactory from "./diagnostics/LoggerFactory";
LogFactory.configure(configController.getBaseConfig().logFolder, true);
LogFactory.LOG.info(`ManagementServer is running as  user : ${process.env["USERPROFILE"]}`);

if (process.argv[2] != "--consoleLog") {
   LogFactory.LOG.info("All console logs are silenced. If you want console log start with : --consoleLog ");
   LogFactory.LOG.disableConsoleLog();
}

import * as ExecutionMeterFactory from "./diagnostics/ExecutionMeterFactory";
let isExecutionMeteringEnabled = false;
if (process.argv[3] === "--executionMetering" || configController.getBaseConfig().enableDiagnostics) {
   isExecutionMeteringEnabled = true;
}
ExecutionMeterFactory.configure(isExecutionMeteringEnabled);

if (cluster.isMaster) { // If this instance is master then  init admin db,normal db  and the monitoring functions
   require('./appMaster.js');
} else {
   require('./appSlave.js'); // this instance is a slave so init admin db , normal db and the routes for HTTP requests.
}


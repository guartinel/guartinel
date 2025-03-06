/**
 * Created by DTAP on 2016.08.11..
 */
import * as mongoose from "mongoose";
import { MSError } from "../../../error/Errors"
import { LOG } from "../../../diagnostics/LoggerFactory";

var localConfigurationSchema = global.include('guartinel/database/admin/schemas/localConfigurationSchema.js');
var adminAccountSchema = global.include('guartinel/database/admin/schemas/adminAccountSchema.js');
var watcherServerSchema = global.include('guartinel/database/admin/schemas/watcherServerSchema.js');
import * as adminConfig from "../../admin/configuration/configurationController";

var securityTool = global.include('guartinel/security/tool.js');

import * as async from "async"
import ConnectionState from "../connectionState";
import Errors = require("../../../error/Errors");
import MSInternalServerError = Errors.MSInternalServerError;
let LocalConfigurationModel: mongoose.Model<any>;
let AdminAccountModel: mongoose.Model<any>;
let WatcherServerModel: mongoose.Model<any>;

export function disconnect(callback: () => void) {
   if (connection == null) {
      return callback();
   }
   connection.close(() => callback());
}

let connection: mongoose.Connection;
var isInit = true;


export function getConnectionState(): ConnectionState {
   return new ConnectionState(connection);
};

export function connect(connectFinishedCallback: (error?: MSError) => void): void {
   let adminDatabaseConfig = adminConfig.getAdminDatabaseConfig();
   adminDatabaseConfig.pass = securityTool.decryptText(adminDatabaseConfig.pass);
   // mongoose.Promise = global.Promise; TODO maybe important...
   connection = mongoose.createConnection(adminDatabaseConfig.address, adminDatabaseConfig);

   LocalConfigurationModel = connection.model('localConfiguration', localConfigurationSchema.getSchema(mongoose));
   AdminAccountModel = connection.model('adminAccount', adminAccountSchema.getSchema(mongoose));
   WatcherServerModel = connection.model('watcherServer', watcherServerSchema.getSchema(mongoose));

   connection.on('open', function (err) {
      if (err && isInit) {
         isInit = false;
         return connectFinishedCallback(new MSError("DB_ERROR").innerError(err));
      }

      LOG.info("Admin database connection is open.");

      //if (isDebug()) {
      //  mongoose.set('debug', true);
      // }
      async.series([
         createDefaultAdminIfNeeded,
         createDefaultConfigIfNeeded],
         function (err, results) {
            if (isInit) {
               isInit = false;
               return connectFinishedCallback();
            }
         });
   });

   connection.on('error', function (err) {
      let error = new MSError("Admin database error").logMessage("mongoose.connection.on(error").innerError(err).logNow();
      if (isInit) {
         isInit = false;
         return connectFinishedCallback(error);
      }
   });
}

export function getAdminAccount (callback) {
   AdminAccountModel.find({}, function (err, adminAccounts) {
      return callback(err, adminAccounts[0]);
   });
}

export function getLocalConfiguration  (callback) {
   LocalConfigurationModel.find({}, function (err, localConfig) {
      return callback(err, localConfig[0]); // TODO remove hardwired index!!!
   });
}

export function getWatcherServers  (callback) {
   WatcherServerModel.find({}, function (err, watcherServers) {
      return callback(err, watcherServers);
   });
}

export function getWatcherServerModel () {
   return new WatcherServerModel();
}

function createDefaultAdminIfNeeded(onFinishCallback) {
   AdminAccountModel.find({}, function (err, adminAccounts) {
      if ((adminAccounts != null && (adminAccounts.length > 0))) {
         return onFinishCallback();
      }
   });
}

function createDefaultConfigIfNeeded(onFinishCallback) {
   AdminAccountModel.findOne({}, function (err, account) {
      LocalConfigurationModel.find({},
         function(err, localConfig) {
            if ((localConfig != null) && (localConfig.length > 0)) {
               return onFinishCallback();
            }
            return onFinishCallback(new MSInternalServerError().logMessage("Cannot find local configuration.").severe().logNow());
         });
   });
}



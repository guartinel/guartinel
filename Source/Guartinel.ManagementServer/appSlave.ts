/**
 * Created by DTAP on 2016.08.12..
 */
var adminDatabase = global.include('guartinel/database/admin/databaseConnector.js');
var publicDatabase = global.include('guartinel/database/public/databaseConnector.js');
var licenseController = global.include("guartinel/license/licenseController.js");
import * as async from "async";
import * as routes from "./routes/routes";

var httpRequester = global.include('guartinel/connection/httpRequester.js');
import * as  http from "http";
import  * as https   from "https";
var fs = require('fs');
var config = global.include('guartinel/admin/configuration/configurationController.js');
var transactionDatabase = global.include('guartinel/database/transaction/databaseConnector.js');

var gcm = global.include('guartinel/connection/gcmManager.js');
import * as configController from "./guartinel/admin/configuration/configurationController";
import * as mail from "./guartinel/connection/emailer";
import { LOG } from "./diagnostics/LoggerFactory";
import { MSError } from "./error/Errors";

initSlaveThread();
// TODO REMOVE
if (configController.getBaseConfig().enableDiagnostics) {
 //  LOG.info(`Diagnostics interface is available on: MS_ADDRESS:MSPORT/appmetrics-dash`);
  // var dash = require('appmetrics-dash').attach();
}

function initSlaveThread() {
   LOG.info("Started initialization");
   async.series([
      initDatabase,
      initAlerting,
      transactionDatabase.init,
      routes.setupExpressApp,
      routes.initAdminRoutes,
      startHTTPServer,
      startHTTPSServer,
      setPublicIP,
      licenseController.createDefaultLicenseIfNeeded,
      routes.initRoutes
   ], function (err, result) {
      LOG.info("Finished initialization");
   });
}

function setPublicIP(finishedCallback) {
   httpRequester.getPublicIP(function (err, publicIP) {
      if (!err) {
         LOG.info("IP:" + publicIP);
         config.getLocalConfiguration(function(err, config) {
            config.address.ip = publicIP;
            config.save(function(err) {
               return finishedCallback();
            });
         });
      }
   });
}

function startHTTPServer(finishedCallback) {
   config.getLocalConfiguration(function (err, config) {
      // global.configGlobal = config;
       
       http.createServer(routes.getApp()).listen(config.address.HTTPPort,'0.0.0.0', function (httpErr) {
         if (httpErr) {
            LOG.logMSError(new MSError("CANNOT_START_HTTP_SERVER").innerError(httpErr));
            return finishedCallback();
         }
         LOG.info("HTTP server started at  " + config.address.fullHTTP);
         return finishedCallback();
      });
   });
}

function startHTTPSServer(finishedCallback) {   
   var httpsOptions = {
      key: fs.readFileSync(global.getGuartinelHome() + 'https_cert\\key.pem'),
      cert: fs.readFileSync(global.getGuartinelHome() + 'https_cert\\cert.pem')
   };
   config.getLocalConfiguration(function (err, config) {
      https.createServer(httpsOptions, routes.getApp()).listen(config.address.HTTPSPort,'0.0.0.0', function (httpErr) {
         if (httpErr) {
            LOG.logMSError(new MSError("CANNOT_START_HTTPS_SERVER").innerError(httpErr));
            return finishedCallback();
         }
         LOG.info("HTTPS server started at  " + config.address.fullHTTPS);
         return finishedCallback();
      });
   });
}

function initDatabase(finishedCallback) {
   adminDatabase.connect(function (err) {
      if (err) {
         new MSError("Cannot connect to to database").innerError(err).logNow();
      }
      publicDatabase.connect(function (err) {
         if (err) {
            new MSError("Cannot connect to to database").innerError(err).logNow();
         }
         return finishedCallback();
      });
   });
}


function initAlerting(finishedCallback) {
   configController.getLocalConfiguration(function (err, config) {
       mail.configure(config.emailConfiguration);
       gcm.configure(config.gcm);
      return finishedCallback();
   });
}

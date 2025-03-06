import { Const } from "../../common/constants";
import managementServerUrls = Const.managementServerUrls;
var sessionManager = global.include("guartinel/security/sessionManager.js");
import * as async from "async";
import { ErrorResponse, SuccessResponse } from "../../guartinel/response/Response";
import { MSError, MSInternalServerError } from "../../error/Errors";
import { LOG } from "../../diagnostics/LoggerFactory";
import * as schedule from "node-schedule";

let traceIfNull = global.utils.string.traceIfNull;
let utils = global.utils;

exports.URL = traceIfNull(managementServerUrls.ADMIN_RUN_JOB);
exports.route = function (req, res) {
   var parameters = {
      jobName: req.body[traceIfNull(Const.commonConstants.ALL_PARAMETERS.JOB_NAME)],//We are going to do some maintenance on the Guartinel servers this Friday afternoon (the 27th) between the hours of 14:00 am and 16:00 am.
      token: req.body[traceIfNull(Const.commonConstants.ALL_PARAMETERS.TOKEN)],
      date: req.body[traceIfNull(Const.commonConstants.ALL_PARAMETERS.START_DATE)],
   };

   global.myRequestValidation(parameters, req, res, function (err) {
      if (!err) {
         doSetWebsiteAddress(parameters, function (result) {
            res.send(result);
         });
      }
   });
}

function doSetWebsiteAddress(parameters, callback) {
   sessionManager.validateAdminToken(parameters.token, function (err) {
      if (err) {
         return callback(new ErrorResponse(err));
      }
      var emailer = global.include('guartinel/connection/emailer.js');
      var database = global.include('guartinel/database/public/databaseConnector.js');

      var monitorWatcherServersJob = global.include('guartinel/admin/jobs/monitorWatcherServersJob.js');
      var packageTimeStampCheckJob = global.include('guartinel/admin/jobs/packageTimeStampCheckJob.js');
      var serverAvailabilityCheckJob = global.include('guartinel/admin/jobs/serverAvailabilityCheckJob.js');
      var unassignedPackageCheckJob = global.include('guartinel/admin/jobs/unassignedPackageCheckJob.js');
      var validateLicensesJob = global.include('guartinel/admin/jobs/validateLicensesJob.js');
      var warnAboutLicenseExpirationJob = global.include('guartinel/admin/jobs/warnAboutLicenseExpirationJob.js');
      var watcherServerPerformanceMonitoringJob = global.include('guartinel/admin/jobs/watcherServerPerformanceMonitoringJob.js');
      var databaseJanitorJob = global.include('guartinel/admin/jobs/databaseJanitorJob.js');
      var dailyPackageAlerterJob = global.include('guartinel/admin/jobs/dailyPackageAlerterJob.js');

      var selectedJob;
      if (parameters.jobName == "monitorWatcherServersJob") {
         monitorWatcherServersJob.initDependencies(database);
         selectedJob = monitorWatcherServersJob;
      }

      if (parameters.jobName == "packageTimeStampCheckJob") {
         packageTimeStampCheckJob.initDependencies(database);
         selectedJob = packageTimeStampCheckJob;
      }

      if (parameters.jobName == "serverAvailabilityCheckJob") {
         serverAvailabilityCheckJob.initDependencies(database);
         selectedJob = serverAvailabilityCheckJob;
      }
      if (parameters.jobName == "unassignedPackageCheckJob") {
         unassignedPackageCheckJob.initDependencies(database);
         selectedJob = unassignedPackageCheckJob;
      }
      if (parameters.jobName == "validateLicensesJob") {
          validateLicensesJob.initDependencies(database, emailer);
          selectedJob = validateLicensesJob;
      }
      if (parameters.jobName == "warnAboutLicenseExpirationJob") {
         warnAboutLicenseExpirationJob.initDependencies(database, emailer);
         selectedJob = warnAboutLicenseExpirationJob;
      }
      if (parameters.jobName == "databaseJanitorJob") {
          databaseJanitorJob.initDependencies(database);
          selectedJob = databaseJanitorJob;
      }
      if (parameters.jobName == "dailyPackageAlerterJob") {
        dailyPackageAlerterJob.initDependencies(database, emailer);
        selectedJob = dailyPackageAlerterJob;
      }
      if (utils.object.isNull(selectedJob)) {
         return callback(new ErrorResponse(new MSError("INVALID_JOB_NAME")));
      }

      schedule.scheduleJob(parameters.date, selectedJob.run );

      return callback(new SuccessResponse());
   });
}
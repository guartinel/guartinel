/**
 * Created by DTAP on 2017.08.09..
 */
var schedule = require('node-schedule');

var emailer = include('guartinel/connection/emailer.js');
var database = include('guartinel/database/public/databaseConnector.js');

var monitorWatcherServersJob = include('guartinel/admin/jobs/monitorWatcherServersJob.js');
var packageTimeStampCheckJob = include('guartinel/admin/jobs/packageTimeStampCheckJob.js');
var serverAvailabilityCheckJob = include('guartinel/admin/jobs/serverAvailabilityCheckJob.js');
var unassignedPackageCheckJob = include('guartinel/admin/jobs/unassignedPackageCheckJob.js');
var validateLicensesJob = include('guartinel/admin/jobs/validateLicensesJob.js');
var warnAboutLicenseExpirationJob = include('guartinel/admin/jobs/warnAboutLicenseExpirationJob.js');
var watcherServerPerformanceMonitoringJob = include('guartinel/admin/jobs/watcherServerPerformanceMonitoringJob.js');
var databaseJanitorJob = include('guartinel/admin/jobs/databaseJanitorJob.js');
var dailyPackageAlerterJob = include('guartinel/admin/jobs/dailyPackageAlerterJob.js');

monitorWatcherServersJob.initDependencies(database);
packageTimeStampCheckJob.initDependencies(database);
serverAvailabilityCheckJob.initDependencies(database);
unassignedPackageCheckJob.initDependencies(database);
validateLicensesJob.initDependencies(database, emailer);
warnAboutLicenseExpirationJob.initDependencies(database, emailer);
databaseJanitorJob.initDependencies(database);
dailyPackageAlerterJob.initDependencies(database, emailer);

exports.scheduleJobs = function () {
   LOG.info("scheduleJobs started.")
   schedule.scheduleJob('/30 * * * * *', watcherServerPerformanceMonitoringJob.run);// every 30  sec
   schedule.scheduleJob('/20 * * * * *', unassignedPackageCheckJob.run); //          every 20 sec 
   schedule.scheduleJob('45 * * * * *', packageTimeStampCheckJob.run);//            every minute 45th sec
   schedule.scheduleJob('20 /2 * * * *', serverAvailabilityCheckJob.run);//          every two minute 20th sec
   schedule.scheduleJob('30 /3 * * * *', monitorWatcherServersJob.run);//            every three minute 30th sec
   schedule.scheduleJob('0 30 1 * * *', validateLicensesJob.run); //                 30min past midnight
   schedule.scheduleJob('0 35 1 * * *', warnAboutLicenseExpirationJob.run); //       35min past midnight
   schedule.scheduleJob('0 45 1 * * *', databaseJanitorJob.run); //      run every day 
   schedule.scheduleJob('0 0 8 * * *', dailyPackageAlerterJob.run); //      run every day    
   LOG.info("scheduleJobs finsihed.")
};

exports.runLicenseValidationNow = function (onFinished) {
   validateLicensesJob.run(onFinished);
};

exports.runWarnAboutLicenseExpirationNow = function (onFinished) {
   warnAboutLicenseExpirationJob.run(onFinished);
};

exports.runPackageTimeStampMonitoring = function (server) {
   packageTimeStampCheckJob.run();
}
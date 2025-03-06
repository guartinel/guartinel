/**
 * Created by DTAP on 2016.08.12..
 */

import {LOG} from "./diagnostics/LoggerFactory";
import * as cluster from "cluster";
import * as adminDatabase from "./guartinel/database/admin/databaseConnector"
import * as mail from "./guartinel/connection/emailer";
var gcm = global.include('guartinel/connection/gcmManager.js');
var database = global.include('guartinel/database/public/databaseConnector.js');
import * as async from "async";
LOG.info("Initializing master");
startInit();

function startInit() {
   async.series([
      initAdminDB,
      initAlerting,
      initPublicDB,
      spawnSlaveThreads,
      initJobs
   ], function (err, result) {
      LOG.info("Finished master initialization");
   });
}

function initAdminDB(onFinished) {
    adminDatabase.connect(function (err) {
       if (err) {
          LOG.error("Cannot connect to admin db. Master process will be stopped.");
          return onFinished();
       } else {
          LOG.info("Connected to AdminDatabase");
       }
       return onFinished();
    });
}

function initAlerting(onFinished) {
   adminDatabase.getLocalConfiguration(function (err, config) {
       mail.configure(config.emailConfiguration);
       gcm.configure(config.gcm);
      return onFinished();
   });
}

function initPublicDB(onFinished) {
   database.connect(function (err) {
      if (err) {
         LOG.error("Cannot connect to to database");
      } else {
         
      }
      LOG.info("Connected to PublicDatabase");
      return onFinished();
   });
}


function spawnSlaveThreads(onFinished) {
    var CPU_CORE_COUNT_FOR_WORKERS = require('os').cpus().length - 1; // one core is needed for the master
    if (CPU_CORE_COUNT_FOR_WORKERS < 2) {
        CPU_CORE_COUNT_FOR_WORKERS = 2;
    }
    LOG.info('Starting ' + CPU_CORE_COUNT_FOR_WORKERS + ' slave(s).');
    for (var i = 1; i < CPU_CORE_COUNT_FOR_WORKERS; i++) {
       var worker = cluster.fork();
    }

    cluster.on('online', function (worker) {
        LOG.info('Worker with PID: ' + worker.process.pid + ' is online');
    });

    cluster.on('exit', function (worker, code, signal) {
        LOG.error('Worker ' + worker.process.pid + ' died with code: ' + code + ', and signal: ' + signal + '\nTo keep server running I"am starting a new worker.');
        //abuseDetector.onFatalError();
        cluster.fork(); // TODO  If uncommented than master thread will spawn new slave threads if one die
    });
   return onFinished();
}

function initJobs(onFinished) {
    LOG.info("Scheduling jobs..");
    var jobRunner = global.include('guartinel/admin/jobs/jobRunner.js');
    jobRunner.scheduleJobs();
   return onFinished();
}


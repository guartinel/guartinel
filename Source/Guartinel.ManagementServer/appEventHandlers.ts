/**
 * Created by DTAP on 2016.08.12..
 */
var database = global.include('guartinel/database/public/databaseConnector.js');
import * as cluster from "cluster";
import { LOG } from "./diagnostics/LoggerFactory";

//var monitorController = global.include('guartinel/admin/watcherServerMonitors/monitorController.js');
var adminDatabase = global.include('guartinel/database/admin/databaseConnector.js');
var isExitInProgress = false;

export function register() {
    process.on('SIGINT', handleExit);
    process.on('uncaughtException', handleUncaughtExceptions);
    process.on('unhandledRejection', handleUncaughtExceptions);
    console.log("Application event handlers registered.");
}

export function handleUncaughtExceptions(err) {
    console.log("UNCAUGHT EXCEPTION HANDLING STARTED!");
    LOG.info("Starting unhandled exception handling.");
     LOG.critical(err,"Uncaught exception.",function(errorID){
         handleExit();
     });  
}

function handleExit() {
    if (isExitInProgress) {
        return;
    }
    isExitInProgress = true;
    if (cluster.isMaster) {
        handleExitMasterThread(finalExit);
    } else {
        handleExitSlaveThread(finalExit);
    }
}

function handleExitMasterThread(onFinish) {
    adminDatabase.disconnect(function() {
        LOG.info("Admin Database Disconnected");
        database.disconnect(function() {
            LOG.info("PublicDatabase Disconnected");
            //   monitorController.onProgramStop();
            for (var worker in cluster.workers) {
                cluster.workers[worker].kill();
            }
            onFinish();
        });
    });
}

function handleExitSlaveThread(onFinish) {
    database.disconnect(function(err) {
        LOG.info("PublicDatabase Disconnected");
        if (err) {
            LOG.error("Cannot close database connection properly. Err:" + err);
        }
        onFinish();
    });
}

function finalExit() {
    LOG.info("Thread is exiting now.");
    process.exit(0);
    //include('logger.js').exitAfterLogFlushed();// To prevent race condition when flushing log buffer we need to wait for the final flush before exiting process.
}
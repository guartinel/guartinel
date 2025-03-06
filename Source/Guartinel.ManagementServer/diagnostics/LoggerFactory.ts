import * as winston from "winston";
import * as fs from "fs";
import * as path from "path";
import * as moment from "moment";
import { MSError, MSInternalServerError } from "../error/Errors";
import * as emailer from "../guartinel/connection/emailer";
import * as gcmManager from "../guartinel/connection/gcmManager";
const DailyRotateFile = require('winston-daily-rotate-file');

const logFileName: string = '-ManagementServer.log';

export function configure(logPath: string, isDebugLog: boolean) {
    LOG = new MSLogger(logPath, isDebugLog);
    global.LOG = LOG; // TODO remove after the whole project migrated to TS
}

export let LOG: MSLogger;

class MSLogger {
    logFolder: string;
    isDebugLogEnabld: boolean;
    isPerformanceMonitoringEnabled: boolean;
    winstonLogger;

    constructor(logFolder: string, isDebugLogEnabled: boolean) {
        this.logFolder = logFolder;
        this.isDebugLogEnabld = isDebugLogEnabled;
        let logFileFullPath = path.join(this.logFolder, logFileName);

        console.log("Logger: initialization with folder path: " + logFolder);
        if (!fs.existsSync(logFolder)) {
            console.log("Logger: missing log folder. Creating one.");
            fs.mkdir(logFolder,
                () => {
                    console.log("Logger: cannot create log folder");
                });
        }

        this.winstonLogger = winston.createLogger({
            format: winston.format.combine(
                winston.format.simple()
            ),
            transports: [
                new DailyRotateFile({
                    datePattern: 'YYYY-MM-DD-HH',
                    filename: "%DATE%.log",
                    dirname: logFolder,
                    zippedArchive: false,
                    maxSize: '20m',
                    maxFiles: '150d',
                    json: false,
                    showLevel: false
                })
            ]
        });
        console.log("Logger: initialized.");
    }

    disableConsoleLog() {
        console.log = function() {}
    }

    info(message: string) {
        this.winstonLogger.log('info', global.utils.string.getProcessInfo() + message);
        console.log(message);
    }

    error(message: string) {
        this.winstonLogger.log('error', global.utils.string.getProcessInfo() + message);
        console.log(message);
    }

    debug(message: string): void {
        if (!this.isDebugLogEnabld) {
            return;
        }
        this.winstonLogger.log('info', global.utils.string.getProcessInfo() + message);
        console.log(message);
    }

    logMSError(msError: MSError): MSError {

        console.log(msError.getFullContent());
        this.winstonLogger.log('error', msError.getFullContent());
        return msError;
    }

    critical(err: Error, errorLabel: string, onHandlingDone: any): void {
        let msError = new MSError(errorLabel).logMessage("Critical error cause: ").innerError(err).logNow();

        let criticalErrorHandler = new CriticalErrorHandler(this.logFolder);
        if (criticalErrorHandler.shouldBeSuppressed(msError)) {
            this.info("Critical error is not sent further.");
            return onHandlingDone();
        }
        criticalErrorHandler.add(msError.getInnerError());
        emailer.sendProblemEmailToAdmin("Critical error!", msError.getFullContent(), afterEmailSend);

        function afterEmailSend(err) {
            LOG.info("afterEmailSend trying to get current gcm ids from the devices.");
            if (err) {
                console.log("Big failure. the email to the admin cannot be send: " + JSON.stringify(err));
            }
            gcmManager.sendGcmToAdminDevices(msError.getFullContent(),
                function() {
                    onHandlingDone(msError.getUUID());
                });
        }
    }
}

export class CriticalError {
    timeStamp: string;
    stack: any;

    getTimeStamp(): moment.Moment {
        return moment(this.timeStamp);
    }

    loadFromObject(obj): CriticalError {
        this.stack = obj.stack;
        this.timeStamp = obj.timeStamp;
        return this;
    }

    loadFromError(err: Error): CriticalError {
        this.stack = err.stack;
        this.timeStamp = moment().toISOString();
        return this;
    }
}

class CriticalErrorHandler {
    CRITICAL_EXCEPTIONS_FILE_NAME = "criticalErrors.json";
    //   fs.appendFileSync(path.join(this.logFolder, 'error.log'), message);
    _logFilePath: string;
    _criticalErrors: Array<CriticalError> = new Array<CriticalError>();

    constructor(logFolder: string) {
        this._logFilePath = logFolder + this.CRITICAL_EXCEPTIONS_FILE_NAME;
        this._criticalErrors = this.loadErrors();
    }

    private loadErrors(): Array<CriticalError> {
        let result: Array<CriticalError> = new Array<CriticalError>();
        if (!fs.existsSync(this._logFilePath)) {
            fs.writeFileSync(this._logFilePath, "");
        }
        let errorsJSON = fs.readFileSync(this._logFilePath, 'utf-8').toString();
        let arrayObjects;
        try {
            arrayObjects = JSON.parse(errorsJSON);
        } catch (err) {
            LOG.info("Cannot parse criticalErrors.json");
        }
        if (arrayObjects == null || !(arrayObjects instanceof Array)) {
            return result;
        }
        for (let i = 0; i < arrayObjects.length; i++) {
            let current = arrayObjects[i];
            result.push(new CriticalError().loadFromObject(current));
        }
        return result;
    }

    private saveErrors(): void {
        let errorsJSON = JSON.stringify(this._criticalErrors);
        fs.writeFileSync(this._logFilePath, errorsJSON);
    }

    add(err: any): void {
        let currentError = new CriticalError().loadFromError(err);
        this._criticalErrors.push(currentError);
        this.saveErrors();
    }

    shouldBeSuppressed(err: MSError): boolean {
        let lastOccurence: moment.Moment = null;
        LOG.info("shouldBeSuppressed start");
        if (this._criticalErrors.length === 0) {
            LOG.info("shouldBeSuppressed criticalErrors length = 0");
            return false;
        }
        for (let i = 0; i < this._criticalErrors.length; i++) {
            let currentError = this._criticalErrors[i];

            if (currentError.stack === err.getInnerError().stack) {
                if (lastOccurence == null) {
                    lastOccurence = currentError.getTimeStamp();
                }
                if (currentError.getTimeStamp().isAfter(lastOccurence)) {
                    lastOccurence = currentError.getTimeStamp();
                    continue;
                }
            }
        }
        if (lastOccurence == null) {
            return false;
        }
        const MINUTES_BETWEEN_SEND = 15;
        let diff = moment().diff(lastOccurence, "minute");
        LOG.info("shouldBeSuppressed Critical error diff from last occurrence : " + diff);
        if ( diff < MINUTES_BETWEEN_SEND) {
            return true;
        }
        return false;
    }
}
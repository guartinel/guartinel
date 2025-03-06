import * as path from "path";
import * as fs from "fs";
import { LOG } from "../../../diagnostics/LoggerFactory";
/** EXAMPLE CONFIG
 *
{
	"transactionDatabase": {
		"user": "root",
		"password": " ",
		"host": "localhost",
		"database": "guartinel",
		"port": "3306"
	},
	"adminDatabase": {
		"user": "guartineladminreadwrite",
		"pass": " ",
		"address": "mongodb://localhost:/GuartinelAdmin"
	},
	"security": {
		"encryptions": [{
			"isCurrent": true,
			"prefix": "01",
			"key": ""
		}]
	},
	"logFolder": "D:\\GuartinelLogs\\ManagementServer\\",
	"enableDiagnostics": true,
	"isEmailingEnabled" : false
}
 */


export class MSConfig {
    isEmailingEnabled: boolean = true;
    isGCMEnabled: boolean = true;
    logFolder: string;
    enableDiagnostics: boolean = false;
    adminDatabase: AdminDatabaseConfig;
    transaction: TransactionDatabaseConfig;
    security: SecurityConfig;
    adminDeviceIds: [string];
    minimumAndroidAppVersionCode: number;

    constructor(configPath: string) {
        let readConfig: string = fs.readFileSync(configPath, 'utf-8').toString();
        console.log("Config read. Length: " + readConfig.length);
        let parsedConfig;
        try {
            parsedConfig = JSON.parse(readConfig);
        } catch (err) {
            console.log("Cannot read configuration from: " + configPath);
            throw err;
        }
        if (parsedConfig.logFolder == null) {

            this.logFolder = path.join("./", "logs");
        }
        this.enableDiagnostics = parsedConfig.enableDiagnostics;
        this.logFolder = parsedConfig.logFolder;
        this.minimumAndroidAppVersionCode = parsedConfig.minimumAndroidAppVersionCode;

        if (parsedConfig.isEmailingEnabled != null) {
            this.isEmailingEnabled = parsedConfig.isEmailingEnabled;
        }
        if (parsedConfig.isGCMEnabled != null) {
            this.isGCMEnabled = parsedConfig.isGCMEnabled;
        }
        this.adminDatabase = new AdminDatabaseConfig(parsedConfig);
        this.transaction = new TransactionDatabaseConfig(parsedConfig);
        this.security = new SecurityConfig(parsedConfig);
        this.adminDeviceIds = parsedConfig.adminDeviceIds;
    }
}

export class Encryption {
    prefix: string;
    key: string;
    isCurrent: boolean;
}

export class SecurityConfig {
    encryptions: Array<Encryption>;

    constructor(parsedConfig) {
        if (parsedConfig.security == null) {
            throw new Error("Missing security property from config. ");
        }

        if (parsedConfig.security.encryptions == null) {
            throw new Error("Missing security.encryptions property from config. ");
        }
        this.encryptions = parsedConfig.security.encryptions;
    }

    getCurrentEncryption(): Encryption {
        for (let i = 0; i < this.encryptions.length; i++) {
            if (this.encryptions[i].isCurrent) {
                return this.encryptions[i];
            }
        }
        throw new Error("There is no default encryption in the configuration!");
    }

    getEncryptionByPrefix(prefix: string): Encryption {
        for (let i = 0; i < this.encryptions.length; i++) {
            if (this.encryptions[i].prefix == prefix) {
                return this.encryptions[i];
            }
        }
        LOG.error("There is no encryption in the configuration with prefix: " + prefix);
        return null;
    }

}

export class AdminDatabaseConfig {
    user: string;
    address: string;
    pass: string;

    constructor(parsedConfig) {
        if (parsedConfig.adminDatabase == null) {
            throw new Error("Missing adminDatabase property from config. ");
        }
        this.user = parsedConfig.adminDatabase.user;
        this.address = parsedConfig.adminDatabase.address;
        this.pass = parsedConfig.adminDatabase.pass;
    }
}

export class TransactionDatabaseConfig {
    user: string;
    password: string;
    host: string;
    database: string;
    port: number;

    constructor(parsedConfig: any) {
        if (parsedConfig.transactionDatabase == null) {
            throw new Error("Missing 'transactionDatabase' property from config. ");
        }
        this.user = parsedConfig.transactionDatabase.user;
        this.password = parsedConfig.transactionDatabase.password;
        this.host = parsedConfig.transactionDatabase.host;
        this.database = parsedConfig.transactionDatabase.database;
        this.port = parsedConfig.transactionDatabase.port;
    }
}
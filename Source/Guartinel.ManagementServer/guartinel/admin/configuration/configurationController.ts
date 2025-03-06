import * as path from "path";
import * as  Configs from "./MSConfig";
import * as watch from "node-watch";
import { LOG } from "../../../diagnostics/LoggerFactory";

const DEFAULT_CONFIG_PATH = path.join("./", 'config', 'config.json');
let configuration = new Configs.MSConfig(DEFAULT_CONFIG_PATH);
let adminDatabaseConnector = global.include('guartinel/database/admin/databaseConnector.js');

var watcher = watch(DEFAULT_CONFIG_PATH, { recursive: true });

watcher.on('change', function (evt, name) {
   LOG.info("Config change detected. Reinitialized config object.");
   try {
      configuration = new Configs.MSConfig(DEFAULT_CONFIG_PATH);
   } catch (err){
      LOG.info("Invalid JSON. Try again");
 }
});


export function getLocalConfiguration(callback: any): void {
   return adminDatabaseConnector.getLocalConfiguration(callback);
}

export function getAdminAccount(callback: any): any {
   return adminDatabaseConnector.getAdminAccount(callback);
}

export function getWatcherServers(callback): any {
   return adminDatabaseConnector.getWatcherServers(callback);
}

export function getAdminDatabaseConfig(): Configs.AdminDatabaseConfig {
   return configuration.adminDatabase;
}

export function getTransactionDatabaseConfig(): any {
   return configuration.transaction;
}

export function getBaseConfig() {
   return configuration;
}


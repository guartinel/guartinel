#pragma once
#include "Arduino.h"
#include "FileSystemManager.h"
#include "FS.h"
#include "LOG.h"
#include"GuartinelConfig.h"
#include <ArduinoJson.hpp>
#include <ArduinoJson.h>

FileSystemManager::FileSystemManager() {}

bool FileSystemManager::IsDeviceRegistered() {}

char* TAG_InitFileSystem = "FileSystemManager.InitFileSystem:";
bool FileSystemManager::InitFileSystem() {
	LOG::Info(TAG_InitFileSystem, "Trying to init SPIFFS");
	bool successFullBegin = SPIFFS.begin();
	if (!successFullBegin) {
		LOG::Info(TAG_InitFileSystem, "Cannot begin SPIFFS, so lets format the file system...");
		bool isFormatSuccessFul = SPIFFS.format();
		LOG::Info(TAG_InitFileSystem, "Cannot format file system either. Returning false as a sign of error");
		return false;
	}
	return true;
}

char* TAG_GetConfig = "FileSystemManager.GetConfig:";
CLIConfig FileSystemManager::GetConfig() {
	CLIConfig config;
	config.isSuccessfull = true;
	File  file = SPIFFS.open("/config.txt", "r");
	if (!file) {
		LOG::Info(TAG_GetConfig, "Cannot open config. File is missing.");
		config.isSuccessfull = false;
		return config;
	}
	LOG::Info(TAG_GetConfig, "Reading and parsing config...");
	String configJSONString = file.readString();
	// const char* configPointer = configJSONString.c_str();
	 //LOG::Info(TAG_GetConfig, configJSONString.c_str());
	StaticJsonBuffer<768> configBuffer;
	JsonObject& configuration = configBuffer.parseObject(configJSONString);

	if (!configuration.success()) {
		LOG::Info(TAG_GetConfig, "Cannot parse configuration");
		return config;
	}
	LOG::Info("Configuration:", "");
	String routerSSID = configuration["routerSSID"];
	config.routerSSID = routerSSID;
	LOG::Info("routerSSID", routerSSID.c_str());
	String routerPassword = configuration["routerPassword"];
	config.routerPassword = routerPassword;
	LOG::Info("routerPassword", routerPassword.c_str());


	String instanceName = configuration["instanceName"];
	config.instanceName = instanceName;
	LOG::Info("instanceName", instanceName.c_str());

	String backendServerHost = configuration["backendServerHost"];
	config.backendServerHost = backendServerHost;
	LOG::Info("backendServerHost", backendServerHost.c_str());

	String backendServerPort = configuration["backendServerPort"];
	config.backendServerPort = backendServerPort;
	LOG::Info("backendServerPort", backendServerPort.c_str());

	String backendServerProtocolPrefix = configuration["backendServerProtocolPrefix"];
	config.backendServerProtocolPrefix = backendServerProtocolPrefix;
	LOG::Info("backendServerProtocolPrefix", backendServerProtocolPrefix.c_str());

	String updateServerHost = configuration["updateServerHost"];
	config.updateServerHost = updateServerHost;
	LOG::Info("updateServerHost", updateServerHost.c_str());

	String updateServerPort = configuration["updateServerPort"];
	config.updateServerPort = updateServerPort;
	LOG::Info("updateServerPort", updateServerPort.c_str());

	String updateServerProtocolPrefix = configuration["updateServerProtocolPrefix"];
	config.updateServerProtocolPrefix = updateServerProtocolPrefix;
	LOG::Info("updateServerProtocolPrefix", updateServerProtocolPrefix.c_str());

	String hardwareType = configuration["hardwareType"];
	config.hardwareType = hardwareType;
	LOG::Info("hardwareType", hardwareType.c_str());

	String startupTime = configuration["startupTime"];
	config.startupTime = startupTime;
	LOG::Info("startupTime", startupTime.c_str());

	if (hardwareType.length() == 0 || backendServerHost.length() == 0) {
		config.isSuccessfull = false;
	}
	configBuffer.clear();
	file.close();
	LOG::Info(TAG_GetConfig, "Config is read and parsed successfully");
	return config;
}

char* TAG_SaveConfigg = "FileSystemManager.SaveConfig:";
void FileSystemManager::SaveConfig(CLIConfig config) {
	LOG::Info(TAG_SaveConfigg, "Starting save");

	File  file = SPIFFS.open("/config.txt", "w");
	if (!file) {
		LOG::Info(TAG_SaveConfigg, "Cannot open  config: Access error");
		return;
	}

	LOG::Info("Config", ":");
	LOG::Info("routerSSID", config.routerSSID.c_str());
	LOG::Info("instanceName", config.instanceName.c_str());
	LOG::Info("backendServerHost", config.backendServerHost.c_str());
	LOG::Info("backendServerPort", config.backendServerPort.c_str());
	LOG::Info("backendServerProtocolPrefix", config.backendServerProtocolPrefix.c_str());
	LOG::Info("updateServerProtocolPrefix", config.updateServerProtocolPrefix.c_str());
	LOG::Info("updateServerPort", config.updateServerPort.c_str());
	LOG::Info("updateServerHost", config.updateServerHost.c_str());
	LOG::Info("hardwareType", config.hardwareType.c_str());
	LOG::Info("startupTime", config.startupTime.c_str());

	StaticJsonBuffer<768> jsonBuffer;

	JsonObject& root = jsonBuffer.createObject();
	root["routerSSID"] = config.routerSSID;
	root["routerPassword"] = config.routerPassword;
	root["instanceName"] = config.instanceName;
	root["backendServerHost"] = config.backendServerHost;
	root["backendServerPort"] = config.backendServerPort;
	root["backendServerProtocolPrefix"] = config.backendServerProtocolPrefix;
	root["updateServerHost"] = config.updateServerHost;
	root["updateServerPort"] = config.updateServerPort;
	root["updateServerProtocolPrefix"] = config.updateServerProtocolPrefix;
	root["hardwareType"] = config.hardwareType;
	root["startupTime"] = config.startupTime;

	String configJSONString;
	root.printTo(configJSONString);
	file.print(configJSONString);
	file.close();
	jsonBuffer.clear();
	LOG::Debug(TAG_SaveConfigg, "Config is written and saved.");
}

void FileSystemManager::Reset() {
	SPIFFS.remove("/config.txt");
}

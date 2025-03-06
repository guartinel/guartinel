#include "Arduino.h"
#include "FileSystemManager.h"
#include "GuartinelConfig.h"
#include "WifiWebServer.h"
#include "LOG.h"
#include <ArduinoJson.hpp>
#include <ArduinoJson.h>
#include <ESP8266WebServer.h>
#include "Utils.h"
#include "CheckerConstansts.h"

CLIConfig  _lastReceivedConfig;
int _freezeTimeStamp = UNDEFINED_VALUE;

WifiWebServer::WifiWebServer() {
	_lastReceivedConfig = CLIConfig();
};
ESP8266WebServer* server;

bool WifiWebServer::isSendingFreezed() {
	int freezeTimeSec = _freezeTimeStamp;
	LOG::Info("Freezetime: ", freezeTimeSec);
	if (freezeTimeSec == UNDEFINED_VALUE) {
		return false;
	}
	int currentTimeSec = Utils::GetSystemTimeSec();
	LOG::Info("CurrentTime: ", currentTimeSec);
	int freezeOverTimeSec = freezeTimeSec + (5 * 60); // + 5 MIN
	LOG::Info("Freeze over: ", freezeOverTimeSec);
	if (currentTimeSec < freezeOverTimeSec) {
		return true;
	}
	freezeTimeSec = UNDEFINED_VALUE;
	return false;
}

bool IsPasswordOK() {
	String pass = server->arg("devicePassword");
	String chipID = Utils::GetChipID();

	if (pass.length() == 0) {
		return false;
	}
	if (pass.equals(chipID + chipID)) {
		return true;
	}
	return false;
}

void SendInvalidPasswordResponse() {
	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["result"] = "invalid_password";
	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}

void onHello() {
	//LOG::Debug("onHello:", "Hello received.");
	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["message"] = "Welcome from Guartinel!";
	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}

void onGetConfig() {
	//LOG::Debug("onGetConfig", "Sending config.");
	if (!IsPasswordOK()) {
		SendInvalidPasswordResponse();
		//LOG::Info("onGetConfig", "Invalid devicePassword.");
		return;
	}
	StaticJsonBuffer<150> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	FileSystemManager fileSystemManager;
	CLIConfig config = fileSystemManager.GetConfig();
	if (config.isSuccessfull) {
		root["result"] = "success";
		root["routerSSID"] = config.routerSSID;
		root["instanceName"] = config.instanceName;
	}
	else {
		root["result"] = "missing_config";
	}

	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}
void onGetDiagnostics() {
	//LOG::Debug("onGetDiagnostics", "onGetDiagnostics received.");

	if (!IsPasswordOK()) {
		SendInvalidPasswordResponse();
		return;
	}

	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["result"] = "success";
	String result = //LOG::GetEntries();
		root["log"] = result.c_str();
	root["test"] = result.length();
	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);

}

void onSetConfig() {
	_lastReceivedConfig.routerSSID = server->arg("routerSSID");
	_lastReceivedConfig.routerPassword = server->arg("routerPassword");
	_lastReceivedConfig.instanceName = server->arg("instanceName");
	_lastReceivedConfig.hardwareType = server->arg("hardwareType");
	_lastReceivedConfig.backendServerHost = server->arg("backendServerHost");
	_lastReceivedConfig.backendServerPort = server->arg("backendServerPort");
	_lastReceivedConfig.backendServerProtocolPrefix = server->arg("backendServerProtocolPrefix");
	_lastReceivedConfig.updateServerHost = server->arg("updateServerHost");
	_lastReceivedConfig.updateServerPort = server->arg("updateServerPort");
	_lastReceivedConfig.updateServerProtocolPrefix = server->arg("updateServerProtocolPrefix");

	LOG::Info("onSetConfig", ":");
	LOG::Info("routerSSID", _lastReceivedConfig.routerSSID.c_str());
	LOG::Info("hardwareType", _lastReceivedConfig.hardwareType.c_str());
	LOG::Info("instanceName", _lastReceivedConfig.instanceName.c_str());
	LOG::Info("backendServerHost", _lastReceivedConfig.backendServerHost.c_str());
	LOG::Info("backendServerPort", _lastReceivedConfig.backendServerPort.c_str());
	LOG::Info("backendServerProtocolPrefix", _lastReceivedConfig.backendServerProtocolPrefix.c_str());
	LOG::Info("updateServerHost", _lastReceivedConfig.updateServerHost.c_str());
	LOG::Info("updateServerPort", _lastReceivedConfig.updateServerPort.c_str());
	LOG::Info("updateServerProtocolPrefix", _lastReceivedConfig.updateServerProtocolPrefix.c_str());

	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["success"] = "SUCCESS";

	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
	delay(5000);
	//LOG::Debug("onSetConfig", String("Output is sent : " + output));

	_lastReceivedConfig.isSuccessfull = true;
	FileSystemManager fileSystemManager;
	fileSystemManager.Reset();
	fileSystemManager.SaveConfig(_lastReceivedConfig);
	ESP.restart();
}

void onReset() {
	if (!IsPasswordOK()) {
		SendInvalidPasswordResponse();
		//LOG::Debug("onReset", "Invalid device password.");
		return;
	}
	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["result"] = "success";

	String output;
	root.printTo(output);
	server->send(200, "text/plain", output);
	FileSystemManager fileSystemManager;
	fileSystemManager.Reset();
	ESP.restart();
}
void onHelloGuartinel() {
	LOG::Info("Webserver", "onHelloGuartinel");
	FileSystemManager fileSystemManager;
	CLIConfig config = fileSystemManager.GetConfig();

	StaticJsonBuffer<150> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["hello"] = "guartinel";
	root["instanceName"] = config.instanceName;
	root["id"] = Utils::GetChipID();
	root["success"] = "SUCCESS";

	String output;
	root.printTo(output);
	//LOG::Info("Incoming onHelloGuartinel response: ", output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}

void onLogin() {
	if (!IsPasswordOK()) {
		SendInvalidPasswordResponse();
		//LOG::Debug("onLogin", "Invalid device password.");
		return;
	}
	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["result"] = "success";

	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}

void onFreeze() {
	LOG::Info("Freeze", "Request");
	String token = server->arg("token");
	if (!token.equals("c0ac94e3-88d8-4304")) {
		StaticJsonBuffer<100> jsonBuffer;
		JsonObject& root = jsonBuffer.createObject();
		root["result"] = "invalid_token";
		String output;
		root.printTo(output);
		jsonBuffer.clear();
		server->send(200, "text/plain", output);
		return;
	}
	_freezeTimeStamp = Utils::GetSystemTimeSec();
	StaticJsonBuffer<100> jsonBuffer;
	JsonObject& root = jsonBuffer.createObject();
	root["result"] = "success";

	String output;
	root.printTo(output);
	jsonBuffer.clear();
	server->send(200, "text/plain", output);
}

void WifiWebServer::StartWebServer() {
	LOG::Info("WifiWebServer", "StartWebserver");
	server = new ESP8266WebServer(80);
	server->begin();
	server->on("/hello", onHello);
	server->on("/setConfig", onSetConfig);
	server->on("/login", onLogin);
	server->on("/getConfig", onGetConfig);
	server->on("/reset", onReset);
	server->on("/helloGuartinel", onHelloGuartinel);
	server->on("/getDiagnostics", onGetDiagnostics);
	server->on("/freeze", onFreeze);

	_lastReceivedConfig.isSuccessfull = false;
	//LOG::Info("WifiWebServer", "Start finished");
}

void WifiWebServer::StopWebServer() {
	_lastReceivedConfig = CLIConfig();
	if (server != NULL && server != nullptr) {
		//   LOG::Info("Webserver", "Stopping ");
		   //server->close();
		server->stop();
		delete server;
	}
}
void WifiWebServer::CheckInComingRequests() {
	_lastReceivedConfig.isSuccessfull = false;
	if (server != NULL && server != nullptr) {
		server->handleClient();
	}
}
CLIConfig  WifiWebServer::GetLatestConfig() {
	return _lastReceivedConfig;
}
bool WifiWebServer::HasConfigChanged() {

	if (_lastReceivedConfig.isSuccessfull) {
		return true;
	}
	return false;
}

int WAIT_DELAY_FOR_CLIENT = 100;
char* TAG_WaitForConfig = "WifiManager.WaitForConfig:";
CLIConfig WifiWebServer::WaitForConfig() {
	_lastReceivedConfig.isSuccessfull = false;
	StartWebServer();
	//LOG::Info(TAG_WaitForConfig, "Starting server and waiting for the config");
	while (!_lastReceivedConfig.isSuccessfull) {
		WiFiClient client = server->client();
		if (!client) {
			delay(WAIT_DELAY_FOR_CLIENT);
		}
		server->handleClient();
	}
	server->stop();
	WiFi.disconnect();
	//LOG::Info(TAG_WaitForConfig, "New config is found lets try it out.");
	return _lastReceivedConfig;
}


char* TAG_WaitForConfigWithMaxTime = "WifiManager.WaitForConfigWithMaxTime:";
CLIConfig WifiWebServer::WaitForConfig(int maxWaitSecond) {
	int currentWaitSec = 0;
	_lastReceivedConfig = CLIConfig();
	_lastReceivedConfig.isSuccessfull = false;
	StartWebServer();
	//LOG::Info(TAG_WaitForConfigWithMaxTime, "Starting server and waiting for the config");
	while (!_lastReceivedConfig.isSuccessfull) {
		WiFiClient client =
			server->client();
		if (!client) {
			currentWaitSec += WAIT_DELAY_FOR_CLIENT;
			delay(WAIT_DELAY_FOR_CLIENT);
		}
		server->handleClient();
		if (currentWaitSec >= maxWaitSecond * 1000) {
			//LOG::Info(TAG_WaitForConfigWithMaxTime, "Wait is over max time so return failed config.");
			server->stop();
			WiFi.disconnect();
			return _lastReceivedConfig;
		}
	}
	server->stop();
	WiFi.disconnect();
	//LOG::Info(TAG_WaitForConfig, "New config is retrieved. Lets try it out.");
	return _lastReceivedConfig;
}

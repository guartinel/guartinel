#include "WifiManager.h"
#include "WifiWebServer.h"
#include "GuartinelConfig.h"
#include <WiFiUdp.h>
#include <WiFiServer.h>
#include <WiFiClientSecure.h>
#include <WiFiClient.h>
#include <ESP8266WiFiType.h>
#include <ESP8266WiFiSTA.h>
#include <ESP8266WiFiScan.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266WiFiGeneric.h>
#include <ESP8266WiFiAP.h>
#include <ESP8266WiFi.h>
#include "LOG.h"
#include "Utils.h"
#include <ESP8266mDNS.h>
#include <Hash.h>

String GUID;


WifiManager::WifiManager(String guid) {
	//LOG::Info("GUID", guid);
	GUID = guid;
	_wifiWebServer = new WifiWebServer();
}


char* TAG_StartAP = "WifiManager.StartAP:";

void WifiManager::StartAP(String instanceName) {
	String  GUARTINEL_PREFIX = "Guartinel-";
	WiFi.softAPdisconnect();
	WiFi.disconnect();
	WiFi.mode(WIFI_STA);
	LOG::Info("Instance name: ", instanceName.c_str());

	//LOG::Info(TAG_StartAP,String( "Starting AP")+ instanceName);

	WiFi.persistent(false);
	WiFi.mode(WIFI_AP);
	IPAddress Ip(192, 168, 8, 1);
	IPAddress NMask(255, 255, 255, 0);
	WiFi.softAPConfig(Ip, Ip, NMask);

	if (instanceName.startsWith(GUARTINEL_PREFIX)) {
		LOG::Info("Removing prefix from instanceName", "");
		instanceName.replace(GUARTINEL_PREFIX, String(""));
		LOG::Info("Removed prefix", instanceName.c_str());
	}

	//char ssid[32];
	//char password[32];
	String ssid;
	String password;

	if (instanceName != NULL && instanceName != nullptr && instanceName.length() != 0) {
		ssid = GUARTINEL_PREFIX + ssid + instanceName;
		password = instanceName;
	}
	else {
		ssid = GUARTINEL_PREFIX + GUID;
		password = GUID;
	}
	LOG::Info("Passbefore SHA", password.c_str());
	WiFi.softAP(ssid.c_str(), sha1(password).c_str());

	IPAddress myIP = WiFi.softAPIP();
	char logSSID[32];
	("SSID:" + String(ssid)).toCharArray(logSSID, 32);
	LOG::Info("SSID", ssid.c_str());
	char logPass[32];
	("Pass:" + String(password)).toCharArray(logPass, 32);
	LOG::Info("AP pass: ", sha1(password).c_str());
	LOG::Info("GUID", GUID.c_str());
}


bool WifiManager::ConnectTo(CLIConfig config) {
	char* TAG_ConnectToWIfi = "WifiManager.ConnectToWIfi:";

	//LOG::Info(TAG_ConnectToWIfi, "Trying to connnect to wifi " + String(config.routerSSID));
	WiFi.softAPdisconnect();
	WiFi.disconnect();
	WiFi.mode(WIFI_OFF);
	WiFi.mode(WIFI_STA);
	delay(100);

	String finalHostName = String("Guartinel-") + (GUID);

	//LOG::Info(TAG_ConnectToWIfi, String("Using hostname: " + finalHostName));

	char ssid[32];
	config.routerSSID.toCharArray(ssid, 32);
	char passwd[32];
	config.routerPassword.toCharArray(passwd, 32);
	WiFi.setAutoReconnect(true);
	WiFi.hostname(finalHostName);
	WiFi.begin(ssid, passwd);
	//MDNS.begin();

	int sumDelay = 0;
	while (WiFi.status() != WL_CONNECTED) {
		wl_status_t status = WiFi.status();
		if (status == WL_CONNECT_FAILED) {
			//LOG::Info(TAG_ConnectToWIfi, "Connection failed.Invalid SSID or password.");
			return false;
		}
		delay(500);
		sumDelay += 500;
		if (sumDelay >= 25000) {
			//LOG::Debug("Wifi status:", status);
			//LOG::Info(TAG_ConnectToWIfi, String("Connection failed by timeout. Invalid SSID or password. SSID: ") + ssid);
			sumDelay = 0;
			return false;
		}
	}
	//LOG::Info(TAG_ConnectToWIfi, String("Connected to wifi.SSID ") + ssid + " IP: " + WiFi.localIP().toString());
	return true;
}

bool WifiManager::isConnectedToWifi() {
	if (WiFi.status() != WL_CONNECTED) {
		return false;
	}
	return true;
}

void WifiManager::Stop() {
	_wifiWebServer->StopWebServer();
	WiFi.disconnect();
}
WifiWebServer* WifiManager::WebServer() {
	return  _wifiWebServer;
}

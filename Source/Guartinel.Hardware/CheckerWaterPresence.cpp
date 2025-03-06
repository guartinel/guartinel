#include "CheckerGeneral.h"
#include "Utils.h"
#include "ManagementServer.h"
#include "LOG.h"
#include "ArduinoJson.h"
#include "CheckerConstansts.h"
#include "CheckerWaterPresence.h"

double CheckerWaterPresence::measureAnalog() {
	double sum = 0.0;
	for (int i = 0; i < 5; i++) {
		double read = analogRead(A0);
		sum = sum + read;
		delay(5);
	}
	double avarage = sum / 5.0;
	//double roundedAvarage = round(avarage * 100) / 100;
	LOG::Info("Analaog result", avarage);
	return avarage;
}
CheckerWaterPresence::CheckerWaterPresence() {}

void CheckerWaterPresence::Update() {}

String CheckerWaterPresence::GetMeasurement() {
	LOG::RemoteLog("CheckerGeneral", "Starting check");

	double A0Value =  measureAnalog();
	LOG::Info("A0Value", A0Value);
	int presentValue = 0;
	if (A0Value > 75) {
		presentValue = 1;
	}
	StaticJsonBuffer<156> jsonBuffer; // if to big wificlientsecure will throw an exception
	JsonObject& root = jsonBuffer.createObject();
	root["water_presence"] = presentValue;

	String jsonString;
	root.printTo(jsonString);
	jsonBuffer.clear();
	return jsonString;
}

void CheckerWaterPresence::Init(CLIConfig _config) {
	config = _config;

	for (int i = 0; i < 10; i++) {
		analogRead(A0);
		delay(30);
	}
}



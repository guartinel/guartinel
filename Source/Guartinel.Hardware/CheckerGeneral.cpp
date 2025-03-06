#include "CheckerGeneral.h"
#include "Utils.h"
#include "ManagementServer.h"
#include <EmonLib.h>
#include "LOG.h"
#include "ArduinoJson.h"
#include "CheckerConstansts.h"

double CheckerGeneral::measureAnalog() {
	double sum = 0.0;
	for (int i = 0; i < 5; i++) {
		double read = analogRead(A0);
		sum = sum + read;
		delay(5);
	}
	double avarage = sum / 5.0;
	//double roundedAvarage = round(avarage * 100) / 100;
	return avarage;
}
CheckerGeneral::CheckerGeneral() {}

void CheckerGeneral::Update() {
	double A0Value = 1024 - measureAnalog();
	double D1Value = flipBit(digitalRead(D1));
	double D2Value = flipBit(digitalRead(D2));
	double D3Value = flipBit(digitalRead(D5));

	// //LOG::Info("GeneralChecker",String("AO_VALUE")+ A0Value + " MIN: "+ minA0 + " MAX: " + maxA0) ;
	if (maxA0 == WRONG_READ_VALUE || A0Value > maxA0) {
		maxA0 = A0Value;
	}
	if (minA0 == WRONG_READ_VALUE || A0Value < minA0) {
		minA0 = A0Value;
	}

	if (maxD1 == WRONG_READ_VALUE || D1Value > maxD1) {
		maxD1 = D1Value;
	}
	if (minD1 == WRONG_READ_VALUE || D1Value < minD1) {
		minD1 = D1Value;
	}

	if (maxD2 == WRONG_READ_VALUE || D2Value > maxD2) {
		maxD2 = D2Value;
	}
	if (minD2 == WRONG_READ_VALUE || D2Value < minD2) {
		minD2 = D2Value;
	}

	if (maxD3 == WRONG_READ_VALUE || D3Value > maxD3) {
		maxD3 = D3Value;
	}
	if (minD3 == WRONG_READ_VALUE || D3Value < minD3) {
		minD3 = D3Value;
	}
}
String CheckerGeneral::GetMeasurement() {
	double A0Value = 1024 - measureAnalog();

	LOG::RemoteLog("CheckerGeneral", "Starting check");

	int d1 = flipBit(digitalRead(D1));
	int d2 = flipBit(digitalRead(D2));
	int d3 = flipBit(digitalRead(D5));
	StaticJsonBuffer<256> jsonBuffer; // if to big wificlientsecure will throw an exception
	JsonObject& root = jsonBuffer.createObject();
	root["D1"] = d1;
	root["D2"] = d2;
	root["D3"] = d3;
	root["A0"] = A0Value;
	root["A0_max"] = maxA0;
	root["A0_min"] = minA0;
	root["D1_max"] = maxD1;
	root["D1_min"] = minD1;
	root["D2_max"] = maxD2;
	root["D2_min"] = minD2;
	root["D3_max"] = maxD3;
	root["D3_min"] = minD3;


	String checkStatus = "";
	checkStatus.concat(" Check results=> D1:");
	checkStatus.concat(d1);
	checkStatus.concat(" D2: ");
	checkStatus.concat(d2);
	checkStatus.concat(" D3: ");
	checkStatus.concat(d3);
	checkStatus.concat(" A0: ");
	checkStatus.concat(A0Value);
	checkStatus.concat(" A0Max: ");
	checkStatus.concat(maxA0);
	checkStatus.concat(" A0Min: ");
	checkStatus.concat(minA0);
	checkStatus.concat(" D1Max: ");
	checkStatus.concat(maxD1);
	checkStatus.concat(" D1Min: ");
	checkStatus.concat(minD1);
	checkStatus.concat(" D2Max: ");
	checkStatus.concat(maxD2);
	checkStatus.concat(" D2Min: ");
	checkStatus.concat(minD2);
	checkStatus.concat(" D3Max: ");
	checkStatus.concat(maxD3);
	checkStatus.concat(" D3Min: ");
	checkStatus.concat(minD3);

	LOG::Info("CheckResult:", checkStatus.c_str());

	minA0 = A0Value;
	maxA0 = A0Value;
	minD1 = d1;
	maxD1 = d1;
	minD2 = d2;
	maxD2 = d2;
	minD3 = d3;
	maxD3 = d3;

	String jsonString;
	root.printTo(jsonString);
	jsonBuffer.clear();
	return jsonString;
}

void CheckerGeneral::Init(CLIConfig _config) {
	config = _config;
	pinMode(A0, INPUT);
	pinMode(D1, INPUT);
	pinMode(D2, INPUT);
	pinMode(D5, INPUT);

	for (int i = 0; i < 10; i++) {
		analogRead(A0);
		delay(30);
	}
}



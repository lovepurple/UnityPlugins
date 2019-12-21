/*
 Name:		SkateBoardDriver.ino
 Created:	9/8/2019 11:52:35 PM
 Author:	purple
*/

#include "DriverMonitor.h"
#include "DynamicBuffer.h"
#include "MessageHandler.h"

void setup()
{
	Serial.begin(9600);
	DynamicBuffer.init();
	while (!Serial)
	{

	}
	DriverMonitor.init();

	SpeedMonitor.EnableHallSensorMonitor(true);
}

void loop() {

	DriverMonitor.Tick();

	MessageHandler.Tick();
}

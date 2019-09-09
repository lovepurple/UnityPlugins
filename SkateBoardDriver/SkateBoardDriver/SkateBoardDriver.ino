/*
 Name:		SkateBoardDriver.ino
 Created:	9/8/2019 11:52:35 PM
 Author:	purple
*/

//#include "NeoSWSerial.h"
//
//NeoSWSerial* pBluetooth;
#include "NeoSWSerial.h"

NeoSWSerial* pBluetooth;

void setup()
{
	Serial.begin(9600);

	pBluetooth = new NeoSWSerial(2, 3);
	pBluetooth->begin(9600);
	pBluetooth->listen();
}

char m_tempBuffer[16];
int m_recvCount = 0;

void loop() {

	while (pBluetooth->available() > 0)
	{
		char c = pBluetooth->read();
		if (c != '\n')
			m_tempBuffer[m_recvCount++]= c;
		else
		{
			m_tempBuffer[m_recvCount] = '\0';
			Serial.print(m_tempBuffer);
			Serial.print('\n');
			m_recvCount = 0;
		}
	}

}

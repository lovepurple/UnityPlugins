// 
// 
// 

#include "SystemController.h"

float SystemControllerClass::GetBatteryVolt()
{
	int analogValue = analogRead(BATTERY_SENSOR_PIN);
	return (analogValue / 1023.0f) * 25;					//analogValue获取的是0~1023 对应在5v下的值
}

void SystemControllerClass::init()
{
	m_pSonar = new NewPing(SONAR_TRIG_PIN, SONAR_ECHO_PIN, 500);
	
}


SystemControllerClass SystemController;

char* SystemControllerClass::Handle_GetSystemRemainingPower()
{
	char* pMessageBuffer = DynamicBuffer.GetBuffer();
	pMessageBuffer[0] = (char)E_D2C_REMAINING_POWER;
	float currentVolt = GetBatteryVolt();

	int voltHundred = int(currentVolt * 100);

	itoa(voltHundred, pMessageBuffer + 1, 10);
	pMessageBuffer[5] = '\0';

	return pMessageBuffer;
}

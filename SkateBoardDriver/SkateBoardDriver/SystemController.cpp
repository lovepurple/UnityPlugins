// 
// 
// 

#include "SystemController.h"

float SystemControllerClass::GetBatteryVolt()
{
	int analogValue = analogRead(BATTERY_SENSOR_PIN);
	return (analogValue / 1023.0f) * 25;					//analogValue��ȡ����0~1023 ��Ӧ��5v�µ�ֵ
}

void SystemControllerClass::init()
{
	
}


SystemControllerClass SystemController;

char* SystemControllerClass::Handle_GetSystemRemainingPower()
{
	char* pMessageBuffer = DynamicBuffer.GetBuffer();
	pMessageBuffer[0] = (char)E_D2C_REMAINING_POWER;
	float currentVolt = GetBatteryVolt();

	UtilityClass::DebugLog(String(currentVolt),true);

	int voltHundred = int(currentVolt * 100);

	itoa(voltHundred, pMessageBuffer + 1, 10);
	pMessageBuffer[5] = '\0';

	return pMessageBuffer;
}

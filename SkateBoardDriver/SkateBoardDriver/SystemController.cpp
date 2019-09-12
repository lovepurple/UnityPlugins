// 
// 
// 

#include "SystemController.h"

void SystemControllerClass::init()
{


}


SystemControllerClass SystemController;

char* SystemControllerClass::Handle_GetSystemRemainingPower()
{
	char* pMessageBuffer = DynamicBuffer.GetBuffer();
	pMessageBuffer[0] = (char)E_D2C_REMAINING_POWER;
}

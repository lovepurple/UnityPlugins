// SystemController.h

#ifndef _SYSTEMCONTROLLER_h
#define _SYSTEMCONTROLLER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif
#include "DynamicBuffer.h"
#include "MessageDefine.h"

/**
 *ÏµÍ³¿ØÖÆ
 */
class SystemControllerClass
{
protected:


public:
	void init();

	char* Handle_GetSystemRemainingPower();
	//Handle_GetCurrentSpeedMessage

};

extern SystemControllerClass SystemController;

#endif


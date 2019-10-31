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
#include "GlobalDefine.h"

/**
 *系统控制
 */
class SystemControllerClass
{
protected:


public:
	void init();

	/**
	 * 处理获取剩余电量消息
	 */
	char* Handle_GetSystemRemainingPower();
	//Handle_GetCurrentSpeedMessage

		/**
	 * 获取电池剩余的电压
	 */
	float GetBatteryVolt();

};

extern SystemControllerClass SystemController;

#endif


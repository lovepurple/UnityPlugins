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
#include "NewPing.h"

/**
 *ϵͳ����
 */
class SystemControllerClass
{
protected:
private:
	NewPing* m_pSonar;

public:
	void init();

	/**
	 * �����ȡʣ�������Ϣ
	 */
	char* Handle_GetSystemRemainingPower();
	//Handle_GetCurrentSpeedMessage

		/**
	 * ��ȡ���ʣ��ĵ�ѹ
	 */
	float GetBatteryVolt();

};

extern SystemControllerClass SystemController;

#endif


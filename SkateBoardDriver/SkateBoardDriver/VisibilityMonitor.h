/**
 * 距离检测
 */

#ifndef _VISIBILITYMONITOR_h
#define _VISIBILITYMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "GlobalDefine.h"
#include "NewPing.h"
#include "MotorController.h"

class VisibilityMonitorClass
{
protected:

private:

	NewPing* m_pSonar;

	bool m_isEnableMonitor;

	//超声波delta 时间
	const unsigned int SONAR_DELTA_TIME = 500;

	//紧急刹车距离
	const unsigned int EMERGENCY_STOP_DISTANCE = 20;


public:
	void init();

	void Tick();

	/**
	 * 是否开启距离监控
	 */
	void EnableVisibilityMonitor(bool isEnable);

	/**
	 * 自动减速
	 */
	void AutoSlowDown();

	unsigned int SonarDistance;
};

extern VisibilityMonitorClass VisibilityMonitor;

#endif


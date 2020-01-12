/**
 * ¾àÀë¼ì²â
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

	//³¬Éù²¨delta Ê±¼ä
	const unsigned int SONAR_DELTA_TIME = 500;

	//½ô¼±É²³µ¾àÀë
	const unsigned int EMERGENCY_STOP_DISTANCE = 20;


public:
	void init();

	void Tick();

	/**
	 * ÊÇ·ñ¿ªÆô¾àÀë¼à¿Ø
	 */
	void EnableVisibilityMonitor(bool isEnable);

	/**
	 * ×Ô¶¯¼õËÙ
	 */
	void AutoSlowDown();

	unsigned int SonarDistance;
};

extern VisibilityMonitorClass VisibilityMonitor;

#endif


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

class VisibilityMonitorClass
{
protected:

private:

	NewPing* m_pSonar;

	bool m_isEnableMonitor;

	//³¬Éù²¨delta Ê±¼ä
	const unsigned int SONAR_DELTA_TIME = 50;

public:
	void init();

	void Tick();

	//ÊÇ·ñ
	void EnableVisibilityMonitor(bool isEnable);

	unsigned int SonarDistance;
};

extern VisibilityMonitorClass VisibilityMonitor;

#endif


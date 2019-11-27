// 
// 
// 

#include "VisibilityMonitor.h"


void VisibilityMonitorClass::init()
{
	m_pSonar = &NewPing(SONAR_TRIG_PIN, SONAR_ECHO_PIN, 500);
}
unsigned long ulSonarStartTime;


void VisibilityMonitorClass::Tick()
{
	if (this->m_isEnableMonitor && (millis() - ulSonarStartTime >= SONAR_DELTA_TIME))
	{
		this->SonarDistance = m_pSonar->ping_cm();
		ulSonarStartTime = millis();
	}
}

void VisibilityMonitorClass::EnableVisibilityMonitor(bool isEnable)
{
	this->m_isEnableMonitor = isEnable;
	if (this->m_isEnableMonitor)
		ulSonarStartTime = millis();
}


VisibilityMonitorClass VisibilityMonitor;


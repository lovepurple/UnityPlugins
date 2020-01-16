// 
// 
// 

#include "VisibilityMonitor.h"


void VisibilityMonitorClass::init()
{
	m_pSonar = new  NewPing(SONAR_TRIG_PIN, SONAR_ECHO_PIN, 500);
}
unsigned long ulSonarStartTime;


void VisibilityMonitorClass::Tick()
{
	if (this->m_isEnableMonitor && (millis() - ulSonarStartTime >= SONAR_DELTA_TIME) && SpeedMonitor.GetMotorRoundPerSecond() > 0)
	{
		this->SonarDistance = m_pSonar->ping_cm();
		ulSonarStartTime = millis();

		UtilityClass::DebugLog("Sonar Distance :" + String(this->SonarDistance) + "cm", true);
	}

	if (SpeedMonitor.GetMotorRoundPerSecond() > 0)
		AutoSlowDown();
}

void VisibilityMonitorClass::EnableVisibilityMonitor(bool isEnable)
{
	this->m_isEnableMonitor = isEnable;
	if (this->m_isEnableMonitor)
		ulSonarStartTime = millis();
}

void VisibilityMonitorClass::AutoSlowDown()
{
	if (this->SonarDistance > 0)
	{
		if (this->SonarDistance <= EMERGENCY_STOP_DISTANCE)
			MotorController.BrakeImmediately();

		//todoÆäËü¾àÀë¼õËÙ
		else if (this->SonarDistance <= 50)
			MotorController.Brake();
	}
}


VisibilityMonitorClass VisibilityMonitor;


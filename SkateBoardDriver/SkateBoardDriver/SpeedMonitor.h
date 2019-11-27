/**
 * ���ڻ����������������ת���������ٶ�
 */

#ifndef _SPEEDMONITOR_h
#define _SPEEDMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "GlobalDefine.h"

class SpeedMonitorClass
{
private:
	//���һȦ��������
	const unsigned int MAGNET_COUNT = 2;

	//ÿ�����ź�֮ǰ��Delta��Сֵ
	const unsigned int SIGNAL_DELTA_TIME = 10;

	bool isEnableMonitor = false;

	//ˢ�´�����״̬
	bool IsSensorValidState();

public:
	//ÿ���ź�����
	unsigned	int SignalCountPerSecond;

	//���ÿ��ת��
	int GetMotorRoundPerSecond();

	void Init();

	void Tick();

	void EnableHallSensorMonitor(bool isEnable);

};

extern SpeedMonitorClass SpeedMonitor;

#endif


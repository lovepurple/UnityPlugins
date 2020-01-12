/**
 * 基于霍尔传感器，检测电机转数，计算速度
 */

#ifndef _SPEEDMONITOR_h
#define _SPEEDMONITOR_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "GlobalDefine.h"
#include "DynamicBuffer.h"
#include "MessageDefine.h"

class SpeedMonitorClass
{
private:
	//电机一圈磁铁数量
	const unsigned int MAGNET_COUNT = 2;

	//每两个信号之前的Delta最小值
	const unsigned int SIGNAL_DELTA_TIME = 10;

	bool isEnableMonitor = false;

	//刷新传感器状态
	bool RefreshSensorValidState();

	float m_kilometersPerHour;


public:
	//每秒信号数量
	unsigned int SignalCountPerSecond;

	//电机每秒转数
	int GetMotorRoundPerSecond();

	//当前速度 km/h
	float GetCurrentSpeedKilometerPerHour();

	void Init();

	void Tick();

	void EnableHallSensorMonitor(bool isEnable);

	/**
	 * 获取当前电机的转数
	 */
	char* GetCurrentMotorRPSMesssage();

	/**
	 * 转换km/h到实际电机转数
	 */
	static int ConvertKilometerPerHourToRPS(float kilometerPerHour);

};

extern SpeedMonitorClass SpeedMonitor;

#endif


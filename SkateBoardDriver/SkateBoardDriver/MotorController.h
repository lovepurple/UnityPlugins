// MotorController.h

#ifndef _MOTORCONTROLLER_h
#define _MOTORCONTROLLER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif
#include "GlobalDefine.h"
#include "TimerOne.h"
#include "Utility.h"
#include "MessageDefine.h"
#include "MessageHandler.h"

class MotorControllerClass
{
private:
	float m_currentMotorDuty = 0.0;
	bool m_hasChangedPower = true;

	void InitializePWM();

	//是否正在刹车中
	bool m_isBraking = false;

	//刹车期间内的时间
	float m_brakingNormalizedTimeMill = 0;

	//减速的目标油门, 0表示刹停
	float m_breakingEndNormalizedAccelerator = 0f;

	unsigned long m_lastSlowMill;

	static float m_GearToPWM[5];

	/**
	 * 根据刹车时间，获取归一化的油门大小（使用抛物线模型）
	 */
	float GetNormalizeAcceleratorByDeltaTime(unsigned long deltaTimeMill);

public:
	float m_skateMaxAccelerator;			//滑板运行时最大油门(后续可以由手机端直接设置)
	unsigned int m_maxSpeedBrakeMillTime = 4500;		//最大油门的刹车时间

	void init();

	bool IsPowerOn();

	void PowerOn();

	void PowerOff();

	void Tick();

	//初始化电调
	void InitializeESC();

	//电机正常启动
	void MotorStarup();

	void MotorMinPower();

	/**
	 * 设置最大油门（只有在设置电调时生效）
	 */
	void MotorMaxPower();

	//使用百分比设置速度
	bool SetMotorPower(const float percentage01);

	//获取当前百分比的油门大小
	float GetMotorNormalizedAccelerator();

	/**
	 * 设置油门大小
	 */
	void SetMotorByNormalizedAccelerator(const float normalizedAccelerator);

	//使用占空比设置速度
	void SetSpeedByDuty(float pwmDuty);

	//使用档位设置速度
	void SetSpeedByGear(unsigned int gearID);

	/**
	 * 转换档位数到PWM
	 */
	float ConvertGearToPWMDuty(unsigned int gearID);

	/**
	 * 转换当PWM到档数位
	 */
	int ConvertPWMToGear(float pwmDuty);

	/**
	 * 刹车
	 */
	void Brake();

	/**
	 * 立即刹停
	 */
	void BrakeImmediately();

	/**************消息处理************************/
	char* Handle_GetCurrentSpeedMessage();

	//设置速度处理
	void Handle_SetPercentageSpeedMessage(Message& message);
};

extern MotorControllerClass MotorController;

#endif

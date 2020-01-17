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


	float m_skateMaxAccelerator;					//��������ʱ�������(�����������ֻ���ֱ������)
	unsigned int m_maxSpeedBrakeMillTime;			//��������ٶȵ�ɲ��ʱ��
	unsigned int m_gearCount = GEAR_COUNT;


	void InitializePWM();

	//�Ƿ�����ɲ����
	bool m_isBraking = false;

	//ɲ���ڼ��ڵ�ʱ��
	float m_brakingNormalizedTimeMill = 0;

	//���ٵ�Ŀ������, 0��ʾɲͣ
	float m_breakingEndNormalizedAccelerator = 0.0f;

	unsigned long m_lastSlowMill;

	float m_GearToPWM[GEAR_COUNT];


	/**
	 * ����ɲ��ʱ�䣬��ȡ��һ�������Ŵ�С��ʹ��������ģ�ͣ�
	 */
	float GetNormalizeAcceleratorByDeltaTime(unsigned long deltaTimeMill);

	/**
	 * ���ݵ�ǰ�������ˢ�µ�λ��Ϣ
	 */
	void RefreshSkateGearByCurrentAccelerator();
public:
	void init();

	bool IsPowerOn();

	void PowerOn();

	void PowerOff();

	void Tick();

	//��ʼ�����
	void InitializeESC();

	//�����������
	void MotorStarup();

	void MotorMinPower();

	/**
	 * ����������ţ�ֻ�������õ��ʱ��Ч��
	 */
	void MotorMaxPower();

	//ʹ�ðٷֱ������ٶ�
	bool SetMotorPower(const float percentage01);

	//��ȡ��ǰ�ٷֱȵ����Ŵ�С
	float GetMotorNormalizedAccelerator();

	/**
	 * �������Ŵ�С
	 */
	void SetMotorByNormalizedAccelerator(const float normalizedAccelerator);

	//ʹ��ռ�ձ������ٶ�
	void SetSpeedByDuty(float pwmDuty);

	//ʹ�õ�λ�����ٶ�
	void SetSpeedByGear(unsigned int gearID);

	/**
	 * ת����λ����PWM
	 */
	float ConvertGearToPWMDuty(unsigned int gearID);

	/**
	 * ת����PWM������λ
	 */
	int ConvertPWMToGear(float pwmDuty);

	/**
	 * ɲ��
	 */
	void Brake();

	/**
	 * ����ɲͣ
	 */
	void BrakeImmediately();

	/**************��Ϣ����************************/
	char* Handle_GetCurrentSpeedMessage();

	//�����ٶȴ���
	void Handle_SetPercentageSpeedMessage(Message& message);

	/**
	 * ���û����������
	 */
	void SetSkateMaxAccelerator(Message& message);

	/**
	 * ���û��嵲λ����
	 */
	void SetSkateGearCount(Message& message);

	/**
	 * ���û���������ŵ�ɲ��ʱ��
	 */
	void SetSkateMaxAcceleratorBrakeTime(Message& message);
};

extern MotorControllerClass MotorController;

#endif

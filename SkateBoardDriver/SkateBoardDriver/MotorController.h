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

public:
	void init();

	bool IsPowerOn();

	void PowerOn();

	void PowerOff();

	//��ʼ�����
	void InitializeESC();

	//�����������
	void MotorStarup();

	void MotorMinPower();

	void MotorMaxPower();

	//ʹ�ðٷֱ������ٶ�
	bool SetMotorPower(const float percentage01);

	//��ȡ��ǰ�ٷֱȵ��ٶ�
	float GetMotorPower();

	//ʹ��ռ�ձ������ٶ�
	void SetSpeedByDuty(float pwmDuty);

	//ɲ��
	void Break();

	/**************��Ϣ����************************/
	//todo:֮����(void*)
	//�����ȡ��ǰ�ٶ���Ϣ
	//C++���õķ�ʽ�������ڴ洫�룬���ں����ڷ����µĶ�����
	char* Handle_GetCurrentSpeedMessage();

	//�����ٶȴ���
	void Handle_SetPercentageSpeedMessage(Message& message);
};

extern MotorControllerClass MotorController;

#endif


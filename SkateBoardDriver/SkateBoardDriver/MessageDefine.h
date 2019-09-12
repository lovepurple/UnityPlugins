// MessageDefine.h

#ifndef _MESSAGEDEFINE_h
#define _MESSAGEDEFINE_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif
//C2D  Client To Driver �ͻ���->������

enum EMessageDefine
{
	E_C2D_MOTOR_POWERON = 110,      //�������
	E_C2D_MOTOR_POWEROFF = 111,     //����ػ�
	E_C2D_MOTOR_MAX_POWER = 112,    //У���������
	E_C2D_MOTOR_MIN_POWER = 113,    //У����С����
	E_C2D_MOTOR_DRIVE = 114,        //�������
	E_C2D_MOTOR_INITIALIZE = 115,   //�����ʼ��
	E_C2D_MOTOR_NORMAL_START = 116, //�����������

	E_C2D_MOTOR_GET_SPEED = 117, //��ȡ�����ǰ�ٶ�
	E_D2C_MOTOR_SPEED = 118,     //���ص����ǰ�ٶ�
	
	E_C2D_REMAINING_POWER = 119,	//��ȡʣ�����
	E_D2C_REMAINING_POWER = 120,	//����ʣ�����
};


struct Message
{
	EMessageDefine messageID;
	char* messageBody;
	size_t messageBodySize;
};


#endif


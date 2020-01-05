#include "MotorController.h"

void MotorControllerClass::InitializePWM()
{
	Timer1.initialize(1000000 / ECS_FREQUENCY);		//在MotorController构造里init 不执行，不确定是否是有延迟或其它位置有更改
}

void MotorControllerClass::init()
{
	pinMode(MOTOR_POWER_PIN, OUTPUT);
	pinMode(ESC_POWER_PIN, OUTPUT);
	PowerOff();

}

bool MotorControllerClass::IsPowerOn()
{
	return digitalRead(MOTOR_POWER_PIN) == MOTOR_POWER_DRIVE_MODE && digitalRead(ESC_POWER_PIN) == ESC_POWN_DRIVE_MODE;
}

void MotorControllerClass::PowerOn()
{
	digitalWrite(MOTOR_POWER_PIN, MOTOR_POWER_DRIVE_MODE);
	digitalWrite(ESC_POWER_PIN, ESC_POWN_DRIVE_MODE);
}

void MotorControllerClass::PowerOff()
{
	digitalWrite(MOTOR_POWER_PIN, !MOTOR_POWER_DRIVE_MODE);
	digitalWrite(ESC_POWER_PIN, !ESC_POWN_DRIVE_MODE);
	MotorMinPower();
	Timer1.stop();

	SpeedMonitor.EnableHallSensorMonitor(false);
}

void MotorControllerClass::InitializeESC()
{
	/*if (!IsPowerOn())
	{*/
	InitializePWM();
	MotorMaxPower();
	delay(500);
	PowerOn();
	//}
}


void MotorControllerClass::MotorStarup()
{
	/*if (!IsPowerOn())
	{*/
	InitializePWM();
	MotorMinPower();
	delay(500);
	PowerOn();

	SpeedMonitor.EnableHallSensorMonitor(true);
	//}
}

void MotorControllerClass::MotorMinPower()
{
	SetMotorPower(0);
	this->m_hasChangedPower = true;
}

void MotorControllerClass::MotorMaxPower()
{
	if (SpeedMonitor.GetMotorRoundPerSecond() < 1.0f)
	{
		SetMotorPower(1);
		this->m_hasChangedPower = true;
	}
}

bool MotorControllerClass::SetMotorPower(const float percentage01)
{
	float speedClampPercentage = Utility.Clamp01(percentage01);

	float currentPercentageSpeed = GetMotorPower();

	float speedToPWMDuty = Utility.Lerp(MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, speedClampPercentage);

	SetSpeedByDuty(speedToPWMDuty);
	return true;
}

float MotorControllerClass::GetMotorPower()
{
	float dutyPercentage01 = Utility.Remap(this->m_currentMotorDuty, MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, 0.0, 1.0);
	return dutyPercentage01;
}

void MotorControllerClass::SetSpeedByDuty(float pwmDuty)
{
	float duty = pwmDuty;
	if (pwmDuty > MOTOR_MAX_DUTY)
		duty = MOTOR_MAX_DUTY;
	else if (pwmDuty < MOTOR_MIN_DUTY)
		duty = MOTOR_MIN_DUTY;

	this->m_currentMotorDuty = duty;
	Timer1.pwm(ESC_A_PIN, duty * 1023);
}

void MotorControllerClass::SetSpeedByGear(unsigned int gearID)
{
	if (gearID >= GEAR_COUNT)
		gearID = GEAR_COUNT - 1;

	int currentGear = ConvertPWMToGear(this->m_currentMotorDuty);

	//不能跳档
	if (gearID - currentGear > 1)
		return;

	SetSpeedByDuty(m_GearToPWM[gearID]);
}


float MotorControllerClass::ConvertGearToPWMDuty(unsigned int gearID)
{
	if (gearID > GEAR_COUNT)
		return m_GearToPWM[GEAR_COUNT - 1];

	return m_GearToPWM[gearID];
}

int MotorControllerClass::ConvertPWMToGear(float pwmDuty)
{
	for (int i = 0; i < 5; ++i)
	{
		if (pwmDuty <= m_GearToPWM[i])
			return i;
	}

	return GEAR_COUNT;
}

char* MotorControllerClass::Handle_GetCurrentSpeedMessage()
{
	if (m_hasChangedPower)
	{
		int speedThousands = int(GetMotorPower() * 1000 - 1);

		char* pMessageBuffer = DynamicBuffer.GetBuffer();
		pMessageBuffer[0] = E_D2C_MOTOR_SPEED;
		itoa(speedThousands, pMessageBuffer + 1, 10);
		pMessageBuffer[4] = '\0';

		m_hasChangedPower = false;

		return pMessageBuffer;
	}

	return nullptr;

}

void MotorControllerClass::Handle_SetPercentageSpeedMessage(Message& message)
{
	//滤掉错包
	if (message.messageBodySize != 3)
	{
		Serial.print("--------------");
		return;
	}
	char* pSpeedBuffer = message.messageBody;
	int speedThousand = atoi(pSpeedBuffer);
	this->m_hasChangedPower = this->SetMotorPower(speedThousand / 999.0f);
}

float MotorControllerClass::m_GearToPWM[5] = { 0.05f,0.055f,0.06f,0.065f,0.07f };


MotorControllerClass MotorController;


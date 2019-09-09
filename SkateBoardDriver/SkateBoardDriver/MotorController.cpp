#include "MotorController.h"

void MotorControllerClass::init()
{
	pinMode(MOTOR_POWER_PIN, OUTPUT);
}

bool MotorControllerClass::IsPowerOn()
{
	return digitalRead(MOTOR_POWER_PIN) == HIGH;
}

void MotorControllerClass::PowerOn()
{
	digitalWrite(MOTOR_POWER_PIN, MOTOR_POWER_DRIVE_MODE);
}

void MotorControllerClass::PowerOff()
{
	digitalWrite(MOTOR_POWER_PIN, !MOTOR_POWER_DRIVE_MODE);
	Timer1.disablePwm(ESC_A);
}

void MotorControllerClass::InitializeESC()
{
	MotorMaxPower();
	delay(2000);
	PowerOn();
}

void MotorControllerClass::MotorMinPower()
{
	SetMotorPower(0);
}

void MotorControllerClass::MotorMaxPower()
{
	SetMotorPower(1);
}

bool MotorControllerClass::SetMotorPower(const float percentage01)
{
	float speedClampPercentage = Utility.Clamp01(percentage01);

	float currentPercentageSpeed = GetMotorPower();
	//if (abs(currentPercentageSpeed - speedClampPercentage) < 0.01f)
	//{
	//	//Serial.println("Speed Close ");
	//	return false;
	//}
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

	Timer1.pwm(ESC_A, duty * 1023);
}

char* MotorControllerClass::Handle_GetCurrentSpeedMessage()
{
	int speedThousands = int(GetMotorPower() * 999);

	char* pMessageBuffer = DynamicBuffer.GetBuffer();
	pMessageBuffer[0] = E_D2C_MOTOR_SPEED;
	itoa(speedThousands, pMessageBuffer + 1, 10);
	pMessageBuffer[4] = '\0';

	return pMessageBuffer;
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
	bool isSuccess = this->SetMotorPower(speedThousand / 999.0f);

	//return;
	//发送新的油门大小到客户端
	if (isSuccess)
	{
		char* messageBuffer = Handle_GetCurrentSpeedMessage();
		//MessageHandler.SendMessage(messageBuffer);
		
		DynamicBuffer.RecycleBuffer(messageBuffer);
	}
}


MotorControllerClass MotorController;


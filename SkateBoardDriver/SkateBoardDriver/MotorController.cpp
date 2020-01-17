#include "MotorController.h"

void MotorControllerClass::InitializePWM()
{
	Timer1.initialize(1000000 / ECS_FREQUENCY);		//在MotorController构造里init 不执行，不确定是否是有延迟或其它位置有更改
}

float MotorControllerClass::GetNormalizeAcceleratorByDeltaTime(unsigned long deltaTimeMill)
{
	//y = 1 - (1/maxTime*maxTime)*x *x
	float a = (float)deltaTimeMill / BRAKE_TOTAL_TIME_MILL;
	float normalizeAccelerator = 1.0 - a * a;
	return normalizeAccelerator;
}

void MotorControllerClass::RefreshSkateGearByCurrentAccelerator()
{
	for (unsigned int i = 0; i < this->m_gearCount; ++i)
		this->m_GearToPWM[i] = Utility.Lerp(MOTOR_MIN_DUTY, m_skateMaxAccelerator, (float)i / this->m_gearCount);
}

void MotorControllerClass::init()
{
	pinMode(MOTOR_POWER_PIN, OUTPUT);
	pinMode(ESC_POWER_PIN, OUTPUT);
	PowerOff();

	this->m_skateMaxAccelerator = Utility.Lerp(MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, ACCELERATOR_FACTOR);
	this->m_maxSpeedBrakeMillTime = BRAKE_TOTAL_TIME_MILL;

	RefreshSkateGearByCurrentAccelerator();
}

bool MotorControllerClass::IsPowerOn()
{
	return digitalRead(MOTOR_POWER_PIN) == MOTOR_POWER_DRIVE_MODE && digitalRead(ESC_POWER_PIN) == ESC_POWN_DRIVE_MODE;
}

void MotorControllerClass::PowerOn()
{
	digitalWrite(MOTOR_POWER_PIN, MOTOR_POWER_DRIVE_MODE);
	digitalWrite(ESC_POWER_PIN, ESC_POWN_DRIVE_MODE);

	this->m_hasChangedPower = true;
}

void MotorControllerClass::PowerOff()
{
	digitalWrite(MOTOR_POWER_PIN, !MOTOR_POWER_DRIVE_MODE);
	digitalWrite(ESC_POWER_PIN, !ESC_POWN_DRIVE_MODE);
	MotorMinPower();
	Timer1.stop();

	DriverController.IsEnable(false);
}

void MotorControllerClass::Tick()
{
	if (this->m_isBraking)
	{
		float currentNormalizedAccelerator = GetMotorNormalizedAccelerator();

		Utility.DebugLog("current Normalized  Accelerator:", false);
		Utility.DebugLog(String(currentNormalizedAccelerator), true);

		if (currentNormalizedAccelerator <= 0.01f)
			this->m_isBraking = false;

		if (currentNormalizedAccelerator <= BRAKE_IMMEDIATELY_ACCELERATOR)
			BrakeImmediately();
		else
		{
			if (millis() - this->m_lastSlowMill >= BRAKE_INTERVAL_MILLS)
			{
				this->m_brakingNormalizedTimeMill += BRAKE_INTERVAL_MILLS;
				float normalizedAccelerator = GetNormalizeAcceleratorByDeltaTime(this->m_brakingNormalizedTimeMill);

				Utility.DebugLog("Braking,normalized accelerator", false);
				Utility.DebugLog(String(normalizedAccelerator), true);

				this->m_lastSlowMill = millis();
				SetMotorByNormalizedAccelerator(normalizedAccelerator);
			}
		}
	}
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

	DriverController.IsEnable(true);

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

	float speedToPWMDuty = Utility.Lerp(MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, speedClampPercentage);

	SetSpeedByDuty(speedToPWMDuty);
	return true;
}

float MotorControllerClass::GetMotorNormalizedAccelerator()
{
	float normalizedAccelerator = Utility.Remap(this->m_currentMotorDuty, MOTOR_MIN_DUTY, this->m_skateMaxAccelerator, 0.0, 1.0);

	return normalizedAccelerator;
}

void MotorControllerClass::SetMotorByNormalizedAccelerator(const float normalizedAccelerator)
{
	float motorDuty = Utility.Remap(normalizedAccelerator, 0, 1, MOTOR_MIN_DUTY, this->m_skateMaxAccelerator);
	SetSpeedByDuty(motorDuty);
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

void MotorControllerClass::Brake()
{
	if (!this->m_isBraking)		//todo:是否打断一上一个
	{
		Utility.DebugLog("Start Soft Brake:", false);
		this->m_brakingNormalizedTimeMill = sqrt(1.0f - GetMotorNormalizedAccelerator()) * m_maxSpeedBrakeMillTime;

		this->m_breakingEndNormalizedAccelerator = 0.0f;
		this->m_isBraking = true;
		this->m_lastSlowMill = millis();

		Utility.DebugLog(String(this->m_brakingNormalizedTimeMill), true);

	}
}

void MotorControllerClass::BrakeImmediately()
{
	MotorMinPower();
	m_isBraking = false;
}

char* MotorControllerClass::Handle_GetCurrentSpeedMessage()
{
	if (m_hasChangedPower)
	{
		int speedThousands = int(GetMotorNormalizedAccelerator() * 1000 - 1);

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
		return;

	char* pSpeedBuffer = message.messageBody;
	int speedThousand = atoi(pSpeedBuffer);

	Utility.DebugLog("C2D_MOTOR_DRIVE", false);
	Utility.DebugLog(String(speedThousand / 999.0f), true);

	this->m_isBraking = false;

	this->m_hasChangedPower = this->SetMotorPower(speedThousand / 999.0f);
}

//消息格式  油门大小 例如: 255  ,0.25油门
void MotorControllerClass::SetSkateMaxAccelerator(Message& message)
{
	//油门设置是个缩放值 例如25转换到实际电机的PWM 就是 Lerp(PWM_MIN,PWM_MAX,25 /100) 为最大油门
	//因为不经过转换太快了
	//比例不能超过0.3 否则太快
	if (message.messageBodySize != 2)
		return;

	char* pAcceleratorMessage = message.messageBody;
	int iAcceleratorMessage = atoi(pAcceleratorMessage);
	float acceleratorFactor = iAcceleratorMessage * 0.01f;

	Utility.DebugLog("E_C2D_SETTING_SKATE_MAX_ACCLERATOR:", false);
	Utility.DebugLog("MaxAccelerator:" + String(acceleratorFactor), false);

	this->m_skateMaxAccelerator = Utility.Lerp(MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, acceleratorFactor);

	RefreshSkateGearByCurrentAccelerator();
}

//消息格式  挡位个数 例如：5 5个挡位
void MotorControllerClass::SetSkateGearCount(Message& message)
{
	if (message.messageBodySize != 1)
		return;

	char* pGearMessage = message.messageBody;
	int iGearCount = atoi(pGearMessage);

	if (iGearCount > GEAR_COUNT)
		return;

	Utility.DebugLog("E_C2D_SETTING_SKATE_GEAR_COUNT:", false);
	Utility.DebugLog(String(iGearCount), true);

	this->m_gearCount = iGearCount;

	RefreshSkateGearByCurrentAccelerator();
}

//消息格式  刹车时间(4个字节) 例如： 4000  4000毫秒刹停
void MotorControllerClass::SetSkateMaxAcceleratorBrakeTime(Message& message)
{
	if (message.messageBodySize != 4)
		return;

	char* pBrakeTimeMill = message.messageBody;
	int brakeTime = atoi(pBrakeTimeMill);

	Utility.DebugLog("E_C2D_SETTING_SKATE_MAX_ACCLERATOR_BRAKE_TIME:", false);
	Utility.DebugLog(String(brakeTime), true);

	this->m_maxSpeedBrakeMillTime = brakeTime;
}



MotorControllerClass MotorController;


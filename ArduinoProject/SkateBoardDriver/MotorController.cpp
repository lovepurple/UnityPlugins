#include "MotorController.h"

MotorController::MotorController(uint8_t motorPowerPin, uint8_t ecsPinA, uint8_t ecsPinB)
{
    this->m_motorPowerPin = motorPowerPin;
    this->m_ecsPinA = ecsPinA;

    pinMode(this->m_motorPowerPin, OUTPUT);
    PowerOff();

    //初始化Timer 50hz = 20000 微秒     占空比 1/20 最低 2/20 最高
    Timer1.initialize(1000000 / ECS_FREQUENCY);
}

MotorController::~MotorController()
{
}

//Static 方法在cpp中不需要加Static
//一个引脚通过一分二可驱动同时多个信号
void MotorController::PowerOn()
{
    digitalWrite(this->m_motorPowerPin, MOTOR_POWER_DRIVE_MODE);
}

void MotorController::PowerOff()
{
    digitalWrite(this->m_motorPowerPin, !MOTOR_POWER_DRIVE_MODE);
    Timer1.disablePwm(this->m_ecsPinA);
}

void MotorController::InitializeESC()
{
    MotorMaxPower();
    delay(2000);
    PowerOn();
}

void MotorController::MotorMinPower()
{
    SetSpeedByDuty(MOTOR_MIN_DUTY);
}

void MotorController::MotorMaxPower()
{
    SetSpeedByDuty(MOTOR_MAX_DUTY);
}

void SetMotorController(MotorController *motorController);

MotorController *GetMotorController();

void MotorController::SetMotorSpeedPercentage(const float percentage01)
{
    float speedClampPercentage = Utility::Clamp01(percentage01);
    
    float currentPercentageSpeed = GetCurrentSpeedPercentage();
    if(abs(currentPercentageSpeed - speedClampPercentage) < 0.01f)
    {   
        Serial.println("Speed Close ");
        return;
    }
    float speedToPWMDuty = Utility::Lerp(MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, speedClampPercentage);


    SetSpeedByDuty(speedToPWMDuty);
}

float MotorController::GetCurrentSpeedPercentage()
{
    float dutyPercentage01 = Utility::Remap(this->m_currentMotorDuty, MOTOR_MIN_DUTY, MOTOR_MAX_DUTY, 0.0, 1.0);
    return dutyPercentage01;
}

void MotorController::SetSpeedByDuty(float pwmDuty)
{
    float duty = pwmDuty;
    if (pwmDuty > MOTOR_MAX_DUTY)
        duty = MOTOR_MAX_DUTY;
    else if (pwmDuty < MOTOR_MIN_DUTY)
        duty = MOTOR_MIN_DUTY;

    this->m_currentMotorDuty = duty;

    Timer1.pwm(m_ecsPinA, duty * 1023);
}

byte *MotorController::Handle_GetCurrentSpeedMessage(char data[5])
{
    int speedThousands = int(GetCurrentSpeedPercentage() * 999);

    char *pResult;      
    pResult = &data[0];     
    pResult[0] = E_D2C_MOTOR_SPEED;
    itoa(speedThousands, pResult + 1, 10);

    return (byte *)pResult;
}

void MotorController::Handle_SetPercentageSpeedMessage(MessageBody& messageBody)
{
    //滤掉错包
    if(messageBody.messageSize != 3)
        return;

    int speedThousand = atoi(messageBody.pMessageBody);
    Serial.println("current speed");
    Serial.println(speedThousand);
    this->SetMotorSpeedPercentage(speedThousand / 999.0f);
}
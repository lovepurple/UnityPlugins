// Utility.h

#ifndef _UTILITY_h
#define _UTILITY_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

class UtilityClass
{
protected:


public:
	//Clamp 到01
	float Clamp01(const float val);

	float Clamp(float val, float minVal, float maxVal);

	//线性插值
	float Lerp(float minVal, float maxVal, float lerpFactor01);

	//区间映射
	float Remap(float val, float inputMin, float inputMax, float outMin, float outMax);
};

extern UtilityClass Utility;

#endif


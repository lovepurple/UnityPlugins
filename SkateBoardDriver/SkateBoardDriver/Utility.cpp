// 
// 
// 

#include "Utility.h"

float UtilityClass::Clamp01(const float val)
{
	return Utility.Clamp(val, 0.0, 1.0);
}

float UtilityClass::Clamp(float val, float minVal, float maxVal)
{
	float clampVal = max(val, minVal);
	clampVal = min(clampVal, maxVal);

	return clampVal;
}

float UtilityClass::Lerp(float minVal, float maxVal, float lerpFactor01)
{
	return minVal + (maxVal - minVal) * lerpFactor01;
}

float UtilityClass::Remap(float val, float inputMin, float inputMax, float outMin, float outMax)
{
	float valClamped = Utility.Clamp(val, inputMin, inputMax);

	float ratio = (outMax - outMin) / (inputMax - inputMin);
	return outMin + ratio * (valClamped - inputMin);
}

#ifndef UTILITY_H
#define UTILITY_H
#include <Arduino.h>

class Utility
{
public:
    //Clamp 到01
    static float Clamp01(const float val);

    static float Clamp(float val,float minVal,float maxVal);

    //线性插值
    static float Lerp(float minVal, float maxVal, float lerpFactor01);

    //区间映射
    static float Remap(float val, float inputMin, float inputMax, float outMin, float outMax);
};
#endif
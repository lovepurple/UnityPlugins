#ifndef DYNAMICBUFFER_H
#define DYNAMICBUFFER_H
#include "QList.h"
#define DYNAMIC_BUFFER_SIZE 32
#define ONE_BUFFER_SIZE 16

class DynamicBuffer
{
private:
    static char Buffer[DYNAMIC_BUFFER_SIZE];

    static QList<char*> BufferQueue;    

    static bool IsInitialzed;

    static void InitialDynamicBuffer();

public:

    static char* GetBuffer();

    static void RecycleBuffer(char* buffer);
};



#endif
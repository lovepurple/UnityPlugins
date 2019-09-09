// DynamicBuffer.h

#ifndef _DYNAMICBUFFER_h
#define _DYNAMICBUFFER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

#include "QList.h"
#define DYNAMIC_BUFFER_SIZE 512
#define ONE_BUFFER_SIZE 16

class DynamicBufferClass
{
private:
	QList<char*> m_bufferQueue = QList<char*>();
	char m_buffer[DYNAMIC_BUFFER_SIZE];


public:
	void init();

	char* GetBuffer();

	void RecycleBuffer(char* recycleBuffer);

	size_t GetBufferPoolCount();

};

extern DynamicBufferClass DynamicBuffer;

#endif


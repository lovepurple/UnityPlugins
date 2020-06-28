// 
// 
// 

#include "DynamicBuffer.h"

void DynamicBufferClass::init()
{
	for (int i = 0; i < DYNAMIC_BUFFER_SIZE / ONE_BUFFER_SIZE; ++i)
	{
		char* chunkedBufferList = m_buffer + (i * ONE_BUFFER_SIZE);
		m_bufferQueue.push_back(chunkedBufferList);
	}
}

char* DynamicBufferClass::GetBuffer()
{
	char* pBuffer = nullptr;
	if (m_bufferQueue.size() > 0)
	{
		pBuffer = m_bufferQueue.front();
		m_bufferQueue.pop_front();
	}
	return pBuffer;			//如果是个空Buffer 直接崩溃
}

void DynamicBufferClass::RecycleBuffer(char* recycleBuffer)
{
	m_bufferQueue.push_back(recycleBuffer);
}

size_t DynamicBufferClass::GetBufferPoolCount()
{
	return m_bufferQueue.size();
}


DynamicBufferClass DynamicBuffer;


#include "DynamicBuffer.h"

void DynamicBuffer::InitialDynamicBuffer()
{
    QList<char*> l = DynamicBuffer::BufferQueue;
    for (int i = 0; i < DYNAMIC_BUFFER_SIZE / ONE_BUFFER_SIZE; ++i)
    {
        char *queueBuffer = DynamicBuffer::Buffer + (i * ONE_BUFFER_SIZE);
        DynamicBuffer::BufferQueue.push_front(queueBuffer);
    }
}

char *DynamicBuffer::GetBuffer()
{
    if(!DynamicBuffer::IsInitialzed)
    {
        InitialDynamicBuffer();
        DynamicBuffer::IsInitialzed = true;
    }

    char* pBuffer = nullptr;
    if (DynamicBuffer::BufferQueue.size() > 0)
    {
        pBuffer = DynamicBuffer::BufferQueue.front();
        DynamicBuffer::BufferQueue.pop_front();
    }

    return pBuffer;
}

void DynamicBuffer::RecycleBuffer(char *buffer)
{
    DynamicBuffer::BufferQueue.push_back(buffer);
}

//静态成员的初始化
char DynamicBuffer::Buffer[]={};            

QList<char*> DynamicBuffer::BufferQueue=QList<char*>();

bool DynamicBuffer::IsInitialzed = false;
/********************************************************************
	created:  2019-09-02 16:28:45
	filename: DigitUtility.cs
	author:	  songguangze@outlook.com
	
	purpose:  数字转换帮助类
*********************************************************************/
using UnityEngine;
using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;

namespace EngineCore.Utility
{
    public static class DigitUtility
    {
        /// <summary>
        /// 获取Char数组所对应的int
        /// </summary>
        /// <param name="digitCharArray"></param>
        /// <returns></returns>
        public static uint GetUInt32(char[] digitCharArray)
        {
            uint result = 0;
            for (int i = 0; i < digitCharArray.Length; ++i)
                result += (uint)Char.GetNumericValue(digitCharArray[i]) * Convert.ToUInt32(Math.Pow(10, digitCharArray.Length - i - 1));

            return result;
        }

        /// <summary>
        /// 获取固定长度的Byte List
        /// </summary>
        /// <param name="inputBufferList">输入BufferList</param>
        /// <param name="fixedLenght">固定长度</param>
        /// <param name="placeHolder">占位符</param>
        /// <returns></returns>
        public static List<byte> GetFixedLengthBufferList(List<byte> inputBufferList, int fixedLenght, byte placeHolder)
        {
            if (inputBufferList.Count > fixedLenght)
                return inputBufferList;

            List<byte> fixedBufferList = new List<byte>();

            for (int i = 0; i < fixedLenght; ++i)
            {
                int index = i - fixedLenght + inputBufferList.Count;

                fixedBufferList.Add(index < 0 ? placeHolder : inputBufferList[index]);
            }

            return fixedBufferList;
        }

    }
}
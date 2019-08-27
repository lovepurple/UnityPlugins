/********************************************************************
	created:  2019-08-27 17:05:24
	filename: AndroidNativeUtility.cs
	author:	  songguangze@outlook.com
	
	purpose:  Android 原生方法帮助类
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AndroidNativeUtility
{
    /// <summary>
    /// 获取Android Plugin中的类
    /// </summary>
    /// <param name="classFullName">类的全名  com.companyname.productname.xxx....</param>
    /// <returns></returns>
    public static AndroidJavaClass GetAndroidNativeClass(string classFullName)
    {
        AndroidJavaClass androidJavaClass = new AndroidJavaClass(classFullName);

        return IsAndroidJavaClassNull(androidJavaClass) ? null : androidJavaClass;
    }

    /// <summary>
    /// Android类是否为空
    /// </summary>
    /// <param name="androidJavaClass"></param>
    /// <returns></returns>
    public static bool IsAndroidJavaClassNull(AndroidJavaClass androidJavaClass)
    {
        return androidJavaClass == null || androidJavaClass.GetRawClass().ToInt32() == 0;
    }

}

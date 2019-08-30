using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static T[] GetJsonList<T>(string jsonStr)
    {
        string wrapJson = "{ \"array\": " + jsonStr + "}";

        return JsonUtility.FromJson<Wrapper<T>>(wrapJson).array;
    }

    public static string GetJsonArrayStr<T>(T[] jsonArray)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = jsonArray;

        return JsonUtility.ToJson(wrapper);
    }

    private class Wrapper<T>
    {
        public T[] array;
    }
}

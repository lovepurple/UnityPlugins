package com.lovepurple.bluetoothcommom;

import java.util.ArrayList;
import java.util.List;

public final class ArrayUtility {

    public static List<byte[]> split(byte[] array, byte splitter) {
        List<byte[]> list = new ArrayList<>();
        List<Byte> subList = new ArrayList<>();
        for (byte element : array) {
            if (element == splitter && !subList.isEmpty()) {
                list.add(convertToPrimitiveArray(subList));
                subList = new ArrayList<>();
            } else if (element != splitter) {
                subList.add(element);
            }
        }
        if (!subList.isEmpty()) {
            list.add(convertToPrimitiveArray(subList));
        }
        return list;
    }

    private static byte[] convertToPrimitiveArray(List<Byte> list) {
        byte[] array = new byte[list.size()];
        for (int i = 0; i < list.size(); i++) {
            array[i] = list.get(i);
        }
        return array;
    }
}

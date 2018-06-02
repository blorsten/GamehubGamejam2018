using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomExtentions
{
    public static Boolean TryGetRoomProperty<T>(this Room room, String key, out T value)
    {
        System.Object v;

        if (room.CustomProperties.TryGetValue(key, out v))
        {
            if (v is T)
            {
                value = (T)v;
                return true;
            }
        }

        value = default(T);
        return false;
    }
}

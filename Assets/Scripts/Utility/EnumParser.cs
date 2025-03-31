using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumParser
{
    public static T Parse<T>(string value) where T : struct, Enum
    {
        if (Enum.TryParse(value, out T result))
        {
            return result;
        }

        return default;
    }
}

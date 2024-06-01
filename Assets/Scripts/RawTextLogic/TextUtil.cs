using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TextUtil
{
    public static string ToSingleLine(string text)
    {
        return Regex.Replace(text, @"[\s\n]+", " ").Trim(' ').Trim('\n');
    }
}

using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class StringToColor
    {
        const float MIXED_WEIGHT = 0.75f;
        const float TEXT_WEIGHT = 0.25f;
        const int SEED = 16777215;
        const int FACTOR = 49979693;

        private const bool MIXED = false;

        // public static Color[] GetColors(string txt) {
        //     var words = _words(text);
        //     var colors = [];
        //     words.forEach(function(word) {
        //         var color = toHex(word);
        //         if (color) colors.push(hexRgb(trimStart(color, '#'), {format: 'array'}));
        //     });
        //     return colors;
        // }

        public static Color MixColors(Color[] colors)
        {
            var len = colors.Length;
            var targetColor = colors.Aggregate( (c0, c1) =>  new Color(c0.r + c1.r, c0.g + c1.g + c0.b + c1.b, c0.a + c1.a) );
            targetColor = new Color(targetColor.r / len, targetColor.g / len, targetColor.b / len);
            return targetColor;
        }

        public static string GenerateColorString(string txt) {
            // var mixed = Color.black;
            // var colors = GetColors(txt);
            // if (colors.Length > 0) mixed = MixColors(colors);
            var b = 1;
            var d = 0;
            var f = 1;
            if (txt.Length > 0) {
                for (int i = 0; i < txt.Length; i++)
                {
                    if (txt[i] > d)
                    {
                        d = txt[i];
                        f = SEED / d;
                    }
                }
                b = (b + txt[0] * f * FACTOR) % SEED;
            }
            var hex = ((b * txt.Length) % SEED).ToString("X");
            hex = $"#{hex.Substring(0, 6)}";
            return hex;
        }
        
        public static Color GenerateColor(string txt)
        {
            var hex = GenerateColorString(txt);
            if (ColorUtility.TryParseHtmlString(hex, out var rgb))
            {
                return rgb;
            }
            return Color.black;
        }

    }
}
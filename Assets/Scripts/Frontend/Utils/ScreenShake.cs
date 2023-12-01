using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenShake 
{
    private static Dictionary<Transform, Tween> _runningShakes = new Dictionary<Transform, Tween>();

    public static void ShakeTransform(Transform tf, float violence = 60 )
    {
        if(_runningShakes.TryGetValue(tf, out var tween))
        {
            tween.Kill();
        }
        _runningShakes[tf] = tf.DOShakePosition(0.15f, violence, vibrato: 20);
    }

    public static void Shake(this Transform tf, float violence = 60) => ShakeTransform(tf, violence: violence);
}

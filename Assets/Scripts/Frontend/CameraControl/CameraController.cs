using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Frontend.CameraControl
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public Transform RaySource;

        public Transform RayModifierPivot;

        public Transform RayDirection;

        public float CameraDistance;


        public float Violence = 6;

        public float Time = 0.25f;

        public int Vibrato = 8;

        private static Dictionary<Transform, Tween> _runningShakes = new Dictionary<Transform, Tween>();


        public void Shake()
        {
            //var dir =  RaySource.position - RayDirection.position;
            //var modDir =  RaySource.position - RayDirectionModified.position;

            //var rng = UnityEngine.Random.insideUnitCircle * violence;


            //var step = 1 / Fanciness;
            //Vector3 circleOffset = Vector3.zero;
            //var tempDir = dir;
            //for (int i = 0; i < Fanciness; i++)
            //{
            //    var upDir = Quaternion.Euler(tempDir) * Vector3.up;
            //    var rightDir = Vector3.Cross(tempDir, upDir);

            //    circleOffset += ((upDir * rng.y) + (rightDir * rng.x) * step);
            //    tempDir = (RaySource.position - RayDirection.position) + circleOffset;
            //}
            
            if(_runningShakes.TryGetValue(RayModifierPivot, out var tween))
            {
                tween.Kill();
            }
            _runningShakes[RayModifierPivot] = RayModifierPivot.DOShakeRotation(Time, Violence, vibrato: 20, randomnessMode: ShakeRandomnessMode.Harmonic);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.K)) 
            {
                Shake();
            }
            Position();
        }

        public void Position()
        {
            Ray r = new Ray(RaySource.position,  RayDirection.position - RaySource.position);

            transform.position =  r.GetPoint(CameraDistance);
            transform.LookAt(RaySource);
        }
        
    }
}

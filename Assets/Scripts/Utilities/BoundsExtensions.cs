using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class BoundsExtensions
    {
        public static Bounds GetCompleteBounds(this MonoBehaviour mb)
        {
            Bounds b = new Bounds();
            var rends = mb.GetComponentsInChildren<Renderer>();
            if (rends.Length > 0)
            {
                b = rends[0].bounds;
            }
            foreach (var r in rends)
            {
                b.Encapsulate(r.bounds);
            }

            return b;
        }
        
        public static List<Renderer> GetAllRenderers(this MonoBehaviour mb)
        {
            return mb.transform.GetComponentsInChildren<Renderer>().ToList();
        }
    }
}
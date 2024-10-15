using System;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeTheVoid.Utils
{
    public static class MathUtils
    {
        public static Vector3[] CreateParabolicCurve(Vector3 p0, Vector3 p1, int steps = 20)
        {
            // algorithm created and written by myself
            // creates a parabolic curve between two points with a given precision
            // the more steps, the smoother the curve is but too many steps may cause an unwanted overhead when used by DOTween
            
            float diffX = p1.x - p0.x;
            float diffY = p1.y - p0.y;
            float diffZ = p1.z - p0.z;
            
            var path = new Vector3[steps];
            path[0] = p0;
            path[path.Length - 1] = p1;
            
            steps--;
            
            float incrementX = diffX / steps;
            float incrementZ = diffZ / steps;
            float a = diffY / (diffX * diffX);
            
            for (int i = 1; i < steps; i++)
            {
                float x = p0.x + incrementX * i;
                float y = a * (x - p0.x) * (x - p0.x) + p0.y;
                float z = p0.z + incrementZ * i;
                
                path[i] = new Vector3(x, y, z);
            }
            
            return path;
        }

        public static Vector3[] CreateBallisticCurve(Vector3 p0, Vector3 p1, float heightMultiplier = 2f, int steps = 20)
        {
            // A ballistic trajectory is basically two parabolas originating from the highest point in space
            // So this method does just that
            
            var midPoint = Vector3.Lerp(p0, p1, 0.5f);
            midPoint.y += Mathf.Max(p1.y - p0.y, p0.y - p1.y) * heightMultiplier;
            
            var path = new List<Vector3>();
            path.AddRange(CreateParabolicCurve(midPoint, p0, steps));
            path.Reverse();
            path.AddRange(CreateParabolicCurve(midPoint, p1, steps));
            
            return path.ToArray();
        }
    }
}

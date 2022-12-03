using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
 
        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
 
        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public static float RadiusLength(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        
        Vector3 intersection = new Vector3();

        if(LineLineIntersection(out intersection, linePoint1, lineVec1, linePoint2, lineVec2))
        {
            return Vector3.Distance(intersection, linePoint1);
        }
        else 
        {
            if(LineLineIntersection(out intersection, linePoint2, lineVec2, linePoint1, lineVec1))
            {
                return Vector3.Distance(intersection, linePoint2);
            }
        }
        
        return Mathf.Infinity;

    }
}

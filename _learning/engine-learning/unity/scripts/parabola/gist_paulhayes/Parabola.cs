using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola {
    Vector3 start;
    Vector3 end;
    Vector3 startVel;
    Vector3 acceleration;

    public float Plot(Vector3 startPos, Vector3 endPos, float speed, Vector3 gravity, bool shortest=true)
    {
        Vector3 direction = endPos - startPos;
        Vector3 horizontalDirection = Vector3.ProjectOnPlane(direction, -gravity.normalized);
        float verticalDifference = Vector3.Dot( direction, -gravity.normalized );
        float horizontalDistance = horizontalDirection.magnitude;
        float duration = 0;

        Debug.DrawLine(startPos,startPos+horizontalDirection,Color.red);
        Debug.DrawLine(startPos + horizontalDirection, startPos + horizontalDirection + Vector3.up * verticalDifference, Color.yellow);
        Vector2 d = new Vector2(horizontalDistance, verticalDifference);
        float speedSqr = speed * speed;
        float g = gravity.magnitude;
        float angle;
        start = startPos;
        end = endPos;

        /* Wikipedia solution. Doesn't work
        var p = speedSqr * speedSqr - g * (g * d.x * d.x + 2 * d.y * speedSqr);
        if (p < 0) {
            throw new System.Exception("No valid solution for parabola");
        }
        float angle1 = Mathf.Min(Mathf.Atan((speedSqr - Mathf.Sqrt(p)) / g * d.x), Mathf.Atan((speedSqr + Mathf.Sqrt(p)) / g * d.x));
        */

        /* A Solution on stack exchange. Didin't work
        float e = 2f * speedSqr / g;
        float rootTerm = e * (e - 2f * d.y) - 2f * d.x * d.x;
        if (rootTerm < 0) {
            Debug.Log(rootTerm);
            throw new System.Exception("No valid solution for parabola");
        }
        float angle2 = Mathf.Min(Mathf.Atan((e + Mathf.Sqrt(rootTerm)) / 2 * d.x), Mathf.Atan((e - Mathf.Sqrt(rootTerm)) / 2 * d.x));
        */

        // sourced from http://physics.stackexchange.com/a/70480
        float k = speedSqr / (g * d.x);
        float rootTerm2 = -1 + speedSqr * (speedSqr - 2 * g * d.y) / (g * g * d.x * d.x);
        if (rootTerm2 < 0)
        {
            throw new System.Exception("No valid solution for parabola");
        }
        float angle3 = 0;
        if (shortest)
        {
            angle3 = Mathf.Min(Mathf.Atan(k - Mathf.Sqrt(rootTerm2)), Mathf.Atan(k + Mathf.Sqrt(rootTerm2)));
        }
        else {
            angle3 = Mathf.Max(Mathf.Atan(k - Mathf.Sqrt(rootTerm2)), Mathf.Atan(k + Mathf.Sqrt(rootTerm2)));

        }

        //Debug.LogFormat("{0} {1} {2}",angle1*Mathf.Rad2Deg, angle2 * Mathf.Rad2Deg, angle3 * Mathf.Rad2Deg);
        angle = angle3;

        var horizontalSpeedComponent = speed * Mathf.Cos(angle);
        var verticalSpeedComponent = speed * Mathf.Sin(angle);

        acceleration = g * Vector3.down;
        startVel = horizontalSpeedComponent * horizontalDirection.normalized + verticalSpeedComponent * Vector3.up;

        Debug.DrawLine(startPos, startPos+startVel, Color.green );


        duration = horizontalDistance / horizontalSpeedComponent;

        return duration;
    }

    public Vector3 GetPosition(float t)
    {
        return start + t * startVel + 0.5f * acceleration * t * t;
    }

    public Vector3 GetDirection(float t) {
        return ( startVel + acceleration * t ).normalized;
    }

    public Vector3 GetStartVelocity() {
        return startVel;
    }

    public static float GetMaxDistance(float speed, float gravity = 9.81f) {
        return speed * speed / gravity;
    }

    public static float GetMaxDistance(float speed, float yDiff, float gravity)
    {
        return Mathf.Sqrt(speed*speed+2* gravity * -yDiff ) * speed / gravity;
    }
}

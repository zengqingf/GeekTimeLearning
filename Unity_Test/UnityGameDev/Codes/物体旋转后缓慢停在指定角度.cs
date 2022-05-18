
/*
ref: https://www.cnblogs.com/suoluo/p/5260221.html
*/
using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    public float targetAngle = 180f;
    public float spinSpeed = 6f;
    public float stopSpeed = 2f;
    public float duration = 3f;
    float tmpAngle;
    Vector3 tmpLocalEulerAngles;

    float countdown;
    float _targetAngle;

    public enum State
    {
        Stopped,
        Spinning,
        Stopping
    }

    public State state
    {
        get
        {
            return _state;
        }
    }

    State _state = State.Stopped;

    public void StartSpin ()
    {
        transform.localEulerAngles = Vector3.zero;
        _targetAngle = targetAngle % 360f;
        countdown = duration;
        _state = State.Spinning;
        enabled = true;
    }

    void OnGUI ()
    {
        if (GUI.Button (new Rect (0f, 0f, 100f, 100f), "Rotate")) {
            StartSpin ();
        }
    }

    void Update ()
    {
        switch (_state) {
            case State.Spinning:
                transform.Rotate (0f, spinSpeed, 0f);
                countdown -= Time.deltaTime;
                if (countdown < 0f) {
                    if (transform.localEulerAngles.y > _targetAngle)
                        _targetAngle = 360f - (transform.localEulerAngles.y - _targetAngle) + transform.localEulerAngles.y;
                    tmpAngle = transform.localEulerAngles.y;
                    _state = State.Stopping;
                }
                break;
            case State.Stopping:
                tmpAngle = Mathf.Lerp (tmpAngle, _targetAngle, Time.deltaTime * stopSpeed);
                tmpLocalEulerAngles = transform.localEulerAngles;
                tmpLocalEulerAngles.y = tmpAngle;
                transform.localEulerAngles = tmpLocalEulerAngles;
                if (tmpAngle < targetAngle + 0.5f && tmpAngle > targetAngle - 0.5f) {
                    _state = State.Stopped;
                }
                break;
            default:
//                enabled = false;
                break;
        }
    }
}
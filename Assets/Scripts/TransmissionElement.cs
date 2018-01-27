using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmissionElement
{
    public enum Types { None, Left, Right, Both };

    public Types type;

    public float duration;

    public Types responseType;

    public float responseDuration;

    public void Set(Types tp, float dr) { type = tp; duration = dr; responseType = Types.None; responseDuration = 0f; }
}

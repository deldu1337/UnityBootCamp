using System;
using UnityEngine;

public enum Projection
{
    Perspective, Orthographic
}

public enum Field_of_View_Axis
{
    Vertical, Horizontal
}

public class Sample2 : MonoBehaviour
{
    public Projection projection;
    public Field_of_View_Axis field_of_view_Axis;
    public int Field_of_View = 60;
    public float Near = 0.3f;
    public int Far = 1000;
    public bool Physical_Camera = false;
}

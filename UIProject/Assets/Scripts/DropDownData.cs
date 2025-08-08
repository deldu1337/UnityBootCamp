using System.Collections.Generic;
using UnityEngine;

public enum DType
{ 
    Job,Gender
}


public class DropDownData : MonoBehaviour
{
    public DType Type;

    public List<string> menu = new List<string>();
}

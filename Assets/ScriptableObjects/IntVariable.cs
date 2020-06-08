using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IntVariable : ScriptableObject
{
    [SerializeField] int originalPoints;
    private int _point;
    public bool changed;
    public int point
    {
        get { return _point; }
        set
        {
            if(value != _point)
            {
                changed = true;
            }
            _point = value;
        }
    }
    public void OnEnable()
    {
        _point = originalPoints;
    }
}

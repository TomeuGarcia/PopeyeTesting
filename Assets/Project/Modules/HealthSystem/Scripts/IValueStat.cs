using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IValueStat
{
    public delegate void ValueStatEvent();
    public ValueStatEvent OnValueUpdate;

    public abstract float GetValuePer1Ratio();

}

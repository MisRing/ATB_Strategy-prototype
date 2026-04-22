using UnityEngine;
using System;

[Serializable]
public class FloatStat
{
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }
    [SerializeField] private float _value;
    
    public event Action<float> OnValueChanged;
    
    public FloatStat(float value)
    {
        _value = value;
        OnValueChanged = null;
    }
    
    public static implicit operator float(FloatStat stat)
    {
        return stat.Value;
    }
}

[Serializable]
public class IntStat
{
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }
    [SerializeField] private int _value;
    
    public event Action<int> OnValueChanged;
    
    public IntStat(int value)
    {
        _value = value;
        OnValueChanged = null;
    }
    
    public static implicit operator int(IntStat stat)
    {
        return stat.Value;
    }
}

[Serializable]
public class RangeIntStat
{
    public IntStat Min = new IntStat(0);
    public IntStat Max = new IntStat(1);

    public RangeIntStat(int min, int max)
    {
        Min.Value = min;
        Max.Value = max;
    }
}
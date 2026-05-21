using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatStat
{
    public float ClearValue
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            OnValueChanged?.Invoke(GetRealValue());
        }
    }
    public float Value { get => GetRealValue(); }

    [SerializeField] private float _value;
    private List<FloatStutBuff> _buffs = new List<FloatStutBuff>();

    public class FloatStutBuff
    {
        public string BuffName;
        public float Value;

        public FloatStutBuff(string name, float value)
        {
            BuffName = name;
            Value = value;
        }
    }

    public event Action<float> OnValueChanged;

    public FloatStat(float value)
    {
        _value = value;
        OnValueChanged = null;
        _buffs = new List<FloatStutBuff>();
    }

    public void SetBuff(FloatStutBuff buff)
    {
        if (_buffs.Contains(buff)) return;
        _buffs.Add(buff);

        OnValueChanged?.Invoke(GetRealValue());
    }

    public void RemoveBuff(FloatStutBuff buff)
    {
        if (!_buffs.Contains(buff)) return;
        _buffs.Remove(buff);

        OnValueChanged?.Invoke(GetRealValue());
    }

    public List<FloatStutBuff> ReadBuffs()
    {
        return new List<FloatStutBuff>(_buffs);
    }

    private float GetRealValue()
    {
        float realValue = _value;
        foreach (FloatStutBuff buff in _buffs)
        {
            realValue += buff.Value;
        }
        return realValue;
    }

    public override string ToString()
    {
        string str = _value.ToString() + " (";
        float buffs = 0;
        foreach (FloatStutBuff buff in _buffs)
        {
            buffs += buff.Value;
        }

        if (buffs >= 0) str += "+";

        str += buffs + ")";

        return str;
    }

    public static implicit operator float(FloatStat stat)
    {
        return stat.Value;
    }
}

[Serializable]
public class IntStat
{
    public int ClearValue
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            OnValueChanged?.Invoke(GetRealValue());
        }
    }
    public int Value { get => GetRealValue(); }

    [SerializeField] private int _value;
    private List<IntStutBuff> _buffs = new List<IntStutBuff>();

    public class IntStutBuff
    {
        public string BuffName;
        public int Value;

        public IntStutBuff(string name, int value)
        {
            BuffName = name;
            Value = value;
        }
    }

    public event Action<int> OnValueChanged;
    
    public IntStat(int value)
    {
        _value = value;
        OnValueChanged = null;
        _buffs = new List<IntStutBuff>();
    }

    public void SetBuff(IntStutBuff buff)
    {
        if (_buffs.Contains(buff)) return;
        _buffs.Add(buff);

        OnValueChanged?.Invoke(GetRealValue());
    }

    public void RemoveBuff(IntStutBuff buff)
    {
        if (!_buffs.Contains(buff)) return;
        _buffs.Remove(buff);

        OnValueChanged?.Invoke(GetRealValue());
    }

    public List<IntStutBuff> ReadBuffs()
    {
        return new List<IntStutBuff>(_buffs);
    }

    private int GetRealValue()
    {
        int realValue = _value;
        foreach (IntStutBuff buff in _buffs)
        {
            realValue += buff.Value;
        }
        return realValue;
    }

    public override string ToString()
    {
        string str = _value.ToString() + " (";
        int buffs = 0;
        foreach (IntStutBuff buff in _buffs)
        {
            buffs += buff.Value;
        }

        if (buffs >= 0) str += "+";

        str += buffs + ")";

        return str;
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
        Min.ClearValue = min;
        Max.ClearValue = max;
    }
}
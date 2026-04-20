using UnityEngine;
using System;

public class UnitStats : MonoBehaviour
{
    [Header("Movement")]
    public FloatStat Speed = new FloatStat(8f);
    
    [Header("Health")]
    public IntStat Health = new IntStat(5);
    public IntStat MaxHealth = new IntStat(5);
    public IntStat Armor = new IntStat(3);
    public IntStat MaxArmor = new IntStat(3);
    
    [Header("Combat")]
    public FloatStat VisionRange = new FloatStat(16f);
    public IntStat Accuracy = new IntStat(50);
    public IntStat Dodge = new IntStat(10);
    public IntStat SelfControll = new IntStat(30);
}
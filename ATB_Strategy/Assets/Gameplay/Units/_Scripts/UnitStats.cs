using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Header("Main")]
    public string Name = "unit-name";
    public Sprite Icon;
    public int ID = 0;

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
    public IntStat SelfControl = new IntStat(30);
}
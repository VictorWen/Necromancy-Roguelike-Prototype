using System.Collections.Generic;
using System;
using UnityEngine;


public class AttributeCalculator
{
    public enum ModifierType
    {
        BASE,
        ADD,
        MULT
    }

    public struct AttributeModifier
    {
        public Attribute attribute;
        public ModifierType type;
        public float value;

        public AttributeModifier(Attribute attribute, ModifierType type, float value)
        {
            this.attribute = attribute;
            this.type = type;
            this.value = value;
        }
    }

    public static Dictionary<Attribute, float> GlobalDefaultValues { get; private set; }

    private readonly Dictionary<Attribute, float> defaultValues;
    private readonly Dictionary<Attribute, HashSet<AttributeModifier>> attributeModifiers;
    private readonly Dictionary<string, List<AttributeModifier>> modifiers;

    static AttributeCalculator()
    {
        GlobalDefaultValues = new Dictionary<Attribute, float>();

        foreach (Attribute attribute in Enum.GetValues(typeof(Attribute)))
        {
            GlobalDefaultValues[attribute] = 0;
        }

        GlobalDefaultValues[Attribute.RELOAD_TIME_MULTIPLIER] = 1;

        GlobalDefaultValues[Attribute.CRITICAL_CHANCE] = 0.03f;
        GlobalDefaultValues[Attribute.CRITICAL_MULTIPLIER] = 2;
    }

    public AttributeCalculator()
    {
        this.defaultValues = new Dictionary<Attribute, float>();
        this.attributeModifiers = new Dictionary<Attribute, HashSet<AttributeModifier>>();
        this.modifiers = new Dictionary<string, List<AttributeModifier>>();

        foreach (Attribute attribute in Enum.GetValues(typeof(Attribute)))
        {
            defaultValues[attribute] = GlobalDefaultValues[attribute];
            attributeModifiers.Add(attribute, new HashSet<AttributeModifier>());
        }
    }

    public AttributeCalculator(List<Attribute> attributes)
    {
        this.defaultValues = new Dictionary<Attribute, float>();
        this.attributeModifiers = new Dictionary<Attribute, HashSet<AttributeModifier>>();
        this.modifiers = new Dictionary<string, List<AttributeModifier>>();

        foreach (Attribute attribute in attributes)
        {
            defaultValues[attribute] = GlobalDefaultValues[attribute];
            attributeModifiers.Add(attribute, new HashSet<AttributeModifier>());
        }
    }

    public void SetDefaultValue(Attribute name, float value)
    {
        defaultValues[name] = value;
    }

    public float GetAttribute(Attribute name)
    {
        float baseValue = defaultValues[name];
        float addValue = 1;
        float multValue = 1;

        foreach (AttributeModifier mod in attributeModifiers[name])
        {
            Debug.Log(mod.value);
            switch (mod.type)
            {
                case ModifierType.BASE:
                    baseValue += mod.value;
                    break;
                case ModifierType.ADD:
                    addValue += mod.value;
                    break;
                case ModifierType.MULT:
                    multValue *= mod.value;
                    break;
            }
        };

        return baseValue * addValue * multValue;
    }

    public bool HasModifier(string name)
    {
        return modifiers.ContainsKey(name);
    }

    public bool RemoveModifier(string name)
    {
        if (!modifiers.ContainsKey(name))
            return false;

        foreach (AttributeModifier mod in modifiers[name])
        {
            attributeModifiers[mod.attribute].Remove(mod);
        }
        modifiers.Remove(name);
        return true;
    }

    public bool AddModifier(string name, List<AttributeModifier> modList)
    {
        if (modifiers.ContainsKey(name))
            return false;

        foreach (AttributeModifier mod in modList)
        {
            attributeModifiers[mod.attribute].Add(mod);
        }
        modifiers.Add(name, modList);
        return true;
    }

    public void SetModifier(string name, List<AttributeModifier> modList)
    {
        RemoveModifier(name);
        AddModifier(name, modList);
    }
}
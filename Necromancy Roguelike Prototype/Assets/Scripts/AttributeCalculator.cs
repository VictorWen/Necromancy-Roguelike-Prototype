using System.Collections.Generic;
using System;
using UnityEngine;

public enum ModifierType
{
    BASE,
    ADD,
    MULT
}

public class AttributeCalculator
{
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

    private readonly Dictionary<Attribute, float> defaultValues;
    private readonly Dictionary<Attribute, HashSet<AttributeModifier>> attributeModifiers;
    private readonly Dictionary<string, List<AttributeModifier>> modifiers;

    public AttributeCalculator()
    {
        this.defaultValues = new Dictionary<Attribute, float>();
        this.attributeModifiers = new Dictionary<Attribute, HashSet<AttributeModifier>>();
        this.modifiers = new Dictionary<string, List<AttributeModifier>>();

        foreach (Attribute attribute in Enum.GetValues(typeof(Attribute)))
        {
            defaultValues[attribute] = 1;
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
using System.Collections.Generic;
using UnityEngine;

public class BandolierItem : BaseItem
{
    public override string ID { get { return "bandolier"; } }

    public override void OnAdd(PlayerController player)
    {
        Debug.Log(Count);
        UpdateModifier(player);
    }

    public override void OnRemove(PlayerController player)
    {
        if (Count <= 0)
            player.Attributes.RemoveModifier("bandolier");
        else
            UpdateModifier(player);
    }

    private void UpdateModifier(PlayerController player)
    {
        player.Attributes.SetModifier("bandolier", new List<AttributeCalculator.AttributeModifier>()
        {
            new AttributeCalculator.AttributeModifier(Attribute.RELOAD_TIME_MULTIPLIER, ModifierType.MULT, Mathf.Pow(0.5f, Count))
        });
    }
}

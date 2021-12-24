using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public bool isPlayerDealtDamage;
    public bool isPlayerFriendlyDamage;
    public bool isRevivalDamage;

    public static DamageInfo CreatePlayerDamageInfo()
    {
        return new DamageInfo
        {
            isPlayerDealtDamage = true,
            isPlayerFriendlyDamage = true
        };
    }

    public static DamageInfo CreateEnemyDamageInfo()
    {
        return new DamageInfo
        {
            isPlayerFriendlyDamage = false,
        };
    }

    public static DamageInfo CreateMinionDamageInfo()
    {
        return new DamageInfo
        {
            isPlayerDealtDamage = false,
            isPlayerFriendlyDamage = true
        };
    }

    public static DamageInfo CreateSoulDamageInfo()
    {
        return new DamageInfo
        {
            isPlayerDealtDamage = true,
            isPlayerFriendlyDamage = true,
            isRevivalDamage = true,
        };
    }
}

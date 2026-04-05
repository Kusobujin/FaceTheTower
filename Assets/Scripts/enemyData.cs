using UnityEngine;

[System.Serializable]
public class enemyData
{
    public string enemyName;
    public int minHP;
    public int maxHP;
    public int damage;

    public int minFloor;
    public int maxFloor;

    // SKILLS
    public EnemySkillType skillType;
    public int skillPower;
    public int skillCooldown;

    [Range(0, 100)]
    public int skillChance;

    public int skillDuration;
}

public enum EnemySkillType
{
    None,
    Thorns,
    Heal,
    Summon
}
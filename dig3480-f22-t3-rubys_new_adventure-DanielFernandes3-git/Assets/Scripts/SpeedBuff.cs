using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]

public class SpeedBuff : PowerupEffect
{
    public float amount;
    public override void Apply(GameObject target)
    {
        target.GetComponent<RubyController>().speed = amount + 5;
        target.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : AnimationController
{

    private const string Speed = "Speed";  
    private const string Attack = "Attack";  

    /// <summary>
    /// 0 = Idle , 0.5f = Walk, 1f = Run
    /// </summary>
    /// <param name="blendValue"></param>
    public void SetMovementBlendTree(float blendValue)
    {
        anim.SetFloat(Speed, blendValue, 0.1f, Time.deltaTime);
    }

    public void PlayAttackAnim()
    {
        anim.SetTrigger(Attack);
    }
}

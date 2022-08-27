using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "Player")]
public class MovementSettings : ScriptableObject
{
    public CharacterController controller;
    public float walkSpeed;
    public float runSpeed;
    public float jumpHeight;

    public Animator anim;
}

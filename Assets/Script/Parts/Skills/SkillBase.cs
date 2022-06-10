using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public virtual void Fire(BaseObject owner) {}

    public System.Action EventHit { get; set; } = null;
    public System.Action EventEnd { get; set; } = null;
    public Vector3 Direction { get; set; } = Vector3.right;
}

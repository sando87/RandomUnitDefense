using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseObject : MonoBehaviour
{
    public BodyCollider Body { get { return GetComponentInChildren<BodyCollider>(); } }
    public Animator Animator { get { return GetComponentInChildren<Animator>(); } }
    public BaseRenderer Renderer { get { return GetComponentInChildren<BaseRenderer>(); } }
    public Health Health { get { return GetComponentInChildren<Health>(); } }
    public CharacterInput CharacterInput { get { return GetComponentInChildren<CharacterInput>(); } }
    public MotionManager MotionManager { get { return GetComponentInChildren<MotionManager>(); } }
    public PlayerBase PlayerState { get { return GetComponentInChildren<PlayerBase>(); } }
    public BuffProperty BuffProp { get { return GetComponentInChildren<BuffProperty>(); } }
    public SpecProperty SpecProp { get { return GetComponentInChildren<SpecProperty>(); } }
    public BuffController BuffCtrl { get { return GetComponentInChildren<BuffController>(); } }
    public UnitBase UnitBase { get { return GetComponentInChildren<UnitBase>(); } }

    public int GetLayerMaskAttackable()
    {
        int mask = 0;
        if (gameObject.layer == LayerID.Player)
        {
            mask |= (1 << LayerID.Enemies);
        }
        else if (gameObject.layer == LayerID.Enemies)
        {
            mask |= (1 << LayerID.Player);
        }
        
        return mask;
    }

    public bool IsOpposite(int layerID)
    {
        if (gameObject.layer == LayerID.Player)
        {
            return layerID == LayerID.Enemies;
        }
        else if (gameObject.layer == LayerID.Enemies)
        {
            return layerID == LayerID.Player;
        }
        
        return false;
    }

    public int GetOppositeLayer()
    {
        if (gameObject.layer == LayerID.Player)
        {
            return LayerID.Enemies;
        }
        else if (gameObject.layer == LayerID.Enemies)
        {
            return LayerID.Player;
        }

        return 0;
    }
}

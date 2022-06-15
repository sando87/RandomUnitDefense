using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스킬 쿨타임마다 주변의 적들에게 총기 난사 모션을 구현한 클래스
public class MotionRandomShooting : MotionBasic
{
    // [SerializeField] private ParticleSystem HitFloorPrefab = null;
    // [SerializeField] float _Cooltime = 5.0f;
    // [SerializeField] float _Range = 3.0f;
    // [Range(0, 1)][SerializeField] float Accuracy = 0.25f; //랜덤 난사가 명중되는 확률 (기본 25%)

    // public Action<UnitBase[]> EventFired { get; set; }
    // private float NextAttackTime = 0;
    // private float PlayTime = 0;
    // private float AnimTotalPlayTime = 0;
    // private UnitMob[] DetectedTargets = null;
    // private ParticleSystem hitFloorObject = null;


    // public override void OnInit()
    // {
    //     base.OnInit();
    //     SetCooltime(_Cooltime * mBaseObject.BuffProp.Cooltime);
    // }

    // public override bool OnReady()
    // {
    //     if (IsCooltime())
    //         return false;

    //     return true;
    // }

    // public override void OnEnter()
    // {
    //     base.OnEnter();
    //     PlayTime = 0;
    //     DetectedTargets = Unit.DetectAround<UnitMob>(Unit.Property.SkillRange);
    //     NextAttackTime = Time.realtimeSinceStartup + Unit.Property.Cooltime;

    //     Unit.Anim.SetTrigger("motionA1");

    //     AnimTotalPlayTime = ReferenceAnim.length / Unit.Anim.GetFloat("motionASpeed");
    //     StartCoroutine("FireAroundTargets");
    // }
    // public override void OnUpdate()
    // {
    //     base.OnUpdate();
    //     PlayTime += Time.deltaTime;
    //     if (PlayTime >= AnimTotalPlayTime)
    //         Unit.FSM.ChangeState(UnitState.Idle);
    // }
    // public override void OnLeave()
    // {
    //     StopCoroutine("FireAroundTargets");
    //     DetectedTargets = null;
    //     PlayTime = 0;
    //     if(hitFloorObject != null)
    //     {
    //         Destroy(hitFloorObject.gameObject);
    //         hitFloorObject = null;
    //     }
    // }

    // private IEnumerator FireAroundTargets()
    // {
    //     float delayTime = AnimTotalPlayTime * 0.2f;
    //     yield return Yielders.GetWaitForSeconds(delayTime);

    //     hitFloorObject = Instantiate(HitFloorPrefab, transform);
    //     float playRate = PlayTime / AnimTotalPlayTime;
    //     while (playRate <= 0.8f)
    //     {
    //         if (DetectedTargets != null)
    //         {
    //             List<UnitMob> mobs = new List<UnitMob>();
    //             foreach (UnitMob mob in DetectedTargets)
    //             {
    //                 if (mob == null || mob.CurrentState == UnitState.Death)
    //                     continue;

    //                 int percent = (int)(Accuracy * 100.0f);
    //                 if (UnityEngine.Random.Range(0, 100) < percent)
    //                     mobs.Add(mob);
    //             }

    //             if(mobs.Count > 0)
    //                 EventFired?.Invoke(mobs.ToArray());
    //         }
    //         yield return Yielders.GetWaitForSeconds(0.05f);
    //         playRate = PlayTime / AnimTotalPlayTime;
    //     }
    //     hitFloorObject.Stop();
    // }
}


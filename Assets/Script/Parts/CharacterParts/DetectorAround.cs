using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 클래스는 해당 캐릭터의 주변 지형 및 유닛들의 감지하는 역할만 한다.

public class DetectorAround : MonoBehaviour
{
    // [SerializeField] public ColliderTriggerProxy FootArea = null;
    // [SerializeField] public ColliderTriggerProxy LadderDetectArea = null;
    // [SerializeField] public ColliderTriggerProxy ObstacleArea = null;
    // [SerializeField] public ColliderTriggerProxy ClimbArea = null;
    // [SerializeField] public ColliderTriggerProxy FrontFootArea = null;
    // [SerializeField] public ColliderTriggerProxy DetectTargetArea = null;
    // [SerializeField] public float FrontRaycastDistance = 0;

    // //현재 캐릭터의 발쪽 콜라이더가 맞닿은 지면 객체 수(현재 캐릭이 땅에 있는지 여부 체크용)
    // private int mCountGrounded = 0;
    // public bool IsGrounded 
    // { 
    //     get 
    //     { 
    //         if (IsOverLadder) return true;
    //         if (mCountGrounded > 0 && mCharacterPhy.Velocity.y <= 0) return true;
    //         return false;
    //     } 
    // }
    // public Collider FootOther { get { return FootArea.Other; } }

    // //현재 캐릭터의 발쪽 콜라이더가 맞닿은 사다리 객체 수(현재 캐릭이 사다리에 있는지 여부 체크용)
    // private int mCountLadder = 0;
    // public bool IsOverLadder { get { return mCountLadder > 0; } }

    // //현재 캐릭터가 걸어가는 발 앞쪽에 약간의 높은 턱이 있는지 여부 판단.
    // private int mCountObstacled = 0;
    // public bool IsObstacled { get { return mCountObstacled > 0; } }
    // public Collider ObstacleOther { get { return ObstacleArea.Other; } }

    // //현재 캐릭터가 벽탈수 있는 객체가 있는지 여부 판단.
    // private int mCountClimbed = 0;
    // public bool IsClimbable { get { return mCountClimbed > 0; } }
    // public Collider ClimbOther { get { return ClimbArea.Other; } }

    // //현재 캐릭터 발 앞쪽에 갈수있는 지형이 있는지 여부 판단.
    // private int mCountFrontFoot = 0;
    // public bool IsFrontFoot { get { return mCountFrontFoot > 0; } }
    // public Collider FrontFootOther { get { return FrontFootArea.Other; } }

    // //현재 캐릭터주변의 Target 감지.
    // public Collider DetectedTarget { get; private set; } = null;

    // private CharacterPhysics mCharacterPhy = null;

    // void OnEnable() 
    // {
    //     mCountGrounded = 0;
    //     mCountObstacled = 0;
    //     mCountClimbed = 0;
    //     mCountFrontFoot = 0;
    //     DetectedTarget = null;
    // }

    // void Start()
    // {
    //     mCharacterPhy = this.GetBaseObject().CharacterPhy;

    //     if(FootArea != null)
    //     {
    //         FootArea.EventTriggerEnter += (other) => { mCountGrounded++; };
    //         FootArea.EventTriggerExit += (other) => { mCountGrounded--; };
    //     }

    //     if (LadderDetectArea != null)
    //     {
    //         LadderDetectArea.EventTriggerEnter += (other) => { mCountLadder++; };
    //         LadderDetectArea.EventTriggerExit += (other) => { mCountLadder--; };
    //     }

    //     if (ObstacleArea != null)
    //     {
    //         ObstacleArea.EventTriggerEnter += (other) => { mCountObstacled++; };
    //         ObstacleArea.EventTriggerExit += (other) => { mCountObstacled--; };
    //     }

    //     if (ClimbArea != null)
    //     {
    //         ClimbArea.EventTriggerEnter += (other) => { mCountClimbed++; };
    //         ClimbArea.EventTriggerExit += (other) => { mCountClimbed--; };
    //     }

    //     if (FrontFootArea != null)
    //     {
    //         FrontFootArea.EventTriggerEnter += (other) => { mCountFrontFoot++; };
    //         FrontFootArea.EventTriggerExit += (other) => { mCountFrontFoot--; };
    //     }

    //     if (DetectTargetArea != null)
    //     {
    //         // DetectPlayerArea가 설정되어 있으면 이 영역 정보로 플레이어를 감지하고
    //         DetectTargetArea.EventTriggerEnter += (other) => { DetectedTarget = other; };
    //         DetectTargetArea.EventTriggerExit += (other) => { DetectedTarget = null; };

    //         // 상대방 layer만 감지하도록 layerMask 세팅
    //         DetectTargetArea.SetLayerMask(1 << this.GetBaseObject().GetOppositeLayer());
    //     }
    //     else
    //     {
    //         // 아니면 1초에 한번씩 Raycast해서 적을 감지한다.
    //         StartCoroutine(CoRaycastToDetectTarget());
    //     }
    // }

    // IEnumerator CoRaycastToDetectTarget()
    // {
    //     BaseObject baseObject = this.GetBaseObject();
    //     int layerMaskAttackable = baseObject.GetLayerMaskAttackable();
    //     while(true)
    //     {
    //         DetectedTarget = null;

    //         if(FrontRaycastDistance > 0)
    //         {
    //             // 전방 방향으로 Raycast해서 첫번째 걸린 객체의 layerID가 적군layer이면 타겟으로 선정
    //             RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.right, FrontRaycastDistance, layerMaskAttackable);
    //             if(hits.Length > 0)
    //             {
    //                 if (baseObject.IsOpposite(hits[0].collider.gameObject.layer))
    //                 {
    //                     DetectedTarget = hits[0].collider;
    //                 }
    //             }
    //         }

    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMarine : UnitUser
{
    [SerializeField] private GameObject BulletSparkPrefab = null;

    public override void Init()
    {
        base.Init();
        GetComponent<MotionSingleAttack>().EventFired = OnAttack;
        BuffCtrl.AddBuff(new BuffSteamPack());

    }
    public override void Release()
    {
        base.Release();
    }

    public void OnAttack(UnitBase target)
    {
        // 20% 확률로 스킬샷 발동 부분
        bool isSkillShoot = UnityEngine.Random.Range(0, 5) == 0;
        if (isSkillShoot)
            ShootSkillGun(target);
        else
            ShootSimpleGun(target);
    }

    public void ShootSimpleGun(UnitBase target)
    {
        UnitMob enemy = target as UnitMob;
        enemy.GetDamaged(Spec);
        Vector3 pos = Utils.Random(enemy.Center, 0.1f);
        GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }
    public void ShootSkillGun(UnitBase target)
    {
        UnitMob enemy = target as UnitMob;
        enemy.GetDamaged(Spec);
        Vector3 pos = Utils.Random(enemy.Center, 0.1f);
        GameObject obj = Instantiate(BulletSparkPrefab, pos, Quaternion.identity);
        Destroy(obj, 1.0f);
    }

    class BuffSteamPack : BuffBase
    {
        //cooltime마다 공속 4배 증가 효과 버프(duration시간만큼 지속)
        public override void StartBuff(UnitBase me)
        {
            Duration = me.Spec.SkillDuration;
            me.BuffValues.AttackSpeed += 300;
        }
        public override void EndBuff(UnitBase me)
        {
            me.BuffValues.AttackSpeed -= 300;
            RepeatBuff(me.Spec.Cooltime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperTower : Tower
{
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    private GameObject currentProjectile;
    private GameObject currentMuzzleEffect;
    private GameObject currentHitEffect;

    public bool IsTargeting;
    public bool IsBomb;
    public SniperTower()
    {
        Name = "Sniper Tower";
        Element = "Normal";
        Damage = 50;
        Range = 15f;
        FireRate = 2f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 50;
        SellPrice = 25;
        TargetPriority = "Most Progress";
        Info = "One Shot One Kill";
    }

    protected override void Start()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
        base.Start();
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(FireRate);
            if (CurrentTarget != null)
            {
                Projectile projectile = ObjectManager.Instance.Spawn<Projectile>(currentProjectile, TowerMuzzle.transform.position);
                projectile.Damage = this.Damage;
                projectile.IsTargeting = this.IsTargeting;
                projectile.IsBomb = this.IsBomb;
                projectile.Target = this.CurrentTarget;

                PooledParticle muzzleEffect = ObjectManager.Instance.Spawn<PooledParticle>(
                         currentMuzzleEffect, TowerMuzzle.transform.position, TowerHead.transform.rotation
                );
                muzzleEffect?.Initialize();
            }
        }
    }

    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            // 기본 스탯 증가
            baseAttackDamage += 10;

            // 버프가 없을 경우 현재 값 설정
            if (originalStats.Count == 0)
            {
                Damage = baseAttackDamage;
            }
        }
        else if (Level == 3)
        {
            FireRate /= 1.5f;
            baseRange += 1.5f;
            baseAttackDamage += 10;

            // 버프가 없을 경우 현재 값 설정
            if (originalStats.Count == 0)
            {
                Range = baseRange;
                Damage = baseAttackDamage;
            }
        }

        SetByLevel();

        // 버프가 있다면 재적용
        if (originalStats.Count > 0)
        {
            // 현재 적용된 모든 버프를 저장
            var currentBuffs = new List<BuffField>(originalStats.Keys);

            // 모든 버프 제거
            foreach (var buff in currentBuffs)
            {
                RemoveBuff(buff);
            }

            // 버프 재적용
            foreach (var buff in currentBuffs)
            {
                ApplyBuff(buff);
            }
        }
    }

    private void SetByLevel()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
    }
}

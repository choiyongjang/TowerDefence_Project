using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTower : Tower
{
    [Header("타워 레벨효과")]
    [SerializeField] private List<GameObject> projectiles;
    [SerializeField] private List<GameObject> muzzleEffects;
    [SerializeField] private List<GameObject> HitEffects;

    [Header("발사 위치")]
    [SerializeField] private Transform leftMuzzle;   // Inspector에서 할당
    [SerializeField] private Transform rightMuzzle;  // Inspector에서 할당
    private bool useLeftMuzzle = true;  // 왼쪽/오른쪽 발사 위치 번갈아가며 사용

    private GameObject currentProjectile;
    private GameObject currentMuzzleEffect;
    private GameObject currentHitEffect;

    private List<Monster> targetMonsters = new List<Monster>();
    public bool IsTargeting;
    public bool IsBomb;
    public int RimitTarget = 10;

    private bool isAttacking = false;

    private int originalDamage;
    private float originalRange;
    private float originalSplashRange;

    public float splashRange = 2f;
    public GameObject explosionEffect;
    public GameObject missilePrefab;

    public MissileTower() 
    {
        Name = "Missile Tower";
        Element = "Explosive";
        Damage = 3;
        Range = 6f;
        FireRate = 1.5f;
        DamageDealt = 0;
        TotalKilled = 0;
        UpgradePrice = 65;
        SellPrice = 33;
        TargetPriority = "Most Progress";
        Info = "Launches missiles that deal splash damage to groups of enemies.";

        originalDamage = Damage;
        originalRange = Range;
        originalSplashRange = splashRange;
    }
    protected override void Start()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
        targetMonsters = new List<Monster>();
        base.Start();
    }

    protected override void Detect()
    {
        targetMonsters.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, Range);
        
        foreach (Collider target in hitColliders)
        {
            if (target != null && target.CompareTag("Monster"))
            {
                Monster monster = target.GetComponent<Monster>();
                if (monster != null && monster.gameObject.activeSelf && !monster.IsDead)
                {
                    targetMonsters.Add(monster);
                    if (targetMonsters.Count >= RimitTarget)
                        break;
                }
            }
        }

        CurrentTarget = targetMonsters.Count > 0 ? targetMonsters[0].transform : null;
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (isAttacking)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(FireRate);

            if (targetMonsters.Count > 0)
            {
                isAttacking = true;
                
                targetMonsters.RemoveAll(monster => 
                    monster == null || !monster.gameObject.activeSelf || monster.IsDead);

                if (targetMonsters.Count > 0)
                {
                    List<Monster> currentTargets = new List<Monster>(targetMonsters);
                    foreach (var monster in currentTargets)
                    {
                        if (monster != null && monster.gameObject.activeSelf && !monster.IsDead)
                        {
                            LaunchMissile(monster);
                            yield return new WaitForSeconds(0.05f);
                        }
                    }
                }

                isAttacking = false;
            }

            yield return null;
        }
    }

    private void LaunchMissile(Monster target)
    {
        if (target == null || !target.gameObject.activeSelf || target.IsDead)
            return;

        // 현재 사용할 발사 위치 선택
        Transform currentMuzzle = useLeftMuzzle ? leftMuzzle : rightMuzzle;
        useLeftMuzzle = !useLeftMuzzle;  // 다음 발사를 위해 전환

        Projectile projectile = ObjectManager.Instance.Spawn<Projectile>(
            currentProjectile,
            currentMuzzle.position  // 선택된 발사 위치 사용
        );

        if (projectile != null)
        {
            projectile.transform.rotation = TowerHead.transform.rotation;
            projectile.Damage = this.Damage;
            projectile.IsTargeting = this.IsTargeting;
            projectile.IsBomb = this.IsBomb;
            projectile.Target = target.transform;
            projectile.Initialize();

            PooledParticle muzzleEffect = ObjectManager.Instance.Spawn<PooledParticle>(
                currentMuzzleEffect,
                currentMuzzle.position,  // 이펙트도 같은 위치에서 생성
                TowerHead.transform.rotation
            );
            muzzleEffect?.Initialize();
        }
    }

    protected override void OnLevelUp()
    {
        if (Level == 2)
        {
            Damage += 1;
            Range += 1f;
            SetByLevel();
        }
        else if (Level == 3)
        {
            FireRate /= 2f;
            Range += 3f;
            SetByLevel();
        }
    }

    private void SetByLevel()
    {
        currentProjectile = projectiles[Level - 1];
        currentMuzzleEffect = muzzleEffects[Level - 1];
        currentHitEffect = HitEffects[Level - 1];
    }

    public override void ApplyBuff(BuffField buff)
    {
        base.ApplyBuff(buff);
        splashRange = originalSplashRange * buff.rangeMultiplier;
    }

    public override void RemoveBuff(BuffField buff)
    {
        base.RemoveBuff(buff);
        splashRange = originalSplashRange;
    }
}
using UnityEngine;

public class BossPattern_Low3 : BossBaseBehaviour
{
    public float moveSpeed = 6f;
    public float fireRate = 0.5f;
    private float fireTimer;
    private float stateTime;
    private float startY;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        stateTime = 0f;
        if (bossTransform != null) startY = bossTransform.position.y;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        stateTime += Time.deltaTime;

        // Movimento frenético mas contínuo
        float targetY = startY + Mathf.Sin(stateTime * moveSpeed) * 3.5f;
        Vector3 targetPos = new Vector3(bossTransform.position.x, targetY, 0);

        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            targetPos,
            10f * Time.deltaTime
        );

        // Anel massivo
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            int bulletCount = 12;
            float angleStep = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = i * angleStep;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;

                Projectile p = boss.bossBulletPool.Get();
                p.transform.position = boss.firePoint.position;
                p.Launch(dir, 10f, 1);
            }
        }
    }
}
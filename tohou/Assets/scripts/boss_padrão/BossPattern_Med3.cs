using UnityEngine;

public class BossPattern_Med3 : BossBaseBehaviour
{
    public float fireRate = 0.7f;
    public float smoothReturnSpeed = 3f;
    private float fireTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Desacelera e desacopla o movimento suavemente se o Boss vinha com aceleração de outro estado
        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            bossTransform.position,
            smoothReturnSpeed * Time.deltaTime
        );

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootCirclePattern(8);
        }
    }

    private void ShootCirclePattern(int bulletCount)
    {
        float angleStep = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;

            Projectile p = boss.bossBulletPool.Get();
            p.transform.position = boss.firePoint.position;
            p.Launch(dir, 8f, 1);
        }
    }
}
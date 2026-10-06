using UnityEngine;

public class BossPattern_Low2 : BossBaseBehaviour
{
    public float fireRate = 0.15f;
    public float rotationSpeed = 90f;
    private float fireTimer;
    private float currentAngle;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        currentAngle = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        currentAngle += rotationSpeed * Time.deltaTime;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;

            Vector2 dir1 = Quaternion.Euler(0, 0, currentAngle) * Vector2.left;
            Vector2 dir2 = Quaternion.Euler(0, 0, currentAngle + 180f) * Vector2.left;

            Projectile p1 = boss.bossBulletPool.Get();
            p1.transform.position = boss.firePoint.position;
            p1.Launch(dir1, 9f, 1);

            Projectile p2 = boss.bossBulletPool.Get();
            p2.transform.position = boss.firePoint.position;
            p2.Launch(dir2, 9f, 1);
        }
    }
}
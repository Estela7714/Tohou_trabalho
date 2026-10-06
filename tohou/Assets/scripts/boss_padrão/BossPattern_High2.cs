using UnityEngine;

public class BossPattern_High2 : BossBaseBehaviour
{
    public float fireRate = 0.7f;
    private float fireTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootVPattern();
        }
    }

    private void ShootVPattern()
    {
        float[] angles = { -15f, 15f };
        foreach (float angle in angles)
        {
            Projectile p = boss.bossBulletPool.Get();
            p.transform.position = boss.firePoint.position;
            p.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;
            p.Launch(dir, 9f, 1);
        }
    }
}
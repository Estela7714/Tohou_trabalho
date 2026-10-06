using UnityEngine;

public class BossPattern_Low1 : BossBaseBehaviour
{
    public float followSpeed = 5f;
    public float fireRate = 0.35f;
    private float fireTimer;
    private Transform playerTransform;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Perseguição rápida e suave
        if (playerTransform != null)
        {
            float targetY = playerTransform.position.y;
            Vector3 targetPos = new Vector3(bossTransform.position.x, targetY, 0);
            bossTransform.position = Vector3.MoveTowards(
                bossTransform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            float[] angles = { -40f, -20f, 0f, 20f, 40f };
            foreach (float angle in angles)
            {
                Projectile p = boss.bossBulletPool.Get();
                p.transform.position = boss.firePoint.position;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;
                p.Launch(dir, 13f, 1);
            }
        }
    }
}
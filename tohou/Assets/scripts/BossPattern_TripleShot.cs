using UnityEngine;

public class BossPattern_TripleShot : BossBaseBehaviour
{
    public float moveSpeed = 3f;
    public float fireInterval = 0.8f;
    private float fireTimer;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Movimento Senoidal
        float newY = Mathf.Sin(Time.time * moveSpeed) * 3f;
        bossTransform.position = new Vector3(bossTransform.position.x, newY, 0);

        // Disparo Triplo
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0;
            ShootPattern();
        }
    }

    private void ShootPattern()
    {
        float[] angles = { -15f, 0f, 15f };
        foreach (float angle in angles)
        {
            Projectile p = boss.bossBulletPool.Get();
            p.transform.position = boss.firePoint.position;
            p.transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;
            p.Launch(dir, 10f, 1);
        }
    }
}
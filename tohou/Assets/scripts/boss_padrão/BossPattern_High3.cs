using UnityEngine;

public class BossPattern_High3 : BossBaseBehaviour
{
    public float followSpeed = 1.5f;
    public float fireRate = 0.9f;
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

        // Segue suavemente o eixo Y do jogador
        if (playerTransform != null)
        {
            float targetY = playerTransform.position.y;
            float newY = Mathf.Lerp(bossTransform.position.y, targetY, followSpeed * Time.deltaTime);
            bossTransform.position = new Vector3(bossTransform.position.x, newY, 0);
        }

        // Tiro Triplo
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootTriple();
        }
    }

    private void ShootTriple()
    {
        float[] angles = { -20f, 0f, 20f };
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
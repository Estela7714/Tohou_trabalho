using UnityEngine;

public class BossPattern_Med1 : BossBaseBehaviour
{
    public float moveSpeed = 3f;
    public float fireRate = 0.5f;
    private float fireTimer;
    private float stateTime;
    private Vector3 initialPos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        stateTime = 0f;
        if (bossTransform != null) initialPos = bossTransform.position;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        stateTime += Time.deltaTime;

        // Movimento em '8' suave baseado na posição de entrada
        float x = Mathf.Sin(stateTime * moveSpeed) * 1.5f;
        float y = Mathf.Sin(stateTime * moveSpeed * 2f) * 2.5f;
        Vector3 targetPos = initialPos + new Vector3(x, y, 0);

        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            targetPos,
            6f * Time.deltaTime
        );

        // Disparo Quádruplo
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            float[] angles = { -30f, -10f, 10f, 30f };
            foreach (float angle in angles)
            {
                Projectile p = boss.bossBulletPool.Get();
                p.transform.position = boss.firePoint.position;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.left;
                p.Launch(dir, 11f, 1);
            }
        }
    }
}
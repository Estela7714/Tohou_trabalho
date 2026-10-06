using UnityEngine;

public class BossPattern_Med1 : BossBaseBehaviour
{
    public float moveSpeed = 3f;
    public float fireRate = 0.5f;
    public float smoothSpeed = 5f; // Velocidade da transição de retorno
    private float fireTimer;
    private static float? baseAnchorY = null;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;

        // Regista o Y inicial central para não acumular desvios de posição
        if (bossTransform != null && baseAnchorY == null)
        {
            baseAnchorY = bossTransform.position.y;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Movimento em '8'
        float x = Mathf.Sin(Time.time * moveSpeed) * 1.5f;
        float y = Mathf.Sin(Time.time * moveSpeed * 2f) * 2.5f;

        float anchorY = baseAnchorY ?? bossTransform.position.y;
        Vector3 targetPos = new Vector3(bossTransform.position.x + x, anchorY + y, bossTransform.position.z);

        // Suaviza a posição em direção ao movimento em '8'
        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
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
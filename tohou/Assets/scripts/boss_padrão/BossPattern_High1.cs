using UnityEngine;

public class BossPattern_High1 : BossBaseBehaviour
{
    public float moveSpeed = 2f;
    public float moveAmount = 2.5f;
    public float fireRate = 0.8f;
    private float fireTimer;
    private float startY;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        if (bossTransform != null) startY = bossTransform.position.y;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Movimento suave para cima e para baixo
        float newY = startY + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        bossTransform.position = new Vector3(bossTransform.position.x, newY, 0);

        // Tiro Frontal Simples
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Projectile p = boss.bossBulletPool.Get();
            p.transform.position = boss.firePoint.position;
            p.transform.rotation = Quaternion.identity;
            p.Launch(Vector2.left, 8f, 1);
        }
    }
}
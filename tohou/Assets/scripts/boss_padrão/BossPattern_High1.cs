using UnityEngine;

public class BossPattern_High1 : BossBaseBehaviour
{
    public float moveSpeed = 2f;
    public float moveAmount = 2.5f;
    public float fireRate = 0.8f;

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

        // Calcula onde o Boss DEVERIA estar
        float targetY = startY + Mathf.Sin(stateTime * moveSpeed) * moveAmount;
        Vector3 targetPosition = new Vector3(bossTransform.position.x, targetY, 0);

        // Move o Boss de forma gradual e fluida até o alvo (elimina o salto instantâneo!)
        bossTransform.position = Vector3.MoveTowards(
            bossTransform.position,
            targetPosition,
            5f * Time.deltaTime
        );

        // Disparo Frontal
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
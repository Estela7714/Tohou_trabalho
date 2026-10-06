using UnityEngine;

public class BossPattern_High1 : BossBaseBehaviour
{
    public float moveSpeed = 2f;
    public float moveAmount = 2.5f;
    public float fireRate = 0.8f;

    private float fireTimer;
    private float stateTime;

    // Variável estática para memorizar o Y inicial absoluto do Boss na cena
    private static float? initialBossY = null;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        stateTime = 0f;

        // Regista o Y original apenas na primeira vez que entra no estado
        if (bossTransform != null && initialBossY == null)
        {
            initialBossY = bossTransform.position.y;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        stateTime += Time.deltaTime;

        // Calcula a posição Y em relação ao centro original fixo
        float targetY = (initialBossY ?? bossTransform.position.y) + (Mathf.Sin(stateTime * moveSpeed) * moveAmount);

        // Suaviza o movimento vindo de outros estados sem acumular posições
        float smoothedY = Mathf.MoveTowards(bossTransform.position.y, targetY, 5f * Time.deltaTime);
        bossTransform.position = new Vector3(bossTransform.position.x, smoothedY, bossTransform.position.z);

        // Disparo
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
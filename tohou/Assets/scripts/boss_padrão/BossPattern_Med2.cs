using UnityEngine;

public class BossPattern_Med2 : BossBaseBehaviour
{
    public float moveSpeed = 3f;
    public float fireRate = 0.6f;

    private float fireTimer;
    private float moveTimer;
    private bool movingLeft = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        moveTimer = 0f;
        movingLeft = true;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Alterna a direção de movimento a cada 1.5 segundos (evita ficar preso na parede)
        moveTimer += Time.deltaTime;
        if (moveTimer >= 1.5f)
        {
            moveTimer = 0f;
            movingLeft = !movingLeft;
        }

        Vector3 moveDir = movingLeft ? Vector3.left : Vector3.right;
        bossTransform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);

        // Tiro direcionado diretamente ao Jogador
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            GameObject player = GameObject.FindWithTag("Player");

            // Garante fallback caso o firePoint não esteja atribuído no Inspector
            Transform launchPoint = (boss.firePoint != null) ? boss.firePoint : bossTransform;

            if (player != null)
            {
                // Calcula a direção em direção ao jogador
                Vector2 dir = (player.transform.position - launchPoint.position).normalized;

                // Rotação visual para a bala apontar para o jogador
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                Projectile p = boss.bossBulletPool.Get();
                p.transform.position = launchPoint.position;
                p.transform.rotation = Quaternion.Euler(0, 0, angle);

                p.Launch(dir, 12f, 1);
            }
        }
    }
}
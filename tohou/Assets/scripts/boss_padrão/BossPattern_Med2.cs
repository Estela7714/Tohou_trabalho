using UnityEngine;

public class BossPattern_Med2 : BossBaseBehaviour
{
    public float moveSpeed = 4f;
    public float fireRate = 0.6f;
    private float fireTimer;
    private Vector3 startPos;
    private bool movingForward = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fireTimer = 0f;
        if (bossTransform != null) startPos = bossTransform.position;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null || boss.isInvulnerable) return;

        // Movimento de avanço e recuo no eixo X
        if (movingForward)
        {
            bossTransform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (bossTransform.position.x <= startPos.x - 3f) movingForward = false;
        }
        else
        {
            bossTransform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (bossTransform.position.x >= startPos.x) movingForward = true;
        }

        // Tiro direcionado diretamente à posição atual do jogador
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Vector2 dir = (player.transform.position - boss.firePoint.position).normalized;
                Projectile p = boss.bossBulletPool.Get();
                p.transform.position = boss.firePoint.position;
                p.Launch(dir, 12f, 1);
            }
        }
    }
}
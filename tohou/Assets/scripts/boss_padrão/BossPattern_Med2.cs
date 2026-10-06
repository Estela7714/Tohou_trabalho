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

        // Avanço e recuo com interpolação de limites
        if (movingForward)
        {
            Vector3 target = new Vector3(startPos.x - 3f, bossTransform.position.y, 0);
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, target, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(bossTransform.position, target) < 0.1f) movingForward = false;
        }
        else
        {
            Vector3 target = new Vector3(startPos.x, bossTransform.position.y, 0);
            bossTransform.position = Vector3.MoveTowards(bossTransform.position, target, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(bossTransform.position, target) < 0.1f) movingForward = true;
        }

        // Tiro direcionado
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
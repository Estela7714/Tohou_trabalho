using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;

    // Referência para a pool nativa do Unity
    private IObjectPool<Projectile> originPool;

    // Trava para evitar devolver o mesmo tiro duas vezes para a pool
    private bool isDeactivated = false;

    public void SetPool(IObjectPool<Projectile> pool)
    {
        originPool = pool;
    }

    public void Launch(Vector2 dir, float spd, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        isDeactivated = false; // Reseta a trava ao reativar da pool
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (isDeactivated) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Se sair dos limites do ecrã, devolve à pool
        if (Mathf.Abs(transform.position.x) > 15f || Mathf.Abs(transform.position.y) > 10f)
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDeactivated) return;

        // Se for uma bala disparada pelo Jogador e atingir o Boss
        if (CompareTag("PlayerBullet") && collision.CompareTag("Boss"))
        {
            BossController boss = collision.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Deactivate();
        }
        // Se for uma bala disparada pelo Boss e atingir o Jogador
        else if (CompareTag("BossBullet") && collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Deactivate();
        }
    }

    private void Deactivate()
    {
        // Garante que só devolve à pool se ainda não tiver sido desativado neste ciclo
        if (isDeactivated) return;
        isDeactivated = true;

        if (originPool != null)
        {
            originPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
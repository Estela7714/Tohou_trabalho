using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private IObjectPool<Projectile> pool;

    public void SetPool(IObjectPool<Projectile> poolRef)
    {
        pool = poolRef;
    }

    public void Launch(Vector2 dir, float spd, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Desativa/Retorna à pool se sair da tela
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f)
        {
            ReleaseToPool();
        }
    }

    public void ReleaseToPool()
    {
        if (gameObject.activeSelf)
        {
            pool?.Release(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tratar colisões com Player/Boss
        if (collision.CompareTag("Boss") && CompareTag("PlayerBullet"))
        {
            collision.GetComponent<BossController>()?.TakeDamage(damage);
            ReleaseToPool();
        }
        else if (collision.CompareTag("Player") && CompareTag("BossBullet"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(damage);
            ReleaseToPool();
        }
    }
}
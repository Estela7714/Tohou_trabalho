using UnityEngine;
using UnityEngine.Pool;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 8f;

    [Header("Tiro e Pool")]
    public Projectile bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.25f; // Entre 0.2s e 1s
    private float nextFireTime;

    [Header("Status")]
    public int maxHealth = 3;
    private int currentHealth;

    private IObjectPool<Projectile> bulletPool;

    private void Awake()
    {
        // Configuração da ObjectPool nativa da Unity
        bulletPool = new ObjectPool<Projectile>(
            createFunc: () => {
                Projectile p = Instantiate(bulletPrefab);
                p.SetPool(bulletPool);
                p.tag = "PlayerBullet";
                return p;
            },
            actionOnGet: (p) => p.gameObject.SetActive(true),
            actionOnRelease: (p) => p.gameObject.SetActive(false),
            actionOnDestroy: (p) => Destroy(p.gameObject),
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance?.UpdatePlayerUI(currentHealth);
    }

    private void Update()
    {
        Move();
        HandleShooting();
        ClampPosition(); // Restringe a posição
    }

    private void ClampPosition()
    {
        // Converte a posição atual do jogador para coordenadas de Viewport (0.0 até 1.0)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // Limita o X e Y entre 5% e 95% do ecrã para a navinha não encostar completamente na borda
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.05f, 0.95f);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.05f, 0.95f);

        // Converte de volta para coordenadas do mundo
        transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
    }

    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(moveX, moveY, 0).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Restringir a navinha dentro dos limites da Câmera
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.05f, 0.95f);
        pos.y = Mathf.Clamp(pos.y, 0.05f, 0.95f);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    private void HandleShooting()
    {
        // Atira no clique inicial OU mantendo pressionado dentro do intervalo
        if (Input.GetButtonDown("Fire1") || (Input.GetButton("Fire1") && Time.time >= nextFireTime))
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        Projectile bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.Launch(Vector2.right, 15f, 1);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameManager.Instance?.UpdatePlayerUI(currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // Chama a tela de Game Over no GameManager (Derrota)
            GameManager.Instance?.GameOver(false);

            // Desativa o jogador para ele sumir e não continuar a atirar/mover
            gameObject.SetActive(false);
        }
    }
}
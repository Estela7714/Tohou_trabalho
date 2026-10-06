using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Atributos")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public bool isInvulnerable = false;

    [Header("Projéteis do Chefão")]
    public Projectile bossBulletPrefab;
    public Transform firePoint;
    public IObjectPool<Projectile> bossBulletPool;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Fases de Vida
    public enum Phase { High, Medium, Low }
    public Phase currentPhase = Phase.High;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        bossBulletPool = new ObjectPool<Projectile>(
            createFunc: () => {
                Projectile p = Instantiate(bossBulletPrefab);
                p.SetPool(bossBulletPool);
                p.tag = "BossBullet";
                return p;
            },
            actionOnGet: (p) => p.gameObject.SetActive(true),
            actionOnRelease: (p) => p.gameObject.SetActive(false),
            actionOnDestroy: (p) => Destroy(p.gameObject),
            defaultCapacity: 50,
            maxSize: 300
        );
    }

    private void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance?.UpdateBossUI(currentHealth, maxHealth, currentPhase);
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        GameManager.Instance?.UpdateBossUI(currentHealth, maxHealth, currentPhase);

        CheckPhaseTransition();

        if (currentHealth <= 0)
        {
            GameManager.Instance?.GameOver(true); // Vitória
            Destroy(gameObject);
        }
    }

    private void CheckPhaseTransition()
    {
        float healthPercent = currentHealth / maxHealth;

        if (currentPhase == Phase.High && healthPercent <= 0.5f)
        {
            StartCoroutine(PhaseTransitionRoutine(Phase.Medium, "ToPhaseMedium"));
        }
        else if (currentPhase == Phase.Medium && healthPercent <= 0.2f)
        {
            StartCoroutine(PhaseTransitionRoutine(Phase.Low, "ToPhaseLow"));
        }
    }

    private IEnumerator PhaseTransitionRoutine(Phase newPhase, string triggerName)
    {
        isInvulnerable = true;
        currentPhase = newPhase;

        // Dispara a transição de fase no Animator
        animator.SetTrigger(triggerName);

        // Efeito de piscar suave durante os 2 segundos de invulnerabilidade
        float elapsed = 0f;
        float duration = 2.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Faz a transparência (Alpha) oscilar suavemente entre 0.3 e 1.0
            float alpha = Mathf.PingPong(elapsed * 5f, 0.7f) + 0.3f;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        // Restaura a opacidade original e remove invulnerabilidade
        spriteRenderer.color = Color.white;
        isInvulnerable = false;
    }
    private void LateUpdate()
    {
        ClampBossPosition();
    }

    private void ClampBossPosition()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // Limites do Boss: mantém-no, por exemplo, na metade direita do ecrã (X entre 0.55 e 0.90) 
        // e com margem em Y (0.10 a 0.90)
        viewportPos.x = Mathf.Clamp(viewportPos.x, 0.55f, 0.90f);
        viewportPos.y = Mathf.Clamp(viewportPos.y, 0.10f, 0.90f);

        transform.position = Camera.main.ViewportToWorldPoint(viewportPos);
    }
}
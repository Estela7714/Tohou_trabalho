using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton para acesso global fácil
    public static GameManager Instance { get; private set; }

    [Header("UI do Jogador (3 Vidas)")]
    [Tooltip("Arraste aqui as 3 imagens/ícones que representam as vidas da navinha")]
    public GameObject[] playerLifeIcons;

    [Header("UI do Chefão")]
    [Tooltip("Arraste aqui a Slider da barra de vida do chefe")]
    public Slider bossHealthSlider;
    [Tooltip("Arraste aqui o componente Image do Fill da barra de vida")]
    public Image bossHealthFill;

    [Header("Cores da Barra do Chefão por Porcentagem")]
    public Color phaseHighColor = Color.green;   // Vida > 50%
    public Color phaseMediumColor = Color.yellow; // 50% >= Vida >= 20%
    public Color phaseLowColor = Color.red;      // Vida < 20%

    [Header("Telas de Fim de Jogo")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    // Controla se a partida já terminou para escutar a tecla R
    private bool isGameOver = false;

    private void Awake()
    {
        // Configuração do Singleton com persistência entre cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garante que o GameManager persista nas transições
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamado automaticamente sempre que uma nova cena for carregada
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGameState();
    }

    private void Start()
    {
        ResetGameState();
    }

    /// <summary>
    /// Reseta o estado do jogo e as telas de UI ao reiniciar/carregar a cena.
    /// </summary>
    private void ResetGameState()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        isGameOver = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Se o jogo acabou (Vitória ou Derrota) e o jogador pressionou 'R', recarrega a partida
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    /// <summary>
    /// Atualiza os ícones de vida da navinha na UI.
    /// Desativa os ícones conforme o jogador toma dano.
    /// </summary>
    public void UpdatePlayerUI(int currentHealth)
    {
        if (playerLifeIcons == null) return;

        for (int i = 0; i < playerLifeIcons.Length; i++)
        {
            if (playerLifeIcons[i] != null)
            {
                // Deixa visível apenas o número de ícones equivalente à vida atual
                playerLifeIcons[i].SetActive(i < currentHealth);
            }
        }
    }

    /// <summary>
    /// Atualiza o valor e a cor da barra de vida do Chefão em tempo real.
    /// </summary>
    public void UpdateBossUI(float currentHealth, float maxHealth, BossController.Phase phase)
    {
        if (maxHealth <= 0) return;

        // Calcula a porcentagem normalizada da vida (0.0 a 1.0)
        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);

        // 1. Atualiza o valor da Slider
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = healthPercent;
        }

        // 2. Altera a cor com base na porcentagem de vida exata
        if (bossHealthFill != null)
        {
            if (healthPercent > 0.5f)
            {
                bossHealthFill.color = phaseHighColor; // Verde (> 50%)
            }
            else if (healthPercent >= 0.2f)
            {
                bossHealthFill.color = phaseMediumColor; // Amarelo (50% a 20%)
            }
            else
            {
                bossHealthFill.color = phaseLowColor; // Vermelho (< 20%)
            }
        }
    }

    /// <summary>
    /// Exibe a tela de Vitória ou Derrota e pausa o jogo.
    /// </summary>
    public void GameOver(bool isVictory)
    {
        isGameOver = true;

        if (isVictory)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
        }
        else
        {
            if (defeatPanel != null) defeatPanel.SetActive(true);
        }

        // Congela o tempo do jogo (para movimentos, tiros e física)
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Método chamado pelo botão "Tentar Novamente" na UI ou via atalho da tecla 'R'.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        // Recarrega a cena do jogo
        SceneManager.LoadScene("Game");
    }
}
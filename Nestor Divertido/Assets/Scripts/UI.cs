using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    public Image fadePlane;
    public Text scoreUI;
    public RectTransform healthBar;

    public Text ClipCountUI;
    public Text AmmoCountUI;
    public Text HPCountUI;

    Spawner spawner;
    Player player;

    public event System.Action OnChangeHPValue;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        Debug.Log(player);
        player.OnChangeLPValue += this.OnUpdateHPValue;
    }

    void Awake ()
    {
     
    }

    private void Update()
    {
        scoreUI.text = Score.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.life / player.startingLife;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnUpdateHPValue(int ammountToSetHP)
    {
        HPCountUI.text = ammountToSetHP.ToString("D2");
        //Debug.Log("sdfsf");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 2f;
        float animatePercent = 0;
        int direction = 1;

        float endDelayTime = Time.time + Time.time + 1 / speed + delayTime;

        while( animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * direction;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }
            yield return null;
        }
    }

    public void StartNewGame() {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("Menu");
    }
}
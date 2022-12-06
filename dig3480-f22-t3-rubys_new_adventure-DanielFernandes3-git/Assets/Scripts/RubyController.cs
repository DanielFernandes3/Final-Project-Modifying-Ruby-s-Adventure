using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public GameObject visitText;
    public TextMeshProUGUI cogText;
    public static int score;
    public static int level;
    int cog;
    bool gameOver;

    public GameObject backgroundMusic;
    public GameObject winMusic;
    public GameObject loseMusic;

    public GameObject projectilePrefab;

    public AudioSource musicSource;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip speakingClip;
    public AudioClip ammoClip;
    public AudioClip shadeTalk;

    public ParticleSystem healthDecrease;
    public ParticleSystem healthIncrease;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    Timer timerScript;

    // Start is called before the first frame update
    void Start()
    {

        gameOver = false;

        cog = 4;
        SetAmmoText();

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        SetCountText();
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        winMusic.SetActive(false);
        loseMusic.SetActive(false);
        visitText.SetActive(false);
    }

    public void SetCountText()
    {
        countText.text = "Robots Fixed: " + score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cog >= 1)
            {
                Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (hit.collider != null)
                {
                    if (EnemyController.score == 4)
                    {
                        SceneManager.LoadScene("Scene 2");
                        level += 1;
                        visitText.SetActive(false);
                        EnemyController.score = 0;
                    }

                    ShadeController character2 = hit.collider.GetComponent<ShadeController>();

                    if (character2 != null)
                    {
                        character2.DisplayDialog();
                        PlaySound(shadeTalk);
                    }          

                    NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                    if (character != null)
                    {
                        character.DisplayDialog();
                        PlaySound(speakingClip);
                    }
                }
            }
        }

        if (level != 1)
        {
            if (EnemyController.score == 4)
            {
                visitText.SetActive(true);
            }
        }

        if (level == 1)
        {
            if (EnemyController.score == 4)
            {
                backgroundMusic.SetActive(false);
                winTextObject.SetActive(true);
                winMusic.SetActive(true);
                gameOver = true;
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }



        timerScript = GameObject.FindGameObjectWithTag("time").GetComponent<Timer>();
        if(timerScript.currentTime == 60)
        {
            loseTextObject.SetActive(true);
            speed = 0.0f;
            gameOver = true;
        }

        if (currentHealth == 0)
        {
            loseTextObject.SetActive(true);
            speed = 0.0f;
            gameOver = true;
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Cogss"))
        {
            cog += 4;
            SetAmmoText();
            other.gameObject.SetActive(false);

            PlaySound(ammoClip);
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            healthDecrease.Play();
            Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);

            PlaySound(hitSound);
        }

        if (amount > 0)
        {
            Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            backgroundMusic.SetActive(false);
        }

        if (currentHealth == 0)
        {
            loseMusic.SetActive(true);
        }
    }

    void Launch()
    {
        cog = cog - 1;
        SetAmmoText();

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");

            PlaySound(throwSound);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void SetAmmoText()
    {
        cogText.text = "Cogs: " + cog.ToString();
    }
}
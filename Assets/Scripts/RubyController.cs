using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public GameObject projectilePrefab;

    public ParticleSystem healthEffectPrefab;
    public ParticleSystem hitEffectPrefab;
    
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; }}
    int currentHealth;

    int score;
    public Text scoreText;
    public Text winText;
    public Text restartText;
    public Text cogCountText;

    bool gameOver = false;
    bool gameWin = false;
    bool gameLose = false;

    public static int level;

    int cogcount;

    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    AudioSource audioSource;

    public AudioClip rubyCogClip;
    public AudioClip rubyHitClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    public GameObject backgroundmusic;




    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        score = 0;
        scoreText.text = "Fixed Robots: " + score.ToString();
        winText.text = "";
        restartText.text = "";
        cogcount = 4;

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");  

        Vector2 move = new Vector2(horizontal, vertical); 

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
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
            {
                isInvincible = false;
            }
        } 

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogcount > 0)
            {
                Launch();
            }
            
        }
        cogCountText.text = "Cogs: " + cogcount.ToString();

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    if(score == 4)
                    {
                        level = 1;
                        SceneManager.LoadScene("MainScene1");
                        
                    }
                }
            }
        }

        if (level == 0)
        {
            if(score == 4)
            {
                winText.text = "Talk with Jambi to visit stage two!";
            }
            
        }


        if (level == 1)
        {
            if (score == 4)
            {
                gameWin = true;
                winText.text = "You Win! Game created by Matthew Reuter.";
                restartText.text = "Press R to restart or ESC to close the game";
                gameOver = true;
            }
        }

        if (currentHealth == 0)
        {
            gameLose = true;
            gameOver = true;
            isInvincible = true;
        }
        
        if (gameOver == true)
        {
            backgroundmusic.SetActive(false);
            
        }

        if (gameLose == true)
        {
            winText.text = "You Lose! Game created by Matthew Reuter.";
            restartText.text = "Press R to restart or ESC to close the game";
            speed = 0;
            gameOver = true;
            
        }

        if (gameWin == true)
        {
            winText.text = "You Win! Game created by Matthew Reuter.";
            restartText.text = "Press R to restart or ESC to close the game";
            gameOver = true;
            
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);    
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("CogAmmo"))
        {
            other.gameObject.SetActive(false);
            cogcount = cogcount + 4;
        }
    }


    void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);

    }

    public void ChangeHealth (int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            invincibleTimer = timeInvincible;

            audioSource.PlayOneShot(rubyHitClip);
            
            Instantiate(hitEffectPrefab, rigidbody2d.position + Vector2.up * 0.7f, Quaternion.identity);

        }
        if ( amount > 0)
        {
            Instantiate(healthEffectPrefab, rigidbody2d.position + Vector2.up * 0.7f, Quaternion.identity);   
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if(currentHealth == 0)
        {
            audioSource.clip = loseClip;
            audioSource.Play();
            audioSource.loop = true;
            
        }
        
        
    }

    public void ChangeScore (int scoreAmount)
    {
        if (scoreAmount > 0)
        {
            score = score + 1;
            scoreText.text = "Fixed Robots: " + score.ToString();
            
            
        }

        if(level == 1)
        {
            if (score == 4)
            {
                audioSource.clip = winClip;
                audioSource.Play();
                audioSource.loop = true;
                
            }
        }

        
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(rubyCogClip);

        cogcount = cogcount - 1;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }


}

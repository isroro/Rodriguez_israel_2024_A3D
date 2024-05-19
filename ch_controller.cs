using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ch_controller : MonoBehaviour
{
    public Animator anim;
    public int velocity = 6;
    public float verticalVel = 3;
    int num = 0;
    int estadoPalanca = 0;
    public int bulletDamage = 10;
    public float cadence = 1f;
    float scattering = 2;
    public float scatter;
    public Sprite[] gunSprite;
    public BoxCollider2D up;
    public BoxCollider2D down;
    public Animator lever;
    public Animator platform;
    public bala scriptbala;
    public BoxCollider2D melee;
    public GameObject sprite;
    //public GameObject sprite2;
    public GameObject shotRef;
    public GameObject bullet;
    public GameObject bigBullet;
    public PlayerInput input;
    public Rigidbody2D rb;
    //public SpriteRenderer ch;
    public Image actualWeapon;
    public SpriteRenderer attackArea;
    public float velMultiplier = 0;
    float velocidadGiroSmooth = 0.2f;
    float giroSmooth = 0.12f;
    float mov;
    bool attacking = false;
    bool turn = false;
    bool lastDirection = false;
    bool dontShoot = false;
    bool dontMove = false;
    bool crouching = false;
    bool shooting = false;
    public bool halfWay = false;
    public bool touchingGrass = true;
    public bool touchingRoof = false;
    bool multiplaying = false;
    public bool interacting;
    bool burst = false;
    bool bigBulletMod = false;
    bool multishoot = false;
    bool bigBurst = false;
    bool revolver = true;
    bool highJump = false;
    public audioManager manager;
    /*
    public AudioSource fusilSound;
    public AudioSource revolverSound;
    public AudioSource roquetSound;
    public AudioSource shotgunSound;
    public AudioSource tommySound;

    public AudioSource interactSound;

    public AudioSource jumpSound;
    public AudioSource fallSound;
    */

    public string guardarValor;

    GameObject box;
    public bool touchBox;
    public bool touchLever;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<audioManager>();
        actualWeapon.sprite = gunSprite[0];
        cadence = Gun_Mod.P_cadence;
        bulletDamage = Gun_Mod.P_damage;
        scattering = Gun_Mod.P_scattering;
        multishoot = false;
        burst = false;
        bigBulletMod = false;
        bigBurst = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("floor"))
        {
            touchingGrass = true;
            anim.SetBool("floor", true);
        }
        if (other.CompareTag("roof"))
        {
            touchingRoof = true;
        }
        //Debug.Log("funciono");
        if (other.CompareTag("gun"))
        {
            touchBox = true;
            box = other.gameObject;
            guardarValor = box.GetComponent<gunsMods>().G_name;
        }
        if (other.CompareTag("palanca"))
        {
            gameObject.transform.SetParent(other.transform);
            touchLever = true;
            platform = other.transform.parent.GetComponent<Animator>();
            lever = other.GetComponent<Animator>();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("floor"))
        {
            touchingGrass = false;
            anim.SetBool("floor", false);
        }
        if (other.CompareTag("roof"))
        {
            touchingRoof = false;
        }
        if (other.CompareTag("gun"))
        {
            box = null;
            //guardarValor = null;
            touchBox = false;
        }
        if (other.CompareTag("palanca"))
        {
            gameObject.transform.SetParent(null);
            touchLever = false;
            platform = null;
            lever = null;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        anim.SetFloat("vertical", rb.velocity.y);
    }
    void Update()
    {
        if (input.actions["exit"].triggered)
        {
            Application.Quit();
        }
        if (touchingRoof && crouching)
        {
            halfWay = true;
        }
        else
        {
            halfWay = false;
        }
        if (multiplaying == false && velMultiplier <= 0.95)
        {
            StartCoroutine(multiply());
        }
        mov = input.actions["movement"].ReadValue<float>();
        if (mov < 0 && !dontMove)
        {
            anim.SetBool("move", true);
            if (lastDirection == true)
            {
                dontShoot = true;
            }
            lastDirection = false;
            StartCoroutine(fullRotation());
            if (!turn)
            {
                velMultiplier = 0.3f;
            }
            turn = true;
            //float angulo = Mathf.SmoothDampAngle(sprite.transform.eulerAngles.y, 180, ref velocidadGiroSmooth, giroSmooth);
            //Debug.Log(angulo);
            //transform.rotation = Quaternion.Euler(0f, angulo, 0f);
            //ch.flipX = true;
        }
        if (mov > 0 && !dontMove)
        {
            anim.SetBool("move", true);
            if (lastDirection == false)
            {
                dontShoot = true;
            }
            lastDirection = true;
            StartCoroutine(fullRotation());
            if (turn)
            {
                velMultiplier = 0.3f;
            }
            turn = false;
            //float angulo = Mathf.SmoothDampAngle(sprite.transform.eulerAngles.y, 0, ref velocidadGiroSmooth, giroSmooth);
            //transform.rotation = Quaternion.Euler(0f, angulo, 0f);
            //ch.flipX = false;
        }
        if (turn)
        {
            float angulo = Mathf.SmoothDampAngle(sprite.transform.eulerAngles.y, 180, ref velocidadGiroSmooth, giroSmooth);
            //Debug.Log(angulo);
            sprite.transform.rotation = Quaternion.Euler(0f, angulo, 0f);
            //sprite2.transform.rotation = Quaternion.Euler(0f, angulo, 0f);
        }
        else
        {
            float angulo = Mathf.SmoothDampAngle(sprite.transform.eulerAngles.y, 0, ref velocidadGiroSmooth, giroSmooth);
            //Debug.Log(angulo);
            sprite.transform.rotation = Quaternion.Euler(0f, angulo, 0f);
            //sprite2.transform.rotation = Quaternion.Euler(0f, angulo, 0f);

        }
        if (input.actions["run"].IsPressed() && !halfWay && !interacting && !shooting && !attacking)
        {
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    down.enabled = false;
                    up.enabled = true;
                }
            }
            anim.SetBool("running", true);
            velocity = 8;
            if (mov != 0)
            {
                verticalVel = 3.25f;
            }
            else
            {
                verticalVel = 3;
            }
            
        }
        else
        {
            anim.SetBool("running", false);
            if (crouching != true)
            {
                velocity = 6;
            }
            verticalVel = 3.2f;
        }

        if (input.actions["jump"].triggered && touchingGrass && !interacting && !touchingRoof && !shooting && !attacking)
        {
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    //anim.SetBool("crouching", false);
                    //anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    down.enabled = false;
                    up.enabled = true;
                    highJump = true;
                }
            }
            
            anim.SetBool("jump", true);
        }
        if (input.actions["attack"].triggered && touchingGrass && !interacting && !attacking && !halfWay && !shooting)
        {
            dontMove = true;
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    //anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    down.enabled = false;
                    up.enabled = true;
                }
            }
            StartCoroutine(attack());
        }

        if (input.actions["shoot"].IsPressed() && touchingGrass && !interacting && !shooting && !dontShoot && !halfWay && !attacking)
        {
            dontMove = true;
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    //anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    down.enabled = false;
                    up.enabled = true;
                }
            }
            StartCoroutine(shoot());
            //Instantiate(bala, refDisparo.transform.position, Quaternion.identity);
        }

        if (input.actions["crouch"].triggered && touchingGrass && !interacting && !halfWay && !shooting && !attacking)
        {
            num++;
            if(num % 2 == 0) // up
            {
                crouching = false;
                anim.SetBool("crouching", false);
                //sprite2.SetActive(false);
                sprite.SetActive(true);
                down.enabled = false;
                up.enabled = true;
            }
            if (num % 2 == 1) // down
            {
                crouching = true;
                anim.SetBool("crouching", true);
                //sprite.SetActive(false);
                //sprite2.SetActive(true);
                up.enabled = false;
                down.enabled = true;
                velocity = 4;
            }
            
        }
        if (input.actions["action"].triggered && touchLever && !interacting && touchingGrass && !shooting && !attacking)
        {
            dontMove = true;
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    //anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    
                    down.enabled = false;
                    up.enabled = true;
                    
                }
            }
            StartCoroutine(leverInteract());

        }
        if (input.actions["action"].triggered && touchBox && !interacting && !shooting && !attacking)
        {

            dontMove = true;
            
            if (crouching == true)
            {
                num++;
                if (num % 2 == 0)
                {
                    crouching = false;
                    //anim.SetBool("crouching", false);
                    //sprite2.SetActive(false);
                    sprite.SetActive(true);
                    
                    down.enabled = false;
                    up.enabled = true;
                    
                }
            }
            StartCoroutine(boxInteract());
        }



        //Debug.Log(mov);
        if (!dontMove)
        {
            Vector2 movement = new Vector2(mov * velMultiplier * velocity, rb.velocity.y);
            rb.velocity = movement;
            if (rb.velocity != Vector2.zero)
            {
                //anim.SetBool("move", true);
            }
            else
            {
                anim.SetBool("move", false);
            }
        }
        else
        {
            anim.SetBool("move", false);
            rb.velocity = Vector2.zero;
        }
        //Debug.Log(rb.velocity);
    }

    /*
    public void changeHitBoxIntereact()
    {
        down.enabled = false;
        up.enabled = true;
    }
    */

    public void canMove()
    {
        dontMove = false;
        interacting = false;
    }

    public void jumpStart()
    {
        if (highJump)
        {
            anim.SetBool("crouching", false);
            verticalVel = 3.75f;
            highJump = false;
        }
        rb.AddForce(new Vector2(0, 3) * verticalVel, ForceMode2D.Impulse);
        verticalVel = 3;
    }

    public void canMove1()
    {
        dontMove = false;
    }

    public void deactivateJump()
    {
        anim.SetBool("jump", false);
    }

    public void deactivatecrouch()
    {
        if (crouching)
        {
            num++;
            if (num % 2 == 0) // up
            {
                crouching = false;
                anim.SetBool("crouching", false);
                //sprite2.SetActive(false);
                sprite.SetActive(true);
                down.enabled = false;
                up.enabled = true;
            }
        }
    }

    public void deactivate()
    {
        Debug.Log("entro en desactivar");
        if (box != null)
        {
            manager.interactSound.Play();
            box.SetActive(false);
        }
    }
    public void changeState()
    {
        if (platform != null && lever != null)
        {
            estadoPalanca++;
            if (estadoPalanca % 2 == 0) // up
            {
                platform.SetBool("mov", false);
                lever.SetBool("active", false);
            }
            if (estadoPalanca % 2 == 1) // down
            {
                platform.SetBool("mov", true);
                lever.SetBool("active", true);
            }
        }
    }


    IEnumerator multiply() 
    {
        multiplaying = true;
        if (velMultiplier <= 1)
        {
            yield return new WaitForSeconds(0.1f);
            velMultiplier += 0.1f;
        }
        multiplaying = false;
    }
    IEnumerator shoot()
    {
        shooting = true;
        if (multishoot)
        {
            manager.shotgunSound.Play();
            anim.SetBool("shotgun", true);
            int g = Random.Range(3,6);
            for (int i = 0; i <= g;i++)
            {
                
                GameObject bulletMod = Instantiate(bullet, shotRef.transform.position, Quaternion.identity);
                scatter = Random.Range(-scattering, scattering);
                bulletMod.transform.rotation = Quaternion.Euler(0f, 0f, scatter);
                scriptbala = bulletMod.GetComponent<bala>();
                dontMove = true;
                if (turn)
                {
                    scriptbala.vel = -scriptbala.vel;
                }
                scriptbala = null;
                yield return new WaitForSeconds(0.025f);
            }
            anim.SetBool("crouching", false);
            anim.SetBool("shotgun", false);
        }
        if (burst)
        {
            manager.fusilSound.Play();
            anim.SetBool("fusil", true);
            for (int i = 0; i <= 2; i++)
            {
                GameObject bulletMod = Instantiate(bullet, shotRef.transform.position, Quaternion.identity);
                scatter = Random.Range(-scattering, scattering);
                bulletMod.transform.rotation = Quaternion.Euler(0f, 0f, scatter);
                scriptbala = bulletMod.GetComponent<bala>();
                dontMove = true;
                if (turn)
                {
                    scriptbala.vel = -scriptbala.vel;
                }
                scriptbala = null;
                yield return new WaitForSeconds(0.2f);
            }
            anim.SetBool("crouching", false);
            anim.SetBool("fusil", false);
        }
        if (bigBurst)
        {
            manager.tommySound.Play();
            anim.SetBool("tommy", true);
            for (int i = 0; i <= 7; i++)
            {
                GameObject bulletMod = Instantiate(bullet, shotRef.transform.position, Quaternion.identity);
                scatter = Random.Range(-scattering, scattering);
                bulletMod.transform.rotation = Quaternion.Euler(0f, 0f, scatter);
                scriptbala = bulletMod.GetComponent<bala>();
                dontMove = true;
                if (turn)
                {
                    scriptbala.vel = -scriptbala.vel;
                }
                scriptbala = null;
                yield return new WaitForSeconds(0.1f);
            }
            anim.SetBool("crouching", false);
            anim.SetBool("tommy", false);
        }

        if (bigBulletMod)
        {
            manager.roquetSound.Play();
            anim.SetBool("roquet", true);
            for (int i = 0; i <= 2; i++)
            {
                GameObject bulletMod = Instantiate(bigBullet, shotRef.transform.position, Quaternion.identity);
                scatter = Random.Range(-scattering, scattering);
                bulletMod.transform.rotation = Quaternion.Euler(0f, 0f, scatter);
                scriptbala = bulletMod.GetComponent<bala>();
                dontMove = true;
                if (turn)
                {
                    scriptbala.vel = -scriptbala.vel;
                }
                scriptbala = null;
            }
            yield return new WaitForSeconds(0.5f);
            anim.SetBool("crouching", false);
            anim.SetBool("roquet", false);
        }
        
        if (revolver)
        {
            manager.revolverSound.Play();
            //Debug.Log("revolver");
            anim.SetBool("revolver", true);
            GameObject bulletMod = Instantiate(bullet, shotRef.transform.position, Quaternion.identity);
            scatter = Random.Range(-scattering, scattering);
            bulletMod.transform.rotation = Quaternion.Euler(0f, 0f, scatter);
            scriptbala = bulletMod.GetComponent<bala>();
            dontMove = true;
            if (turn)
            {
                scriptbala.vel = -scriptbala.vel;
            }
            scriptbala = null;
            yield return new WaitForSeconds(0.5f);
            anim.SetBool("crouching", false);
            anim.SetBool("revolver", false);
        }
        //dontMove = false;
        yield return new WaitForSeconds(cadence);
        shooting = false;
        
    }
    IEnumerator fullRotation()
    {
        yield return new WaitForSeconds(cadence * 1.2f);
        dontShoot = false;
        
    }
    IEnumerator leverInteract()
    {
        interacting = true;
        anim.SetBool("interact", true);
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("crouching", false);
        anim.SetBool("interact", false);
    }

    IEnumerator boxInteract()
    {
        interacting = true;
        anim.SetBool("interact", true);
        Debug.Log("entro corrutina");
        switch (/*box.GetComponent<gunsMods>().G_name*/ guardarValor)
        {

            case "shotgun":
                actualWeapon.sprite = gunSprite[4];
                //actualWeapon.sprite = Instantiate(Resources.Load<Sprite>(@"images/guns/shotgun"));
                //actualWeapon.color = new Color(0,0,0,1);
                cadence = Gun_Mod.S_cadence;
                bulletDamage = Gun_Mod.S_damage;
                scattering = Gun_Mod.S_scattering;
                bigBulletMod = false;
                multishoot = true;
                burst = false;
                bigBurst = false;
                revolver = false;
                break;

            case "fusile":
                actualWeapon.sprite = gunSprite[3];

                cadence = Gun_Mod.F_cadence;
                bulletDamage = Gun_Mod.F_damage;
                scattering = Gun_Mod.F_scattering;
                bigBulletMod = false;
                multishoot = false;
                burst = true;
                bigBurst = false;
                revolver = false;
                break;

            case "roquet":
                actualWeapon.sprite = gunSprite[2];
                cadence = Gun_Mod.R_cadence;
                bulletDamage = Gun_Mod.R_damage;
                scattering = Gun_Mod.R_scattering;
                bigBulletMod = true;
                multishoot = false;
                burst = false;
                bigBurst = false;
                revolver = false;
                break;

            case "tommy":
                actualWeapon.sprite = gunSprite[1];
                cadence = Gun_Mod.T_cadence;
                bulletDamage = Gun_Mod.T_damage;
                scattering = Gun_Mod.T_scattering;
                bigBulletMod = false;
                multishoot = false;
                burst = false;
                bigBurst = true;
                revolver = false;
                break;

            case "revolver":
                actualWeapon.sprite = gunSprite[0];
                cadence = Gun_Mod.P_cadence;
                bulletDamage = Gun_Mod.P_damage;
                scattering = Gun_Mod.P_scattering;
                bigBulletMod = false;
                multishoot = false;
                burst = false;
                bigBurst = false;
                revolver = true;
                break;
        }
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("crouching", false);
        anim.SetBool("interact", false);

    }
    IEnumerator attack()
    {
        anim.SetBool("melee", true);
        attacking = true;
        yield return new WaitForSeconds(0.6f);
        //manager.attackSound.Play();
        //attackArea.enabled = true;
        //melee.enabled = true;
        //yield return new WaitForSeconds(0.2f);
        //attackArea.enabled = false;
        //melee.enabled = false;
        anim.SetBool("crouching", false);
        anim.SetBool("melee", false);
        //dontMove = false;
        yield return new WaitForSeconds(0.5f);
        attacking = false;
        //dontMove = false;
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("funciono");
        if (collision.gameObject.CompareTag("gun"))
        {
            switch (collision.gameObject.GetComponent<gunsMods>().G_name)
            {
                case "shotgun":
                    cadence = Gun_Mod.S_cadence;
                    bulletDamage = Gun_Mod.S_damage;
                    break;

                case "fusile":
                    cadence = Gun_Mod.F_cadence;
                    bulletDamage = Gun_Mod.F_damage;
                    break;

                case "roquet":
                    cadence = Gun_Mod.R_cadence;
                    bulletDamage = Gun_Mod.R_damage;
                    break;

                case "tommy":
                    cadence = Gun_Mod.T_cadence;
                    bulletDamage = Gun_Mod.T_damage;
                    break;

                case "revolver":
                    cadence = Gun_Mod.P_cadence;
                    bulletDamage = Gun_Mod.P_damage;
                    break;
            }
            /*
            gunsMods gunMod = collision.gameObject.GetComponent<gunsMods>();
            cadence = gunMod.cadence;
            bulletDamage = gunMod.damage;
            
            collision.gameObject.gameObject.SetActive(false);
            

        }
    }
    */
    public void callSound()
    {
        manager.attackSound.Play();
    }
}

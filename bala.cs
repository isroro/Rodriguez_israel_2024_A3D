using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bala : MonoBehaviour
{
    public float vel = 8;
    public float damage;
    ch_controller ch;
    Rigidbody2D rb;
    public GameObject bulletBody;
    public float scattering;
    // Start is called before the first frame update
    void Start()
    {
        ch = FindObjectOfType<ch_controller>();
        damage = ch.bulletDamage;
        scattering = ch.scatter / 90f;
        if (vel < 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 180, 0f);
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(1, scattering) * vel, ForceMode2D.Impulse);
        StartCoroutine(disapear());
    }
    IEnumerator disapear() 
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class enemy : MonoBehaviour
{
    public float health = 100;
    public Transform spawn;
    public GameObject enemyObject;
    public GameObject enemyBody;
    public Image healthBar;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        healthBar.fillAmount = health / 100;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Instantiate(enemyObject,spawn.position,Quaternion.identity);
            enemyBody.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("melee"))
        {
            Debug.Log("me han pegao un navajaso");
            health -= 20;
            healthBar.fillAmount = health / 100;
        }
        if (other.CompareTag("bullet"))
        {
            Debug.Log("la paraste de pecho colorado");
            if (other.GetComponent<bala>().damage == 100)
            {
                FindObjectOfType<audioManager>().explosion.Play();
            }
            other.gameObject.SetActive(false);
            float d = other.GetComponent<bala>().damage;
            health -= d;
            healthBar.fillAmount = health / 100;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    public AudioSource fusilSound;
    public AudioSource revolverSound;
    public AudioSource roquetSound;
    public AudioSource shotgunSound;
    public AudioSource tommySound;
    public AudioSource attackSound;

    public AudioSource interactSound;

    public AudioSource jumpSound;
    public AudioSource fallSound;

    public AudioSource explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fusilSound.isPlaying)
        {
            Debug.Log("sueno");
        }
    }
    public void explode()
    {
        explosion.Play();
    }
}

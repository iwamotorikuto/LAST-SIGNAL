using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEScript : MonoBehaviour
{
    public AudioSource audioSource;
    public float interval = 1.5f;


    void Start()
    {
        StartCoroutine(LoopWithDelay());
    }
    IEnumerator LoopWithDelay()
    {
        while (true)
        {
            //指定した時間を待ってから再生
            yield return new WaitForSeconds(interval);
            audioSource.Play();
        }
    }
}


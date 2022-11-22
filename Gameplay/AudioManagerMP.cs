using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMP : MonoBehaviour
{

    public static bool IsPlayingSpeach(AudioSource[] speach) {

        bool flag = false;

        foreach (AudioSource audio in speach) {

            if (audio.isPlaying) {

                flag = true;
                break;

            }

        }

        return flag;

    }

    public static AudioSource GetRandomSpeach(AudioSource[] speach) {

        return speach[Random.Range(0, speach.Length)];

    }

    public GameObject audiosGo;

    AudioSource[] audios;

    AudioSource currentMusic;

    private void Start()
    {
        audios = audiosGo.GetComponents<AudioSource>();

        currentMusic = getRandomMusic();
        currentMusic.Play();
    }

    private void Update()
    {

        if (!currentMusic.isPlaying)
        {


            currentMusic = getRandomMusic();

            currentMusic.Play();

        }


        if (Input.GetKey(KeyCode.C))
        {

            currentMusic.Stop();

            currentMusic = getRandomMusic();

            currentMusic.Play();

        }

    }

    public AudioSource getRandomMusic()
    {

        return audios[Random.Range(0, audios.Length)];

    }

}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayButton : MonoBehaviour
{
    public AudioPlayer AudioPlayer { get; set; }

    public async void OnClick()
    {
        await AudioPlayer.Play();
    }



}

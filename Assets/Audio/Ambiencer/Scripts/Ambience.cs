using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ambience : ScriptableObject {

    public enum ClipBehaviour
    {
        Forever, Repeat, Looped, Oneshot
    }



    [System.Serializable]
    public class AmbienceSetting
    {
        public AudioClip audioClip;
        public ClipBehaviour clipBehaviour;
        public Vector2 randomRange;


        public float Delay()
        {
          return Random.Range(randomRange.x, randomRange.y);
        }
    }

    public float volume = 1;
    public Vector2 fadeRange = new Vector2(.1f,2.0f);
    public float Fade()
    {
        return Random.Range(fadeRange.x, fadeRange.y);
    }

    public List<AmbienceSetting> ambienceSettings;


}

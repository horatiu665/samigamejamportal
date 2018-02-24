using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Atomtwist.Audio;
using Kaae;
using UnityEngine;
using UnityEngine.Audio;

public class AmbienceDesigner : Singleton<AmbienceDesigner>
{




    public AudioMixerGroup mixerGroup;
    public int level;
    public List<Ambience> ambiences;


    private List<AudioSourcePRO> _sources = new List<AudioSourcePRO>();
    private List<AudioSourcePRO> _playingSources = new List<AudioSourcePRO>();
    private AudioSource oneShotSource;


    void OnEnable()
    {
        var gop = new GameObject();
        gop.transform.SetParent(transform);
        oneShotSource = gop.AddComponent<AudioSource>();
        oneShotSource.outputAudioMixerGroup = mixerGroup;
        oneShotSource.spatialBlend = 0;
        var ambienceSettings = ambiences.SelectMany(a => a.ambienceSettings).ToList();
        //create audiosources
        foreach (var setting in ambienceSettings)
        {

            var go = new GameObject();
            go.transform.SetParent(transform);
            var s = go.AddComponent<AudioSourcePRO>();
            s.outputAudioMixerGroup = mixerGroup;
            _sources.Add(s);
        }
    }

    private bool isPlaying;
    [DebugButton]
    public void Play()
    {
        isPlaying = true;
        var ambience = ambiences[level];
        for (int i = 0; i < ambience.ambienceSettings.Count; i++)
        {
            var setting = ambience.ambienceSettings[i];
            var s = _sources[0];
            s.clip = setting.audioClip;
            bool immediate = false;
            var r = new System.Random();
            if(r.Next(100) < 50)
            {
                immediate = true;
            }
            switch (setting.clipBehaviour)
            {
                case Ambience.ClipBehaviour.Forever:
                    s.timeSamples = Random.Range(0, s.clip.samples);
                    s.loop = true;
                    s.Play(ambience.Fade());
                    break;
                case Ambience.ClipBehaviour.Looped:
                    s = _sources[0];
                    s.clip = setting.audioClip;
                    s.loop = true;

                    StartCoroutine(RandomPlayer(s, setting, ambience, immediate));
                    break;
                case Ambience.ClipBehaviour.Repeat:
                    s = _sources[0];
                    s.clip = setting.audioClip;
                    StartCoroutine(RandomOneshotPlayer(s, setting, ambience, immediate));
                    break;
                case Ambience.ClipBehaviour.Oneshot:
                    oneShotSource.PlayOneShot(setting.audioClip,1);
                    break;
            }
            s.ambience = ambience;
            _sources.Remove(s);
            _playingSources.Add(s);
        }


    }

    IEnumerator RandomPlayer(AudioSourcePRO s, Ambience.AmbienceSetting setting, Ambience ambience,bool immediate=false)
    {
        if (immediate)
        {
            yield return new WaitForSeconds(setting.Delay());
        }
        if (!s.isPlaying)
        {
            s.timeSamples = Random.Range(0, s.clip.samples);
            s.Play(ambience.Fade());
        }
        yield return new WaitForSeconds(setting.Delay());
        s.Stop(ambience.Fade());
        yield return new WaitForSeconds(setting.Delay());
        StartCoroutine(RandomPlayer(s, setting, ambience));
    }

    IEnumerator RandomOneshotPlayer(AudioSourcePRO s, Ambience.AmbienceSetting setting, Ambience ambience,bool immediate=false)
    {
        if (immediate)
        {
            //yield return new WaitForSeconds(setting.Delay());
        }
        if (!s.isPlaying)
        {
            s.Play(0);
        }

        yield return new WaitForSeconds(setting.Delay());
        StartCoroutine(RandomPlayer(s, setting, ambience));
    }


    private void Update()
    {
        foreach (var source in _playingSources)
        {
            source.volume = source.ambience.volume;
        }
    }


    public void PlayLevel(int level)
    {
          if(!isPlaying)
              Play();

        for (int i = 1; i < level; i++)
        {
            if (i < ambiences.Count)
            {
               Next();
            }

        }
    }

    [DebugButton]
    public void Stop()
    {
        level = 0;
        StopAllCoroutines();
        for (int i = 0; i < _playingSources.Count; i++)
        {
            var source =_playingSources[i];
            source.Stop(ambiences[0].Fade());

        }
        _sources = _playingSources.ToList();
        _playingSources.Clear();

        isPlaying = false;
    }

    [DebugButton]
    public void Next()
    {

        level++;
        if(level < ambiences.Count)
            Play();
        else
        {
            level--;
        }
    }

}

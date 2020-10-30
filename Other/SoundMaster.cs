using UnityEngine;

public static class SoundMaster
{
    #region Fields
    private static AudioSource AS;
    public static bool useSounds = true;
    #endregion
    #region Methods
    public static void StartSoundSystem()
    {
        AS = GameObject.Find("-Main-").GetComponent<AudioSource>();
    }
    public static bool SoundIsUsedCheck()
    {
        return useSounds;
    }
    public static void SoundOn()
    {
        useSounds = true;
    }
    public static void SoundOff()
    {
        useSounds = false;
    }
    public static void CreateSound(Sound name)
    {
        if (useSounds)
            AS.PlayOneShot(Resources.Load<AudioClip>("sound_" + name));
    }
    #endregion
}
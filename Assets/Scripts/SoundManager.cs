using UnityEngine;
using System.Runtime.InteropServices;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip wallBounce;
    public AudioClip paddleHit;
    public AudioClip score;
    public AudioClip win;

    private AudioSource audioSource;
    private bool muted;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _SetAudioSessionPlayback();
#endif

    void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();

#if UNITY_IOS && !UNITY_EDITOR
        _SetAudioSessionPlayback();
#endif
    }

    public bool IsMuted() => muted;

    public void SetMuted(bool value)
    {
        muted = value;
        audioSource.mute = value;
    }

    public void PlayWallBounce()  { if (!muted) audioSource.PlayOneShot(wallBounce, 0.5f); }
    public void PlayPaddleHit()   { if (!muted) audioSource.PlayOneShot(paddleHit, 0.7f); }
    public void PlayScore()       { if (!muted) audioSource.PlayOneShot(score, 0.8f); }
    public void PlayWin()         { if (!muted) audioSource.PlayOneShot(win, 1.0f); }

    /// <summary>
    /// Plays a brief click on game start. Doubles as the WebGL AudioContext
    /// unlock — browsers require audio playback to originate from a user gesture
    /// handler, and the START button click is exactly that. Harmless on other
    /// platforms; provides nice arcade feedback that the game has begun.
    /// Ignores the mute flag so the AudioContext gets primed even if the user
    /// has toggled mute before starting.
    /// </summary>
    public void PrimeAudio()
    {
        if (paddleHit != null)
            audioSource.PlayOneShot(paddleHit, muted ? 0f : 0.6f);
    }

    // --- Procedural sound generation ---

    public static AudioClip GenerateWallBounce()
    {
        // Short, low blip
        return GenerateTone(220f, 0.08f, ToneShape.Square, fadeOut: true);
    }

    public static AudioClip GeneratePaddleHit()
    {
        // Brighter, punchier hit
        return GenerateTone(440f, 0.1f, ToneShape.Square, fadeOut: true);
    }

    public static AudioClip GenerateScore()
    {
        // Descending two-tone buzz
        int sampleRate = 44100;
        float duration = 0.4f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float normalizedT = (float)i / sampleCount;

            // Two-part: high tone then low tone
            float freq = normalizedT < 0.5f ? 330f : 196f;
            float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            float envelope = 1f - normalizedT;
            samples[i] = wave * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Score", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    public static AudioClip GenerateWin()
    {
        // Ascending arpeggio fanfare
        int sampleRate = 44100;
        float duration = 0.8f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        float[] notes = { 262f, 330f, 392f, 523f }; // C4, E4, G4, C5
        float noteDuration = duration / notes.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float normalizedT = (float)i / sampleCount;
            int noteIndex = Mathf.Min((int)(normalizedT * notes.Length), notes.Length - 1);
            float freq = notes[noteIndex];

            float noteT = (t % noteDuration) / noteDuration;
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.6f
                       + Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.2f; // add harmonic
            float envelope = 1f - (noteT * 0.5f); // gentle fade per note
            float masterFade = normalizedT < 0.9f ? 1f : (1f - normalizedT) * 10f;

            samples[i] = wave * envelope * masterFade * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Win", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    enum ToneShape { Sine, Square }

    static AudioClip GenerateTone(float frequency, float duration, ToneShape shape, bool fadeOut)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float normalizedT = (float)i / sampleCount;

            float wave;
            if (shape == ToneShape.Square)
                wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * frequency * t));
            else
                wave = Mathf.Sin(2f * Mathf.PI * frequency * t);

            float envelope = fadeOut ? (1f - normalizedT) : 1f;
            samples[i] = wave * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("Tone_" + frequency, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}

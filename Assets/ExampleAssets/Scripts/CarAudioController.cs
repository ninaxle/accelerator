using UnityEngine;

public class CarAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip lowClip;
    [SerializeField] private AudioClip medClip;
    [SerializeField] private AudioClip highClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource idleSource;
    [SerializeField] private AudioSource lowSource;
    [SerializeField] private AudioSource medSource;
    [SerializeField] private AudioSource highSource;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float basePitch = 1f;
    [SerializeField] private float pitchRange = 0.3f;
    [SerializeField] private float maxVolume = 0.02f;

    private CarController carController;
    private bool hasStarted = false;
    private float targetIdle = 0.8f;
    private float targetLow;
    private float targetMed;
    private float targetHigh;

    private void Start()
    {
        carController = GetComponent<CarController>();

        SetupSource(idleSource, idleClip);
        SetupSource(lowSource, lowClip);
        SetupSource(medSource, medClip);
        SetupSource(highSource, highClip);

        idleSource.Play();
        lowSource.Play();
        medSource.Play();
        highSource.Play();

        idleSource.volume = 0f;
        lowSource.volume = 0f;
        medSource.volume = 0f;
        highSource.volume = 0f;

        Invoke(nameof(StartEngine), 0.5f);
    }

    private void SetupSource(AudioSource source, AudioClip clip)
    {
        if (source == null) return;
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    private void StartEngine()
    {
        hasStarted = true;
        idleSource.volume = 0.05f;
    }

    private void Update()
    {
        if (!hasStarted || carController == null) return;

        float throttle = carController.CurrentSpeedRatio;
        float speed = throttle + (carController.IsBoosting ? 0.2f : 0f);

        if (speed < 0.05f)
        {
            targetIdle = 1f; targetLow = 0f; targetMed = 0f; targetHigh = 0f;
        }
        else if (speed < 0.35f)
        {
            targetIdle = Mathf.Lerp(1f, 0f, (speed - 0.05f) / 0.3f);
            targetLow = Mathf.InverseLerp(0.05f, 0.35f, speed);
            targetMed = 0f; targetHigh = 0f;
        }
        else if (speed < 0.65f)
        {
            targetIdle = 0f;
            targetLow = Mathf.Lerp(1f, 0f, (speed - 0.35f) / 0.3f);
            targetMed = Mathf.InverseLerp(0.35f, 0.65f, speed);
            targetHigh = 0f;
        }
        else if (speed < 0.85f)
        {
            targetIdle = 0f; targetLow = 0f;
            targetMed = Mathf.Lerp(1f, 0f, (speed - 0.65f) / 0.2f);
            targetHigh = Mathf.InverseLerp(0.65f, 0.85f, speed);
        }
        else
        {
            targetIdle = 0f; targetLow = 0f; targetMed = 0f; targetHigh = 1f;
        }

        float currentIdle = Mathf.MoveTowards(idleSource.volume / maxVolume, targetIdle, fadeSpeed * Time.deltaTime);
        float currentLow = Mathf.MoveTowards(lowSource.volume / maxVolume, targetLow, fadeSpeed * Time.deltaTime);
        float currentMed = Mathf.MoveTowards(medSource.volume / maxVolume, targetMed, fadeSpeed * Time.deltaTime);
        float currentHigh = Mathf.MoveTowards(highSource.volume / maxVolume, targetHigh, fadeSpeed * Time.deltaTime);

        idleSource.volume = currentIdle * maxVolume;
        lowSource.volume = currentLow * maxVolume;
        medSource.volume = currentMed * maxVolume;
        highSource.volume = currentHigh * maxVolume;

        float dominant = Mathf.Max(currentIdle, currentLow, currentMed, currentHigh);
        float pitch = Mathf.Lerp(basePitch - pitchRange, basePitch + pitchRange, dominant);

        idleSource.pitch = pitch;
        lowSource.pitch = pitch;
        medSource.pitch = pitch;
        highSource.pitch = pitch;
    }
}
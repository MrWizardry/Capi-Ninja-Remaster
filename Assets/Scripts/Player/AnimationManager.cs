using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnimationManager : MonoBehaviour
{
    #region ANIMATION PROPERTIES
    [Header("Animation Properties")]
    [SerializeField] private Animator animManager;
    [SerializeField] private float animDirection;
    private float animWalldirection;
    public float AnimDirection => animDirection;
    private bool isDoingAction;
    [SerializeField] private AnimationClip attackClip;
    #endregion
    #region POST PROCESSING
    [SerializeField] private VolumeProfile volume;
    private ChromaticAberration chromAb;
    [SerializeField] private float chromValue = 0.75f;
    public CinemachineVirtualCamera virtualCamera;
    public float shakeAmplitude = 1.0f;
    public float shakeFrequency = 2.0f;
    public float shakeDuration = 0.5f;

    private CinemachineBasicMultiChannelPerlin _perlin;
    #endregion
    private void Start()
    {
        isDoingAction = false;
        animManager = GetComponent<Animator>();

        virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>().GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            _perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (_perlin == null)
            {
                Debug.LogWarning("CinemachineBasicMultiChannelPerlin component not found on Virtual Camera.");
            }
        }
        else
        {
            Debug.LogError("Virtual Camera not assigned in CameraShakeController.");
        }
        
        if (volume != null)
        {
            volume.TryGet(out chromAb);

            chromAb.intensity.Override(0);
        }
        else Debug.LogWarning("Post Process Volume not found!");

        PlayAnimation("Game Start");
    }
    void Update()
    {
        animManager.SetFloat("Direction",animDirection);
    }
    private void PlayAnimation(string name)
    {
        animManager.Play(name);
    }
    public void PlayActionAnimation(string name)
    {
            if(!isDoingAction) PlayAnimation(name);
    }
    public void PlayHighPriority(string name)
    {
        PlayAnimation(name);
        isDoingAction = true;

        float length = attackClip.length;
        StartCoroutine(waitToChange(length));
    }
    private IEnumerator waitToChange(float length)
    {
        yield return new WaitForSeconds(length);
        isDoingAction = false;
    }
    public bool ReturnAnimState()
    {
        return isDoingAction;
    }
    public void SetAnimState(bool value)
    {
        isDoingAction = value;
    }
    public void StartDash()
    {
        chromAb.intensity.Override(Mathf.Lerp(chromAb.intensity.value, chromValue, 1f));
        _perlin.m_FrequencyGain = 2;
    }
    public void EndDash()
    {

        chromAb.intensity.Override(Mathf.Lerp(chromAb.intensity.value, 0, 1f));
        _perlin.m_FrequencyGain = 0;
    }
    public void SetDirection(float value)
    {
        animDirection = value;
    }
    public void SetWallDirection(float value)
    {
        animWalldirection = value;
    }
    public float ReturnDirection()
    {
        return animDirection;
    }
    private void StopInputs()
    {
        Game.Instance.CanNotReceiveInputsNow();
    }
    private void StartInputs()
    {
        Game.Instance.CanReceiveInputsNow();
    }
}

public enum AnimationStages
{
    noAction, doingAction

    //noAction: Animações que "passivas" e que não se sobrepoem como correr, idle, pulo
    //doingAction: Animações com Triggers e que sobrescrevem as "passivas", como Dash, Ataque, Bloqueio
}

using System.Collections;
using System.Collections.Generic;
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
    private LensDistortion lensDistortion;
    [SerializeField] private float lensValue = -0.4f;
    #endregion
    private void Start()
    {
        isDoingAction = false;
        animManager = GetComponent<Animator>();
        if (volume != null)
        {
            volume.TryGet(out chromAb);
            volume.TryGet(out lensDistortion);

            chromAb.intensity.Override(0);
            lensDistortion.intensity.Override(0);
        }
        else Debug.LogWarning("Post Process Volume not found!");
    }
    void Update()
    {
        animManager.SetFloat("Direction",animDirection);
        animManager.SetFloat("Wall Direction",animWalldirection);
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
        lensDistortion.intensity.Override(Mathf.Lerp(lensDistortion.intensity.value, lensValue, 1f));
    }
    public void EndDash()
    {

        chromAb.intensity.Override(Mathf.Lerp(chromAb.intensity.value, 0, 1f));
        lensDistortion.intensity.Override(Mathf.Lerp(lensDistortion.intensity.value, 0, 1f));
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
}

public enum AnimationStages
{
    noAction, doingAction

    //noAction: Animações que "passivas" e que não se sobrepoem como correr, idle, pulo
    //doingAction: Animações com Triggers e que sobrescrevem as "passivas", como Dash, Ataque, Bloqueio
}

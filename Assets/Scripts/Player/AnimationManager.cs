using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animManager;

    [SerializeField] private VolumeProfile volume;
    private ChromaticAberration chromAb;
    [SerializeField] private float chromValue = 0.75f;
    private LensDistortion lensDistortion;
    [SerializeField] private float lensValue = -0.4f;
    // Start is called before the first frame update

    private void Start()
    {
        if (volume != null)
        {
            volume.TryGet(out chromAb);
            volume.TryGet(out lensDistortion);

            chromAb.intensity.Override(0);
            lensDistortion.intensity.Override(0);
        }
        else Debug.LogWarning("Post Process Volume not found!");
    }
    private void PlayAnimation(string name)
    {
        animManager.Play(name);
    }
    public void PlayActionAnimation(string name)
    {
        PlayAnimation(name);
    }
    public void ReturnIdleAnimation()
    {
        animManager.Play("Idle");
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
}

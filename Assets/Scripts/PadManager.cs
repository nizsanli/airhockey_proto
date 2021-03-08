using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PadManager : MonoBehaviour
{
    public Animator handAnimator;

    public Transform pad;

    public float maxUpdateDistance = .2f;
    public float distancePower = 2f;
    public float angleAdjust = 1.5f;

    public string animLayerNamePad = "Pad Grab Layer";
    public string animParamNamePad = "Pad";
    private int animLayerIndexPad = -1;

    public Renderer handRender;

    // Start is called before the first frame update
    void Start()
    {
        animLayerIndexPad = handAnimator.GetLayerIndex(animLayerNamePad);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimator();
    }

    void UpdateHandAnimator()
    {
        float distanceProportion = 1f - Mathf.Clamp01(Mathf.Pow((pad.transform.position - transform.position).magnitude, distancePower) / maxUpdateDistance);
        float angleProportion = Vector3.Dot(transform.right, pad.up);

        float aboutToGrab = Mathf.Clamp01(distanceProportion * angleProportion * angleAdjust);

        handAnimator.SetLayerWeight(animLayerIndexPad, aboutToGrab);
        handAnimator.SetFloat(animParamNamePad, aboutToGrab);
    }
}

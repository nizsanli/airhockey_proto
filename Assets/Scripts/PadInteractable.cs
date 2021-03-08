using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using UnityEngine.InputSystem;

public class PadInteractable : XRBaseInteractable
{
    Rigidbody rigidBody;

    Vector3 positionOffset;

    Vector3 interactorLastPosition;
    Quaternion interactorLastRotation;

    public Transform puckPrefab;
    public Transform puck;
    public InputActionProperty puckResetInput;

    // Start is called before the first frame update
    void Start()
    {
        if (puck == null)
        {
            ResetPuck();
        }

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (puck.position.y <= .1f || puckResetInput.action.ReadValue<float>() > 0f)
        {
            Destroy(puck.gameObject);
            ResetPuck();
        }
    }

    void ResetPuck()
    {
        puck = Instantiate<Transform>(puckPrefab, Vector3.left + Vector3.up, Quaternion.identity);
    }

    void FixedUpdate()
    {
        if (selectingInteractor != null)
        {
            Vector3 deltaInteractorPosition = selectingInteractor.transform.position - interactorLastPosition;

            Vector3 positionErrorVector = rigidBody.position - (interactorLastPosition - positionOffset);
            float errorMag = positionErrorVector.magnitude + .000001f;

            // Subtract off portion of velocity in the direction of the actual position
            // This keeps the interactor and paddle position in sync, even if the interactor goes through colliders
            float scalarAlongErrorVector = Mathf.Clamp01(Vector3.Dot(deltaInteractorPosition, positionErrorVector) / (errorMag * errorMag));
            deltaInteractorPosition -= scalarAlongErrorVector * positionErrorVector;

            rigidBody.velocity = deltaInteractorPosition / Time.deltaTime;

            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, Quaternion.identity, Time.deltaTime * 25f));

            interactorLastPosition = selectingInteractor.transform.position;
            interactorLastRotation = selectingInteractor.transform.rotation;
        }
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);

        args.interactor.transform.GetChild(0).gameObject.SetActive(false);

        rigidBody.useGravity = false;

        positionOffset = args.interactor.transform.position - rigidBody.position;

        interactorLastPosition = args.interactor.transform.position;
        interactorLastRotation = args.interactor.transform.rotation;
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);

        args.interactor.transform.GetChild(0).gameObject.SetActive(true);

        rigidBody.useGravity = true;
    }
}

[CustomEditor(typeof(PadInteractable))]
public class PadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

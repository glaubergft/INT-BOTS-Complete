using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    private Animator anim;

    [SerializeField]
    private Transform target = null;

    [SerializeField]
    private float weightForPosition = 0.8f;

    [SerializeField]
    private float weightForRotation = 0.8f;

    [SerializeField]
    private float weightForLook = 0.8f;

    [SerializeField]
    private float bodyWeight = 0.5f;

    [SerializeField]
    private float headWeight = 1f;

    [SerializeField]
    private float eyesWeight = 1f;

    [SerializeField]
    private float clampWeight = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weightForPosition);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weightForRotation);

        anim.SetLookAtPosition(target.position);
        anim.SetLookAtWeight(weightForLook, bodyWeight, headWeight, eyesWeight, clampWeight);
    }

}

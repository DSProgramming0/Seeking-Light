using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHook : MonoBehaviour
{
    //Accesses Animator linked to the gameObject it has been added to

    [SerializeField] private Animator thisAnimator;

    public void setAnim(float _speed)
    {
        thisAnimator.SetFloat("Speed", _speed);
    }
}

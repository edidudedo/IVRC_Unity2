using System.Collections.Generic;
using UnityEngine;
 
public class EyeController : MonoBehaviour
{
    [SerializeField, Range(0, 2)] public int eyeTransition = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

	void Update() 
    {
        int EyeTransition = animator.GetInteger("EyeTransition");
        animator.SetInteger("EyeTransition", eyeTransition);
	}
}
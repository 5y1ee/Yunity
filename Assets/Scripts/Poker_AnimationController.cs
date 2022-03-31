using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker_AnimationController : MonoBehaviour
{
    private Animator animator;
    Animation anim;
    int num = 0;
    private void Awake() {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
    }

    void Update()
    {
        if(num>0){
//            animator.SetInteger("New Int", num);
            PlayAnim();
        }
    }

    public void PlayAnim(){
        Debug.Log("Animation");
        anim.Play("Deck_Animation");
        num--;
    }

    private void OnMouseDown() {
        num = 4;
    }
}

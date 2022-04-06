using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker_AnimationController : MonoBehaviour
{   // 사실상 지금 쓸모없는 스크립트
    private Animator animator;
    int num = 0;
    private void Awake() {
        animator = GetComponent<Animator>();
        // animation = GetComponent<Animation>();
    }



}

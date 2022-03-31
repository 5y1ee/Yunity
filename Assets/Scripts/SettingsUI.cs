using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField]
    private Button MouseControlButton;
    [SerializeField]
    private Button KeyboardMouseControlButton;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {   // gameObject가 활성화될 때 호출되는 "이벤트 콜백 함수"
        switch(PlayerSettings.controlType){
            case EControlType.Mouse:
                MouseControlButton.image.color = Color.green;
                KeyboardMouseControlButton.image.color = Color.white;
                break;
            case EControlType.KeyboardMouse:
                MouseControlButton.image.color = Color.white;
                KeyboardMouseControlButton.image.color = Color.green;
                break;
        }
    }

    public void SetControlMode(int controlType){    // 버튼의 On Click()에 부착하고 거기에서 인자를 전달해줌
        PlayerSettings.controlType = (EControlType)controlType; // enum형 변수지만 int로 넘겨줘도 읽힌다
        switch(PlayerSettings.controlType){
            case EControlType.Mouse:
                MouseControlButton.image.color = Color.green;
                KeyboardMouseControlButton.image.color = Color.white;
                break;
            case EControlType.KeyboardMouse:
                MouseControlButton.image.color = Color.white;
                KeyboardMouseControlButton.image.color = Color.green;
                break;
        }
    }
    

    public void Close(){    // 얘는 Background에 OnClick() 이벤트로 부착시켜둠.
        StartCoroutine(CloseAfterDelay());
    }
    private IEnumerator CloseAfterDelay(){
        animator.SetTrigger("close");   // Animator로 자연스럽게 설정
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        animator.ResetTrigger("close");
    }
}

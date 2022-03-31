using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EControlType{
    Mouse, KeyboardMouse
}

public class PlayerSettings
{
    // controlType 변수가 class에 존재하므로 설정창을 껐다 켜도 이전의 설정이 남아있게 된다
    public static EControlType controlType;
}

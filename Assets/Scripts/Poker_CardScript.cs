using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Poker_CardScript : MonoBehaviour
{
    public void CardScript_Alpha(int mode){ // mode 1 : 진행할 때, mode 0 : 죽었을 때, mode 2 : 기본 상태(초기화할때씀)
        spr = GetComponent<SpriteRenderer>();
        Color color = spr.color;

        if(mode==1){
            if(opcl == OpenClose.Close){
                color.a = 0.7f;
                if(playerSide == PlayerSide.Opponent){ // 일단 상대 카드는 어둡게 (검정으로 안보이게) 설정
                    color.r = 0f;
                    color.g = 0f;
                    color.b = 0f;
                }
                spr.color = color;
            }
            else{   // 0,0,0 이 검정 / 1,1,1이 본래 색깔(다 섞으면 흰색)
                color.r = 1f;
                color.g = 1f;
                color.b = 1f;
                spr.color = color;
            }
        }
        else if(mode==0){
            ResetCard();
        }
        else if(mode==2){
            color.a = 1.0f;
            if(playerSide == PlayerSide.Opponent){ // 일단 상대 카드는 어둡게 (검정으로 안보이게) 설정
                // Debug.Log("처음에 거멓게 설정");
                color.r = 0f;
                color.g = 0f;
                color.b = 0f;
            }
            spr.color = color;
        }
        else if(mode==3){   // mode 3 : 다 끝나고 공개할 때,
            color.a=1.0f;
            color.r=1.0f;
            color.g=1.0f;
            color.b=1.0f;
            
            spr.color = color;
        }

    }

    // CardScript는 각 카드 마다 적용되는 Script 이므로, value가 하나만 존재하면 된다. 또한 Reset시 에도 본인만 초기화하면 된다.
    public enum OpenClose{
        Open, Close
    }
    public enum Clicked{
        False, True
    }
    public enum PlayerSide{
        Ally, Opponent
    }
    public OpenClose opcl;
    public Clicked click;
    public GameObject Open1;
    public PlayerSide playerSide;
    public int value = 0;

    public void openCard(){
        // 초기 3장의 카드 중 open 되는 카드를 골라주는...
        // 근데 생각해보면 처음에 패 받으면 선택을 해도 되니까 이 기능 딱히 필요 없을지도?
        // switch(opcl){
        //     case OpenClose.Open:
        //         break;
        //     case OpenClose.Close:

                // int num = gameObject.GetComponent<Poker_CardScript>().value;
                // gameObject.GetComponent<Poker_CardScript>().value = Open1.GetComponent<Poker_CardScript>().value;
                // Open1.GetComponent<Poker_CardScript>().value = num;

                // Sprite pic = gameObject.GetComponent<SpriteRenderer>().sprite;
                // gameObject.GetComponent<SpriteRenderer>().sprite = Open1.GetComponent<SpriteRenderer>().sprite;
                // Open1.GetComponent<SpriteRenderer>().sprite = pic;
                // break;
        // }
        int num = value;
        value = Open1.GetComponent<Poker_CardScript>().value;
        Open1.GetComponent<Poker_CardScript>().value = num;

        Sprite pic = gameObject.GetComponent<SpriteRenderer>().sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = Open1.GetComponent<SpriteRenderer>().sprite;
        Open1.GetComponent<SpriteRenderer>().sprite = pic;

    }


    Transform CardScale;
    Vector3 defaultScale;
    SpriteRenderer spr;
    Color defaultColor;
    void Start() {
        CardScale = GetComponent<Transform>();
        defaultScale = CardScale.localScale;

        spr = GetComponent<SpriteRenderer>();
        defaultColor = spr.color;
    }

    public void CS_ThrowCard(){
        if(click == Clicked.True){
            //true면 버려야지.
            gameObject.GetComponent<Poker_CardScript>().ResetCard();
        }
    }
    public void CS_ShowCard(){
        if(click == Clicked.True){
            // true면 까야지.
            openCard();
        }
    }
    public bool isThrow=false;
    public bool isShow=false;
    private void OnMouseDown() {
        // Click 이벤트는 "Collider" 컴포넌트가 있어야 수행가능
        Debug.Log("Click!");
//        openCard();
        if(isThrow){
            Debug.Log("isThrow is true");
            click = Clicked.True;
            CS_ThrowCard();
            isThrow = false;
            GameObject.Find("GameManager").GetComponent<Poker_GMScript>().isQuit = true;
        }
        else if(isShow){
            Debug.Log("isShow is true");
            click = Clicked.True;
            CS_ShowCard();
            isShow = false;
            GameObject.Find("GameManager").GetComponent<Poker_GMScript>().isQuit = true;
        }
    }
    
    private void OnMouseEnter() {
        // 특정 gameTurn에서 GameManager의 update부분과 충돌하여 카드 색이 밝혀지지 않는 이슈가 있음..
        Color color = spr.color;
        color.r = 1.0f;
        color.g = 1.0f;
        color.b = 1.0f;

        spr.color = color;
        
        CardScale.localScale = defaultScale*1.2f;
        GetComponent<SpriteRenderer>().sortingOrder += 10;

    }
    private void OnMouseExit() {
        if(playerSide == PlayerSide.Opponent && opcl == OpenClose.Close){
            Color color = spr.color;
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;

            spr.color = color;
        }

        CardScale.localScale = defaultScale;
        GetComponent<SpriteRenderer>().sortingOrder -= 10;
    }


    
    public int GetValue(){
        return value;
    }
    public void SetValue(int newValue){
        value = newValue;
    }
    public string GetSpriteName()
    {
        return GetComponent<SpriteRenderer>().sprite.name;
    }
    public void SetSprite(Sprite newSprite){
        gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    public void ResetCard(){
        Sprite back = GameObject.Find("Deck").GetComponent<Poker_DeckScript>().GetBack();
        gameObject.GetComponent<SpriteRenderer>().sprite = back;
        value = 0;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poker_DeckScript : MonoBehaviour
{
    Animator animator;
    public int num_ani=0;

    // 카드를 저장할 Sprite 배열, 4*13+1(뒷장카드)
    public Sprite[] cardSprites;
    // 카드 숫자들 처리할 int 형 배열
    public int [] cardValues = new int[53];
    public int currentIndex = 0;   // Deck에서 카드를 빼가는 만큼, 현재 몇 번째 카드를 참조하고 있는지 기록하는 함수
   
    void Start()
    {
//      얘를 동적할당처럼 해주고 싶었는데, start함수 밖에선 cardSprites.Length 를 쓸 수 없고 Start함수에서 객체를 만들어주면 
//      DeckScript의 Start에서 호출되는 Shuffle과 순서가 겹치는지(?) 제대로 수행되지 않는다. 구동방식을 더 알아봐야겠다..
//        cardValues = new int[cardSprites.Length];
//        Debug.Log(cardValues.Length);
        GetCardValues();

        animator = GetComponent<Animator>();
    }

    void GetCardValues(){
        // Deck script의 cardValues라는 배열을 초기화해주는 배열, 즉 Sprite배열에 대응되는 value를 저장해주는것
        int num=0, pic=0;
        cardValues[0] = -1;
        for(int i=1; i<cardSprites.Length; i++){
            num = i%13;
            pic = i/13;
            
            if(num == 0){
                num = 13;
                pic -= 1;
            }
            // num은 숫자, pic은 문양 -> 즉 0번대 100번대 200번대 300번대가 클.다.하.스.
            cardValues[i] = 100*pic+num++;
        }
    }

    public void Shuffle(){
        // Shuffle은 다른 script도 참조하므로 public 으로 설정
        Debug.Log("Shuffle.. cardSprites.Length : " + cardSprites.Length);

        for(int i=cardSprites.Length-1; i>0; i--){
            // 1부터 52까지 랜덤으로 뽑는 숫자
            int j = Mathf.FloorToInt(Random.Range(1, cardSprites.Length));
            
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int value = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = value;
        }
        // Shuffle하면 처음 index부터 참조하도록 설정, 게임 도중에 셔플할 경우엔 바꿔줘야할듯.
        currentIndex = 1;
    }

    public Sprite GetBack(){
        return cardSprites[0];
    }


    public int DeckCard(Poker_CardScript CS){
        // SpreadCard_num(1);
        CS.SetSprite(cardSprites[currentIndex]);
        CS.SetValue(cardValues[currentIndex]);
        currentIndex++;

        return CS.GetValue();
    }

    IEnumerator coAnim(){
        while(true){
            // Debug.Log(num_ani);
            // animation.Play("Deck_Animation");
            num_ani--;
            yield return new WaitForSecondsRealtime(2.0f);
            if(num_ani<-59)  break;
        }
    }

    // int num=0;
    // public IEnumerator SpreadCard_4(int n){
    //     yield return null;
    //     while(n>=0){
    //         n--;
    //         animator.SetInteger("New Int", num);
    //         yield return new WaitForSeconds(1.6f);
    //     }
    //     num--;
    // }
    float ani_time = 1.45f;
    public IEnumerator co_SpreadCard(int n){
        yield return null;
        // Debug.Log("before"+n);
        while(n>=0){
            if(n==0){
                animator.SetInteger("New Int", n);
                break;
            }
            animator.SetInteger("New Int", n);
            n-=1;
            yield return new WaitForSeconds(ani_time);
            // Debug.Log("after"+n);
        }
    }
    private void OnMouseDown() {
        Debug.Log("Deck clicked");
        SpreadCard_num(1);        
    }

    public void SpreadCard_num(int n){
        StartCoroutine(co_SpreadCard(n));
    }

}

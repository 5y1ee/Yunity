using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandRankings : MonoBehaviour
{
    [SerializeField]
    private Poker_PlayerScript playerScript;
    [SerializeField]
    private Poker_ComScript[] comScripts;
    [SerializeField]
    private Text text_Pair, text_TwoPair, text_Triple, text_Straight, text_Flush, text_FullHouse, text_FourCard, text_StraightFlush;
    
    // 전체 카드 배열
    public int [] totalCards = new int[52]; // 4*(1~13)
    public List<int> ListCards = new List<int>();

    public int gameTurn;
    public void Proba_Calc() {
        if(Turn.instance.gameTurn>=3){
            gameTurn = Turn.instance.gameTurn;
            CountCard();
        }
    }

    public void ResetRanking(){
        // totalCards 배열은 idx를 돌면서 새롭게 초기화가 되는데, <List>는 .Add 메소드로 인해 계속해서 추가만 되는 상황 발생.
        ListCards.Clear();
        
        for(int i=0; i<totalCards.Length; i++){                         // 0~12,  13~25,  26~38,  39~51
            // 카드 value값 : 1~13까지 숫자, 0, 100, 200, 300번대가 문양 => 1~13, 101~113, 201~213, 301~313 (C.D.H.S)
            totalCards[i] = 100*(i/13) + (i%13) + 1;
            ListCards.Add(100*(i/13) + (i%13) + 1);
        }
    }

    public void CountCard(){
        //공개된 카드 counting -> 3,4,5,6 카드가 공개카드임 => 7턴에 받은 내 카드도 빼야지
        if(gameTurn>3 && gameTurn<7){
            // 3턴 이후로는 1장씩만 검사하면 됨.
            int value, idx;
            for(int i=0; i<4; i++){
                value = comScripts[i].hand[gameTurn-1].GetComponent<Poker_CardScript>().value;
                idx = (value/100)*13 + value%100 - 1;
                totalCards[idx] = -1;
                ListCards.Remove(value);
            }
            value = playerScript.hand[gameTurn-1].GetComponent<Poker_CardScript>().value;
            idx = (value/100)*13 + value%100 - 1;
            totalCards[idx] = -1;
            ListCards.Remove(value);
        }

        else if(gameTurn==3){
            // 3턴에는 3장검사
            int value, idx;
            for(int i=0; i<4; i++) {    // 여기서 4는 상대 Component의 수 인데, 이건 나중에 변수로 통제할 수도 있을 듯.
                // 상대 컴포넌트는 3번째 카드만 오픈카드니까,
                value = comScripts[i].hand[2].GetComponent<Poker_CardScript>().value;
                idx = (value/100)*13 + value%100 - 1;
                totalCards[idx] = -1;
                ListCards.Remove(value);
            }
            for(int i=0; i<3; i++){
                // 자신의 패는 3장 모두 빼준다.
                value = playerScript.hand[i].GetComponent<Poker_CardScript>().value;
                idx = (value/100)*13 + value%100 - 1;
                totalCards[idx] = -1;
                ListCards.Remove(value);
            }
        }

        Calculate();
        // CountCard
    }

    public void Calculate(){
        // 3턴부터 시작되는걸로 간주하고 ㄱㄱ
        // Royal Flush / Straight Flush / Four Card / Full House / Flush / Straight / Triple / Two Pair / One Pair / Top
        float Pair=0.0f, TwoPair=0.0f,
        Triple=0.0f, Straight=0.0f,
        Flush=0.0f, FullHouse=0.0f,
        FourCard=0.0f, StraightFlush=0.0f;

        // 우선 순열 조합 써서 확률 구하는 방식으로 ㄱㄱㄱㄱㄱ
        // 플러쉬, 스트레이트, 풀하우스, 포카드, 트리플, 투페어, 원페어 각각 따로 계산.. 해야겠지..?
        int RemainTurns = 7 - gameTurn;

        // Pair : 12바퀴 돌리면 되것지.

        // Flush : 같은 문양 5개
        for(int i=0; i<4; i++) {
            float proba = 0.0f;
            if(playerScript.Ary_pic[i] + RemainTurns>=5){
                // 남은카드 : RemainCards  == 앞으로 받을 카드의 수
                // 필요한 문양 수 : 5-Ary_pic[i]
                // 전체카드 : ListCards.Count
                // 전체카드 중 "특정문양"의 카드 : ListCards < 100 or ListCards/100 해서 계산
                int n = ListCards.Count;
                uint r = 0;
                int remain = 5 - playerScript.Ary_pic[i];
                // uint remain = 5-
                for(int k=0; k<n; k++){
                    if(ListCards[k]/100==i)
                        r++;
                }
                // Debug.Log("n:"+n+", r:"+ r+", remain:"+ remain);
                ulong son=0;// = combi(r, (uint)remain);
                for(int j=0; j<=RemainTurns-remain; j++){
                    son += combi(r, (uint)RemainTurns-(uint)j) * combi((uint)n-r, (uint)j);
                }
                ulong mom = combi((uint)n, (uint)RemainTurns);
                // Debug.Log("@@son"+son+", mom"+ mom);

                proba = (float)son/mom;
                if(Flush<proba)   Flush=proba;
            }

        }
        // Debug.Log("Flush "+Flush);
        // text_Flush.text = "Flush : {0,5}%"+Flush*100+"%";
        text_Flush.text = string.Format("Flush : {0,5}%", Math.Round(Flush*100, 3));
        
        // calculate
    }

    ulong combi(uint n, uint r){
        if(r==0) return 1;
        else{
            if(n-r<r) {
                r=n-r;
            }
            ulong cnt=0,  mom=1;
            ulong son=1;

            for(uint i=n-r+1; i<=n; i++) son*=i;
            for(uint k=1; k<=r; k++) mom*=k;
            // for(int j=1; j<=n-r; j++) mom*=j;
            // Debug.Log("son"+son+", mom"+ mom);
            cnt = son/mom;

            return cnt;
        }
    }









// 이 밑으론 안 쓴 함수들..
    void combination(int depth, int next, int n, int r){
        // n개 중에 r개 선택
        if(depth == r){
            return;
        }

        for(int i = next; i <= n; i++){
            combination(depth + 1, i + 1, n, r);
        }
    }
    private int Max(int[] list){
        int max=0;
        foreach(var i in list){
            if(i>max)   max=i;
        }
        return max;
    }
    private int Idx(int[] list, int val){
        int idx=-1;
        foreach(var i in list){
            if(i==val)  idx=i;
        }
        return idx;
    }

}

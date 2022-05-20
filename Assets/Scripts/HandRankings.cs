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
        gameTurn = Turn.instance.gameTurn;
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

    // 남은카드 : RemainTurns  == 앞으로 받을 수 있는 카드의 수 -> Num_Cards_CanGet
    // "메이드까지" 필요한 문양 수 : 5-Ary_pic[i] -> remain -> Num_Cards_Required
    // 전체카드 : ListCards.Count -> n -> Num_Cards_Total
    // 전체카드 중 "특정문양"의 카드 : ListCards < 100 or ListCards/100 해서 계산 -> r -> Num_Cards_Target
    public void Calculate(){
        // 3턴부터 시작
        float Pair=0.0f, TwoPair=0.0f,
        Triple=0.0f, Straight=0.0f,
        Flush=0.0f, FullHouse=0.0f,
        FourCard=0.0f, StraightFlush=0.0f;

        uint RemainTurns = 7 - (uint)gameTurn;
        uint Num_Cards_Total = (uint)ListCards.Count;
        uint Num_Cards_Target, Num_Cards_Required;
        ulong mom = combi((uint)Num_Cards_Total, (uint)RemainTurns);
        ulong son;
        float proba;

        // Pair : 13바퀴 돌리면 되것지.
        for(int i=1; i<14; i++) {
            proba = 0.0f;
            if(playerScript.Ary_num[i] + RemainTurns >=2){
                son = 0;
                Num_Cards_Target = 0;
                Num_Cards_Required = 2 - (uint)playerScript.Ary_num[i];
                for(int k=0; k<Num_Cards_Total; k++){
                    if(ListCards[k]%100==i)
                        Num_Cards_Target++;
                }
                son = calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns);

                proba = (float)son/mom;
                if(Pair<proba)   Pair=proba;
            }
        }
        text_Pair.text = string.Format("Pair : {0,5}%", Math.Round(Pair*100, 3));

        // Two Pair : Pair가 0개인 경우, Pair가 1개인 경우.. / 일단 1페어일 경우 확률을 따져보자.
        if(Pair == 1) {
            int idx=0;
            for(int i=1; i<14; i++)
                if(playerScript.Ary_num[i]==2)  idx=i;
            for(int i=1; i<14; i++) {
                if(i==idx)  continue;
                proba = 0.0f;
                if(playerScript.Ary_num[i] + RemainTurns >= 2) {
                    son = 0;
                    Num_Cards_Target = 0;
                    Num_Cards_Required = 2 - (uint)playerScript.Ary_num[i];
                    for(int k=0; k<Num_Cards_Total; k++){
                        if(ListCards[k]%100==i)
                            Num_Cards_Target++;
                    }
                    son = calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns);
                    
                    proba = (float)son/mom;
                    if(TwoPair<proba)   TwoPair=proba;
                }
            }

        }
        text_TwoPair.text = string.Format("TwoPair : {0,5}%", Math.Round(TwoPair*100, 3));

        // Triple : Pair에서 Req만 바꾸면 될 듯.
        for(int i=1; i<14; i++) {
            proba = 0.0f;
            if(playerScript.Ary_num[i] + RemainTurns >=3){
                son = 0;
                Num_Cards_Target = 0;
                Num_Cards_Required = 3 - (uint)playerScript.Ary_num[i];
                for(int k=0; k<Num_Cards_Total; k++){
                    if(ListCards[k]%100==i)
                        Num_Cards_Target++;
                }
                son = calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns);

                proba = (float)son/mom;
                if(Triple<proba)   Triple=proba;
            }
        }
        text_Triple.text = string.Format("Triple : {0,5}%", Math.Round(Triple*100, 3));

        // Straight : 얘는 어떻게 처리할까.
        // A 2 3 4 5 6 7 8 9 10 J Q K A
        ulong tot=0;
        for(int i=1; i<11; i++) {
            proba = 0.0f;
            int cnt=5;
            int [] Idx = new int[5];
            for(int x=0; x<5; x++)  Idx[x]=i+x;
            List<int> Idxx = new List<int>(Idx);
            // Debug.Log("조회할 리스트 "+string.Join(", ", Idxx));
            for(int k=0; k<5; k++) {
                if(playerScript.Ary_num[i+k]>0) {
                    cnt--;
                    Idxx.Remove(i+k);
                }
            }
            // Debug.Log("필요한 숫자 : "+string.Join(",", Idxx));            
            if(RemainTurns - cnt>= 0) {
                son = 1;
                Num_Cards_Required = 1;
                // 1-5-9-13
                // 1 6 11 || 2 7 12 || 3 8 13 || 1 4 9 14
                for(int j=0; j<Idxx.Count; j++){
                    // Idxx[j] : 타겟 값
                    Num_Cards_Target = 0;
                    for(int h=0; h<Num_Cards_Total; h++){
                        if(ListCards[h]%100==Idxx[j])
                            Num_Cards_Target++;
                    }
                    Debug.Log("Idxx[J] : "+Idxx[j]+" Target : "+Num_Cards_Target);
                    // son += calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns-(uint)cnt);
                    son *= combi(Num_Cards_Target, Num_Cards_Required);
                    Debug.Log("Son : "+son);
                    // son *= combi(Num_Cards_Total-(uint)cnt, RemainTurns-(uint)cnt);
                }
                son *= combi(Num_Cards_Total-(uint)cnt, RemainTurns-(uint)cnt);
                tot+=son;
                Debug.Log("Tot : "+tot+" / mom : "+mom);
                // proba = (float)tot/mom;
                // if(Straight<proba)   Straight=proba;
            }
        }
        Straight = (float)tot/mom;
        
        text_Straight.text = string.Format("Straight : {0,5}%", Math.Round(Straight*100, 3));

        // Flush : 같은 문양 5개
        for(int i=0; i<4; i++) {
            proba = 0.0f;
            if(playerScript.Ary_pic[i] + RemainTurns>=5){
                son = 0;
                Num_Cards_Target = 0;
                Num_Cards_Required = 5 - (uint)playerScript.Ary_pic[i];

                for(int k=0; k<Num_Cards_Total; k++){
                    if(ListCards[k]/100==i)
                        Num_Cards_Target++;
                }
                // Debug.Log("n:"+n+", r:"+ r+", remain:"+ remain);
                son = calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns);
                // Debug.Log("@@son"+son+", mom"+ mom);

                proba = (float)son/mom;
                if(Flush<proba)   Flush=proba;
            }
        }
        text_Flush.text = string.Format("Flush : {0,5}%", Math.Round(Flush*100, 3));
        
        // FourCard
        for(int i=1; i<14; i++) {
            proba = 0.0f;
            if(playerScript.Ary_num[i] + RemainTurns >=4){
                son = 0;
                Num_Cards_Target = 0;
                Num_Cards_Required = 4 - (uint)playerScript.Ary_num[i];
                for(int k=0; k<Num_Cards_Total; k++){
                    if(ListCards[k]%100==i)
                        Num_Cards_Target++;
                }
                son = calc_son(Num_Cards_Total, Num_Cards_Required, Num_Cards_Target, RemainTurns);

                proba = (float)son/mom;
                if(FourCard<proba)   FourCard=proba;
            }
        }
        text_FourCard.text = string.Format("FourCard : {0,5}%", Math.Round(FourCard*100, 3));


        // calculate
    }

    ulong combi(uint n, uint r){
        if(r==0) return 1;
        else{
            if(n-r<r) {
                r=n-r;
            }
            ulong cnt=0, mom=1, son=1;

            for(uint i=n-r+1; i<=n; i++) son*=i;
            for(uint k=1; k<=r; k++) mom*=k;
            // for(int j=1; j<=n-r; j++) mom*=j;
            // Debug.Log("son"+son+", mom"+ mom);
            cnt = son/mom;

            return cnt;
        }
    }

    ulong calc_son(uint Num_Cards_Total, uint Num_Cards_Required, uint Num_Cards_Target, uint RemainTurns){
        ulong son = 0;
        for(int k=0; k<=RemainTurns-Num_Cards_Required; k++)
            son += combi(Num_Cards_Target, RemainTurns-(uint)k) * combi(Num_Cards_Total-Num_Cards_Target, (uint)k);
        return son;
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

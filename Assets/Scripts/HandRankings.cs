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
    private Text text_Flush;
    
    // 전체 카드 배열
    public int [] totalCards = new int[52]; // 4*(1~13)
    public List<int> ListCards = new List<int>();
    private void Awake() {
        for(int i=0; i<totalCards.Length; i++){                         // 0~12,  13~25,  26~38,  39~51
            // 카드 value값 : 1~13까지 숫자, 0, 100, 200, 300번대가 문양 => 1~13, 101~113, 201~213, 301~313
            totalCards[i] = 100*(i/13) + (i%13) + 1;
            ListCards.Add(100*(i/13) + (i%13) + 1);
        }
    }
    public int gameTurn;
    private void Update() {
        if(Turn.instance.gameTurn>=3){
            gameTurn = Turn.instance.gameTurn;
            CountCard();
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
            for(int i=0; i<4; i++) {
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
        float Flush=0.0f, Straight, Triple, Pair, Quadra;

        // 우선 순열 조합 써서 확률 구하는 방식으로 ㄱㄱㄱㄱㄱ
        // 플러쉬, 스트레이트, 풀하우스, 포카드, 트리플, 투페어, 원페어 각각 따로 계산.. 해야겠지..?
        int RemainCards = 7 - gameTurn;

        // Flush : 같은 문양 5개
        // float Flush=0.0f;
        for(int i=0; i<4; i++) {    // 4문양 훑으면서 확률 계산 | CDHS            
            float proba = 0.0f;
            if(playerScript.Ary_pic[i] + RemainCards>=5){
                // 남은카드 : RemainCards  == 앞으로 받을 카드의 수
                // 필요한 문양 수 : 5-Ary_pic[i]
                // 전체카드 : ListCards.Count
                // 전체카드 중 "특정문양"의 카드 : ListCards < 100 or ListCards/100 해서 계산
                int n = ListCards.Count;
                uint r = 0;
                // uint remain = 5-
                for(int k=0; k<n; k++){
                    if(ListCards[k]/100==i)
                        r++;
                }
                Debug.Log("n"+n+" r"+ r+" remain"+ RemainCards);
                ulong son = combi(r, (uint)RemainCards);
                ulong mom = combi((uint)n, (uint)RemainCards);
                Debug.Log("@@son"+son+", mom"+ mom);

                proba = (float)son/mom;
                if(Flush<proba)   Flush=proba;
            }

        }
        Debug.Log("Flush "+Flush);
        text_Flush.text = "Flush"+Flush*100+"%";

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
            Debug.Log("son"+son+", mom"+ mom);
            cnt = son/mom;

            return cnt;
        }
    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_PlayerScript : MonoBehaviour
{
    public Poker_DeckScript DeckScript;
    public Poker_CardScript[] CardScript;
    
    public GameObject[] hand;
    public int handIdx = 0, Ranking;
    public Text handText;
    
    public void setAlpha(int mode){
        for(int i=0; i<handIdx; i++){
            CardScript[i].CardScript_Alpha(mode);
        }
    }

    public int GetCard(){
        int cardValue = DeckScript.DeckCard(hand[handIdx].GetComponent<Poker_CardScript>());
        hand[handIdx].GetComponent<Renderer>().enabled = true;  // 받은 카드는 화면에 보이도록 설정.
        handIdx++;  // 카드 받았으면 패 인덱스도 증가시켜야지
        return cardValue;
    }

    public void GameStart(){
        // 게임 시작 함수,, 처음엔 카드 4장 뿌리고 뭘 버리고 선택할지 골라야겠지..?
        for(int i=0;i<4;i++){
            GetCard();
        }
    }
    public void ThrowCard(){
        // 먼저 받은 4장의 카드 스크립트... 버릴 카드 선택
        for(int i=0; i<4; i++)
            CardScript[i].isThrow = true;
    }
    public void ThrowEnd(){
        for(int i=0; i<4; i++)
            CardScript[i].isThrow = false;
    }
    public void ShowCard(){
        // CardScript.isShow = true;
        for(int i=0; i<4; i++)
            CardScript[i].isShow = true;
    }
    public void ShowEnd(){
        for(int i=0; i<4; i++)
            CardScript[i].isShow = false;
        Relocate();
    }
    public void Relocate(){
        //  버린 카드 빼고, 남은 카드 정렬.
        handIdx=3;  // 계속 빼버려서,, 지정을 해줘야함..
        int tmp=-1;
        for(int i=0; i<4; i++){
            if(hand[i].GetComponent<Poker_CardScript>().value==0)
                tmp = i;
        }
        if(tmp!=3){
            // tmp 번째의 카드와 idx==3의 카드를 swap하면 됨
            int num = hand[tmp].GetComponent<Poker_CardScript>().value;
            hand[tmp].GetComponent<Poker_CardScript>().value = hand[3].GetComponent<Poker_CardScript>().value;
            hand[3].GetComponent<Poker_CardScript>().value = num;

            Sprite pic = hand[tmp].GetComponent<SpriteRenderer>().sprite;
            hand[tmp].GetComponent<SpriteRenderer>().sprite = hand[3].GetComponent<SpriteRenderer>().sprite;
            hand[3].GetComponent<SpriteRenderer>().sprite = pic;
        }
        hand[3].GetComponent<SpriteRenderer>().enabled = false;

    }
    public void ResetHand(){
        for(int i=0; i<hand.Length; i++){
            hand[i].GetComponent<Poker_CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        handIdx=0;
    }


    // "패 카드들" 그림과 숫자 저장할 배열
    public int[] hand_pic = new int[7];
    public int[] hand_num = new int[7];

    // 패의 그림, 숫자가 몇 갠지 셀 배열
    public int[] Ary_num = new int[15]; // 1~13 idx=> A~K, 0은 그냥 비워둘것!! + 맨마지막에 "A"를 중복해서 저장. A는 1과 14를 담당.
    public int[] Ary_pic = new int[4]; //0~3 idx => clov, dia, heart, spade

    public void Rank(){
        string[] Pictures = new string[] {"Clover", "Diamond", "Heart", "Spade"};
        string[] Numbers = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"};

        int isFlush=0, Straight=0, Quadra=0, Pair=0, Triple=0, Mountain=0;
        List<int> Pairlist = new List<int>();
        List<int> Triplelist = new List<int>();


        for(int i=0; i<7; i++){
            hand_num[i]=0;
            hand_pic[i]=0;
        }
        
        // 패 배열들 초기화
        for(int i=0; i<14; i++)
            Ary_num[i]=0;
        for(int i=0; i<4; i++)
            Ary_pic[i]=0;

        for(int i=0; i<handIdx; i++){
            // 패 카드들 저장하는 배열 초기화
            hand_pic[i] = CardScript[i].value / 100; // 0,1,2,3
            hand_num[i] = CardScript[i].value % 100; // 1~13

            Ary_pic[hand_pic[i]] += 1;
            Ary_num[hand_num[i]] += 1;

            if(hand_num[i]==1)   Ary_num[14]=Ary_num[1];  // A는 14도 같이 쳐주자
        }

        // 패 검사 코드
        for(int i=0; i<4; i++){ // 플러쉬 체크
            if(Ary_pic[i]>=5)   isFlush=i;
        }
        // A만 따로 검사 좀 하는건데 되려나,,
        if(Ary_num[1]==4) Quadra=14;
        else if(Ary_num[1]==3) {
            Triplelist.Add(1);
            Triplelist.Add(14);
            Triple++;
        }
        else if(Ary_num[1]==2) {
            Pairlist.Add(1);
            Pairlist.Add(14);
            Pair++;
        }

        for(int i=2; i<14; i++){ // 포카드,트리플,페어 체크 & 트리플 숫자 체크 & 페어 리스트 추가
            if(Ary_num[i]==4) Quadra=i;
            else if(Ary_num[i]==3) {
                Triplelist.Add(i);
                Triple++;
            }
            else if(Ary_num[i]==2) {
                Pairlist.Add(i);
                Pair++;
            }
        }
        if(Ary_num[10]!=0 && Ary_num[11]!=0 && Ary_num[12]!=0 && Ary_num[13]!=0 && Ary_num[1]!=0)
            Mountain=1; // 마운틴 10 J Q K A 스트레이트
        for(int i=1; i<10; i++){ // 스트레이트 체크 1~5부터 10~A까지 총 10가지 9~K까지가 여기선 마지막이지.
            if(Ary_num[i]!=0 && Ary_num[i+1]!=0 && Ary_num[i+2]!=0 && Ary_num[i+3]!=0 && Ary_num[i+4]!=0)
                Straight=i;
        }

        if(Straight != 0 && isFlush != 0){
            handText.text = Numbers[Straight-1] + " Straight Flush!";   Ranking=1000+Straight;
        }
        else if(Quadra!=0){
            handText.text = Numbers[Quadra-1] + " FourCard!";   Ranking=900+Quadra;
        }
        else if(Triple!=0 && Pair!=0){
            handText.text = Numbers[Triplelist[Triplelist.Count-1]-1] + ", " + Numbers[Pairlist[Pairlist.Count-1]-1] + " Full House!";
            Ranking=800+Triplelist[Triplelist.Count-1];
        }
        else if(isFlush!=0){
            handText.text = Pictures[isFlush] + " Flush!";
            Ranking=705-isFlush;    // isFlush는 0:스페이드가 젤 쎈거니까, 빼기로 구분하자
        }
        else if(Mountain!=0){
            handText.text = "'10-J-K-Q-A' Mountain!";
            Ranking=600;    // 얘는 A의 문양을 구분해야하는데 너무 귀찮은걸...
        }
        else if(Straight!=0){
            handText.text = Numbers[Straight-1] + "~" + Numbers[Straight+3] + " Straight";
            Ranking=500+Straight;
        }
        else if(Triple!=0){
            handText.text = Numbers[Triplelist[Triplelist.Count-1]-1] + " Triple";
            Ranking=400+Triplelist[Triplelist.Count-1];
        }
        else if(Pair>=2)// 페어가 2개 이상 => 투페어
        {
            // handText.text = Pairlist[0] + ", " + Pairlist[1] + " Two Pair";
            handText.text = Numbers[Pairlist[Pairlist.Count-2]-1] + ", " + Numbers[Pairlist[Pairlist.Count-1]-1] + " Two Pair";
        // else if(Pair==3)
        //     handText.text = Pairlist[1] + ", " + Pairlist[2] + " Two Pair";
            Ranking=300+Pairlist[Pairlist.Count-1]+Pairlist[Pairlist.Count-2];
        }
        else if(Pair==1){
            handText.text = Numbers[Pairlist[0]-1] + " One Pair";
            Ranking=200+Pairlist[Pairlist.Count-1];
        }
        
        else if(Ary_num[1]!=0){
            handText.text = "A Top";
            Ranking=100;
        }
        else {
            int max=1;
            for(int i=0; i<handIdx; i++)
                if(max<hand_num[i])  max=hand_num[i];
            handText.text = Numbers[max-1] + " Top";
            Ranking=0+max;
        }

    }

    public int getRank(){
        return Ranking;
    }

}

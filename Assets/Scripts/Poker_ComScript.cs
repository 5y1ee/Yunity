using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poker_ComScript : MonoBehaviour
{
    public Poker_DeckScript DeckScript;
    public Poker_CardScript[] CardScript;
    
    public GameObject[] hand;
    public int handIdx = 0;
    public int Ranking;
    void Start()
    {
        // DeckScript = GameObject.Find("Deck").GetComponent<Poker_DeckScript>();
        // CardScript = GameObject.Find("Close01").GetComponent<Poker_CardScript>();

    }
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
        // com은 3장만 뿌리자! 우선,,,
        for(int i=0;i<3;i++){
            GetCard();
        }

    }


    public void ResetHand(){
        for(int i=0; i<hand.Length; i++){
            hand[i].GetComponent<Poker_CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        handIdx=0;
    }


    void Update()
    {
        
    }


    public Text handText;
/*
    public void Rank(){
        string[] Flush = new string[] {"Clover", "Diamond", "Heart", "Spade"};

        int isFlush=0, Straight=0, Quadra=0, Pair=0, Triple=0, Mountain=0;
        List<int> Pairlist = new List<int>();
        List<int> Triplelist = new List<int>();

        // "패 카드들" 그림과 숫자 저장할 배열
        int[] pic = new int[7];
        int[] num = new int[7];

        // 패의 그림, 숫자가 몇 갠지 셀 배열
        int[] Ary_num = new int[14];    // 0~13 idx=> A~K, 0은 그냥 비워둘것!!
        int[] Ary_pic = new int[4]; //0~3 idx => clov, dia, heart, spade

        for(int i=0; i<14; i++)
            Ary_num[i]=0;
        for(int i=0; i<4; i++)
            Ary_pic[i]=0;

        for(int i=0; i<handIdx; i++){
            pic[i] = CardScript[i].value / 100; // 0,1,2,3
            num[i] = CardScript[i].value % 100; // 1~13

            Ary_pic[pic[i]] += 1;
            // if(num[i]>-1) // 카드 버리면 value가 0이라, num이 -1이 되어서 인덱스로 쓸 수 없다.
            Ary_num[num[i]] += 1;
        }

        // 패 검사 코드
        for(int i=0; i<4; i++){ // 플러쉬 체크
            if(Ary_pic[i]>=5)   isFlush=i;
        }
        for(int i=1; i<14; i++){ // 포카드,트리플,페어 체크 & 트리플 숫자 체크 & 페어 리스트 추가
            if(Ary_num[i]==4) Quadra=i+1;
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

        if(Straight != 0 && isFlush != 0)
            handText.text = Straight + " Straight Flush!";
        else if(Quadra!=0)
            handText.text = Quadra + " FourCard!";
        else if(Triple!=0 && Pair!=0)
            handText.text = Triplelist[0] + ", " + Pair + " Full House!";
        else if(isFlush!=0)
            handText.text = Flush[isFlush] + " Flush!";
        else if(Mountain!=0)
            handText.text = "'10-J-K-Q-A' Mountain!";
        else if(Straight!=0)
            handText.text = Straight + "~" + (Straight+4) + " Straight";
        else if(Triple!=0)
            handText.text = Triplelist[0] + " Triple";
        else if(Pair==2)// 페어가 2개 이상 => 투페어
            handText.text = Pairlist[0] + ", " + Pairlist[1] + " Two Pair";
        else if(Pair==3)
            handText.text = Pairlist[1] + ", " + Pairlist[2] + " Two Pair";
        else if(Pair==1)
            handText.text = Pairlist[0] + " One Pair";
        else if(Ary_num[1]!=0)
            handText.text = "A Top";
        else {
            int max=0;
            for(int i=0; i<handIdx; i++)
                if(max<num[i])  max=num[i];
            handText.text = max + " Top";
        }

    }
*/
    public void Rank(){
        string[] Pictures = new string[] {"Clover", "Diamond", "Heart", "Spade"};
        string[] Numbers = new string[] {"A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"};

        int isFlush=0, Straight=0, Quadra=0, Pair=0, Triple=0, Mountain=0;
        List<int> Pairlist = new List<int>();
        List<int> Triplelist = new List<int>();

        // "패 카드들" 그림과 숫자 저장할 배열
        int[] pic = new int[7];
        int[] num = new int[7];

        // 패의 그림, 숫자가 몇 갠지 셀 배열
        int[] Ary_num = new int[15];    // 0~13 idx=> A~K, 0은 그냥 비워둘것!! + 맨마지막에 "A"를 중복해서 저장. A는 1과 14를 담당.
        int[] Ary_pic = new int[4]; //0~3 idx => clov, dia, heart, spade

        for(int i=0; i<14; i++)
            Ary_num[i]=0;
        for(int i=0; i<4; i++)
            Ary_pic[i]=0;

        for(int i=0; i<handIdx; i++){
            pic[i] = CardScript[i].value / 100; // 0,1,2,3
            num[i] = CardScript[i].value % 100; // 1~13

            Ary_pic[pic[i]] += 1;
            // if(num[i]>-1) // 카드 버리면 value가 0이라, num이 -1이 되어서 인덱스로 쓸 수 없다.
            Ary_num[num[i]] += 1;
            if(num[i]==1)   Ary_num[14]++;  // A는 14도 같이 쳐주자
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
                if(max<num[i])  max=num[i];
            handText.text = Numbers[max-1] + " Top";
            Ranking=0+max;
        }

    }

    public int getRank(){
        return Ranking;
    }

}

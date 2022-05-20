using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Poker_GMScript : MonoBehaviour
{
    // Scripts
    [SerializeField]
    private Poker_DeckScript DeckScript;
    [SerializeField]
    private Poker_PlayerScript PlayerScript;
    [SerializeField]
    private Poker_ComScript[] comScripts;
    [SerializeField]
    private HandRankings rankScripts;

    // Buttons
    public Button startBtn, halfBtn, callBtn, allinBtn, dieBtn, nextBtn;

    public Text infoText, handText, halfText, dieText, turnText;
    public Text[] comhandText; // 버튼들, 텍스트들 다 드래그로 넣어줌

    // public int gameTurn;    // 초기화 안해주면 0으로 되는듯
    void Start()
    {
        startBtn.gameObject.SetActive(true);
        startBtn.onClick.AddListener(()=>StartClicked());
        halfBtn.onClick.AddListener(()=>HalfClicked());
        dieBtn.onClick.AddListener(()=>dieClicked());
        nextBtn.onClick.AddListener(()=>NextClicked()); // 이거 여기에 켜둬야함!
    }

    public int gameTurn;
    public bool isDie;

    void Update()
    {
        // if(gameTurn==0)
        //     StopAllCoroutines();
        
        if(gameTurn!=Turn.instance.gameTurn){
            // DeckCard는 버튼 클릭 시 실행됨! (새 카드 분배), 클릭 시  isDone도 False로 변경.
            gameTurn = Turn.instance.gameTurn;
            turnText.text = "Turn : " + gameTurn;

            if(gameTurn==1){    //첫 턴은 버리는 카드 선택
                StopCoroutine(co_Turn_7());

                rankScripts.ResetRanking();
                PlayerScript.setAlpha(2);
                for(int i=0; i<4; i++)
                    comScripts[i].setAlpha(2);

                infoText.gameObject.SetActive(true);
                infoText.text = "Choose a card to throw away";
                PlayerScript.ThrowCard();   // hand의 카드 4장의 isThrow=true로 설정
                StartCoroutine(co_Turn_1());

            }
            else if(gameTurn==2){   // 두번째 턴은 보일 카드 선택,
                StopCoroutine(co_Turn_1());

                infoText.gameObject.SetActive(true);
                infoText.text = "Choose a card to show";
                PlayerScript.ShowCard();
                StartCoroutine(co_Turn_2());

            }
            else if(gameTurn==3){
                StopCoroutine(co_Turn_2());
                handText.gameObject.SetActive(true);
                PlayerScript.setAlpha(1);
                for(int i=0; i<4; i++)
                    comScripts[i].setAlpha(1);
                
                // gameTurn++;
            }

            else if(gameTurn==7){
                PlayerScript.setAlpha(1);
                for(int i=0; i<4; i++)
                    comScripts[i].setAlpha(1);

                StartCoroutine(co_Turn_7());
            }

            else if(gameTurn==8){
                // Debug.Log("정산 드갑시다");
                PlayerScript.setAlpha(3);
                for(int i=0; i<4; i++)
                    comScripts[i].setAlpha(3);

                StartCoroutine(co_Turn_8());
            }

            if(gameTurn>0 && gameTurn<8){
                PlayerScript.Rank();
                for(int i=0; i<4; i++)
                    comScripts[i].Rank();
                
                // Player만을 위한 확률 계산이므로,, Die 상태에서는 실행X
                if(!isDie)
                    rankScripts.Proba_Calc();
            }
        }
    }

    private void StartClicked(){
        Debug.Log("StartClicked");
        startBtn.gameObject.SetActive(false);
        
        isDie = false;
        isQuit = false;

        // 패 처음에 털어주고,
        PlayerScript.ResetHand();
        for(int i=0; i<comScripts.Length; i++)
            comScripts[i].ResetHand();

        // 심심하니까 두 번 셔플
        DeckScript.Shuffle();
        DeckScript.Shuffle();
        // GetCard 4번, 애니메이션 실행 
        // DeckScript.num_ani = 4;
        DeckScript.SpreadCard_num(4);   // 4번 실행. 애니메이션은 턴이 관리하는게 맞다.!

        PlayerScript.GameStart();
        for(int i=0; i<comScripts.Length; i++){
            comScripts[i].GameStart();
        }

        Turn.instance.gameTurn=1;
    }
    private void HalfClicked(){
        Debug.Log("HalfClicked");
        if(gameTurn>2 && gameTurn<7){    // 3턴부터 눌리도록.
            DeckScript.SpreadCard_num(1);   // 애니메이션 1번 실행. 애니메이션은 "턴"이 관리해주는게 맞다.
            // 근데 update 에선 무한 반복되니, 나중엔 isTurnOver 변수를 둬서 턴이 끝나면 gameTurn++, 애니메이션 실행하도록 하고
            // 지금은 턴 자체가 나만 누르면 진행되는 구조니까 일단 버튼이 애니메이션이랑 gameTurn 변수를 처리하도록 하자.
            PlayerScript.GetCard();
            for(int i=0; i<comScripts.Length; i++)
                comScripts[i].GetCard();
            // 하프 눌렀으면 턴을 늘려야지. 다이든 뭐든.
            Turn.instance.gameTurn++;
        }
    }
    private void dieClicked(){
        Debug.Log("DieClicked");
        if(gameTurn>2 && gameTurn<7){
            isDie = true;

            PlayerScript.setAlpha(0);

            halfBtn.gameObject.SetActive(false);
            callBtn.gameObject.SetActive(false);
            allinBtn.gameObject.SetActive(false);
            dieBtn.gameObject.SetActive(false);
            nextBtn.gameObject.SetActive(true);
            // Listener 를 여기에 켜두니까 중복 생성되면 한번 눌러도 2번, 3번씩 불리게 되더라!!
            // nextBtn.onClick.AddListener(()=>NextClicked());

        }
    }

    private void NextClicked(){
        Debug.Log("NextClicked");
        DeckScript.SpreadCard_num(1);
        for(int i=0; i<comScripts.Length; i++)
            comScripts[i].GetCard();

        Turn.instance.gameTurn++;
    }

    // isQuit : 첫 턴에서 true가 될 때 까지 무한루프를 돌리는 변수, throw와 show가 완료되면 true로 설정해준다.
    public bool isQuit = false;
    IEnumerator co_Turn_1(){
        while(true){
            for(int i=0; i<4; i++){
                if(PlayerScript.hand[i].GetComponent<Poker_CardScript>().isThrow == false){
                    // 여기서 검출이 안되고 CardScript에서 직접 isQuit를 true로 설정해줬음.
                    isQuit = true;
                    break;
                }
            }
            if(isQuit==true){
                Debug.Log("turn 1 end");
                PlayerScript.ThrowEnd();
                Turn.instance.gameTurn=2;
                isQuit=false;
                break;
            }

            yield return new WaitForSeconds(0.2f);

        }
    }
    IEnumerator co_Turn_2(){
        while(true){
            if(isQuit==true){
                Debug.Log("turn 2 end");
                PlayerScript.ShowEnd();
                Turn.instance.gameTurn=3;
                infoText.gameObject.SetActive(false);
                
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator co_Turn_7(){
        infoText.gameObject.SetActive(true);
        infoText.text = "GameEnd";
        
        yield return new WaitForSeconds(2f);
        // 1초 후에 게임 결과 나오도록
        // 아니면 com들 패 하나씩 오픈되도록,
        int[] playerRank = new int[5];
        int Winner = 0;

        playerRank[0] = PlayerScript.getRank();
        for(int i=1; i<5; i++){
            playerRank[i] = comScripts[i-1].getRank();
        }

        int max=playerRank[0];
        for(int i=1; i<5; i++){
            if(max<playerRank[i])
                max = playerRank[i];
        }
        for(int i=0; i<5; i++){
            if(max==playerRank[i]){
                Winner += (int)Mathf.Pow(10,i);
            }
        }

        infoText.text = "Winner is " + Winner;
        Turn.instance.gameTurn=8;
    }

    IEnumerator co_Turn_8(){
        // 여기 애니메이션 추가하던가,
        yield return new WaitForSeconds(2.5f);
        // die 했을 때 버튼들 active false로 해둔거 살리려고
        halfBtn.gameObject.SetActive(true);
        callBtn.gameObject.SetActive(true);
        allinBtn.gameObject.SetActive(true);
        dieBtn.gameObject.SetActive(true);
        nextBtn.gameObject.SetActive(false);
        infoText.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(true);

        // Turn.instance.gameTurn=0;
        StopAllCoroutines();
    }

}

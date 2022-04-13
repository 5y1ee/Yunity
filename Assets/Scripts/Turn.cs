using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    public static Turn instance = null;
    
    private void Awake() {
        if(instance==null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            if(instance!=this)
                Destroy(this.gameObject);
        }
    }
    public int gameTurn=0;
}
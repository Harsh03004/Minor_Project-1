using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spellcasting : MonoBehaviour
{
    public Animator anim;
    
   void Animation()
   {
    if(Input.GetKey(KeyCode.T))
    {
        anim.SetBool("slash",true);
    }
    else{
        anim.SetBool("slash",false);
    }
   }
}

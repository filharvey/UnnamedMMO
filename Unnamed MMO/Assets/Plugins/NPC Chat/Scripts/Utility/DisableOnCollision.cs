using UnityEngine;
using System.Collections;

public class DisableOnCollision : MonoBehaviour {

	public string playername;
	public Behaviour userControl;

	void Awake(){
		userControl = GameObject.Find(playername).GetComponent<UserControlThirdPerson> ();
	}

	void OnTriggerEnter(Collider col){
		if(col.tag == "Player"){
			//userControl = col.GetComponent<UserControlThirdPerson> ();
			DisablePlayerControl();
		}
	}

	void OnTriggerExit(Collider col){
		if(col.tag == "Player"){
			userControl.enabled = true;
		}
	}

	public void EnablePlayer(){
		userControl.enabled = true;
	}

	public void DisablePlayerControl(){
		userControl.enabled = false;
		Animator anim = userControl.gameObject.GetComponent<Animator> ();
		anim.SetFloat ("Forward", 0);
		anim.SetFloat ("Turn", 0);
		anim.SetFloat ("JumpLeg", 0);
	}

}

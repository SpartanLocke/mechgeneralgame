using UnityEngine;
using System.Collections;

public class StartSceneButtonRotater : MonoBehaviour {


	public GameObject QuickPlay;
	public GameObject Mulitplayer;
	public GameObject LevelEditor;
	public GameObject Credits;
	public float QuickDegree;
	public float MultiDegree;
	public float LevelDegree;
	public float CreditDegree;
	public float DegreeRotateSpeed=3;
	public int DegreeActivationThreshold;

	public bool SpinningMulti = false;
	public bool SpinningLevel = false;
	public bool SpinningCredit = false;


	void Start () {
		QuickPlay.transform.rotation = Quaternion.Euler (90, 0, 0);
		Credits.transform.rotation = Quaternion.Euler (90, 0, 0);
		Mulitplayer.transform.rotation = Quaternion.Euler (90, 0, 0);
		LevelEditor.transform.rotation = Quaternion.Euler (90, 0, 0);
		QuickDegree = 90;
		MultiDegree = 90;
		LevelDegree = 90;
		CreditDegree = 90;
		StartCoroutine (SpinQuickPlay());


	}


	IEnumerator SpinQuickPlay(){
		while (QuickDegree!=0) {
			QuickDegree = QuickDegree-=DegreeRotateSpeed;
			QuickPlay.transform.rotation = Quaternion.Euler (QuickDegree, 0, 0);
			if (QuickDegree <DegreeActivationThreshold &!SpinningMulti){
				StartCoroutine(SpinMultiPlayer());
			}
			yield return  new WaitForSeconds(0.001f);
		}
	}

	IEnumerator SpinMultiPlayer(){
		SpinningMulti = true;
		while (MultiDegree!=0) {
			MultiDegree = MultiDegree-=DegreeRotateSpeed;
			Mulitplayer.transform.rotation = Quaternion.Euler (MultiDegree, 0, 0);
			if (MultiDegree <DegreeActivationThreshold &!SpinningLevel){
				StartCoroutine(SpinLevelEditor());
			}
			yield return  new WaitForSeconds(0.001f);
		}
	}

	IEnumerator SpinLevelEditor(){
		SpinningLevel = true;
		while (LevelDegree!=0) {
			LevelDegree = LevelDegree-=DegreeRotateSpeed;
			LevelEditor.transform.rotation = Quaternion.Euler (LevelDegree, 0, 0);
			if (LevelDegree <DegreeActivationThreshold & !SpinningCredit){
				StartCoroutine(SpinCredits());
			}
			yield return  new WaitForSeconds(0.001f);
		}
	}

	IEnumerator SpinCredits(){
		SpinningCredit = true;
		while (CreditDegree!=0) {
			CreditDegree = CreditDegree-=DegreeRotateSpeed;
			Credits.transform.rotation = Quaternion.Euler (CreditDegree, 0, 0);
			yield return  new WaitForSeconds(0.001f);
		}
	}



}
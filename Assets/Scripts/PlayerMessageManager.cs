using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerMessageManager : MonoBehaviour {

	public Color StandardColor;
	public Color Player1Color;
	public Color Player2Color;
	public Quaternion BaseRotation;

	public Vector3 HiddenOnLeft;
	public Vector3 HiddenOnRight;
	public Vector3 CentrePosition;
	public Vector3 CurrentPosition;

	public bool DisplayedWelcomeMessage = false;

	public new float OffSet=0f;

	void Start () {
		gameObject.GetComponent<RectTransform> ().rotation = BaseRotation;
        if (!ApplicationManager.isNetworkPlay)
        {
            StartCoroutine(TextAnimation("Player 1 Turn", Player1Color));
        }
        else if (ApplicationManager.localPlayerFaction == 0)
        {
            StartCoroutine(TextAnimation("You are player 1", Player1Color));
        } else
        {
            StartCoroutine(TextAnimation("You are player 2", Player2Color));

        }

    }

	void FixedUpdate () {
		gameObject.GetComponent<RectTransform> ().localPosition = CurrentPosition;
	}

	public void TextAnimationCaller(string MessageText, Color MessageColor){
		StartCoroutine (TextAnimation (MessageText, MessageColor));
	}

	IEnumerator TextAnimation(string MessageText, Color MessageColor){
		OffSet = 0;
		if (DisplayedWelcomeMessage == false) {
			DisplayedWelcomeMessage =true;
			gameObject.GetComponent<Text> ().text = "Let The Battle Begin!!";
			CurrentPosition = HiddenOnLeft;
			gameObject.GetComponent<Text> ().color = StandardColor;
			while (CurrentPosition.x!=0) {
				CurrentPosition.x = Mathf.Lerp (CurrentPosition.x, 0, 0.08f);

				if (CurrentPosition.x > -30) {
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds (0.2f);
			while (CurrentPosition.x!=HiddenOnRight.x) {
				CurrentPosition.x = Mathf.Lerp (CurrentPosition.x, HiddenOnRight.x, OffSet);
				OffSet += 0.018f;
				yield return new WaitForEndOfFrame();
				if (CurrentPosition.x > 995) {
					break;
				}

			}
		}

		gameObject.GetComponent<Text> ().text = MessageText;
		OffSet = 0;
			CurrentPosition = HiddenOnLeft;
			gameObject.GetComponent<Text> ().color = MessageColor;
			while (CurrentPosition.x!=0) {
				CurrentPosition.x = Mathf.Lerp (CurrentPosition.x, 0, 0.08f);
				
				if (CurrentPosition.x > -30) {
					break;
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds (0.2f);
			while (CurrentPosition.x<HiddenOnRight.x) {
				CurrentPosition.x = Mathf.Lerp (CurrentPosition.x, HiddenOnRight.x, OffSet);
				OffSet += 0.018f;
				yield return new WaitForEndOfFrame();
//				if (CurrentPosition.x > 995) {
//					break;
//				}
				
			}
		}


}
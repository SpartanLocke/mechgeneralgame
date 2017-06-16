using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SoldierCircularFill : MonoBehaviour {

    public SoldierManager soldier;
	public Image HPImage1;
	public Image HPImage2;
	public Image HPImage3;
    public Image AmmoImage;

	public Image PowerUpImage1;
	public Image PowerUpImage2;
	public Image PowerUpImage3;
    List<Image> PowerUpImages = new List<Image>();
	public Sprite Blank;

    Color GrenadeFillColor = new Color(255 / 255f, 153 / 255f, 0 / 255f, 1f);
    Color normalFillColor = Color.white;

    public Sprite DamageUpImage;
	public Sprite ActionUpImage;
	public Sprite GrenadeUpImage;
	public Color SoldierFactionOutline0;
	public Color SoldierFactionOutline1;
	public int RotateAngleSpeed;

    public float selectedAlpha;
    public float unSelectedAlpha;

    public Color HPFullColor;
    public Color HPEmptyColor;

    //TODO: Add cached states to improve performace.

	public void DamagePickedUp(){ // yeah, dont ask me to code back end stuf lol gg
		if (PowerUpImage1.enabled == false) {
			PowerUpImage1.enabled = true;
			PowerUpImage1.sprite = DamageUpImage;
            PowerUpImage1.color = normalFillColor;
			return;
		}
		if (PowerUpImage2.enabled ==false) {
			PowerUpImage2.enabled = true;
			PowerUpImage2.sprite = DamageUpImage;
            PowerUpImage2.color = normalFillColor;

            return;
		}
		if (PowerUpImage3.enabled == false) {
			PowerUpImage3.enabled = true;
			PowerUpImage3.sprite = DamageUpImage;
            PowerUpImage3.color = normalFillColor;

            return;
		}
	}

	public void ActionPickedUp(){
		if (PowerUpImage1.enabled == false) {
			PowerUpImage1.enabled = true;
			PowerUpImage1.sprite = ActionUpImage;
            PowerUpImage1.color = normalFillColor;

            return;
		}
		if (PowerUpImage2.enabled == false) {
			PowerUpImage2.enabled = true;
			PowerUpImage2.sprite = ActionUpImage;
            PowerUpImage2.color = normalFillColor;

            return;
		}
		if (PowerUpImage3.enabled == false) {
			PowerUpImage3.enabled = true;
			PowerUpImage3.sprite = ActionUpImage;
            PowerUpImage3.color = normalFillColor;

            return;
		}
	}

	public void GrenadePickedUp(){
		if (PowerUpImage1.enabled == false) {
			PowerUpImage1.enabled = true;
			PowerUpImage1.sprite = GrenadeUpImage;
            PowerUpImage1.color = GrenadeFillColor;
			return;
		}
		if (PowerUpImage2.enabled == false) {
			PowerUpImage2.enabled = true;
			PowerUpImage2.sprite = GrenadeUpImage;
            PowerUpImage1.color = GrenadeFillColor;

            return;
		}
		if (PowerUpImage3.enabled == false) {
			PowerUpImage3.enabled = true;
			PowerUpImage3.sprite = GrenadeUpImage;
            PowerUpImage1.color = GrenadeFillColor;

            return;
		}
	}

    public void GrenadeUsed()
    {
        foreach (Image image in PowerUpImages)
        {
            if (image.enabled && image.sprite == GrenadeUpImage)
            {
                image.enabled = false;
                image.color = normalFillColor;
                return;
            }
        }
    }

	// Use this for initialization
	void Start () {
        PowerUpImages.Add(PowerUpImage1);
        PowerUpImages.Add(PowerUpImage2);
        PowerUpImages.Add(PowerUpImage3);

        HPImage1.GetComponent<Image>().color = HPFullColor;
		HPImage2.GetComponent<Image>().color = HPFullColor;
		HPImage3.GetComponent<Image>().color = HPFullColor;
		PowerUpImage1.enabled = false;
		PowerUpImage2.enabled = false;
		PowerUpImage3.enabled = false;


		if (gameObject.GetComponentInParent<SoldierManager>().faction==0) {
			HPImage1.GetComponent<Outline>().effectColor=SoldierFactionOutline0;
			HPImage2.GetComponent<Outline>().effectColor=SoldierFactionOutline0;
			HPImage3.GetComponent<Outline>().effectColor=SoldierFactionOutline0;

		}
		if (gameObject.GetComponentInParent<SoldierManager>().faction==1) {
			HPImage1.GetComponent<Outline>().effectColor=SoldierFactionOutline1;
			HPImage2.GetComponent<Outline>().effectColor=SoldierFactionOutline1;
			HPImage3.GetComponent<Outline>().effectColor=SoldierFactionOutline1;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//fillPercentage(HPImage, soldier.health, GameManager.maxHealth);
		//fillPercentage(AmmoImage, soldier.ammo, GameManager.maxAmmo);
		CheckHP ();
		if (soldier.isSelected) {
			setAlpha (selectedAlpha);
			gameObject.GetComponent<RectTransform> ().Rotate (0, 0, RotateAngleSpeed);

		} 
		else {
			setAlpha (unSelectedAlpha);
		
		}
	}
	
	void CheckHP(){
		if (gameObject.GetComponentInParent<SoldierManager> ().health == 2) {
			HPImage1.GetComponent<Image>().enabled=false;
		}
		if (gameObject.GetComponentInParent<SoldierManager> ().health == 1) {
			HPImage2.GetComponent<Image>().enabled=false;
			HPImage1.GetComponent<Image>().enabled=false;
		}
		if (gameObject.GetComponentInParent<SoldierManager> ().health == 0) {
			HPImage3.GetComponent<Image>().enabled=false;		
		}
	}
	


    void setAlpha(float alpha)
    {
        HPImage1.color = new Color(HPImage1.color.r, HPImage1.color.g, HPImage1.color.b, alpha);
		HPImage2.color = new Color(HPImage2.color.r, HPImage2.color.g, HPImage2.color.b, alpha);
		HPImage3.color = new Color(HPImage3.color.r, HPImage3.color.g, HPImage3.color.b, alpha);

        //AmmoImage.color = new Color(AmmoImage.color.r, AmmoImage.color.g, AmmoImage.color.b, alpha);
    }
}

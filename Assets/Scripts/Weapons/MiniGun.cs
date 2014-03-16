using UnityEngine;
using System.Collections;

public class MiniGun : Weapon {

	bool firing; //Gibt an ob die Minigun bereits schießt
	float firingDelayTimer;
	public AudioSource audio;
	public GameObject bullet;
	public AudioClip[] audioClips = new AudioClip[3];
	// Use this for initialization
	void Start () {
		timer = 0.0f;
		maxTime = 0.1f;
		minAmmo = 64;
		maxAmmo = 256;
		incAmmo = 64;

		_weaponType = WeaponType.MINIGUN;

		audio = this.GetComponentInChildren<AudioSource>();
		firingDelayTimer = 0.0f;
		
	}
	
	// Update is called once per frame
	void Update () {

		this.GetComponentInChildren<RealAudioSource>().setPosition(new Vector3());
		//Sobald der Button gedrückt wird schießt die Minigun
		//EVTL DELAY EINFÜGEN, WEGEN ANLAUFEN
		if(buttonPressed){
			//firing = true;
			if(firingDelayTimer == 0.0f){
				audio.Stop();
				audio.clip = audioClips[0];
				audio.Play();
			}
			firingDelayTimer += Time.deltaTime;
		}
		else {
			firing = false;
			firingDelayTimer = 0.0f;
			if(audio.isPlaying && audio.clip != audioClips[2]){
				audio.Stop();
				audio.clip = audioClips[2];
				audio.Play();
			}
		}

		if(firingDelayTimer>0.6f){
			firing = true;
		}
		//Wenn der Timer sein Limit erreicht hat wird ein Raycast ausgeführt der in gerader Richtung nach
		//Vorne verläuft, das erste Objekt das getroffen wird erhält schaden
		//EVTL SPRAY EINFÜGEN
		if(ammo > 0){
			if(firing){
				if(!audio.isPlaying){
					audio.clip = audioClips[1];
					audio.Play();
				}
				timer+=Time.deltaTime;
				if(timer>=maxTime){
					timer = 0.0f;
					ammo--;
					RaycastHit hit;
					Vector3 bulletDirection = transform.forward;
					float x = Random.Range(-0.05f,0.05f);
					float y = Random.Range(-0.05f,0.05f);
					bulletDirection= bulletDirection + transform.right*x + transform.up*y;
					GameObject bulletInstance;
					if(Network.connections.Length > 0){
						bulletInstance = (GameObject)Network.Instantiate(bullet,this.transform.position,this.transform.rotation,0);
					} else {
						bulletInstance = (GameObject)GameObject.Instantiate(bullet,this.transform.position,this.transform.rotation);
					}
					bulletInstance.rigidbody.velocity = bulletDirection*600;
					if(Physics.Raycast(transform.position,bulletDirection,out hit,300)){

						if(hit.collider.GetComponent<AbstractDestructibleObject>())
						{
							hit.collider.GetComponent<AbstractDestructibleObject>().receiveDamage(5.0f);
							if(Network.connections.Length > 0){
								hit.collider.networkView.RPC("receiveDamage",hit.collider.networkView.owner,5.0f);
							}
						} 
						else 
						{
							Debug.Log("Minigun hit something, but could't apply damage!");	
						}
					}
				}
			}
		}else {
			timer = 0.0f;
		}
	}

}

#pragma strict

import Roar.Components;

private var roar : IRoar = null;

public var maxHealth : float = 100.0;
public var health : float = 100.0;
public var regenerateSpeed : float = 0.0;
public var invincible : boolean = false;
public var dead : boolean = false;

public var damagePrefab : GameObject;
public var damageEffectTransform : Transform;
public var damageEffectMultiplier : float = 1.0;
public var damageEffectCentered : boolean = true;

public var scorchMarkPrefab : GameObject = null;
private var scorchMark : GameObject = null;

public var damageSignals : SignalSender;
public var dieSignals : SignalSender;

private var lastDamageTime : float = 0;
private var damageEffect : ParticleEmitter;
private var damageEffectCenterYOffset : float;

private var colliderRadiusHeuristic : float = 1.0;


function Awake () {
	roar = GameObject.Find("Roar").GetComponent(DefaultRoar) as IRoar;
	enabled = false;
	if (damagePrefab) {
		if (damageEffectTransform == null)
			damageEffectTransform = transform;
		var effect : GameObject = Spawner.Spawn (damagePrefab, Vector3.zero, Quaternion.identity);
		effect.transform.parent = damageEffectTransform;
		effect.transform.localPosition = Vector3.zero;
		damageEffect = effect.particleEmitter;
		var tempSize : Vector2 = Vector2(collider.bounds.extents.x,collider.bounds.extents.z);
		colliderRadiusHeuristic = tempSize.magnitude * 0.5;
		damageEffectCenterYOffset = collider.bounds.extents.y;
		
	}
	if (scorchMarkPrefab) {
		scorchMark = GameObject.Instantiate(scorchMarkPrefab, Vector3.zero, Quaternion.identity);
		scorchMark.active = false;
	}
}

function process_kill (name) {
	Debug.Log("DESTROYED [" + name + "]");
	switch (name)
	{
	case "EnemySpider": roar.Tasks.Execute("destroy_spider", null); break;
	case "KamikazeBuzzer": roar.Tasks.Execute("destroy_buzzer", null); break;
	case "EnemyMech": roar.Tasks.Execute("destroy_mech", null); break;
	default: break;
	}
}

function OnDamage (amount : float, fromDirection : Vector3, attacker : Transform) {
	// Take no damage if invincible, dead, or if the damage is zero
	if(invincible)
		{Debug.Log("..... BECAUSE INVINCIBLE"); return;}
	if (dead)
		{Debug.Log("......BECAUSE DEAD"); return;}
	if (amount <= 0)
		{Debug.Log("......BECAUSE AMOUNT"); return;}
	
	// Decrease health by damage and send damage signals
	
	// @HACK: this hack will be removed for the final game
	//  but makes playing and showing certain areas in the
	//  game a lot easier
	/*	
	#if !UNITY_IPHONE && !UNITY_ANDROID
	if(gameObject.tag != "Player")
		amount *= 10.0;
	#endif
	*/
	
	health -= amount;
	damageSignals.SendSignals (this);
	lastDamageTime = Time.time;
	
	if (health <= 0) {
		if (attacker != null) {
			var comp = attacker.GetComponent(typeof(PhotonView));
			if (comp != null) {
				if (comp.isMine) process_kill(gameObject.name);
				else Debug.Log("SOMEONE ELSE SCORED!");
			}
		}
	}
	
	// Enable so the Update function will be called
	// if regeneration is enabled
	if (regenerateSpeed > 0)
		enabled = true;
	
	// Show damage effect if there is one
	if (damageEffect) {
		damageEffect.transform.rotation = Quaternion.LookRotation (fromDirection, Vector3.up);
		if(!damageEffectCentered) {
			var dir : Vector3 = fromDirection;
			dir.y = 0.0;
			damageEffect.transform.position = (transform.position + Vector3.up * damageEffectCenterYOffset) + colliderRadiusHeuristic * dir;
		}
		// @NOTE: due to popular demand (ethan, storm) we decided
		// to make the amount damage independent ...
		//var particleAmount = Random.Range (damageEffect.minEmission, damageEffect.maxEmission + 1);
		//particleAmount = particleAmount * amount * damageEffectMultiplier;
		damageEffect.Emit();// (particleAmount);
	}
	
	var view : PhotonView = GetComponent(typeof(PhotonView));
	if(view.isMine && !view.isSceneView){
		Camera.main.SendMessage("OnDamage");
	}
	
	if(PhotonNetwork.isMasterClient){
	    // Die if no health left
	    if (health <= 0) {		
			//if (attacker != null) {
			//	var component = attacker.GetComponent(typeof(PhotonView));
			//	if (component != null) {
			//		if (component.isMine) process_kill(gameObject.name);
			//		else Debug.Log("SOMEONE ELSE SCORED!");
			//	}
			//}
	        if(view.isSceneView){
	            view.RPC("Die", PhotonTargets.AllBuffered);
	        }else{
	        view.RPC("Die", PhotonTargets.All);
	     }
		}
		
		var diff : float = lastSharedHP - health;
		if(Mathf.Abs(diff)>5){
		    view.RPC("SetHP", PhotonTargets.Others, health);
		}		
	}
}

@RPC
function SetHP(newVal : float){
    health = newVal;
}
var lastSharedHP : float = 100;

@RPC
function Die(){
	health = 0;
	dead = true;
	dieSignals.SendSignals (this);
	enabled = false;
	
	// scorch marks
	if (scorchMark) {
		scorchMark.active = true;
		// @NOTE: maybe we can justify a raycast here so we can place the mark
		// on slopes with proper normal alignments
		// @TODO: spawn a yield Sub() to handle placement, as we can
		// spread calculations over several frames => cheap in total
		var scorchPosition : Vector3 = collider.ClosestPointOnBounds (transform.position - Vector3.up * 100);
		scorchMark.transform.position = scorchPosition + Vector3.up * 0.1;
		scorchMark.transform.eulerAngles.y = Random.Range (0.0, 90.0);
	}
}

function OnEnable () {
	Regenerate ();	
}

// Regenerate health

function Regenerate () {
	if (regenerateSpeed > 0.0f) {
		while (enabled) {
			if (Time.time > lastDamageTime + 3) {
				health += regenerateSpeed;
				
				yield;
				
				if (health >= maxHealth) {
					health = maxHealth;
					enabled = false;
				}
			}
			yield WaitForSeconds (1.0f);
		}
	}
}

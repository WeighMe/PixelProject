﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangedWeapon : Weapon {

	public GameObject bullet; 	//发射的子弹
	public float accurate;		//精准性
	public int magazineSize;	//弹夹容量
	public float reloadTime;	//填装时间
	public int maxAmmo;			//子弹总数
	public float critChance;	//暴击率
	public float critMultiplier;//暴击倍率

	private int curMagazine;
	private int curAmmo;
	private float onceAttactTime;
	private float attactTimer = 0f;
	private bool canAttact;
	private List<Transform> firePoint = new List<Transform>();		//针对多枪管的情况
	private int fireIndex;
	
	void Awake(){
		onceAttactTime = 1/attackRate;
		fireIndex = 0;
		Transform[] children = transform.GetComponentsInChildren<Transform>();
		foreach(Transform child in children){
			if(child.tag == "FirePoint"){
				firePoint.Add(child);
			}
		}
		curMagazine = magazineSize;
		curAmmo = maxAmmo;
	}
	
	void FixedUpdate(){

		//计算能否射击
		if(attactTimer > 0){ 								//若还在冷却则继续计时
			attactTimer -= Time.deltaTime;
		}else if(canAttact == false && curMagazine > 0){	//若弹夹中还有子弹则继续射击
			canAttact = true;
		}else if(curMagazine == 0 && curAmmo > 0){			//若弹夹中没有子弹并且还有弹药则装弹(针对没有子弹后捡到子弹的情况)
			if(curAmmo > magazineSize){
				curMagazine = magazineSize;
				curAmmo -= magazineSize;
			}else{
				curMagazine = curAmmo;
				curAmmo = 0;
			}
		}
	}

	
	override public void Attack(object[] message){
		if(canAttact){
			float attack = (float)message[0] * attackFix;
			if(Random.Range(0,1) < critChance){
				attack *= critMultiplier;
			}
			Vector3 target = (Vector3)message[1];
			bool isPlayer = (bool)message[2];

			GunController.instance.CreateOneBullet(firePoint[fireIndex],bullet,attack,attackRange,accurate,target,isPlayer);
			canAttact = false;

			if(--curMagazine == 0){
				attactTimer = reloadTime;
				if(curAmmo >= magazineSize){
					curMagazine = magazineSize;
					curAmmo -= magazineSize;
				}else if(curAmmo > 0){
					curMagazine = curAmmo;
					curAmmo = 0;
				}
			}else{
				attactTimer = onceAttactTime;
			}

			if(++fireIndex > firePoint.Count-1){
				fireIndex = 0;
			}
		}
	}
}

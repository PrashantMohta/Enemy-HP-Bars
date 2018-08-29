﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnemyHPBar
{
    public class EnemyHPBar : Mod
    {
        private const string version = "1.2.0";

        public override string GetVersion()
        {
            return version;
        }

        public override void Initialize()
        {
            Log("Initializing EnemyHPBars");

            ModHooks.Instance.OnEnableEnemyHook += Instance_OnEnableEnemyHook;
            ModHooks.Instance.OnRecieveDeathEventHook += Instance_OnRecieveDeathEventHook;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            Log("Initialized EnemyHPBars");
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            activeBosses = new List<string>();
        }

        private bool Instance_OnRecieveDeathEventHook(EnemyDeathEffects enemyDeathEffects, bool eventAlreadyRecieved, ref float? attackDirection, ref bool resetDeathEvent, ref bool spellBurn, ref bool isWatery)
        {
            Log($@"Enemy {enemyDeathEffects.gameObject.name} ded");
            if (enemyDeathEffects.gameObject.GetComponent<HPBar>() != null)
            {
                if (enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP == 0)
                {
                    GameObject placeHolder = new GameObject();
                    placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
                    HealthManager phhm = placeHolder.AddComponent<HealthManager>();
                    HPBar phhp = placeHolder.AddComponent<HPBar>();
                    phhp.currHP = 18;
                    phhp.maxHP = 18;
                    phhm.hp = 0;
                }
                else
                {
                    GameObject placeHolder = new GameObject();
                    placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
                    HealthManager phhm = placeHolder.AddComponent<HealthManager>();
                    HPBar phhp = placeHolder.AddComponent<HPBar>();
                    EnemyDeathEffects phede = placeHolder.AddComponent<EnemyDeathEffects>();
                    phhp.currHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP;
                    phhp.maxHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().maxHP;
                    phhm.hp = 0;
                }
            }

            return eventAlreadyRecieved;
        }

        private bool Instance_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead)
        {
            HealthManager hm = enemy.GetComponent<HealthManager>();
            EnemyDeathEffects ede = enemy.GetComponent<EnemyDeathEffects>();
            EnemyDeathTypes deathType = (EnemyDeathTypes) ede?.GetType()
                .GetField("enemyDeathType", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(ede);

            bool isBoss = hm?.hp >= 200 || deathType == EnemyDeathTypes.LargeInfected;

            if (!isBoss)
            {
                HPBar hpbar = hm?.gameObject.GetComponent<HPBar>();
                if (hpbar != null || hm?.hp >= 5000 || isAlreadyDead) return isAlreadyDead;
                hm?.gameObject.AddComponent<HPBar>();
                Log($@"Added hp bar to {enemy.name}");
            }
            else
            {
                BossHPBar bossHpBar = hm?.gameObject.GetComponent<BossHPBar>();
                if (bossHpBar != null || hm?.hp >= 5000 || isAlreadyDead) return isAlreadyDead;
                hm?.gameObject.AddComponent<BossHPBar>();
                activeBosses.Add(enemy.name);
                Log($@"Added hp bar to boss {enemy.name}");
            }

            return false;
        }
        public static List<string> activeBosses;
    }
}

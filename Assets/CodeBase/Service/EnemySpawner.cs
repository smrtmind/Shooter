﻿using CodeBase.Mobs;
using CodeBase.UI;
using CodeBase.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CodeBase.Utils.Enums;
using Random = UnityEngine.Random;

namespace CodeBase.Service
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Storages")]
        [SerializeField] private DependencyContainer dependencyContainer;
        [SerializeField] private EnemyStorage enemyStorage;

        [Space]
        [SerializeField] private List<SpawnParameters> enemies;

        private Bounds screenBounds;
        private List<Coroutine> spawnCoroutines = new List<Coroutine>();

        private void OnEnable()
        {
            UserInterface.OnLevelLoaded += InitSpawner;
        }

        private void OnDisable()
        {
            UserInterface.OnLevelLoaded -= InitSpawner;
        }

        private void InitSpawner()
        {
            screenBounds = dependencyContainer.ScreenBounds.borderOfBounds;

            BurstSpawnEnemies();
            StartSpawnEnemies(true);
        }

        private void BurstSpawnEnemies()
        {
            foreach (SpawnParameters unit in enemies)
            {
                if (unit.SpawnUnitsOnStart > 0)
                {
                    for (int i = 0; i < unit.SpawnUnitsOnStart; i++)
                        SpawnNewObject(unit);
                }
            }
        }

        private IEnumerator SpawnEnemies(SpawnParameters unit)
        {
            while (true)
            {
                yield return new WaitForSeconds(unit.SpawnCooldown);

                SpawnNewObject(unit);
            }
        }

        private void SpawnNewObject(SpawnParameters unit)
        {
            float yPosition = screenBounds.min.y;/*Random.value > 0.5 ? screenBounds.min.y : screenBounds.max.y;*/
            float xPosition = Random.Range(screenBounds.min.x, screenBounds.max.x);

            Vector3 randomPosition = new Vector3(xPosition, yPosition);

            Enemy newEnemy = GetFreeEnemy(unit);
            newEnemy.transform.position = randomPosition;
            newEnemy.transform.rotation = Quaternion.identity;
            newEnemy.SetBusyState(true);           
        }

        public void StartSpawnEnemies(bool start)
        {
            if (start)
            {
                for (int i = 0; i < enemies.Count; i++)
                    spawnCoroutines.Add(StartCoroutine(SpawnEnemies(enemies[i])));
            }
            else
            {
                spawnCoroutines.ForEach(routine => StopCoroutine(routine));
            }
        }

        public Enemy GetFreeEnemy(SpawnParameters unit)
        {
            Enemy freeEnemy = unit.EnemiesPool.Find(enemy => !enemy.IsBusy);
            if (freeEnemy == null)
                freeEnemy = CreateNewEnemy(unit);

            return freeEnemy;
        }

        private Enemy CreateNewEnemy(SpawnParameters unit)
        {
            var enemyUnit = enemyStorage.GetEnemyUnit(unit.Type);

            Enemy newEnemy = Instantiate(enemyUnit.Prefabs[Random.Range(0, enemyUnit.Prefabs.Count)], dependencyContainer.ParticlePool.EnemyContainer);
            unit.EnemiesPool.Add(newEnemy);

            return newEnemy;
        }
    }

    [Serializable]
    public class SpawnParameters
    {
        [field: SerializeField] public EnemyType Type { get; private set; }
        [field: SerializeField, Range(0, 50)] public int SpawnUnitsOnStart { get; private set; }
        [field: SerializeField] public float SpawnCooldown { get; private set; }
        [field: SerializeField] public List<Enemy> EnemiesPool { get; private set; }
    }
}

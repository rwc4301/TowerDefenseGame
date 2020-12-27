using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Mission {
	public enum MissionType { Defense, Sabotage, Exterminate, Survival }

	public class MissionController : MonoBehaviour
	{
		[Serializable]
		public class TurretPositionDictionary : SerializableDictionary<Vector3Int, bool> { }

		public const int MAX_WBC = 10, ENEMIES_PER_SCENE = 4, SURVIVAL_TOTAL_LIVES = 10, SECONDS_BEFORE_WAVE = 5;

		public static MissionController current { get { return FindObjectOfType<MissionController>(); } }

		[SerializeField] private MissionType m_missionType = MissionType.Defense;
		[SerializeField] private int m_numWaves = 1, m_enemiesPerWaveConst = 0;
		[SerializeField] private int[] m_enemiesPerWave = new int[0];
		[SerializeField] private float m_countdownTimer = 0;
		[SerializeField] private Enemy m_enemyTargetObject = null;
		[SerializeField] private DefenseTarget m_defenseTargetObject = null;
		[SerializeField] private Vector3[] m_entryPoints = new Vector3[0], m_exitPoints = new Vector3[0];
		[SerializeField] private TurretPositionDictionary m_positionDictionary = new TurretPositionDictionary();

		private int m_whiteBloodCells, m_currentWave, m_enemiesThisWave, m_enemiesReachedExit;
		private bool m_waveComplete, m_allEnemiesInstantiated;

		public MissionType MissionType { get { return m_missionType; } }
		public Enemy PrimaryEnemyTarget { get { return m_enemyTargetObject; } }
		public DefenseTarget DefenseTarget { get { return m_defenseTargetObject; } }

		private List<ICard> CardDeck { get; set; }
		public Enemy[] EnemyTypes { get; set; }
		public int WBCPerSecond { get; set; }

		public Vector3[] ExitPoints { get { return m_exitPoints; } }

		public Vector3 RandomEntryTarget { get { return m_entryPoints[Random.Range(0, m_entryPoints.Length)]; } }
		public Vector3 RandomExitTarget { get { return m_exitPoints[Random.Range(0, m_exitPoints.Length)]; } }

		public event UpdateHandler OnUpdate;
		public event CardDeckChangedHandler OnCardDeckChanged;
		public event WBCCountChangedHandler OnWBCCountChanged;

		public delegate void UpdateHandler();
		public delegate void CardDeckChangedHandler(List<ICard> deck);
		public delegate void WBCCountChangedHandler(int wbc);

		private IEnumerator Start()
		{
			WBCPerSecond = 1;

			m_whiteBloodCells = 0;
			m_currentWave = 0;
			m_waveComplete = true;
			m_enemiesReachedExit = SURVIVAL_TOTAL_LIVES;

			switch (MissionType) {
				case MissionType.Defense:
					if (!m_defenseTargetObject)
						Debug.LogWarning("Defense mission will not start unless a turret target object is set in the mission controller.");
					else
						OnUpdate += DefenseUpdate;
					break;
				case MissionType.Exterminate:
					OnUpdate += ExterminateUpdate;
					break;
				case MissionType.Sabotage:
					if (!m_enemyTargetObject)
						Debug.LogWarning("Sabotage mission will not start unless an enemy target object is set in the mission controller.");
					else
						OnUpdate += SabotageUpdate;
					break;
				case MissionType.Survival:
					OnUpdate += SurvivalUpdate;
					break;
			}

			//update WBC count
			while (true) {
				WaitForSeconds delay = new WaitForSeconds(1);

				if (m_whiteBloodCells < MAX_WBC) {
					m_whiteBloodCells += WBCPerSecond;
					OnWBCCountChanged?.Invoke(m_whiteBloodCells);
				}

				yield return delay;
			}
		}

		protected virtual void DefenseUpdate()
		{
			if (m_waveComplete) {
				if (m_currentWave == m_numWaves) {
					GameManager.CompleteMission(true);
					OnUpdate -= DefenseUpdate;
				}
				else
					StartCoroutine(StartNextWave());
			}
			else if (!m_defenseTargetObject)
				GameManager.CompleteMission(false);
		}

		protected virtual void ExterminateUpdate()	//TODO: wave mechanic is such that stronger variations of enemies spawn each time
		{
			if (m_waveComplete) {
				if (m_currentWave == m_numWaves) {
					GameManager.CompleteMission(true);
					OnUpdate -= ExterminateUpdate;
				}
				else
					StartCoroutine(StartNextWave());
			}
			else if (!FindObjectOfType<Tower>() || m_enemiesReachedExit == 0)
				GameManager.CompleteMission(false);
		}

		protected virtual void SabotageUpdate()
		{
			//sabotage missions must be completed within a set time
			m_countdownTimer -= Time.deltaTime;

			if (m_waveComplete) {
				if (!m_enemyTargetObject) {
					GameManager.CompleteMission(true);
					OnUpdate -= SabotageUpdate;
				}
				else
					StartCoroutine(StartNextWave());
			}
			else if (m_countdownTimer <= 0)
				GameManager.CompleteMission(false);
		}

		protected virtual void SurvivalUpdate() //TODO: wave mechanic is such that stronger variations of enemies spawn each time
		{
			if (m_waveComplete) {
				if (m_currentWave == m_numWaves) {
					GameManager.CompleteMission(true);
					OnUpdate -= ExterminateUpdate;
				}
				else
					StartCoroutine(StartNextWave());
			}
			else if (!FindObjectOfType<Tower>() || m_enemiesReachedExit == 0)
				GameManager.CompleteMission(false);
		}

		private void Update()
		{
			OnUpdate?.Invoke();
		}

		public void AddWBC(int value)
		{
			m_whiteBloodCells += value;
		}

		public bool GetCanBuildOnPosition(int xPos, int zPos)
		{
			return m_positionDictionary != null && m_positionDictionary.ContainsKey(new Vector3Int(xPos, 0, zPos)) && m_positionDictionary[new Vector3Int(xPos, 0, zPos)];
		}

		public void SetCanBuildOnPosition(int xPos, int zPos, bool value)
		{
			if (m_positionDictionary.ContainsKey(new Vector3Int(xPos, 0, zPos))) {
				if (value)
					m_positionDictionary[new Vector3Int(xPos, 0, zPos)] = value;
				else
					m_positionDictionary.Remove(new Vector3Int(xPos, 0, zPos));
			}
			else if (value)
				m_positionDictionary.Add(new Vector3Int(xPos, 0, zPos), value);
		}

		public bool UseCard(ICard card, Vector3 targetPosition)
		{
			int index = CardDeck.IndexOf(card);

			if (index > 4)
				throw new Exception("Card is outside of the useable range");

			if (card.WBCCost > m_whiteBloodCells) {
				Debug.Log("White blood cell count is too low to instantiate " + card.Name);
				return false;
			}

			m_whiteBloodCells -= card.WBCCost;

			Instantiate(card.gameObject, targetPosition, Quaternion.identity);

			if (!card.OneTimeUse)	//reinsert the used card at the bottom of the deck
				CardDeck.Insert(Random.Range(7, CardDeck.Count), card);

			//move the next card (index 5) to the index
			CardDeck[index] = CardDeck[5];
			CardDeck.RemoveAt(5);

			OnCardDeckChanged?.Invoke(CardDeck);

			return true;
		}

		public void SetCardDeck(List<ICard> cards)
		{
			CardDeck = cards;
			OnCardDeckChanged?.Invoke(CardDeck);
		}

		private IEnumerator StartNextWave()
		{
			m_waveComplete = false;
			m_currentWave++;

			if (m_currentWave <= m_numWaves) {

				yield return new WaitForSeconds(SECONDS_BEFORE_WAVE);

				//should we use (m_enemiesPerWaveConst * m_currentWave) instead?
				for (int i = 0; i < m_enemiesPerWave[m_currentWave - 1]; i++) {   //spawn number of enemies in this wave, choose randomly from each enemy type and entry target
					if (EnemyTypes == null || EnemyTypes.Length == 0) {
						Debug.LogWarning("Mission controller has no enemy prefabs to instantiate.");
						yield break;
					}

					Enemy e = Instantiate(EnemyTypes[Random.Range(0, EnemyTypes.Length)], RandomEntryTarget, Quaternion.identity);

					e.OnDestroyed += () =>  //this is called when an enemy is destroyed; if all enemies in the wave have been destroyed, the wave is complete
					{
						m_enemiesThisWave--;

						if (m_allEnemiesInstantiated && m_enemiesThisWave == 0)
							m_waveComplete = true;
					};
					e.OnReachedExit += () =>    //this is called when an enemy reaches an exit point on the map, in survival mode the player's lives decrease by 1
					{
						m_enemiesReachedExit--;
					};
					m_enemiesThisWave++;

					yield return new WaitForSeconds(0.5f);
				}

				m_allEnemiesInstantiated = true;
			}
		}
	}
}
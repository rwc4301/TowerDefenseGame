using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Mission;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using BodyOverview;
using Newtonsoft.Json.Linq;
using System;
using Random = UnityEngine.Random;
using TDGame.UI;

[Serializable] public class CardList : SerializableInterfaceList<ICard> { }

public class GameManager : MonoBehaviour
{
	public const string SCENE_BODYOVERVIEW = "BodyOverview";
	private const int XP_SUCCESS = 100, XP_FAILURE = 50;

	private static GameManager instance;
	private static int m_XP = 0, m_level = 0;
	//private static Inventory m_inventory = new Inventory();
	private static List<Enemy> m_enemies = new List<Enemy>();
	private static Dictionary<ICard, int> m_inventory = new Dictionary<ICard, int>();

	[SerializeField] private SelectMissionScreen m_selectMissionScreen;
	[SerializeField] private CompleteMissionScreen m_completeMissionScreen;
	[SerializeField] private PharmacyScreen m_pharmacyScreen;

	[SerializeField] public CardList testing_inventory;

	public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
	{
		ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
		NullValueHandling = NullValueHandling.Ignore
	};

	private void Awake()
	{
		if (!instance)
			instance = this;
		DontDestroyOnLoad(this);

		for (int i = 0; i < testing_inventory.Count; i++) {
			if (m_inventory.ContainsKey(testing_inventory.GetT(i)))
				m_inventory[testing_inventory.GetT(i)]++;
			else
				m_inventory.Add(testing_inventory.GetT(i), 1);
		}

		SaveGame();
	}

	public static Enemy[] GetEnemiesForMission(MissionType type, EnemyType[] enemyTypes)
	{
		Enemy[] enemies = new Enemy[enemyTypes.Length];

		switch (type) {
			case MissionType.Defense:
				for (int i = 0; i < enemies.Length; i++) {
					var select = m_enemies.Where(x => x.Type == enemyTypes[i] && x.Level <= m_level);
					enemies[i] = select.ElementAt(Random.Range(0, select.Count()));
				}
				return enemies;

		}
		return null;
	}

	public static void ShowMissionPopupScreen(MissionNode mission)
	{
		var popup = Instantiate(instance.m_selectMissionScreen, FindObjectOfType<Canvas>().transform);

		popup.transform.Find("MissionName").GetComponent<Text>().text = mission.Name;
		popup.transform.Find("MissionDescription").GetComponent<Text>().text = mission.Description;

		for (int i = 0; i < MissionController.ENEMIES_PER_SCENE; i++) {
			popup.transform.Find("Enemy" + i).GetComponentInChildren<Text>().text = "Enemy";
		}

		popup.transform.Find("Play").GetComponent<Button>().onClick.AddListener(delegate { LoadMission(mission.SceneName, mission.GetComponent<Testing_MissionEnemyList>()?.enemies, new List<ICard>()); });
	}

	public static void CompleteMission(bool success)
	{
		GameObject popup = Instantiate(Resources.Load<GameObject>("UI/MissionCompletePopup"));

		popup.transform.Find("SuccessText").GetComponent<Text>().text = success ? "Success" : "Failure";
		popup.transform.Find("XPText").GetComponent<Text>().text = "Earned XP: " + (success ? XP_SUCCESS : XP_FAILURE);
		popup.transform.Find("ButtonFinish").GetComponent<Button>().onClick.AddListener(LoadBodyOverviewScreen);

		if (success) {
			popup.transform.Find("ButtonOpen").GetComponent<Button>().onClick.AddListener(NaniteBlock.CreateRandom);
		}
		else {
			popup.transform.Find("NaniteChestImage").gameObject.SetActive(false);
			popup.transform.Find("ButtonOpen").gameObject.SetActive(false);
		}

		m_XP += success ? XP_SUCCESS : XP_FAILURE;
	}

	public static void LoadMission(string scene, Enemy[] enemies, List<ICard> playerDeck)
	{
		instance.StartCoroutine(LoadMission_Internal(scene, enemies, playerDeck));
	}

	public static void LoadBodyOverviewScreen()
	{
		instance.StartCoroutine(LoadBodyOverviewScreen_Internal());
	}

	private static void SaveGame()
	{
		BodyOverviewController controller = FindObjectOfType<BodyOverviewController>();
		if (!controller) {
			Debug.LogWarning("Body overview screen must be loaded before game can be saved.");
			return;
		}

		var gamedata = new
		{
			XP = m_XP,
			Level = m_level,
			Inventory = m_inventory.ToDictionary(x => x.Key.name, x => x.Value),
			Game = controller
		};

		using (StreamWriter stream = new StreamWriter(Application.persistentDataPath + "/game.sav")) {
			stream.WriteLine(JsonConvert.SerializeObject(gamedata, Formatting.Indented, serializerSettings));
		}
	}

	private static void LoadGame()
	{
		BodyOverviewController controller = FindObjectOfType<BodyOverviewController>();
		if (!controller) {
			Debug.LogWarning("Body overview screen must be loaded before save file can be deserialized.");
			return;
		}

		StreamReader stream = new StreamReader(Application.persistentDataPath + "/game.sav");
		try {
			JObject gamedata = JsonConvert.DeserializeObject<JObject>(stream.ReadToEnd());

			m_XP = gamedata["XP"].ToObject<int>();
			m_level = gamedata["Level"].ToObject<int>();
			//m_inventory = gamedata["Inventory"].ToObject<Dictionary<ICard, int>>();

			JsonConvert.PopulateObject(gamedata["Game"].ToString(), controller);
		}
		catch (Exception) { Debug.LogWarning("Couldn't open save file."); }
		finally {
			stream?.Dispose();
		}
	}

	private static IEnumerator LoadMission_Internal(string scene, Enemy[] enemies, List<ICard> playerDeck)
	{
		SaveGame();

		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

		yield return operation;

		var controller = FindObjectOfType<MissionController>();
		controller.EnemyTypes = enemies;
		controller.SetCardDeck(playerDeck);
	}

	private static IEnumerator LoadBodyOverviewScreen_Internal()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(SCENE_BODYOVERVIEW);

		yield return operation;

		var controller = FindObjectOfType<BodyOverviewController>();
		controller.ReturnFromMission();

		SaveGame();
	}

	/// <summary>
	/// If the game manager starts at the same time as the body overview controller, load from game file
	/// </summary>
	private void Start()
	{
		if (FindObjectOfType<BodyOverviewController>())
			LoadGame();
	}
}
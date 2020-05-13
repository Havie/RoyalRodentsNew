using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    //Make a singleton
    private static TutorialController _instance;

    public GameObject TutorialButtonPrefab;

	//Tutorial Flags
	private bool hasCompletedGeneralTut = false, hasCompletedBattleTut = false, hasCompletedStaminaTut = false; //Has or has not gotten to the end of tutorial
	private bool hasStartedGeneralTut = false, hasStartedBattleTut = false, hasStartedStaminaTut = false;

	//private bool generalFlag, battleFlag, staminaFlag; //if true, it has been triggered to do the tutorial
	private bool flag = false;

	private int tutorialNum = 0, pageNum = 0;
	private string[] generalTutorialStrings = new string[17];
	private string[] battleTutorialStrings = new string[4];
	private string[] staminaTutorialStrings = new string[2];

	public void Start()
	{
		StartTutorial(0);
	}


	//Create Instance singleton
	public static TutorialController Instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<TutorialController>();
			return _instance;
		}
	}
	private void Awake()
	{
		if (_instance == null)
		{
			//if not, set instance to this
			_instance = this;
		}
		//If instance already exists and it's not this:
		else if (_instance != this)
		{
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	public void IncrementPage()
	{
		pageNum++;

		if (tutorialNum == 0)
		{
			if (pageNum > generalTutorialStrings.Length - 1)
			{
				hasCompletedGeneralTut = true;
			}
			else
				CreateTutorialButton();
		}
		else if (tutorialNum == 1)
		{
			if (pageNum > battleTutorialStrings.Length - 1)
			{
				hasCompletedBattleTut = true;
			}
			else
				CreateTutorialButton();
		}
		else if (tutorialNum == 2)
		{
			if (pageNum > staminaTutorialStrings.Length - 1)
			{
				hasCompletedStaminaTut = true;
			}
			else
				CreateTutorialButton();
		}
		
	}

	private void StartTutorial(int tutNum)
	{
		tutorialNum = tutNum;
		pageNum = 0;
		InitializeTutorial(tutorialNum);

		if (tutorialNum == 0)
		{
			hasStartedGeneralTut = true;
		}
		else if (tutorialNum == 1)
		{
			hasStartedBattleTut = true;
		}
		else if (tutorialNum == 2)
		{
			hasStartedStaminaTut = true;
		}

		CreateTutorialButton();
	}

	private void CreateTutorialButton()
	{
		string str;

		if (tutorialNum == 0)
		{
			str = generalTutorialStrings[pageNum];
		}
		else if (tutorialNum == 1)
		{
			str = battleTutorialStrings[pageNum];
		}
		else if (tutorialNum == 2)
		{
			str = staminaTutorialStrings[pageNum];
		}
		else
			str = "nUll eRRoR ubuh";

		//Create New Tutorial Button Object
		GameObject iii = Instantiate(TutorialButtonPrefab, transform.position, transform.rotation);
		iii.transform.SetParent(gameObject.transform);
		TutorialButton scr = iii.GetComponent<TutorialButton>();
		if (scr)
			scr.setButton(str);
	}

	private void InitializeTutorial(int tutNum)
	{
		//0 = General Tutorial
		if (tutNum == 0)
		{
			generalTutorialStrings[0] = "Welcome to Royal Rodents! And welcome to your own backyard kingdom!";
			generalTutorialStrings[1] = "This game is about constructing your base and recruiting rodents to work for you.";
			generalTutorialStrings[2] = "There is a crown on your head. When you die, you lose your crown. Don’t lose your crown!";
			generalTutorialStrings[3] = "Your goal is to raid neighboring kingdoms and defeat the enemy king to steal their crown.";
			generalTutorialStrings[4] = "Steal two enemy crowns and you win!";
			generalTutorialStrings[5] = "To start you off, you should recruit a rodent. Tap the screen to walk to a rodent.";
			generalTutorialStrings[6] = "Rodents that are not wearing a hat are available to recruit. Click an available rodent to recruit.";
			generalTutorialStrings[7] = "Recruiting costs Food! You can find Food in a Garbage Can. ";
			generalTutorialStrings[8] = "Search the area until you find a Garbage Can. Walk over to the Garbage Can and click it to search.";
			generalTutorialStrings[9] = "Keep clicking to keep searching, but be careful! Searching uses up Stamina!";
			generalTutorialStrings[10] = "Garbage Cans also give Trash! Trash is a building material. Other building materials include Wood, Stone, and Shinies.";
			generalTutorialStrings[11] = "Click a dirt mound to construct with your materials.";
			generalTutorialStrings[12] = "To see what each building does, look in your Guide Book.";
			generalTutorialStrings[13] = "You must assign the rodent to construct the building. Click the Assignment Mode button on the bottom left.";
			generalTutorialStrings[14] = "Drag the rodent to the assignment bubble to assign them to start construction.";
			generalTutorialStrings[15] = "Assigning rodents to gatherable buildings like Farms and Garbage Cans increments your resources over time.";
			generalTutorialStrings[16] = "You have completed the General Tutorial! Look in your Guide Book if you need more help. See you next time!";
		}
		//1 = Battle Tutorial
		else if (tutNum == 1)
		{
			battleTutorialStrings[0] = "At night you could be raided by enemy rodents from both sides.";
			battleTutorialStrings[1] = "Defend your kingdom or choose to raid your neighboring territory at night.";
			battleTutorialStrings[2] = "Click the fences at the edges of the map to select which Outposts you want to take to battle.";
			battleTutorialStrings[3] = "Blue indicates player zone. White is neutral zone. Red is enemy zone.";
		}
		//2 = Stamina Tutorial
		else if (tutNum == 2)
		{
			staminaTutorialStrings[0] = "Click the yellow food button to increase your stamina.";
			staminaTutorialStrings[1] = "Searching, fighting, and digging uses stamina, so always have some Food on you.";
		}
	}
}

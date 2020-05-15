using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationFeed : MonoBehaviour
{
	//Make a singleton
	private static NotificationFeed _instance;

	public GameObject NotificationPrefab;

	public static int iconSpritesSize = 1;
    public Sprite[] iconSprites = new Sprite[iconSpritesSize];

	//Create Instance singleton
	public static NotificationFeed Instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<NotificationFeed>();
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

		//DontDestroyOnLoad(gameObject);
	}

	public void NewNotification(string title, string des, int iconIndex, float posX)
    {
		//If Other Notifications Exist, Destroy Them
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		//Create New Notification Object
		GameObject iii = Instantiate(NotificationPrefab, transform.position, transform.rotation);
        iii.transform.SetParent(gameObject.transform);
		NotificationButton scr = iii.GetComponent<NotificationButton>();
        if (scr)
            scr.setButton(title, des, iconSprites[iconIndex], posX);
    }
}

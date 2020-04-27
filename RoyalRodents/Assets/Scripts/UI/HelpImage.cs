using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpImage : MonoBehaviour
{
	void OnEnable()
	{
		transform.localPosition = new Vector3(469, transform.localPosition.y, transform.localPosition.z);
	}
}

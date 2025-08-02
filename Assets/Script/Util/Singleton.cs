using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{

	private static T _instance = null;
	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				if (FindObjectOfType<T>() != null)
					_instance = FindObjectOfType<T>();
				//else
				//	new GameObject().AddComponent<T>().name = "Singleton_"+  typeof(T).ToString();
			}

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance != null && _instance.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
		{
			Debug.LogError("Singleton already exist "+ _instance.gameObject.name);
			Destroy(this.gameObject);
		}
		else
			_instance = this.GetComponent<T>();
	}

}


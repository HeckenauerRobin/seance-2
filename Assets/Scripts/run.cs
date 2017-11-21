using UnityEngine;
using System.Collections;
using System.IO; 

public class run : MonoBehaviour {

	public TextAsset st_helens;
	public GameObject point;


	private float approximation;
	private GameObject clone;

	void Start () {
		string[] lignes = st_helens.text.Split('\n');
		string[] caracteristiques;

		approximation = 80.0f;

		for(int i=0;i<lignes.Length; i++) {
			caracteristiques = lignes[i].Split(' ');

			clone = Instantiate(point, new Vector3(float.Parse(caracteristiques[0])/approximation, float.Parse(caracteristiques[1])/approximation, float.Parse(caracteristiques[2])/approximation), Quaternion.identity) as GameObject;
			clone.transform.GetChild (0).GetComponent<Renderer> ().material.SetColor ("_Color", new Color (int.Parse (caracteristiques [3]) * 1.0f, int.Parse (caracteristiques [4]) * 1.0f, int.Parse (caracteristiques [5]) * 1.0f));
			Debug.Log (clone.transform.GetChild (0).GetComponent<Renderer> ().material.color);
			clone.transform.parent = GameObject.Find("nuage").transform;
		}
	}
}

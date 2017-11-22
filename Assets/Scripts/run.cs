using UnityEngine;
using System.Collections;
using System.IO;
using System.Globalization;
using System;

public class run : MonoBehaviour {

    // objets donnés par l'utilisateur
	public TextAsset st_helens;
	public GameObject point;
	public GameObject boite_anglobante;
    public Material material;
    public float approximation; // par soucis de performance on réduit l'échelle du nuage de point

    // attributs privés
    private NumberStyles styles;
    private IFormatProvider provider;
    private float min_x, min_y, min_z,  max_x, max_y, max_z;
    private bool pointsPretAEtreRelie = false;
    


    // fonction lancée au démarage
    void Start () {
        // tableaux utilisés pour récupérer le contenu du fichier texte
        string[] lignes = st_helens.text.Split('\n');
		string[] caracteristiques;
        
        // on parcourt chaque point
        for (int i=0;i<lignes.Length; i++) {
			caracteristiques = lignes[i].Split(' ');

            // récupération des minima et maxima de x, y, z (utile pour calculer la boite englobante)
            if(i == 0) {
                min_x = float.Parse(caracteristiques[0]);
                max_x = float.Parse(caracteristiques[0]);
                min_y = float.Parse(caracteristiques[1]);
                max_y = float.Parse(caracteristiques[1]);
                min_z = float.Parse(caracteristiques[2]);
                max_z = float.Parse(caracteristiques[2]);
            } else {
                if (float.Parse(caracteristiques[0]) < min_x) min_x = float.Parse(caracteristiques[0]);
                if (float.Parse(caracteristiques[0]) > max_x) max_x = float.Parse(caracteristiques[0]);
                if (float.Parse(caracteristiques[1]) < min_y) min_y = float.Parse(caracteristiques[1]);
                if (float.Parse(caracteristiques[1]) > max_y) max_y = float.Parse(caracteristiques[1]);
                if (float.Parse(caracteristiques[2]) < min_z) min_z = float.Parse(caracteristiques[2]);
                if (float.Parse(caracteristiques[2]) > max_z) max_z = float.Parse(caracteristiques[2]);
            }

            // on créée un point en lui affectant une position x, y, z
            GameObject clone = Instantiate(point, new Vector3(float.Parse(caracteristiques[0]) /approximation, float.Parse(caracteristiques[1]) /approximation, float.Parse(caracteristiques[2]) /approximation), Quaternion.identity) as GameObject;
            // on affecte une couleur au point
            clone.transform.GetComponent<Renderer>().material.SetColor("_Color", new Color(float.Parse(caracteristiques[3]), float.Parse(caracteristiques[4]), float.Parse(caracteristiques[5])));
            // un point est une partie d'un nuage (permet d'organiser le plan de travail)
            clone.transform.parent = GameObject.Find("nuage").transform;

            // affichage console d'un point avec ses coordonnées et sa couleur
            Debug.Log("Coordonnées [" + caracteristiques[0] + "; " + caracteristiques[1] + "; " + caracteristiques[2] + "] Couleurs [" + caracteristiques[3] + "; " + caracteristiques[4] + "; " + caracteristiques[5] + "]");
        }

        // créer la boite anglobante
        GameObject boite = Instantiate(boite_anglobante, new Vector3((min_x/approximation) + ((max_x - min_x) / 100.0f)/2, (min_y / approximation) + ((max_y - min_y) / 100.0f) / 2, (min_z / approximation) + ((max_z - min_z) / 100.0f) / 2), Quaternion.identity) as GameObject;
        boite.transform.localScale = new Vector3((max_x - min_x) / 100.0f, (max_y - min_y) / 100.0f, (max_z - min_z) / 100.0f);

        // affichage des minima et maxima de x, y, z
        Debug.Log("Minima [" + min_x + "; " + min_y + "; " + min_z + "] Maxima [" + max_x + "; " + max_y + "; " + max_z +"]");

        // étape de création des points terminée, on peut maintenant relier les points entre eux
        // pointsPretAEtreRelie = true;

        // on créée le fichier obj
        creerFichierOBJ();
    }

    // fonction lancée à chaque frame qui permet la création d'éléments graphiques
    private void OnDrawGizmos() {
        if (pointsPretAEtreRelie) {
            GL.PushMatrix();
            GL.Begin(GL.TRIANGLES);
            material.SetPass(0);
            GL.Vertex3(transform.GetChild(0).position.x, transform.GetChild(0).position.y, transform.GetChild(0).position.z);
            GL.Vertex3(transform.GetChild(1).position.x, transform.GetChild(1).position.y, transform.GetChild(1).position.z);
            GL.Vertex3(transform.GetChild(2).position.x, transform.GetChild(2).position.y, transform.GetChild(2).position.z);
            GL.End();
            GL.PopMatrix();
        }
    }

    // creation d'un fichier qui contiendra les figure précédente format obj
    private void creerFichierOBJ() {
        // en-tête
        string texte = "# fichier obj créer depuis le code\n\n";

        // v : sommet
        for (int i = 0; i < transform.childCount; i++) {
            texte += "v  " + (transform.GetChild(i).position.x * approximation)+ "  " + (transform.GetChild(i).position.y * approximation) + "  " + (transform.GetChild(i).position.z * approximation) + "\n";
        }
        texte += "\n";

        // vt : coordonnée de texture
        texte += "vt  0.0  0.0\n";
        texte += "\n";

        // vn : normale
        texte += "vn  0.0  0.0  0.0\n";
        texte += "\n";

        // f : face
        for (int i = 0; i < transform.childCount - 2; i++) {
            texte += "f  " + (i+1) + "/1/1  " + (i+2) + "/1/1  " + (i+3) + "/1/1\n";
        }

        // affichage du fichier obj dans la console
        Debug.Log(texte);

        // créer le fichier et le remplir avec le contenu
        StreamWriter sw = new StreamWriter("Assets\\Ressources\\obj\\out.obj");
        sw.WriteLine(texte);
        sw.Close();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEngine.UI;

/// <summary>
/// The square script should be attached to a gameobject, there should be 64, they should be placed and named according to where on the board theyre located
/// </summary>
public class cgSquareScript : MonoBehaviour
{
    /// <summary>
    /// The unique name is used to extensively to place pieces on their correct square, the name should always be marked correctly.
    /// </summary>
    public string uniqueName;
    public Color startColor;
    public Color recentMoveColor = Color.red;
    public Color legalMoveToColor = Color.magenta;

    // Use this for initialization
    void Awake()
    {
        startColor = GetComponent<SpriteRenderer>().color;
    }

    /// <summary>
    /// This adds a small text on the location specifying the name in both index and coordinate form.
    /// </summary>
    public void addDebugText(int i = -1)
    {
        GameObject gobj = new GameObject("Text");
        gobj.transform.SetParent(this.transform.root);
        //gobj.transform.localPosition.Set(0, 0, 0);
        gobj.transform.position = Camera.main.WorldToViewportPoint(this.transform.position);
        gobj.transform.position = new Vector3(gobj.transform.position.x - .02f, gobj.transform.position.y + .05f, gobj.transform.position.z);
        gobj.AddComponent<Text>();
        gobj.GetComponent<Text>().text = uniqueName+" "+i;
        gobj.GetComponent<Text>().color = Color.yellow;
    }

    /// <summary>
    /// Temporarily highlight this square with provided color.
    /// </summary>
    /// <param name="highlightColor"></param>
    public void highlightTemporarily(Color highlightColor)
    {
        //if (highlightColor == null) highlightColor = new Color(0, .5f, 0, .5f);
        StartCoroutine(highlighterTimer(highlightColor));
    }
    IEnumerator highlighterTimer(Color hightlightColor)
    {
        GetComponent<SpriteRenderer>().color = hightlightColor;
        yield return new WaitForSeconds(5f);
        GetComponent<SpriteRenderer>().color = startColor;
    }

    /// <summary>
    /// Change the color of this square.
    /// </summary>
    /// <param name="color"></param>
    public void changeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}

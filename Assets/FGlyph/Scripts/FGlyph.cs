using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGlyph
{
    //Contains the drawing / glyph under the form of x-y coordinates
    public List<List<Vector2>> coords;

    //Alternative for more accurate matching using the effective (higher cpu cost) matching algorithm
    //Indicates if a Vector2 point has already been returned as the closest point to a point of a preset, and if it has been, it shall not be available anymore for calculations.
    public bool[] beenChecked;
    //Used for the effecient matching algorithm, indicates how many points are in a specific square
    public int[,] squares;
    //Name that defines the shape / glyph
    public string name;
    //Description that gives information regarding the shape / glyph *optional field*
    public string description;
    //Sets each drawing to have the EXACT number of coordinates. Higher number --> Higher accuracy --> greater CPU cost
    public int pointsCap;
    //Sets each drawing to be devided into the EXACT number of squares. Higher number --> higher accuracy --> greater CPU cost
    public int squareAccuracy;
    //Number of strokes of drawing
    public int numberOfStrokes;

    //Determines whether drawing will be centered before normalized or not
    bool centralizedNormalization;
    public int numberOfSquaresFilled;

    // Public texture drawing the user drawn glyph on the GUI   
    public Texture2D texture;

    //Constructor that initalizez a glyph and prepares it for calculations.
    public FGlyph(string _name, string _description, int _pointsCap, int _squareAccuracy,int _numberOfStrokes,bool _centralizedNormalization, List<List<Vector2>> _coords)
    {
        coords = _coords;
        name = _name;
        description = _description;
        pointsCap = _pointsCap;
        squareAccuracy = _squareAccuracy;
        numberOfStrokes = _numberOfStrokes;
        centralizedNormalization = _centralizedNormalization;
        texture = new Texture2D(100, 100);

        beenChecked = new bool[pointsCap];
        squares = new int[squareAccuracy,squareAccuracy];
        ResetChecking();

        SetTextureToBlack();
        CalculateTextureEffective();
        NormalizeTexture();
        
        
        CountSquares();

    }

    /// <summary>
    /// Method used to reset the bool aray after a preset has been compared
    /// </summary>
    public void ResetChecking()
    {
        for (int i = 0; i < beenChecked.Length; i++)
        {
            beenChecked[i] = false;
        }
    }
    /// <summary>
    /// IF a drawing does not have exactly the SAME EXACT NUMBER of coordinates, this method will either smooth or reduce the complexity of the drawing by adding or removing coordinates
    /// </summary>
    public void CalculateTextureEffective()
    {
        //Draws with white pixels the userdrawn Glyph
        foreach (var item in coords)
        {
            for (int i = 0; i < item.Count; i++)
            {
                int x = (int)(item[i].x / Screen.width * texture.width);
                int y = (int)(item[i].y / Screen.height * texture.height);

                texture.SetPixel(x, y, Color.white);
            }
            texture.Apply();
        }

        //If the drawing has less points, than it's supposed to, this while sequence will add points until the cap is reached.
        //Added points are drawn with green on the texture
        foreach (var item in coords)
        {
            try
            {
                //  Debug.Log("Pointscap / numberOfStroke: " + pointsCap + " / " + numberOfStrokes);
                while (item.Count < pointsCap / numberOfStrokes)
                {
                    int index = 0;
                    for (int i = 0; i < item.Count - 1; i++)
                    {
                        if (Vector2.Distance(item[i], item[i + 1]) > Vector2.Distance(item[index], item[index + 1]))
                            index = i;
                    }
                    // Debug.Log("Count: " + item.Count + " Coords " +item[index].x + " " + item[index + 1].x + " " + item[index].y + " " + item[index + 1].y);
                    int x = (int)((item[index].x + item[index + 1].x) / 2.0f);
                    int y = (int)((item[index].y + item[index + 1].y) / 2.0f);

                    item.Insert(index + 1, new Vector2(x, y));

                    x = (int)(item[index + 1].x / Screen.width * texture.width);
                    y = (int)(item[index + 1].y / Screen.height * texture.height);
                    texture.SetPixel(x, y, Color.green);
                }
                // Same sequence, but this time it will remove points until the cap is reached.
                //Remove points are drawn with red on the texture.
                while (item.Count > pointsCap / numberOfStrokes)
                {
                    int index = 0;
                    for (int i = 0; i < item.Count - 2; i++)
                    {
                        if (Vector2.Distance(item[i], item[i + 2]) < Vector2.Distance(item[index], item[index + 2]))
                            index = i;
                    }
                    int x = (int)(item[index + 1].x / Screen.width * texture.width);
                    int y = (int)(item[index + 1].y / Screen.height * texture.height);
                    texture.SetPixel(x, y, Color.red);
                    item.RemoveAt(index + 1);

                }
            }
            catch(Exception ex)
            {
                Debug.Log("Exception triggered, continuing to match " + ex.Message);
            }
        }
        texture.Apply();
        
    }
    /// <summary>
    /// Normalizez the coordinates, centreing the drawing and scaling it to 1:1 ratio
    /// </summary>
    public void NormalizeTexture()
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        // Fetch minimum values
        foreach (var item in coords)
        {
            foreach (Vector2 vector in item)
            {
                if (vector.x < minX)
                {
                    minX = vector.x;
                }
                if (vector.x > maxX)
                {
                    maxX = vector.x;
                }
                if (vector.y < minY)
                {
                    minY = vector.y;
                }
                if (vector.y > maxY)
                {
                    maxY = vector.y;
                }
            }
        }
        foreach (var item in coords)
        {
            float xRange = maxX - minX;
            float yRange = maxY - minY;
            float maxRange = Mathf.Max(xRange, yRange);
            for (int i = 0; i < item.Count; i++)
            {
                float x, y;
                //Without centering
                if (centralizedNormalization == false)
                {
                    x = (item[i].x - minX) / (maxX - minX);
                    y = (item[i].y - minY) / (maxY - minY);

                }
                //With Centering
                else
                {
                    if (xRange > yRange)
                    {
                        x = (item[i].x - minX) / maxRange;
                        y = (item[i].y - minY) / maxRange + 0.5f - yRange / maxRange / 2f;
                    }
                    else
                    {
                        x = (item[i].x - minX) / maxRange + 0.5f - xRange / maxRange / 2f;
                        y = (item[i].y - minY) / maxRange;
                    }
                }
                item[i] = new Vector2(x, y);
                texture.SetPixel((int)(x * texture.width), (int)(y * texture.height), Color.yellow);
            }
        }
        texture.Apply();
    }
 
    /// <summary>
    /// Initliazes the suares[,] two dimensional array, counting how many pixels are to be found in 1 square
    /// </summary>
    public void CountSquares()
    {
        //if (name == "UserGlyph")
        //GameManager.DebugApp("", "squares.X.Length: " + squares.Length);
        
        foreach (var item in coords)
        {
            for (int i = 0; i < item.Count; i++)
            {
                int Xindex = (int)(item[i].x * squareAccuracy);
                int Yindex = (int)(item[i].y * squareAccuracy);
                if (Xindex == squareAccuracy)
                    Xindex--;
                if (Yindex == squareAccuracy)
                    Yindex--;

                //if (name == "UserGlyph")
                //    GameManager.DebugApp("", "Xindex: " + Xindex  + " Yindex: " + Yindex);
                squares[Xindex, Yindex]++;
            }
        }
        for (int i = 0; i < squareAccuracy; i++)
        {
            for (int j = 0; j < squareAccuracy; j++)
            {
                if (squares[i, j] != 0)
                    numberOfSquaresFilled++;
            }
        }
    }

    //Sets all pixels in the texture to black
    public void SetTextureToBlack()
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, Color.black);
            }
        }
        texture.Apply();
    }
    //Clears all cords in the preset and sets the texture to black
    public void Clear()
    {
        coords.Clear();
        SetTextureToBlack();
    }

    // Overridden ToString() method to return the object as a hard copy to be pasted in the Start() function
    override
    public string ToString()
    {
        Debug.Log("Entered manual ToSTring");
        string s = "presets.Add(new FGlyph(\"" + name + "\",\"description\"," + pointsCap.ToString() +"," +squareAccuracy + ", " + numberOfStrokes +",centralizedNormalization" + ", new List<List<Vector2>>() { ";
        foreach (var item in coords)
        {
            s += " new List<Vector2>() { ";
            foreach (Vector2 v in item)
            {
                s += "new Vector2(" + v.x.ToString().Replace(",",".") + "f," + v.y.ToString().Replace(",", ".") + "f),";
            }
            s = s.Remove(s.Length - 1);
            s += "},";
        }
        s = s.Remove(s.Length - 1);
        s += "}));\n";
        return s;
    }
    /// <summary>
    /// Used for a different idea of matching algorithm, has been dropped off
    /// </summary>
    public void SortPoints()
    {
        foreach (var item in coords)
        {
            item.Sort(new FGlyphComparerONX());
        }
    }
}
public class FGlyphComparerONX : IComparer<Vector2>
{
    public int Compare(Vector2 v1, Vector2 v2)
    {
        if (avgDistanceToCorners(v1) > avgDistanceToCorners(v2))
        {
            return 1;
        }
        else if (avgDistanceToCorners(v1) < avgDistanceToCorners(v2))
        {
            return -1;
        }
        else
            return 0;
        
    }
    private float avgDistanceToCorners (Vector2 v)
    {
        return (Vector2.Distance(v, new Vector2(0, 1)) + Vector2.Distance(v, new Vector2(1, 0)) + Vector2.Distance(v, new Vector2(1, 1)) ) / 3f;
    }
}

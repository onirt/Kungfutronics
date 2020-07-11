using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameManager : MonoBehaviour, IDebug
{

    public string filterTag = "MUSIC";
    [SerializeField]
    protected int _level = 0;
    public int level
    {
        get { return _level; }
        set
        {
            //bool change = _level != value;
            _level = value;
            /*if (change) {
                fogColor = configure.SetEnviroment();
                //if (_level == 0) configure.LoadFromFile();
            }*/
        }
    }
    public static GameManager _obj;

    public static string APPTAG = "Kungfutronics";
    public virtual void Print(string tag, string message)
    {
        //if (filterTag == "" || filterTag == tag)
        //{
            Debug.Log("[" + APPTAG + ":" + tag + "]: " + message);
            //SetMessage(message);
        //}
    }

    public static void DebugApp( string message)
    {
        DebugApp("", message);
    }

    public static void DebugApp(string tag, string message)
    {
        _obj.Print(tag, message);
    }
    public abstract void SetPoints(int newpoints);
    public abstract void SetMessage(string message);
}

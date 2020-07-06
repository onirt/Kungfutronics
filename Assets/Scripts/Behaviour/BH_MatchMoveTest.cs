using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BH_MatchMoveTest : MonoBehaviour
{
    MeshRenderer _render;
    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<MeshRenderer>();
    }
    private void OnEnable()
    {

        VRGlyphInput.OnMatchResult += MatchResult;
    }
    private void OnDisable()
    {

        VRGlyphInput.OnMatchResult -= MatchResult;
    }


    public void MatchResult(string stringResult, float matchResult, float ms)
    {
        GameManager.DebugApp("MatchMove" + stringResult);
        _render.material.color = Color.green;
        transform.localScale = new Vector3(.5f,.5f,.5f);
        StartCoroutine(DelayOff());
    }
    IEnumerator DelayOff()
    {
        yield return new WaitForSeconds(1);
        _render.material.color = Color.white;
        transform.localScale = Vector3.one;
    }
}

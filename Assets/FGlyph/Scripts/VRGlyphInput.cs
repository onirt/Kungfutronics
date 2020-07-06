using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class VRGlyphInput : MonoBehaviour
{

    public delegate void MatchResult(string stringResult, float matchResult, float ms);
    public static event MatchResult OnMatchResult;


    [SerializeField]
    private Texture2D[] presetTextures;

    // Use this for initialization
    private enum AlgorithmType { Efficient, Effective };
    private enum NumberOfStrokes { Single, Multiple };

    [SerializeField]
    [Tooltip("Effective: Uses more processing but has better matching results\n Efficient: Uses less processing but provides less accurate results.")]
    private AlgorithmType selectedAlgorithmType;

    [SerializeField]
    [Tooltip("Single: Matching will start once mouse is released. \n Multiple: Matching must be called be developer when he decides user has finished drawing.")]
    private NumberOfStrokes selectedNumberOfStrokes;

    [SerializeField]
    [Tooltip("True: Centers image before normalization. \nFalse: Simple normalization.")]
    private bool centralizedNormalization;

    [SerializeField]
    [Tooltip("Number of minimal distance in pixel required between two coordinates to register coordinates.")]
    private float spammThreshhold = 3;
    private Vector2[] lastPos;
    [SerializeField]
    [Tooltip("Fixed number of coordinates that defines a glyph. Higher values increase accuracy but also processing.")]
    private int pointsCap = 50;
    [SerializeField]
    [Tooltip("Used to define the accuracy of the Efficient matching algorithm. Higher values increase processing and accuracy.")]
    private int squareAccuracy = 16;

    [SerializeField]
    [Tooltip("For observation purposes only, coordinates of drawing will be stored in this List at runtime.")]
    public List<List<Vector2>>[] handCoords;

    //Used to store the current userdrawn Glyph
    private FGlyph[] userGlyph;
    //Holds all the presets
    private List<FGlyph> presets;


    //Holds the number of strokes
    private int[] numberOfStrokes;

    private bool[] textureDrawn;
    [SerializeField]
    private bool drawOnGUI = false;
    private string resultGUI = "";

    [Tooltip("Contains the result of a matching in form of a string")]
    public static string stringResult = "";
    public static float matchResult = 0;
    public static float miliSecondsToMatch = 0;
    public static float drawSize = 0;
    public static Vector3 centerPointOfDrawing = Vector3.zero;

    [Tooltip("Contains values that indicate how similar the user drawn glyph is compared to each preset")]
    public float[] matches;
    private float[] negativeMatches;
    private float drawIndex = 0;

    [SerializeField]
    TrailRenderer drawingTrail;

    private float downX, downY, upX, upY;

    HandBehaviour hand;


    void Start()
    {
        hand = GetComponent<HandBehaviour>();
        numberOfStrokes = new int[3];
        textureDrawn = new bool[3];
        userGlyph = new FGlyph[3];
        handCoords = new List<List<Vector2>>[3];
        lastPos = new Vector2[3];
        for (int i=0; i < 3; i++)
        {
            handCoords[i] = new List<List<Vector2>>();
            lastPos[i] = new Vector2();
            numberOfStrokes[i] = 0;
            textureDrawn[i] = false;
        }
        presets = new List<FGlyph>();

        //Load textures if there are any assigned
        LoadTexturePresets();

        //Add Presets here
        /*presets.Add(new FGlyph("Horizontal Line", "Horizontal Line", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0f, 0.4972678f), new Vector2(0.008196721f, 0.4945355f), new Vector2(0.01912568f, 0.4945355f), new Vector2(0.03005465f, 0.4945355f), new Vector2(0.03551913f, 0.4945355f), new Vector2(0.04371585f, 0.4945355f), new Vector2(0.05464481f, 0.4945355f), new Vector2(0.06557377f, 0.4945355f), new Vector2(0.07650273f, 0.4972678f), new Vector2(0.08743169f, 0.5f), new Vector2(0.09836066f, 0.5f), new Vector2(0.1092896f, 0.5f), new Vector2(0.1202186f, 0.5f), new Vector2(0.1311475f, 0.5f), new Vector2(0.1420765f, 0.5f), new Vector2(0.147541f, 0.5f), new Vector2(0.1557377f, 0.5f), new Vector2(0.1666667f, 0.5f), new Vector2(0.1721312f, 0.5f), new Vector2(0.1803279f, 0.5027322f), new Vector2(0.1939891f, 0.5027322f), new Vector2(0.2076503f, 0.5027322f), new Vector2(0.2213115f, 0.5027322f), new Vector2(0.2295082f, 0.5027322f), new Vector2(0.2377049f, 0.5027322f), new Vector2(0.2459016f, 0.5027322f), new Vector2(0.2540984f, 0.5027322f), new Vector2(0.2650273f, 0.5027322f), new Vector2(0.2786885f, 0.5027322f), new Vector2(0.2868852f, 0.5027322f), new Vector2(0.2978142f, 0.5027322f), new Vector2(0.3087432f, 0.5027322f), new Vector2(0.3224044f, 0.5027322f), new Vector2(0.3333333f, 0.5027322f), new Vector2(0.3442623f, 0.5027322f), new Vector2(0.3551913f, 0.5027322f), new Vector2(0.3661202f, 0.5027322f), new Vector2(0.3770492f, 0.5027322f), new Vector2(0.3907104f, 0.5027322f), new Vector2(0.4016393f, 0.5027322f), new Vector2(0.4153005f, 0.5027322f), new Vector2(0.4289618f, 0.5027322f), new Vector2(0.442623f, 0.5027322f), new Vector2(0.4508197f, 0.5027322f), new Vector2(0.4590164f, 0.5027322f), new Vector2(0.4672131f, 0.5027322f), new Vector2(0.4754098f, 0.5027322f), new Vector2(0.4836065f, 0.5027322f), new Vector2(0.4918033f, 0.5027322f), new Vector2(0.5f, 0.5027322f), new Vector2(0.5081967f, 0.5027322f), new Vector2(0.5163934f, 0.5f), new Vector2(0.5273224f, 0.5f), new Vector2(0.5355191f, 0.5f), new Vector2(0.5437158f, 0.5f), new Vector2(0.5519125f, 0.5f), new Vector2(0.5601093f, 0.5f), new Vector2(0.568306f, 0.4972678f), new Vector2(0.5765027f, 0.4972678f), new Vector2(0.5901639f, 0.4972678f), new Vector2(0.6038252f, 0.4972678f), new Vector2(0.6174864f, 0.4972678f), new Vector2(0.6256831f, 0.4972678f), new Vector2(0.6338798f, 0.4972678f), new Vector2(0.6448088f, 0.4945355f), new Vector2(0.65847f, 0.4945355f), new Vector2(0.6693989f, 0.4945355f), new Vector2(0.6830601f, 0.4945355f), new Vector2(0.6912568f, 0.4945355f), new Vector2(0.7021858f, 0.4945355f), new Vector2(0.715847f, 0.4945355f), new Vector2(0.7295082f, 0.4945355f), new Vector2(0.7377049f, 0.4945355f), new Vector2(0.7486339f, 0.4945355f), new Vector2(0.7622951f, 0.4945355f), new Vector2(0.7759563f, 0.4945355f), new Vector2(0.7814208f, 0.4945355f), new Vector2(0.7896175f, 0.4972678f), new Vector2(0.8032787f, 0.4972678f), new Vector2(0.8169399f, 0.4972678f), new Vector2(0.8306011f, 0.4972678f), new Vector2(0.8360656f, 0.4972678f), new Vector2(0.8442623f, 0.5f), new Vector2(0.852459f, 0.5f), new Vector2(0.8606557f, 0.5f), new Vector2(0.8688524f, 0.5f), new Vector2(0.8770492f, 0.5f), new Vector2(0.8879781f, 0.5f), new Vector2(0.8989071f, 0.5f), new Vector2(0.9071038f, 0.5f), new Vector2(0.9153005f, 0.5027322f), new Vector2(0.9234973f, 0.5027322f), new Vector2(0.931694f, 0.5027322f), new Vector2(0.9398907f, 0.5027322f), new Vector2(0.9480875f, 0.5027322f), new Vector2(0.9617487f, 0.5027322f), new Vector2(0.9699454f, 0.5027322f), new Vector2(0.9781421f, 0.5054645f), new Vector2(0.989071f, 0.5054645f), new Vector2(1f, 0.5054645f) } }));
        presets.Add(new FGlyph("verticalLine", "verticalLine", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.4853896f, 1f), new Vector2(0.4853896f, 0.9902598f), new Vector2(0.4853896f, 0.9837663f), new Vector2(0.4853896f, 0.9707792f), new Vector2(0.4853896f, 0.9577922f), new Vector2(0.4853896f, 0.9480519f), new Vector2(0.4853896f, 0.9383117f), new Vector2(0.4886364f, 0.9285714f), new Vector2(0.4886364f, 0.9155844f), new Vector2(0.4886364f, 0.9025974f), new Vector2(0.4918831f, 0.8896104f), new Vector2(0.4951299f, 0.8766234f), new Vector2(0.4951299f, 0.8636364f), new Vector2(0.4951299f, 0.8538961f), new Vector2(0.4951299f, 0.8409091f), new Vector2(0.4983766f, 0.8311688f), new Vector2(0.4983766f, 0.8181818f), new Vector2(0.5016234f, 0.8084416f), new Vector2(0.5016234f, 0.7987013f), new Vector2(0.5016234f, 0.7922078f), new Vector2(0.5048701f, 0.7792208f), new Vector2(0.5048701f, 0.7662337f), new Vector2(0.5081169f, 0.7532467f), new Vector2(0.5081169f, 0.7402598f), new Vector2(0.5113636f, 0.7305195f), new Vector2(0.5113636f, 0.7175325f), new Vector2(0.5113636f, 0.7045454f), new Vector2(0.5113636f, 0.6948052f), new Vector2(0.5113636f, 0.6883117f), new Vector2(0.5113636f, 0.6753247f), new Vector2(0.5113636f, 0.6655844f), new Vector2(0.5146104f, 0.6590909f), new Vector2(0.5146104f, 0.6461039f), new Vector2(0.5146104f, 0.6363636f), new Vector2(0.5146104f, 0.6266234f), new Vector2(0.5146104f, 0.6201299f), new Vector2(0.5146104f, 0.6071429f), new Vector2(0.5146104f, 0.5974026f), new Vector2(0.5146104f, 0.5876623f), new Vector2(0.5146104f, 0.5779221f), new Vector2(0.5146104f, 0.5714286f), new Vector2(0.5146104f, 0.5616883f), new Vector2(0.5146104f, 0.5519481f), new Vector2(0.5146104f, 0.5422078f), new Vector2(0.5146104f, 0.5324675f), new Vector2(0.5146104f, 0.5227273f), new Vector2(0.5146104f, 0.5162337f), new Vector2(0.5146104f, 0.5064935f), new Vector2(0.5146104f, 0.4967532f), new Vector2(0.5146104f, 0.487013f), new Vector2(0.5146104f, 0.4805195f), new Vector2(0.5146104f, 0.4707792f), new Vector2(0.5146104f, 0.4610389f), new Vector2(0.5146104f, 0.4512987f), new Vector2(0.5146104f, 0.4415585f), new Vector2(0.5146104f, 0.4318182f), new Vector2(0.5146104f, 0.4253247f), new Vector2(0.5146104f, 0.4155844f), new Vector2(0.5146104f, 0.4090909f), new Vector2(0.5146104f, 0.3993506f), new Vector2(0.5146104f, 0.3928571f), new Vector2(0.5146104f, 0.3831169f), new Vector2(0.5146104f, 0.3766234f), new Vector2(0.5146104f, 0.3668831f), new Vector2(0.5146104f, 0.3603896f), new Vector2(0.5146104f, 0.3506494f), new Vector2(0.5146104f, 0.3441558f), new Vector2(0.5146104f, 0.3344156f), new Vector2(0.5146104f, 0.3279221f), new Vector2(0.5146104f, 0.3149351f), new Vector2(0.5146104f, 0.3051948f), new Vector2(0.5146104f, 0.2987013f), new Vector2(0.5146104f, 0.2889611f), new Vector2(0.5146104f, 0.2824675f), new Vector2(0.5146104f, 0.2727273f), new Vector2(0.5146104f, 0.2662338f), new Vector2(0.5146104f, 0.2564935f), new Vector2(0.5146104f, 0.25f), new Vector2(0.5146104f, 0.237013f), new Vector2(0.5146104f, 0.2272727f), new Vector2(0.5146104f, 0.2142857f), new Vector2(0.5146104f, 0.2045455f), new Vector2(0.5146104f, 0.1948052f), new Vector2(0.5146104f, 0.1850649f), new Vector2(0.5146104f, 0.1753247f), new Vector2(0.5146104f, 0.1688312f), new Vector2(0.5146104f, 0.1590909f), new Vector2(0.5146104f, 0.1493506f), new Vector2(0.5146104f, 0.1331169f), new Vector2(0.5146104f, 0.1201299f), new Vector2(0.5146104f, 0.1103896f), new Vector2(0.5146104f, 0.1006493f), new Vector2(0.5146104f, 0.08441558f), new Vector2(0.5146104f, 0.07142857f), new Vector2(0.5146104f, 0.05844156f), new Vector2(0.5146104f, 0.0487013f), new Vector2(0.5146104f, 0.03571429f), new Vector2(0.5146104f, 0.02597403f), new Vector2(0.5146104f, 0.01298701f), new Vector2(0.5146104f, 0f) } }));
        presets.Add(new FGlyph("backwardSlash", "backwardSlash", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0f, 0.8978102f), new Vector2(0.0109489f, 0.8941606f), new Vector2(0.02554744f, 0.890511f), new Vector2(0.03284672f, 0.8832117f), new Vector2(0.04014599f, 0.879562f), new Vector2(0.05474453f, 0.8722628f), new Vector2(0.06569343f, 0.8613139f), new Vector2(0.07664233f, 0.8540146f), new Vector2(0.08394161f, 0.8467153f), new Vector2(0.09124088f, 0.8394161f), new Vector2(0.09854015f, 0.8321168f), new Vector2(0.1094891f, 0.8248175f), new Vector2(0.120438f, 0.8138686f), new Vector2(0.1277372f, 0.8065693f), new Vector2(0.1350365f, 0.8029197f), new Vector2(0.1459854f, 0.7919708f), new Vector2(0.1569343f, 0.7846715f), new Vector2(0.1678832f, 0.7737226f), new Vector2(0.1788321f, 0.7664233f), new Vector2(0.1861314f, 0.7591241f), new Vector2(0.1934307f, 0.7554744f), new Vector2(0.2007299f, 0.7481752f), new Vector2(0.2080292f, 0.7445256f), new Vector2(0.2153285f, 0.7372262f), new Vector2(0.2226277f, 0.729927f), new Vector2(0.229927f, 0.7226278f), new Vector2(0.2408759f, 0.7153285f), new Vector2(0.2518248f, 0.7043796f), new Vector2(0.2627737f, 0.6934307f), new Vector2(0.2737226f, 0.6824818f), new Vector2(0.2810219f, 0.6751825f), new Vector2(0.2883212f, 0.6715329f), new Vector2(0.2956204f, 0.6642336f), new Vector2(0.3029197f, 0.6605839f), new Vector2(0.310219f, 0.6532847f), new Vector2(0.3175182f, 0.649635f), new Vector2(0.3284672f, 0.6350365f), new Vector2(0.3394161f, 0.6240876f), new Vector2(0.350365f, 0.6167883f), new Vector2(0.3649635f, 0.609489f), new Vector2(0.3759124f, 0.5985401f), new Vector2(0.3868613f, 0.5912409f), new Vector2(0.3978102f, 0.580292f), new Vector2(0.4087591f, 0.5729927f), new Vector2(0.4160584f, 0.5656934f), new Vector2(0.4270073f, 0.5583941f), new Vector2(0.4343066f, 0.5510949f), new Vector2(0.4416058f, 0.5437956f), new Vector2(0.4489051f, 0.5364963f), new Vector2(0.459854f, 0.5328467f), new Vector2(0.4708029f, 0.5218978f), new Vector2(0.4817518f, 0.5145985f), new Vector2(0.4927007f, 0.5036497f), new Vector2(0.5036497f, 0.4927007f), new Vector2(0.5145985f, 0.4854015f), new Vector2(0.5255474f, 0.4781022f), new Vector2(0.5328467f, 0.4708029f), new Vector2(0.540146f, 0.4635037f), new Vector2(0.5474452f, 0.4562044f), new Vector2(0.5547445f, 0.4489051f), new Vector2(0.5620438f, 0.4416058f), new Vector2(0.5729927f, 0.4379562f), new Vector2(0.580292f, 0.4306569f), new Vector2(0.5912409f, 0.4233577f), new Vector2(0.6058394f, 0.4124088f), new Vector2(0.620438f, 0.4014598f), new Vector2(0.6350365f, 0.3905109f), new Vector2(0.649635f, 0.3795621f), new Vector2(0.6605839f, 0.3686131f), new Vector2(0.6751825f, 0.3613139f), new Vector2(0.6824818f, 0.3540146f), new Vector2(0.6934307f, 0.3467153f), new Vector2(0.7043796f, 0.3394161f), new Vector2(0.7153285f, 0.3321168f), new Vector2(0.7226278f, 0.3211679f), new Vector2(0.7335767f, 0.3138686f), new Vector2(0.7481752f, 0.3029197f), new Vector2(0.7627738f, 0.2919708f), new Vector2(0.7737226f, 0.2846715f), new Vector2(0.7846715f, 0.2773723f), new Vector2(0.7956204f, 0.2664233f), new Vector2(0.8065693f, 0.2591241f), new Vector2(0.8175182f, 0.2481752f), new Vector2(0.8321168f, 0.2408759f), new Vector2(0.8430657f, 0.229927f), new Vector2(0.8576642f, 0.2189781f), new Vector2(0.8649635f, 0.2116788f), new Vector2(0.8722628f, 0.2043796f), new Vector2(0.8832117f, 0.1934307f), new Vector2(0.8941606f, 0.1861314f), new Vector2(0.9087591f, 0.1751825f), new Vector2(0.9233577f, 0.1642336f), new Vector2(0.9343066f, 0.1532847f), new Vector2(0.9452555f, 0.1459854f), new Vector2(0.9562044f, 0.1386861f), new Vector2(0.9671533f, 0.1313869f), new Vector2(0.9744526f, 0.1240876f), new Vector2(0.9854015f, 0.1167883f), new Vector2(0.9927008f, 0.1094891f), new Vector2(1f, 0.1021898f) } }));
        presets.Add(new FGlyph("forwardSlash", "forwardSlash", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(1f, 0.9959514f), new Vector2(0.9919028f, 0.9878542f), new Vector2(0.9878542f, 0.9797571f), new Vector2(0.9757085f, 0.9676113f), new Vector2(0.9635627f, 0.9595141f), new Vector2(0.9554656f, 0.9554656f), new Vector2(0.9433199f, 0.9433199f), new Vector2(0.9311741f, 0.9352227f), new Vector2(0.9190283f, 0.9271255f), new Vector2(0.9109312f, 0.9230769f), new Vector2(0.902834f, 0.9149798f), new Vector2(0.8947368f, 0.9068826f), new Vector2(0.8866397f, 0.8987854f), new Vector2(0.8785425f, 0.8906882f), new Vector2(0.8663968f, 0.8825911f), new Vector2(0.854251f, 0.874494f), new Vector2(0.8461539f, 0.8623482f), new Vector2(0.8380567f, 0.854251f), new Vector2(0.8299595f, 0.8461539f), new Vector2(0.8218623f, 0.8380567f), new Vector2(0.8137652f, 0.8299595f), new Vector2(0.805668f, 0.8218623f), new Vector2(0.7975708f, 0.8137652f), new Vector2(0.7894737f, 0.805668f), new Vector2(0.777328f, 0.7935222f), new Vector2(0.7692308f, 0.7854251f), new Vector2(0.7611336f, 0.7732794f), new Vector2(0.7530364f, 0.7651822f), new Vector2(0.7449393f, 0.7530364f), new Vector2(0.7368421f, 0.7449393f), new Vector2(0.7246963f, 0.7327935f), new Vector2(0.7165992f, 0.7246963f), new Vector2(0.708502f, 0.7165992f), new Vector2(0.7044535f, 0.708502f), new Vector2(0.6923077f, 0.6963563f), new Vector2(0.680162f, 0.6842105f), new Vector2(0.6720648f, 0.6761134f), new Vector2(0.659919f, 0.6639676f), new Vector2(0.6518219f, 0.6518219f), new Vector2(0.6396761f, 0.6396761f), new Vector2(0.6315789f, 0.6275303f), new Vector2(0.6234818f, 0.6153846f), new Vector2(0.6153846f, 0.6072875f), new Vector2(0.6032389f, 0.5951417f), new Vector2(0.5910931f, 0.5870445f), new Vector2(0.5789474f, 0.5748988f), new Vector2(0.5708502f, 0.562753f), new Vector2(0.5587044f, 0.5506073f), new Vector2(0.5465587f, 0.5425101f), new Vector2(0.534413f, 0.5263158f), new Vector2(0.5222672f, 0.5141701f), new Vector2(0.5101215f, 0.5020243f), new Vector2(0.4979757f, 0.4898785f), new Vector2(0.4817814f, 0.4777328f), new Vector2(0.4696356f, 0.465587f), new Vector2(0.4574899f, 0.4493927f), new Vector2(0.4453441f, 0.437247f), new Vector2(0.437247f, 0.4291498f), new Vector2(0.4291498f, 0.4210526f), new Vector2(0.4129555f, 0.4089069f), new Vector2(0.4008097f, 0.3967611f), new Vector2(0.388664f, 0.3846154f), new Vector2(0.3765182f, 0.3724696f), new Vector2(0.368421f, 0.3603239f), new Vector2(0.3562753f, 0.3522267f), new Vector2(0.3481781f, 0.3441296f), new Vector2(0.340081f, 0.3319838f), new Vector2(0.3319838f, 0.3238866f), new Vector2(0.319838f, 0.3117409f), new Vector2(0.3117409f, 0.3036437f), new Vector2(0.2995951f, 0.291498f), new Vector2(0.2874494f, 0.2834008f), new Vector2(0.2753036f, 0.271255f), new Vector2(0.2672065f, 0.2591093f), new Vector2(0.2510121f, 0.2469636f), new Vector2(0.2388664f, 0.2348178f), new Vector2(0.2307692f, 0.2267206f), new Vector2(0.2226721f, 0.2186235f), new Vector2(0.2105263f, 0.2064777f), new Vector2(0.194332f, 0.194332f), new Vector2(0.1821862f, 0.1821862f), new Vector2(0.1740891f, 0.1740891f), new Vector2(0.1659919f, 0.1659919f), new Vector2(0.1497976f, 0.1538462f), new Vector2(0.1417004f, 0.145749f), new Vector2(0.1336032f, 0.1376518f), new Vector2(0.1255061f, 0.1295547f), new Vector2(0.1174089f, 0.1214575f), new Vector2(0.1052632f, 0.1093117f), new Vector2(0.09716599f, 0.1012146f), new Vector2(0.08906882f, 0.09311741f), new Vector2(0.08097166f, 0.08502024f), new Vector2(0.06882591f, 0.07287449f), new Vector2(0.05668016f, 0.06072874f), new Vector2(0.04453441f, 0.048583f), new Vector2(0.03238866f, 0.04048583f), new Vector2(0.0242915f, 0.02834008f), new Vector2(0.01619433f, 0.02024291f), new Vector2(0.008097166f, 0.01214575f), new Vector2(0f, 0.004048583f) } }));

        presets.Add(new FGlyph("I", "description", 100, 25, 3, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.1958763f, 0f), new Vector2(0.2164948f, 0f), new Vector2(0.2371134f, 0f), new Vector2(0.2577319f, 0f), new Vector2(0.2783505f, 0f), new Vector2(0.2989691f, 0f), new Vector2(0.3195876f, 0f), new Vector2(0.3402062f, 0f), new Vector2(0.3608247f, 0f), new Vector2(0.3814433f, 0f), new Vector2(0.4020618f, 0f), new Vector2(0.4226804f, 0f), new Vector2(0.443299f, 0f), new Vector2(0.4639175f, 0f), new Vector2(0.4845361f, 0f), new Vector2(0.5051546f, 0f), new Vector2(0.5257732f, 0f), new Vector2(0.5463917f, 0f), new Vector2(0.5670103f, 0f), new Vector2(0.5876288f, 0f), new Vector2(0.6082474f, 0f), new Vector2(0.628866f, 0f), new Vector2(0.6494845f, 0f), new Vector2(0.6701031f, 0f), new Vector2(0.6907216f, 0f), new Vector2(0.7113402f, 0f), new Vector2(0.7319587f, 0f), new Vector2(0.7525773f, 0f), new Vector2(0.7628866f, 0f), new Vector2(0.7731959f, 0f), new Vector2(0.7835051f, 0f), new Vector2(0.7938144f, 0f), new Vector2(0.8041237f, 0f) }, new List<Vector2>() { new Vector2(0.1958763f, 1f), new Vector2(0.2164948f, 1f), new Vector2(0.2371134f, 1f), new Vector2(0.2577319f, 1f), new Vector2(0.2783505f, 1f), new Vector2(0.2989691f, 1f), new Vector2(0.3195876f, 1f), new Vector2(0.3402062f, 1f), new Vector2(0.3608247f, 1f), new Vector2(0.3814433f, 1f), new Vector2(0.4020618f, 1f), new Vector2(0.4226804f, 1f), new Vector2(0.443299f, 1f), new Vector2(0.4639175f, 1f), new Vector2(0.4845361f, 1f), new Vector2(0.5051546f, 1f), new Vector2(0.5257732f, 1f), new Vector2(0.5463917f, 1f), new Vector2(0.5670103f, 1f), new Vector2(0.5876288f, 1f), new Vector2(0.6082474f, 1f), new Vector2(0.628866f, 1f), new Vector2(0.6494845f, 1f), new Vector2(0.6701031f, 1f), new Vector2(0.6907216f, 1f), new Vector2(0.7113402f, 1f), new Vector2(0.7319587f, 1f), new Vector2(0.7525773f, 1f), new Vector2(0.7628866f, 1f), new Vector2(0.7731959f, 1f), new Vector2(0.7835051f, 1f), new Vector2(0.7938144f, 1f), new Vector2(0.8041237f, 1f) }, new List<Vector2>() { new Vector2(0.4948454f, 0.02061856f), new Vector2(0.4948454f, 0.06185567f), new Vector2(0.4948454f, 0.1030928f), new Vector2(0.4948454f, 0.1443299f), new Vector2(0.4948454f, 0.185567f), new Vector2(0.4948454f, 0.2268041f), new Vector2(0.4948454f, 0.2680412f), new Vector2(0.4948454f, 0.3092783f), new Vector2(0.4948454f, 0.3505155f), new Vector2(0.4948454f, 0.3917526f), new Vector2(0.4948454f, 0.4329897f), new Vector2(0.4948454f, 0.4742268f), new Vector2(0.4948454f, 0.5154639f), new Vector2(0.4948454f, 0.556701f), new Vector2(0.4948454f, 0.5979381f), new Vector2(0.4948454f, 0.6185567f), new Vector2(0.4948454f, 0.6391752f), new Vector2(0.4948454f, 0.6597938f), new Vector2(0.4948454f, 0.6804124f), new Vector2(0.4948454f, 0.7010309f), new Vector2(0.4948454f, 0.7216495f), new Vector2(0.4948454f, 0.742268f), new Vector2(0.4948454f, 0.7628866f), new Vector2(0.4948454f, 0.7835051f), new Vector2(0.4948454f, 0.8041237f), new Vector2(0.4948454f, 0.8247423f), new Vector2(0.4948454f, 0.8453608f), new Vector2(0.4948454f, 0.8659794f), new Vector2(0.4948454f, 0.8865979f), new Vector2(0.4948454f, 0.9072165f), new Vector2(0.4948454f, 0.927835f), new Vector2(0.4948454f, 0.9484536f), new Vector2(0.4948454f, 0.9793814f) } }));
        presets.Add(new FGlyph("N", "description", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0f, 0.02083333f), new Vector2(0f, 0.0625f), new Vector2(0f, 0.1041667f), new Vector2(0f, 0.1458333f), new Vector2(0f, 0.1875f), new Vector2(0f, 0.2291667f), new Vector2(0f, 0.2708333f), new Vector2(0f, 0.3125f), new Vector2(0f, 0.3541667f), new Vector2(0f, 0.3958333f), new Vector2(0f, 0.4375f), new Vector2(0f, 0.4791667f), new Vector2(0f, 0.5208333f), new Vector2(0f, 0.5625f), new Vector2(0f, 0.6041667f), new Vector2(0f, 0.6458333f), new Vector2(0f, 0.6875f), new Vector2(0f, 0.7291667f), new Vector2(0f, 0.7708333f), new Vector2(0f, 0.8125f), new Vector2(0f, 0.8541667f), new Vector2(0f, 0.8958333f), new Vector2(0f, 0.9375f), new Vector2(0.02083333f, 0.96875f), new Vector2(0.04166667f, 0.9479167f), new Vector2(0.0625f, 0.9270833f), new Vector2(0.08333334f, 0.90625f), new Vector2(0.1041667f, 0.8854167f), new Vector2(0.125f, 0.8645833f), new Vector2(0.1458333f, 0.84375f), new Vector2(0.1666667f, 0.8229167f), new Vector2(0.1875f, 0.8020833f), new Vector2(0.2083333f, 0.78125f), new Vector2(0.2291667f, 0.7604167f), new Vector2(0.25f, 0.7395833f), new Vector2(0.2708333f, 0.71875f), new Vector2(0.2916667f, 0.6979167f), new Vector2(0.3125f, 0.6770833f), new Vector2(0.3333333f, 0.65625f), new Vector2(0.3541667f, 0.6354167f), new Vector2(0.375f, 0.6145833f), new Vector2(0.3958333f, 0.59375f), new Vector2(0.4166667f, 0.5729167f), new Vector2(0.4375f, 0.5520833f), new Vector2(0.4583333f, 0.53125f), new Vector2(0.4791667f, 0.5104167f), new Vector2(0.5f, 0.4895833f), new Vector2(0.5208333f, 0.46875f), new Vector2(0.5416667f, 0.4479167f), new Vector2(0.5625f, 0.4270833f), new Vector2(0.5833333f, 0.40625f), new Vector2(0.6041667f, 0.3854167f), new Vector2(0.625f, 0.3645833f), new Vector2(0.6458333f, 0.34375f), new Vector2(0.6666667f, 0.3229167f), new Vector2(0.6875f, 0.3020833f), new Vector2(0.7083333f, 0.28125f), new Vector2(0.7291667f, 0.2604167f), new Vector2(0.75f, 0.2395833f), new Vector2(0.7708333f, 0.21875f), new Vector2(0.7916667f, 0.1979167f), new Vector2(0.8125f, 0.1770833f), new Vector2(0.8333333f, 0.15625f), new Vector2(0.8541667f, 0.1354167f), new Vector2(0.875f, 0.1145833f), new Vector2(0.8958333f, 0.09375f), new Vector2(0.9166667f, 0.07291666f), new Vector2(0.9375f, 0.05208333f), new Vector2(0.9583333f, 0.03125f), new Vector2(1f, 0.03125f), new Vector2(1f, 0.07291666f), new Vector2(1f, 0.1145833f), new Vector2(1f, 0.15625f), new Vector2(1f, 0.1979167f), new Vector2(1f, 0.2395833f), new Vector2(1f, 0.28125f), new Vector2(1f, 0.3229167f), new Vector2(1f, 0.3645833f), new Vector2(1f, 0.40625f), new Vector2(1f, 0.4479167f), new Vector2(1f, 0.4895833f), new Vector2(1f, 0.53125f), new Vector2(1f, 0.5729167f), new Vector2(1f, 0.6145833f), new Vector2(1f, 0.65625f), new Vector2(1f, 0.6770833f), new Vector2(1f, 0.6979167f), new Vector2(1f, 0.71875f), new Vector2(1f, 0.7395833f), new Vector2(1f, 0.7604167f), new Vector2(1f, 0.78125f), new Vector2(1f, 0.8020833f), new Vector2(1f, 0.8229167f), new Vector2(1f, 0.84375f), new Vector2(1f, 0.8645833f), new Vector2(1f, 0.8854167f), new Vector2(1f, 0.90625f), new Vector2(1f, 0.9270833f), new Vector2(1f, 0.9479167f), new Vector2(1f, 0.9791667f) } }));
        */
        //presets.Add(new FGlyph("punch1", "Kung fu punch", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.296019f, 0.8014306f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0f, 0.307393f), new Vector2(0.6915826f, 0.5052304f), new Vector2(1f, 0.1985694f), new Vector2(0.682097f, 0.4936877f) } }));
        //presets.Add(new FGlyph("punch2", "Kung fu punch", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.4139098f, 0.9933252f), new Vector2(0.5054032f, 0.8323724f), new Vector2(0.5808575f, 0.6216977f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.288975f, 0f), new Vector2(0.6580005f, 0.3953945f), new Vector2(0.711025f, 0.2207965f), new Vector2(0.6681169f, 0.4199388f), new Vector2(0.5768541f, 0.6243473f), new Vector2(0.4958211f, 0.8129951f), new Vector2(0.4343247f, 1f) } }));
        //presets.Add(new FGlyph("punch3", "Kung fu punch", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.6272188f, 0.9933252f), new Vector2(0.5526696f, 0.8013327f), new Vector2(0.5022075f, 0.6216977f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4187102f, 0f), new Vector2(0.4489275f, 0.3953945f), new Vector2(0.3727812f, 0.2207965f), new Vector2(0.4296659f, 0.4199388f), new Vector2(0.4973359f, 0.6243473f), new Vector2(0.5339262f, 0.8129951f), new Vector2(0.5493208f, 1f) } }));
        //presets.Add(new FGlyph("Test1", "description", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.461364f, 0.09404188f), new Vector2(0.1833825f, 0.1402893f), new Vector2(0.0219259f, 0.3734757f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.3673781f, 0.1329525f), new Vector2(0.02651617f, 0.6830114f), new Vector2(0.2112233f, 0.9133818f), new Vector2(0.4939311f, 1f), new Vector2(0.7623594f, 0.9009258f), new Vector2(0.9383908f, 0.6683991f), new Vector2(0.9780741f, 0.3816172f), new Vector2(0.8745885f, 0.116611f), new Vector2(0.6013398f, 0f), new Vector2(0.3423506f, 0.08722185f), new Vector2(0.2118989f, 0.3219373f) } }));
        //presets.Add(new FGlyph("Test2", "description", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.4860834f, 0.9973694f), new Vector2(0.14989f, 0.9119167f), new Vector2(0.4672026f, 0.8005332f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.372416f, 0f), new Vector2(0.85011f, 0.8730278f), new Vector2(0.5458696f, 0.9922194f), new Vector2(0.2058623f, 1f) } }));
        //presets.Add(new FGlyph("Test3","description",50,25, 1,centralizedNormalization, new List<List<Vector2>>() {  new List<Vector2>() { new Vector2(0.7590995f,0.6212345f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0f,0.3787655f),new Vector2(0.7705399f,0.3787655f),new Vector2(0.841258f,0.5816183f),new Vector2(0.9182517f,0.5671919f),new Vector2(1f,0.5657427f),new Vector2(0.9279708f,0.600376f),new Vector2(0.8436518f,0.6065154f),new Vector2(0.7657353f,0.6129007f)}}));
        //presets.Add(new FGlyph("UserGlyph", "description", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 559387f, 0, 8359648f), new Vector2(0, 5517241f, 0, 8359648f), new Vector2(0, 526687f, 0, 8309892f), new Vector2(0, 5019157f, 0, 828302f), new Vector2(0, 4636016f, 0, 8129762f), new Vector2(0, 4214559f, 0, 8053135f), new Vector2(0, 3846531f, 0, 7866991f), new Vector2(0, 348659f, 0, 7708308f), new Vector2(0, 3103448f, 0, 7555051f), new Vector2(0, 2716369f, 0, 7332456f), new Vector2(0, 2337165f, 0, 7133595f), new Vector2(0, 2059654f, 0, 6935371f), new Vector2(0, 1800767f, 0, 6750453f), new Vector2(0, 1525118f, 0, 6538287f), new Vector2(0, 1264367f, 0, 6328997f), new Vector2(0, 1051671f, 0, 6110659f), new Vector2(0, 08429121f, 0, 5907541f), new Vector2(0, 06393151f, 0, 5591395f), new Vector2(0, 04597696f, 0, 5294514f), new Vector2(0, 03185935f, 0, 4995769f), new Vector2(0, 01915703f, 0, 4719802f), new Vector2(0, 007662795f, 0, 4451602f), new Vector2(0, 003831351f, 0, 4106774f), new Vector2(0f, 0, 3647004f), new Vector2(0f, 0, 3378805f), new Vector2(0, 007662795f, 0, 2957349f), new Vector2(0, 02422312f, 0, 2704901f), new Vector2(0, 04214561f, 0, 2459265f), new Vector2(0, 07279688f, 0, 2114437f), new Vector2(0, 09142193f, 0, 195655f), new Vector2(0, 1128034f, 0, 1803826f), new Vector2(0, 1326576f, 0, 1651101f), new Vector2(0, 1532566f, 0, 150141f), new Vector2(0, 1769477f, 0, 1360925f), new Vector2(0, 2029109f, 0, 1223473f), new Vector2(0, 2273468f, 0, 108602f), new Vector2(0, 2528735f, 0, 09650116f), new Vector2(0, 2762187f, 0, 08569336f), new Vector2(0, 3006546f, 0, 07652988f), new Vector2(0, 3235633f, 0, 06583916f), new Vector2(0, 348659f, 0, 058187f), new Vector2(0, 3709079f, 0, 0536212f), new Vector2(0, 394636f, 0, 05052416f), new Vector2(0, 4396339f, 0, 04445773f), new Vector2(0, 4625426f, 0, 04140323f), new Vector2(0, 48659f, 0, 03902992f), new Vector2(0, 5287356f, 0, 03519847f), new Vector2(0, 5694498f, 0, 03376701f), new Vector2(0, 6130269f, 0, 03519847f), new Vector2(0, 6513409f, 0, 03519847f), new Vector2(0, 6931567f, 0, 0536212f), new Vector2(0, 7130109f, 0, 06278467f), new Vector2(0, 7356321f, 0, 07351268f), new Vector2(0, 76341f, 0, 09791133f), new Vector2(0, 7931035f, 0, 1233211f), new Vector2(0, 8199233f, 0, 150141f), new Vector2(0, 8306088f, 0, 1727464f), new Vector2(0, 8428268f, 0, 195655f), new Vector2(0, 8535175f, 0, 2185637f), new Vector2(0, 8659005f, 0, 242095f), new Vector2(0, 8855897f, 0, 2781263f), new Vector2(0, 9080461f, 0, 314892f), new Vector2(0, 927203f, 0, 3532061f), new Vector2(0, 9451523f, 0, 3911425f), new Vector2(0, 9655173f, 0, 4298345f), new Vector2(0, 9731802f, 0, 452823f), new Vector2(0, 9808429f, 0, 4758115f), new Vector2(0, 9879152f, 0, 5041587f), new Vector2(0, 9961687f, 0, 5332828f), new Vector2(1f, 0, 571597f), new Vector2(0, 9986058f, 0, 6049569f), new Vector2(1f, 0, 6405625f), new Vector2(0, 9833334f, 0, 6813192f), new Vector2(0, 9693486f, 0, 7248537f), new Vector2(0, 955843f, 0, 7439362f), new Vector2(0, 9425288f, 0, 7631679f), new Vector2(0, 9268253f, 0, 7805901f), new Vector2(0, 9130801f, 0, 7989171f), new Vector2(0, 8978077f, 0, 8172441f), new Vector2(0, 8850574f, 0, 8359648f), new Vector2(0, 8505747f, 0, 8666162f), new Vector2(0, 8290816f, 0, 8768066f), new Vector2(0, 8092274f, 0, 8874974f), new Vector2(0, 770115f, 0, 9087617f), new Vector2(0, 7466103f, 0, 9149877f), new Vector2(0, 7241379f, 0, 9240874f), new Vector2(0, 682466f, 0, 9363692f), new Vector2(0, 6436781f, 0, 9509073f), new Vector2(0, 6198489f, 0, 9546962f), new Vector2(0, 5977012f, 0, 9585702f), new Vector2(0, 559387f, 0, 9624016f), new Vector2(0, 5190507f, 0, 9638596f), new Vector2(0, 4789273f, 0, 966233f), new Vector2(0, 4444444f, 0, 966233f), new Vector2(0, 4167252f, 0, 9577506f), new Vector2(0, 3908046f, 0, 9509073f), new Vector2(0, 3716475f, 0, 9432445f), new Vector2(0, 3510537f, 0, 928733f), new Vector2(0, 3333333f, 0, 9164246f), new Vector2(0, 3218391f, 0, 9125931f) } }));
        //presets.Add(new FGlyph("UserGlyph", "description", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.5342105f, 0.8328947f), new Vector2(0.5126937f, 0.8306213f), new Vector2(0.4917142f, 0.8285233f), new Vector2(0.4707347f, 0.8264254f), new Vector2(0.4508041f, 0.8243274f), new Vector2(0.4298246f, 0.8222295f), new Vector2(0.408845f, 0.8201315f), new Vector2(0.3878655f, 0.8180336f), new Vector2(0.367935f, 0.8169846f), new Vector2(0.3469554f, 0.8148866f), new Vector2(0.3259759f, 0.8127887f), new Vector2(0.3049964f, 0.8106908f), new Vector2(0.2850658f, 0.8085928f), new Vector2(0.2640863f, 0.8064948f), new Vector2(0.2431068f, 0.8043969f), new Vector2(0.2221272f, 0.8022989f), new Vector2(0.2026316f, 0.8013158f), new Vector2(0.1759723f, 0.7771235f), new Vector2(0.1507968f, 0.752997f), new Vector2(0.1256214f, 0.7288706f), new Vector2(0.100446f, 0.7047441f), new Vector2(0.07527053f, 0.6806176f), new Vector2(0.05009509f, 0.6564912f), new Vector2(0.02491966f, 0.6323647f), new Vector2(0f, 0.6092106f), new Vector2(0.0007931988f, 0.5830628f), new Vector2(0.001842175f, 0.5589364f), new Vector2(0.002891152f, 0.533761f), new Vector2(0.004989105f, 0.5096345f), new Vector2(0.006038081f, 0.4844591f), new Vector2(0.007087057f, 0.4603326f), new Vector2(0.008136034f, 0.4351572f), new Vector2(0.01052632f, 0.4118422f), new Vector2(0.04065431f, 0.3952961f), new Vector2(0.07107462f, 0.3806104f), new Vector2(0.1014949f, 0.3648757f), new Vector2(0.1319153f, 0.3501901f), new Vector2(0.1623356f, 0.3344554f), new Vector2(0.1927559f, 0.3197698f), new Vector2(0.2231762f, 0.3040351f), new Vector2(0.2546455f, 0.2893494f), new Vector2(0.2850658f, 0.2736148f), new Vector2(0.3154861f, 0.2578802f), new Vector2(0.3459065f, 0.2421455f), new Vector2(0.3763268f, 0.2274598f), new Vector2(0.4067471f, 0.2117252f), new Vector2(0.4371674f, 0.1970395f), new Vector2(0.4675877f, 0.1813049f), new Vector2(0.5f, 0.1671053f), new Vector2(0.5273794f, 0.1697661f), new Vector2(0.5557017f, 0.173962f), new Vector2(0.5840241f, 0.177109f), new Vector2(0.6133955f, 0.1813049f), new Vector2(0.6417178f, 0.1844518f), new Vector2(0.6710892f, 0.1886477f), new Vector2(0.6994115f, 0.1917946f), new Vector2(0.7287828f, 0.1959905f), new Vector2(0.7571052f, 0.1991375f), new Vector2(0.7854276f, 0.2022844f), new Vector2(0.81375f, 0.2054313f), new Vector2(0.8431213f, 0.2096272f), new Vector2(0.8714436f, 0.2127742f), new Vector2(0.899766f, 0.2169701f), new Vector2(0.9280884f, 0.220117f), new Vector2(0.9578947f, 0.225f), new Vector2(0.9627046f, 0.263125f), new Vector2(0.9648026f, 0.2830556f), new Vector2(0.9679495f, 0.3029861f), new Vector2(0.9700475f, 0.3229167f), new Vector2(0.9731944f, 0.3428472f), new Vector2(0.9752923f, 0.3627778f), new Vector2(0.9784393f, 0.3827083f), new Vector2(0.9805372f, 0.4015899f), new Vector2(0.9836841f, 0.4215205f), new Vector2(0.9857821f, 0.441451f), new Vector2(0.988929f, 0.4613816f), new Vector2(0.991027f, 0.4802631f), new Vector2(0.9941739f, 0.5001937f), new Vector2(0.9962718f, 0.5201243f), new Vector2(1f, 0.5407895f), new Vector2(0.9752923f, 0.5704751f), new Vector2(0.9522148f, 0.6008955f), new Vector2(0.9291373f, 0.6313158f), new Vector2(0.9060599f, 0.6627851f), new Vector2(0.8819335f, 0.6932054f), new Vector2(0.858856f, 0.7236257f), new Vector2(0.8357785f, 0.754046f), new Vector2(0.8131579f, 0.7855263f), new Vector2(0.7801827f, 0.7897112f), new Vector2(0.7476645f, 0.7949561f), new Vector2(0.7151462f, 0.799152f), new Vector2(0.6836769f, 0.8043969f), new Vector2(0.6511586f, 0.8085928f), new Vector2(0.6186403f, 0.8138376f), new Vector2(0.586122f, 0.8190826f), new Vector2(0.5552632f, 0.8249999f), new Vector2(0.5336732f, 0.8190826f), new Vector2(0.5137427f, 0.8148866f), new Vector2(0.4927632f, 0.8106908f), new Vector2(0.4736841f, 0.8065789f) } }));

        //presets.Add(new FGlyph("UserGlyph", "description", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 7378364f, 0, 5324807f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 2621636f, 0f), new Vector2(0, 6957321f, 1f) } }));
        //presets.Add(new FGlyph("UserGlyph", "description", 100, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 4298874f, 0, 5324807f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 4654425f, 0f), new Vector2(0, 5701126f, 1f) } }));
        // Reset points cap if user changed it
        
        presets.Add(new FGlyph("punch1", "description", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.6620247f, 0.8766757f), new Vector2(0.8188381f, 0.7374161f), new Vector2(0.8726487f, 0.5391296f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.1273514f, 0f), new Vector2(0.8042575f, 0.7669964f), new Vector2(0.8016594f, 1f) } }));
        presets.Add(new FGlyph("punch2", "description", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.6357911f, 0.6390329f), new Vector2(0.6683987f, 0.4849178f), new Vector2(0.7200547f, 0.3224848f), new Vector2(0.7597151f, 0.1637573f), new Vector2(0.7660269f, 0f), new Vector2(0.6784025f, 0.1240042f), new Vector2(0.7079639f, 0.2735455f), new Vector2(0.7212056f, 0.4258446f), new Vector2(0.7193356f, 0.6293047f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.2339731f, 0.3081667f), new Vector2(0.6286139f, 0.8474994f), new Vector2(0.632883f, 1f) } }));
        presets.Add(new FGlyph("punch3", "description", 50, 25, 1, centralizedNormalization, new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0.746194f, 0.8407753f), new Vector2(0.7525688f, 0.6799536f), new Vector2(0.7157714f, 0.4890863f), new Vector2(0.658377f, 0.3153415f), new Vector2(0.5686434f, 0.1455605f), new Vector2(0.4863108f, 0f), new Vector2(0.5027928f, 0.1919306f), new Vector2(0.6017479f, 0.3609228f), new Vector2(0.7367817f, 0.5223241f), new Vector2(0.8411437f, 0.6435281f), new Vector2(0.9287753f, 0.7744723f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.07122473f, 0.3000263f), new Vector2(0.9144417f, 1f) } }));

        foreach (var item in presets)
        {
            item.pointsCap = pointsCap;
            item.CalculateTextureEffective();
        }
    }
    float lastouch;
    bool[] press = new bool[3];
    void Update()
    {
        drawIndex += Time.deltaTime;

        if (OVRInput.GetDown(OVRInput.RawButton.X))
            drawOnGUI = !drawOnGUI;

        // Starts saving mouse coordinates as long as the user has the left mouse button pressed
        // Multiple Strokes Matching
        if (selectedNumberOfStrokes == NumberOfStrokes.Multiple)
        {

            //Hold mouse button down
            if (hand.touch > 0)
            {

                //First frame on which the button has been pressed
                if (hand.touch > lastouch)
                {
                    drawingTrail.emitting = true;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!press[i]) {
                            press[i] = true;
                            numberOfStrokes[i]++;
                            handCoords[i].Add(new List<Vector2>());
                        }
                    }
                }
                Vector2[] position = new Vector2[2];

                position[0] = new Vector2(transform.position.x, transform.position.y);
                position[1] = new Vector2(transform.position.x, transform.position.z);
                position[2] = new Vector2(transform.position.y, transform.position.z);
                for (int i=0; i < 3; i++) {
                    //Add position 
                    if (Vector2.Distance(position[i], lastPos[i]) > spammThreshhold)
                    {
                        if (numberOfStrokes[i] == 0)
                        {
                            numberOfStrokes[i]++;
                            handCoords[i].Add(new List<Vector2>());
                        }
                        handCoords[i][numberOfStrokes[i] - 1].Add(position[i]);
                        lastPos[i] = position[i];
                    }
                }


            }
            else
            {
                drawingTrail.emitting = false;
            }
            //Since we allow more than 1 stroke, we can start matching only based on an event:
            if (lastouch > hand.touch)
            {
                drawingTrail.emitting = false;

                StartMatching();
            }
        }
        //Single Stoke Matching
        else if (selectedNumberOfStrokes == NumberOfStrokes.Single)
        {
            if (hand.touch > 0)
            {

                Vector2[] position = new Vector2[3];
                position[0] = new Vector2(transform.position.x, transform.position.y);
                position[1] = new Vector2(transform.position.x, transform.position.z);
                position[2] = new Vector2(transform.position.y, transform.position.z);
                for (int i=0; i < 3; i++) {
                    if (hand.touch > lastouch && !press[i])
                    {
                        press[i] = true;
                        handCoords[i].Clear();
                        if (userGlyph[i] != null)
                        {
                            userGlyph[i].Clear();
                            textureDrawn[i] = false;
                            drawSize = 0;
                        }
                        drawingTrail.emitting = true;

                        numberOfStrokes[i] = 1;
                        handCoords[i].Add(new List<Vector2>());
                    }
                    float distance = Vector2.Distance(position[i], lastPos[i]);
                    if (distance > spammThreshhold)
                    {
                        //GameManager.DebugApp("", i + " Distance: " + distance + " position: " + position[i].x + " " + position[i].y + " last position: " + lastPos[i].x + " " + lastPos[i].y);
                        handCoords[i][numberOfStrokes[i] - 1].Add(position[i]);
                        lastPos[i] = position[i];
                    }
                }
            }
            else
            {
                drawingTrail.emitting = false;
                //GameManager.DebugApp("1 Disabling trail hand " + hand.touch + " lastotuch: " + lastouch);
            }
            //Lift finger off the mouse, match: 
            if (lastouch > hand.touch)
            {
                drawingTrail.emitting = false;
                StartMatching(); 
            }
        }

        // hard reset of drawing
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            for (int i = 0; i < 3; i++)
            {
                textureDrawn[i] = false;
                handCoords[i].Clear();
                numberOfStrokes[i] = 0;
            }
        }
        
        //Will copy to clipboard a hard copy of the last user drawns glyph under the form of : "presets.Add (new Fglyph (....));
        //After you press S, just paste the clipbloard string into the "Start" function to have it added to the presets
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            for (int i=0; i < 3; i++) {
                if (userGlyph[i] == null)
                {
                    userGlyph[i] = new FGlyph("UserGlyph", "User drawn Glyph", pointsCap, squareAccuracy, numberOfStrokes[i], centralizedNormalization, handCoords[i]);
                }

                //GUIUtility.systemCopyBuffer = userGlyph[i].ToString();
                UnityEngine.Debug.Log(GameManager.APPTAG +  i + "- VRGlyph: " + userGlyph[i].ToString());
            }
        }
        lastouch = hand.touch;

    }
    public void StartMatching()
    {
     
        for (int i=0; i < 3; i++) {
            if (press[i])
            {
                press[i] = false;
                int pointsCount = 0;
                //Count the points of the user drawing
                /*
                 * [Kungfutronics]:  Percentage hit: 3 14 21,42857%
                 * [Kungfutronics]:  Percentage hit: 3 15 20%
                 * [Kungfutronics]:  Percentage hit: 4 14 28,57143%
                 */
                foreach (var item in handCoords[i])
                {
                    pointsCount += item.Count;
                }
                //Small check to not return random stuff if use barely drew anything
                if (pointsCount > 10)
                {
                    userGlyph[i] = new FGlyph("UserGlyph", "User drawn Glyph", pointsCap, squareAccuracy, numberOfStrokes[i], centralizedNormalization, handCoords[i]);
                    textureDrawn[i] = true;
                    //Start Matching
                    MatchGlyphs(i);

                    print("Percentage hit: " + matchResult + " " + userGlyph[i].numberOfSquaresFilled + " " + (matchResult / userGlyph[i].numberOfSquaresFilled * 100) + "%");
                    GameManager.DebugApp("", "Percentage hit: " + matchResult + " " + userGlyph[i].numberOfSquaresFilled + " " + (matchResult / userGlyph[i].numberOfSquaresFilled * 100) + "%");
                    matchResult = matchResult / userGlyph[i].numberOfSquaresFilled * 100;
                    //Throw event
                    OnMatchResult?.Invoke(stringResult, matchResult, miliSecondsToMatch);
                    hand.MatchGesture(stringResult, matchResult, miliSecondsToMatch);

                }

                // Clear right after if it's multiple stroke enabled
                if (selectedNumberOfStrokes == NumberOfStrokes.Multiple)
                {
                    handCoords[i].Clear();
                    numberOfStrokes[i] = 0;
                }
            }

        }
    }
    /// <summary>
    /// Matches 
    /// </summary>
    private void MatchGlyphs(int plane)
    {

        List<Vector2> userCoords = new List<Vector2>();
        foreach (var item in userGlyph[plane].coords)
        {
            userCoords.AddRange(item);
        }
        //Stopwatch to count the milliseconds required to match a glyph to a preset
        Stopwatch sw = new Stopwatch();
        sw.Start();
        matches = new float[presets.Count];
        negativeMatches = new float[presets.Count];
        if (presets.Count != 0)
        {
            if (selectedAlgorithmType == AlgorithmType.Efficient)
            {
                for (int i = 0; i < presets.Count; i++)
                {
                    for (int j = 0; j < squareAccuracy; j++)
                    {
                        for (int k = 0; k < squareAccuracy; k++)
                        {
                            if (userGlyph[plane].squares[j, k] != 0 && presets[i].squares[j, k] != 0)
                            {
                                //We found a similarity 
                                matches[i]++;
                            }
                            else
                            {
                                //We found something different, punish it
                                negativeMatches[i]++;
                            }
                        }
                    }
                }
                //Search for the smallest offset between two presets
                int index = 0;
                for (int i = 1; i < matches.Length; i++)
                {
                    if (matches[index] - negativeMatches[index] < matches[i] - negativeMatches[i])
                        index = i;
                }
                // Save result:
                stringResult = presets[index].name;
                matchResult = matches[index];
                resultGUI = "MATCH: " + matches[index] + " Sign: " + presets[index].name + " Elapsed ms: " + sw.ElapsedMilliseconds;
            }
            else
            {

                for (int i = 0; i < presets.Count; i++)
                {
                    List<Vector2> presetCoords = new List<Vector2>();
                    foreach (var item in presets[i].coords)
                    {
                        presetCoords.AddRange(item);
                    }
                    //Calculate delta between two points, and add it to a matching value
                    for (int j = 0; j < presets[i].coords.Count; j++)
                    {
                        Vector2 crt = presetCoords[j];
                        int minIndex = minDistanceIndex(crt, userCoords, plane);
                        matches[i] += Mathf.Pow(Vector2.Distance(crt, userCoords[minIndex]) + 1, 5f);

                        // DrawLine(userGlyph.texture, (int)(presets[i].coords[j].x * userGlyph.texture.width), (int)(presets[i].coords[j].y * userGlyph.texture.width), (int)(userGlyph.coords[minIndex].x * userGlyph.texture.width), (int)(userGlyph.coords[minIndex].y * userGlyph.texture.width) , Color.white);
                        // userGlyph.texture.SetPixel((int)(presets[i].coords[j].x * userGlyph.texture.width), (int)(presets[i].coords[j].y * userGlyph.texture.height), Color.red);

                    }
                    userGlyph[plane].ResetChecking();
                }
                userGlyph[plane].texture.Apply();

                //Search for the smallest offset, that will indicate which preset is the most similar
                int indexMin = 0;
                for (int i = 1; i < matches.Length; i++)
                {
                    if (matches[i] < matches[indexMin])
                    {
                        indexMin = i;
                    }
                }
                //store result:
                stringResult = presets[indexMin].name;
                matchResult = matches[indexMin];
                resultGUI = "MATCH: " + matches[indexMin] + " Sign: " + presets[indexMin].name + " Elapsed ms: " + sw.ElapsedMilliseconds;
                miliSecondsToMatch = sw.ElapsedMilliseconds;
            }
        }
    }

    /*private void OnGUI()
    {
        if (drawOnGUI)
        {
            for (int i=0; i < 3; i++) {
                if (textureDrawn[i] && userGlyph[i] != null)
                {
                    GUI.DrawTexture(new Rect(Screen.width - 200, 10 + i * 200, 200, 200), userGlyph[i].texture, ScaleMode.StretchToFill, false, 10.0F);
                    //  GameObject.Find("Test").GetComponent<SpriteRenderer>().sprite = ;
                }
                string aux = "";
                if (presets.Count != 0)
                {
                    foreach (FGlyph fglyph in presets)
                    {
                        aux += fglyph.name + " ";
                    }
                    GUI.DrawTexture(new Rect(Screen.width - 400, 10 + i * 200, 200, 200), presets[(int)(drawIndex) % presets.Count].texture, ScaleMode.ScaleToFit, false, 1f);
                }
                GUI.TextField(new Rect(10, 10 + i * 200, 700, 100), "Presets available:\n" + aux + "\nPress KeyPadEnter to match | Press Esc to Clear: " + resultGUI + "\n Result (Float): " + matchResult + "% String: " + stringResult);
            }
        }
    }*/

    /// <summary>
    /// What the function does it basically:
    /// Searching for a point, once it finds it, it keeps searching for neighbors
    /// When all neighbors are found and saved, searching for any other stroke until all are found
    /// </summary>
    private void LoadTexturePresets()
    {
        for (int plane = 0; plane < 3; plane++)
        {
            string aux = "";

            bool[,] isChecked = new bool[100, 100];
            Vector2 firstFound = Vector2.zero;
            Vector2 secondFound = Vector2.zero;

            bool returnedToSecond = false;

            int strokeNumber = 0;
            foreach (var texture in presetTextures)
            {

                for (int i = 0; i < texture.height; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {
                        if (texture.GetPixel(i, j).a > 0.8f && isChecked[i, j] == false)
                        {
                            handCoords[plane].Add(new List<Vector2>());
                            strokeNumber++;
                            returnedToSecond = false;
                            print("AddStroke: " + strokeNumber + "  i / j :" + i + " " + j);
                            int a = i, b = j;
                            do
                            {
                                isChecked[i, j] = true;
                                firstFound = Vector2.zero;
                                if (returnedToSecond == false)
                                {
                                    handCoords[plane][strokeNumber - 1].Add(new Vector2(i, j));
                                }
                                else
                                {
                                    handCoords[plane][strokeNumber - 1].Insert(0, new Vector2(i, j));
                                }


                                if (texture.GetPixel(i - 1, j - 1).a > 0.8f && isChecked[i - 1, j - 1] == false) //top left
                                {
                                    firstFound = new Vector2(i - 1, j - 1);
                                }
                                if (texture.GetPixel(i - 1, j).a > 0.8f && isChecked[i - 1, j] == false) // top middle
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i - 1, j);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i - 1, j);
                                    }
                                }
                                if (texture.GetPixel(i - 1, j + 1).a > 0.8f && isChecked[i - 1, j + 1] == false) // top right
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i - 1, j + 1);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i - 1, j + 1);
                                    }

                                }
                                if (texture.GetPixel(i, j - 1).a > 0.8f && isChecked[i, j - 1] == false) // middle left
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i, j - 1);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i, j - 1);
                                    }
                                }
                                if (texture.GetPixel(i, j + 1).a > 0.8f && isChecked[i, j + 1] == false) // middle right
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i, j + 1);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i, j + 1);
                                    }
                                }
                                if (texture.GetPixel(i + 1, j - 1).a > 0.8f && isChecked[i + 1, j - 1] == false) // bot left
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i + 1, j - 1);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i + 1, j - 1);
                                    }
                                }
                                if (texture.GetPixel(i + 1, j).a > 0.8f && isChecked[i + 1, j] == false) // bot middle
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i + 1, j);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i + 1, j);
                                    }
                                }
                                if (texture.GetPixel(i + 1, j + 1).a > 0.8f && isChecked[i + 1, j + 1] == false) // bot right
                                {
                                    if (firstFound != Vector2.zero)
                                    {
                                        secondFound = new Vector2(i + 1, j + 1);
                                    }
                                    else
                                    {
                                        firstFound = new Vector2(i + 1, j + 1);
                                    }
                                }

                                if (firstFound != Vector2.zero)
                                {
                                    i = (int)firstFound.x;
                                    j = (int)firstFound.y;
                                    //print("Next point found: " + i + " " + j + "second: " + secondFound.x + " " +secondFound.y);
                                }
                                if (firstFound == Vector2.zero && secondFound != Vector2.zero)
                                {
                                    // print("ATTEMPTING TO RETURN");
                                    if (isChecked[(int)secondFound.x, (int)secondFound.y] == false)
                                    {
                                        i = (int)secondFound.x;
                                        j = (int)secondFound.y;
                                        //print("RETURNING TO SECOND " + i + " " + j);
                                    }
                                    returnedToSecond = true;
                                    firstFound = secondFound;
                                    secondFound = Vector2.zero;
                                }

                                // print("pixel found: " + i + " / " + j);
                            } while (firstFound != Vector2.zero);
                            i = a;
                            j = b;
                        }
                        isChecked[i, j] = true;
                    }
                }

                int total = 0;
                foreach (var list in handCoords)
                {
                    total += list.Count;
                }
                print("Total coords: " + total);
                print(pointsCap + " " + squareAccuracy + " " + strokeNumber);
                FGlyph auxGlyph = new FGlyph(texture.name, "Description", pointsCap, squareAccuracy, strokeNumber, centralizedNormalization, handCoords[plane]);
                presets.Add(auxGlyph);
                aux += auxGlyph.ToString();

                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        isChecked[i, j] = false;
                    }
                }
                handCoords[plane].Clear();
                strokeNumber = 0;
            }
            GUIUtility.systemCopyBuffer = aux;
            /*foreach (var texture in presetTextures)
            {
                mouseCoords.Add(new List<Vector2>());
                for (int i = 0; i < texture.height; i++)
                {

                    for (int j = 0; j < texture.width; j++)
                    {
                        if(texture.GetPixel(i,j).a > 0.8f)
                        {
                            mouseCoords[0].Add(new Vector2(i, j));
                        }
                    }
                }
                FGlyph auxGlyph = new FGlyph(texture.name, "PresetGlyphDescript", pointsCap, squareAccuracy, 1, mouseCoords);
                presets.Add(auxGlyph);
                aux += auxGlyph.ToString();
                mouseCoords.Clear();
            }


            GUIUtility.systemCopyBuffer = aux;*/
        }
    }

    /// <summary>
    /// Method to clear current drawing
    /// </summary>
    public void ClearDrawing()
    {
        for (int i = 0; i < 3; i++) {
            numberOfStrokes[i] = 0;
            handCoords[i].Clear();
            textureDrawn[i] = false;
        }
    }

    /// <summary>
    /// Returns the index of the closest point to the Vector2 point sent as parameter in the user drawn glyph.
    /// /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int minDistanceIndex(Vector2 point, List<Vector2> list, int plane)
    {
        int index = 0;
        for (int i = 1; i < userGlyph[plane].coords.Count; i++)
        {
            if (Vector2.Distance(point, list[index]) > Vector2.Distance(point, list[i]))
            {
                index = i;
            }
        }
        userGlyph[plane].beenChecked[index] = true;
        return index;
    }

    public void DrawOnGUIButton()
    {
        drawOnGUI = !drawOnGUI;
    }


    /// <summary>
    /// Function that draws a line on a given texture2d between two points
    /// </summary>
    /// <param name="tex"> Texture2D object to be drawn on</param>
    /// <param name="x1"> X coordinate of point A</param>
    /// <param name="y1"> Y coordinate of point A</param>
    /// <param name="x0"> X coordinate of point B</param>
    /// <param name="y0"> Y coordinate of point B</param>
    /// <param name="col"> Color for drawing</param>
    public void DrawLine(Texture2D tex, int x1, int y1, int x0, int y0, Color col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }
}

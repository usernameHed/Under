//C# Example
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public class Screenshot : EditorWindow
{
    //Setup
    public UnityEngine.Object sourceMap3d;
    public UnityEngine.Object sourceMap2d;

    private GameObject sourceObject3d;
    private GameObject sourceObject2d;

    private bool setup = false;
    ///////////////////////////////////////////////////Screenshot

    /// <summary>
    /// take screenshot
    /// </summary>
	int resWidth = Screen.width*4; 
	int resHeight = Screen.height*4;

	public Camera myCamera;
	int scale = 1;

	string path = "";
	bool showPreview = true;
	RenderTexture renderTexture;

	bool isTransparent = true;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Tools/Under3d/Collider 2d")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow editorWindow = EditorWindow.GetWindow(typeof(Screenshot));
		editorWindow.autoRepaintOnSceneChange = true;
		editorWindow.Show();
		editorWindow.title = "Screenshot";
	}

	float lastTime;

    private void OnEnable()
    {
        sourceMap3d = GameObject.Find("Map 3d");
        sourceMap2d = GameObject.Find("Map 2d");
        Debug.Log("ici");
    }

    void OnGUI()
	{
        //Setup
        EditorGUILayout.BeginHorizontal();
        sourceMap3d = EditorGUILayout.ObjectField(sourceMap3d, typeof(UnityEngine.Object), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        sourceMap2d = EditorGUILayout.ObjectField(sourceMap2d, typeof(UnityEngine.Object), true);
        EditorGUILayout.EndHorizontal();

        

        ////////////////// screenshot
        EditorGUILayout.LabelField ("Resolution", EditorStyles.boldLabel);
		resWidth = EditorGUILayout.IntField ("Width", resWidth);
		resHeight = EditorGUILayout.IntField ("Height", resHeight);

		EditorGUILayout.Space();

		scale = EditorGUILayout.IntSlider ("Scale", scale, 1, 15);

		EditorGUILayout.HelpBox("The default mode of screenshot is crop - so choose a proper width and height. The scale is a factor " +
			"to multiply or enlarge the renders without loosing quality.",MessageType.None);

		
		EditorGUILayout.Space();
		
		
		GUILayout.Label ("Save Path", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.TextField(path,GUILayout.ExpandWidth(false));
		if(GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
			path = EditorUtility.SaveFolderPanel("Path to Save Images",path,Application.dataPath);
        path = Application.dataPath + "/_Rendu/Screen";

        EditorGUILayout.EndHorizontal();

		EditorGUILayout.HelpBox("Choose the folder in which to save the screenshots ",MessageType.None);
		EditorGUILayout.Space();



		//isTransparent = EditorGUILayout.Toggle(isTransparent,"Transparent Background");



		GUILayout.Label ("Select Camera", EditorStyles.boldLabel);


		myCamera = EditorGUILayout.ObjectField(myCamera, typeof(Camera), true,null) as Camera;


		if(myCamera == null)
		{
			myCamera = Camera.main;
		}

		isTransparent = EditorGUILayout.Toggle("Transparent Background", isTransparent);


		EditorGUILayout.HelpBox("Choose the camera of which to capture the render. You can make the background transparent using the transparency option.",MessageType.None);

		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField ("Default Options", EditorStyles.boldLabel);


		if(GUILayout.Button("Set To Screen Size"))
		{
			resHeight = (int)Handles.GetMainGameViewSize().y;
			resWidth = (int)Handles.GetMainGameViewSize().x;
		
		}


		if(GUILayout.Button("Default Size"))
		{
			resHeight = 1440;
			resWidth = 2560;
			scale = 1;
		}



		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ("Screenshot will be taken at " + resWidth*scale + " x " + resHeight*scale + " px", EditorStyles.boldLabel);

        if (GUILayout.Button("SETUP"))
        {
            sourceObject3d = sourceMap3d as GameObject;
            sourceObject3d.GetComponent<MeshRenderer>().enabled = true;

            sourceObject2d = sourceMap2d as GameObject;
            sourceObject2d.GetComponent<SpriteRenderer>().enabled = true;
            sourceObject2d.GetComponent<SpriteRenderer>().sprite = null;
            sourceObject2d.GetComponent<PolygonCollider2D>().enabled = false;
            Debug.Log("ici");
            setup = true;
        }

        if (GUILayout.Button("Take Screenshot",GUILayout.MinHeight(60)))
		{
            if (!setup)
                return;

			if(path == "")
			{
				path = EditorUtility.SaveFolderPanel("Path to Save Images",path,Application.dataPath);
				Debug.Log("Path Set");
				TakeHiResShot();
			}
			else
			{
				TakeHiResShot();
			}
            Debug.Log("ici");

        }

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Open Last Screenshot",GUILayout.MaxWidth(160),GUILayout.MinHeight(40)))
		{
			if(lastScreenshot != "")
			{
				Application.OpenURL("file://" + lastScreenshot);
				Debug.Log("Opening File " + lastScreenshot);
			}
		}

		if(GUILayout.Button("Open Folder",GUILayout.MaxWidth(100),GUILayout.MinHeight(40)))
		{

			Application.OpenURL("file://" + path);
		}

		if(GUILayout.Button("Convert2sprite last",GUILayout.MaxWidth(100),GUILayout.MinHeight(40)))
		{
            if (lastScreenshot != "")
            {
                if (!setup)
                    return;
                Debug.Log("ici");
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                Debug.Log(path);
                Debug.Log(System.Text.RegularExpressions.Regex.Replace(lastScreenshot, "(?<=Assets).*$", String.Empty));

                TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                tImporter.textureType = TextureImporterType.Sprite;
                tImporter.isReadable = true;
                //tImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
                //tImporter.textureFormat = TextureImporterFormat.ARGB32;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();

                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                Sprite newSprite = asset as Sprite;

                //sourceObject3d.GetComponent<MeshRenderer>().enabled = false;
                sourceObject2d.GetComponent<SpriteRenderer>().sprite = newSprite;
                sourceObject2d.GetComponent<PolygonCollider2D>().enabled = true;
                setup = false;
            }
        }

		EditorGUILayout.EndHorizontal();


		if (takeHiResShot) 
		{
			int resWidthN = resWidth*scale;
			int resHeightN = resHeight*scale;
			RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
			myCamera.targetTexture = rt;

			TextureFormat tFormat;
			if(isTransparent)
				tFormat = TextureFormat.ARGB32;
			else
				tFormat = TextureFormat.RGB24;


			Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat,false);
			myCamera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
			myCamera.targetTexture = null;
			RenderTexture.active = null; 
			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidthN, resHeightN);
			
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));
            //Application.OpenURL(filename);


            //Debug.Log(path + filename);
            AssetDatabase.Refresh();

            Debug.Log("ici");

            takeHiResShot = false;
		}

		EditorGUILayout.HelpBox("In case of any error, make sure you have Unity Pro as the plugin requires Unity Pro to work.",MessageType.Info);


	}


	
	private bool takeHiResShot = false;
	public string lastScreenshot = "";
	
		
	public string ScreenShotName(int width, int height) {

		string strPath="";

		strPath = string.Format("{0}/screen_{1}x{2}_{3}.png", 
		                     path, 
		                     width, height, 
		                               System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
		lastScreenshot = strPath;
	
		return strPath;
	}



	public void TakeHiResShot() {
		Debug.Log("Taking Screenshot");
		takeHiResShot = true;
	}

}


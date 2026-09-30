using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
//using Unity.VisualScripting;

public class AppConfig : Singleton<AppConfig>
{
    public string fileName = "AppConfig.json";
    Config _config;

    [HideInInspector]
    public UnityEvent OnConfigLoaded;

    protected bool _configLoaded = false;

    [System.Serializable]
    public class Config
    {
        public bool debugMode;
        public bool showFullScreen;
        public bool showMouse;
        public LightControlSettings lightControlSettings;
        public KioskCommsDataSettings kioskCommsDataSettings;
        public ShowControlSettings showControlSettings;
        public AttractSettings attract;
        public AmbientModeTiming ambientModeTiming;
        public Window window;
        public TreeColorPresetsDef treeColorPresets;
    }
    
    [System.Serializable]
    public class LightControlSettings
    {
        public string lightTransmitIpAddress;
        public int lightTransmitPort;
        public int lightTransmitStartAddress;
        public int numberOfChannelsPerLight;
        public bool doConnectToLightController;
    }
    
    [System.Serializable]
    public class KioskCommsDataSettings
    {
        public string appID;
        public string ipAddress;
        public int port;
        public bool doConnectToInterComputerClient = true;
    }
    
    [System.Serializable]
    public class TreeColorPresetsDef
    {
        public TreeColorPresetDef[] treeColorPreset;
    }
    [System.Serializable]
    public class TreeColorPresetDef
    {
        public string name;
        public string theme;
        public TreeColorDef treeColorA;
        public TreeColorDef treeColorB;
        public TreeColorDef treeColorC;
    }
    [System.Serializable]
    public class TreeColorDef
    {
        public string trunkGradA;
        public string trunkGradB;
        public string canopyGradA;
        public string canopyGradB;
        public string globe;
    }
    
    [System.Serializable]
    public class AttractSettings
    {
        public int timeout = 30; // seconds
        public bool doTimeout;
    }

    [System.Serializable]
    public class AmbientModeTiming
    {
        public int sweepTime; // frames
        public int pingPongHoldOnTime;
        public int pingPongAdvanceTime;
    }


    /*
    [System.Serializable]
    public class NetworkSettings
    {
        public string ipAddress;
        public int port;
    } */
    
    [System.Serializable]
    public class ShowControlSettings
    {
        public string ipAddress;
        public int receivePort;
        public int sendPort;
        public bool doConnectToShowControl;
        public string stringToSend;
    }

    [System.Serializable]
    public class Window
    {
        [System.Serializable]
        public class Dimensions
        {
            public int width;
            public int height;
        }

        public float frameRate;
        public Vector2 position;
        public Dimensions dimensions;
        public bool forcePosition;
        public bool topMost;
        public bool runInBackground;
        public bool fullscreen;
        public int offset;
    }

    public Config GetConfig {
       get{ 
            if( _config == null)
            {
                Debug.LogWarning("Config is null. Loading defaults.");
                _config = new Config();
            }
            return _config; 
        }
    }

    public bool ConfigLoaded
    {
        get
        { 
            return _configLoaded;
        }
    }


    void Start()
    {
        LoadConfig();

        Application.targetFrameRate = 30;
    }

    private void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Mouse");

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = !Cursor.visible;
        }*/
    }

    private void LoadConfig()
    {
        Debug.Log($"AppConfig.LoadConfig {ConfigLoaded}");

        if (ConfigLoaded) return;

        _configLoaded = true;

        string jsonString = "";

        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        if ( File.Exists(path))
        {
            var reader = new StreamReader(path);
            jsonString = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            Debug.LogError($"Config file is missing path:{path}");
            return;
        }

        if(jsonString.Length > 0)
        {
            _config = JsonUtility.FromJson<Config>(jsonString);


            if( !_config.showMouse)
            {
                Cursor.visible = false;
            }

            if (_config != null && OnConfigLoaded != null) OnConfigLoaded.Invoke();
        
        } else
        {
            Debug.LogError($"Failed to parse config file @ path:{path}");
            return;
        }

        Application.runInBackground = _config.window.runInBackground;

        Debug.Log($"window offset:{_config.window.offset}");
    }

    private void OnDestroy()
    {
        if(OnConfigLoaded!= null) OnConfigLoaded.RemoveAllListeners();
    }
}

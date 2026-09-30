using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using AAMVC.CameraViewControl;
using AAMVC.CommunicationsAndControl;
using TMPro;


namespace AAMVC.Unity
{
    public class ApplicationControl : Singleton<ApplicationControl>
    {
        private AppConfig appConfig;
        private AppConfig.Config config;
        
        private bool dataLoadCompleted = false;
        private LogTextCtrl logTextCtrl;
        private bool configLoaded = false;

        private bool pausingBeforeInit = true;
        private int pauseCounter = 0;
        private bool initComplete = false;
        
        public Camera SceneCamera;
        private SceneViewCameraControl SceneViewCameraControlVar;
        private bool currentlyInOrthographicView = true;
        private bool currentlyDisplayDebugInfo = true;
        
        private Vector2 gameWindowResolution;
        private bool screenResized = false;

        private bool textLogIsVisible = true;
        //public LogTextObj textLog;
        public Camera UICamera;
        
        //private OpenAndReadMonumentConfigXML getConfigFromXML;

        public GameObject debugGrid2D;
        //public GameObject debugGrid3D;
        public GameObject debugFPSText;
        private FPSTextObj fpsTextCtrl;
        public GameObject debugEventLogger;

        public GameObject titleFeedbackObj;
        private feedbackMessagesCtrl titleFeedbackCtrl;
        
        private float attractTimer = 0f;

        private bool isWindowsPlayer = true;
        
        private bool MouseIsVisible = true;
        
        [Header("Communications Objects **************************************")]
        public GameObject networkEventCtrlObj;
        private NetworkEventCtrl networkEventCtrl;

        [Header("Light Control Objects **************************************")]
        
        public GameObject lightControlObj;
        private Z_LightCtrl lightControl;

        public GameObject colorEffectObj;
        private TreeColorEffectCtrl treeColorEffectCtrl;

        //public GameObject sacnControlObj;
        //private SACNCtrl sacnControl;

        //public GameObject audioCtrlObj;
        //private FeedbackAudioCtrl feedbackAudioCtrl;
        
        //public GameObject dmxCtrlObj;
        //private DMXLightCtrl dmxLightCtrl;

        //private NetworkEventTransmitter audioEventTransmitter;

        //public GameObject dmxCtrlObj;
        //private DMXLightCtrl dmxLightCtrl;
        
        //public GameObject sensorCtrlListenerObj01;
        //private SensorListenerCtrl sensorListenCtrl01;
        
        //public GameObject sensorProxFeedbackObj;
        //private ProxFeedbackCtrl proxFeedbackCtrl;

        //public GameObject showControlListenerObj;
        //private ShowControlListenerNoIntercomputerClientCtrl showControlCtrl;

        //private TransmitDataToDMX transmitDataToDmx01;
        //private TransmitDataToDMX transmitDataToDmx02;
        
        public GameObject loadingMessage;

        private static int numberOfArtNetUniverses = 4;
        public GameObject artnetControlObj;
        private ArtnetCtrl artnetCtrl;

        [Header("Mode and State Objects **************************************")]
        //public string currentLightTheme = "none";

        private bool doProceedToNextThemeOnNextUpdate = false; // for commands coming in from network
        private bool doProceedToNextPresetOnNextUpdate = false;
        public TextMeshPro modeFeedbackText;


        public ApplicationState currentApplicationState;
        private ApplicationState prevApplicationState;

        private int currentTreeColorPresetIndex = -1;
        
        public enum ApplicationState
        {
            loading,
            allOn,
            allOff,
            testPreset,
            newDay,
            peakDay,
            lateDay,
            restPeriod
        }


        private void Awake()
        {
            appConfig = AppConfig.Instance;
            appConfig.OnConfigLoaded.AddListener( OnConfigLoaded);
            logTextCtrl = LogTextCtrl.Instance;
        }
        
        void OnConfigLoaded()
        {
            if (configLoaded)
                return; // we already did this
        
            config = appConfig.GetConfig;
            //debugMode = config.debugMode;
            
            if (!config.showMouse) // hide the mouse
            {
                if (MouseIsVisible)
                    toggleMouse();
            }
            else
            {
                if (!MouseIsVisible)
                    toggleMouse();
            }

            /*
            if (config.showFullScreen)
            {
                if (!isFullScreen)
                    toggleFullScreen();
            }
            else
            {
                if (isFullScreen)
                    toggleFullScreen();
            }*/
        
            configLoaded = true;
        }
        
        IEnumerator Init()
        {
            Debug.Log("[APPCTRL] scene initialization");
            //gameWindowResolution = new Vector2(Screen.width, Screen.height);
            gameWindowResolution = new Vector2(100, 200); // force update based upon screen res on init
            SceneViewCameraControlVar = new SceneViewCameraControl(Screen.width, Screen.height);
            SceneViewCameraControlVar.init();

            Application.runInBackground = true;
            
            // force the program to run at 60fps:
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;
            
            //ambientAudioController = new AmbientAudioControlObj();
            
            if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.WindowsEditor))
                isWindowsPlayer = true;
            else
                isWindowsPlayer = false;
            yield return true;
        }

        // Start is called before the first frame update
        void Start()
        {
            int i;
            currentApplicationState = ApplicationState.loading;
            
            StartCoroutine(Init());
           
            logTextCtrl.logText("[APPCTRL] App Control Init", true);
            fpsTextCtrl = debugFPSText.GetComponent<FPSTextObj>();
           

            lightControl = lightControlObj.GetComponent<Z_LightCtrl>();
            treeColorEffectCtrl = colorEffectObj.GetComponent<TreeColorEffectCtrl>();
            
            artnetCtrl = artnetControlObj.GetComponent<ArtnetCtrl>();
            
            networkEventCtrl = networkEventCtrlObj.GetComponent<NetworkEventCtrl>();
            
            loadingMessage.SetActive(true);
            
            modeFeedbackText.text = "Mode: LOADING";
           
            //showControlCtrl = showControlListenerObj.GetComponent<ShowControlListenerNoIntercomputerClientCtrl>();
        }

        private void initializeClassesAfterPause()
        {
            int i;

            /*
            if (config.depthDataSettings.doConnectToDepth)
            {
                sensorListenCtrl01 = sensorCtrlListenerObj01.GetComponent<SensorListenerCtrl>();
                sensorListenCtrl01.setNetworkSettingsAndInitConnection(config.depthDataSettings.numberOfPanelsToTrack, config.depthDataSettings.port, config.depthDataSettings.ipAddress);
            }*/
            
            lightControl.initLights();
            
            //proxFeedbackCtrl.initProxFeedback(config.depthDataSettings.numberOfPanelsToTrack);
            
            loadingMessage.SetActive(false);
            
            //transmitDataToDmx01 = new TransmitDataToDMX(config.lightControlSettings.lightTransmitIpAddress, config.lightControlSettings.lightTransmitPort, config.lightControlSettings.doConnectToLightController);

            if (config.kioskCommsDataSettings.doConnectToInterComputerClient)
            {
                networkEventCtrl.initializeNetworkConnection(config.kioskCommsDataSettings.ipAddress, config.kioskCommsDataSettings.port, config.kioskCommsDataSettings.appID);
            }
            
            artnetCtrl.initAndConnectArtNet();
            
            //sacnControl.initAndConnectSACN();
            

            //lightControl.assignActualNumberOfRegions(config.depthDataSettings.numberOfPanelsToTrack);
            treeColorEffectCtrl.init();
            
            initComplete = true;

            /*
            if (config.showControlSettings.doConnectToShowControl)
            {
                showControlCtrl.retreiveCurrentModeFromShowControl();
            }
            else
            { */
                currentApplicationState = ApplicationState.allOff;
            //}

            networkEventCtrl.requestCurrentModeFromOrchestrator();

        }


        // Update is called once per frame
        void Update()
        {
            if (pausingBeforeInit)
            {
                pauseCounter += 1;
                if (pauseCounter > 60)
                {
                    if (configLoaded)
                    {
                        pausingBeforeInit = false;
                        initializeClassesAfterPause();
                    }
                    else
                    {
                        logTextCtrl.logText("[APPCTRL] Waiting for appconfig.json to load...", true);
                        pauseCounter = 0;
                    }
                }
            }

            if (doProceedToNextThemeOnNextUpdate)
            {
                doProceedToNextThemeOnNextUpdate = false;
                doProceedToNextTheme();
            }

            if (doProceedToNextPresetOnNextUpdate)
            {
                doProceedToNextPresetOnNextUpdate = false;
                adjustTreeColorsToNewCurrentTheme();
            }
            
            if (currentApplicationState != prevApplicationState)
            {
                adjustToNewApplicationState();
            }
            
            if (gameWindowResolution.x != Screen.width || gameWindowResolution.y != Screen.height)
            {
                // do stuff
                screenResized = true;
                resizeAllGameElements();

                gameWindowResolution.x = Screen.width;
                gameWindowResolution.y = Screen.height;
            }

            
            if (!currentlyInOrthographicView)
            {
                int doUpdateCamera = SceneViewCameraControlVar.plotCameraPos();

                SceneCamera.transform.position = SceneViewCameraControlVar.getCameraPosition();
                SceneCamera.transform.LookAt(SceneViewCameraControlVar.getCameraTarget());
            }
        }

        
        private void resizeAllGameElements()
        {
            SceneViewCameraControlVar.setWindowSize(Screen.width, Screen.height);
            //userInterfaceCtrl.setWindowSize(Screen.width, Screen.height);
            logTextCtrl.setWindowSize(Screen.width, Screen.height);
            fpsTextCtrl.setWindowSize(Screen.width, Screen.height);
            if (UICamera != null)
            {
                UICamera.orthographicSize = Screen.height / (100f*2f);
            }
        }

        public void updateFPS(float whichFPS) //disseminate current fps to child objects
        {
            //userInterfaceCtrl.adjustFrameRateTo(whichFPS);
            //_trackAndScaleObj.updateCurrentFPSTo(whichFPS);
        }
        
        public void logText(string whichText)
        {
            logTextCtrl.logText(whichText, config.debugMode);
        }

        public void updateActiveStatus(bool whichState)
        {
            if (whichState)
            {
                logText("[APPCTRL] presence detected");
            }
            else
            {
                logText("[APPCTRL] presence reset");
            }
        }

        
        public void toggleOutputFromUI(int whichOutput)
        {
            //phidgetCtrl.toggleStateOfOuput(whichOutput);
        }

        public void manualMotionTriggerFromUI()
        {
            //visionSystemCtrl.activateIndicator(); 
            //phidgetCtrl.startOutputSequence();
        }
        
        public void outputConnected(int whichOutput)
        {
            //userInterfaceCtrl.activateButton(whichOutput);
        }
        public void outputDisconnected(int whichOutput)
        {
            //userInterfaceCtrl.deactivateButton(whichOutput);
        }
        
        public void noCameraDetected()
        {
            logText("[APPCTRL] NO USB CAMERA DETECTED");
        }

        public void adjustFrameRateTo(int whichFramerate)
        {
            //userInterfaceCtrl.adjustFrameRateTo(whichFramerate);
            // real time adjustments to animations based upon current framerate
        }

        public void updateIndividualArtNetValue(int whichUniverse, int whichChannel, byte whichValue)
        {
            if (initComplete)
                artnetCtrl.updateArtnetDataChannel(whichUniverse, whichChannel, whichValue);
        }

        /*
        private void updateIndividualDetectionStatus()
        {
            for (int i = 0; i < config.depthDataSettings.numberOfPanelsToTrack; ++i)
            {

                proxFeedbackCtrl.updateIndividualStatusText(i, sensorListenCtrl01.retrieveIndividualStatusReportText(i));
            }
        }*/
        #region interactivity

        // ************************************************************************************************
        // start of interactivity

        

        /*
        public void testSendCommandToShowControl()
        {
            
            showControlCtrl.retreiveCurrentModeFromShowControl();
        }*/

        
        /*
        public void updateFromSensors(int whichRegion, int whichCode)
        {
            proxFeedbackCtrl.updateRegionText(whichRegion, whichCode);
            //lightControl.adjustHighlight(whichRegion, whichCode);
        }*/
        
        

        // **************************************
       
        // **************************************
        

        public void toggleManualVsAutoDataFromKeyboard()
        {
            for (int i = 0; i < numberOfArtNetUniverses; ++i)
            {
                //artnetCtrl.toggleManualInput();
            }

        }

        public void toggleArtnetConnection()
        {
            for (int i = 0; i < numberOfArtNetUniverses; ++i)
            {
                //artnetCtrl.toggleConnection();
            }
        }
        
        public void startStopCameraOrbit()
        {
            SceneViewCameraControlVar.toggleCameraOrbit();
        }

        public void sendTestSacnData()
        {
            //sacnControl.toggleSendTestDataFromKeyboard3();
        }

        public void toggleSacnStreamFromKeyboard()
        {
            //sacnControl.toggleSacnStreamFromKeyboard();
        }
        
        
        // ************************************************
        // 3D scene object interactivity
        public void toggleMouse()
        {
            if (MouseIsVisible)
            {
                Cursor.visible = false;
                MouseIsVisible = false;
            }
            else
            {
                Cursor.visible = true;
                MouseIsVisible = true;
            }
        }
       
        // ************************************************
        // UI events
        
        
        // end of interactivity
        // ************************************************************************************************

        #endregion interactivity

        
        #region control object interrops

        
        private void adjustToNewApplicationState()
        {
            logTextCtrl.logText("[APPCTRL] switching state from " + prevApplicationState.ToString() + " to "+currentApplicationState.ToString(), config.debugMode);

            prevApplicationState = currentApplicationState;
            
            
            
            switch (currentApplicationState)
            {
                case ApplicationState.allOn:
                    lightControl.turnOnAllLights();
                    modeFeedbackText.text = "MODE: "+currentApplicationState.ToString();
                    break;
                case ApplicationState.testPreset: // special case presets in the config file for testing
                    adjustTreeColorsToNewCurrentTheme();
                    break;
                case ApplicationState.allOff:
                    lightControl.turnOffAllLights();
                    lightControl.stopAmbientGlobeSparkle();
                    modeFeedbackText.text = "MODE: "+currentApplicationState.ToString();
                    break;
                case ApplicationState.newDay:
                    lightControl.startAmbientGlobeSparkle();
                    adjustTreeColorsToNewCurrentTheme();
                    break;
                case ApplicationState.peakDay:
                    lightControl.startAmbientGlobeSparkle();
                    adjustTreeColorsToNewCurrentTheme();
                    break;
                case ApplicationState.lateDay:
                    lightControl.startAmbientGlobeSparkle();
                    adjustTreeColorsToNewCurrentTheme();
                    break;
                case ApplicationState.restPeriod:
                    lightControl.startAmbientGlobeSparkle();
                    adjustTreeColorsToNewCurrentTheme();
                    break;
            }
        }
        
        
        // **************************************
        
        
        private void setDebugMode(bool whichDebugMode)
        {
            if (currentlyDisplayDebugInfo)
            {
                if (!whichDebugMode)
                {
                    toggleDebugGraphics();
                }
            }
            else
            {
                if (whichDebugMode)
                {
                    toggleDebugGraphics();
                }
            }
        }

        public void toggleDebugGraphics()
        {
            if (currentlyDisplayDebugInfo)
            {
                debugGrid2D.SetActive(false);
                //debugGrid3D.SetActive(false);
                debugFPSText.SetActive(false);
                debugEventLogger.SetActive(false);
                textLogIsVisible = false;
            }
            else
            {
                debugGrid2D.SetActive(true);
                //debugGrid3D.SetActive(true);
                debugFPSText.SetActive(true);
                debugEventLogger.SetActive(true);
                textLogIsVisible = true;
            }

            currentlyDisplayDebugInfo = !currentlyDisplayDebugInfo;
        }

        public void toggleTextFeedbackWindow()
        {
            if (textLogIsVisible)
            {
                debugEventLogger.SetActive(false);
                textLogIsVisible = false;
            }
            else
            {
                debugEventLogger.SetActive(true);
                textLogIsVisible = true;
            }
        }
        
        // **************************************


        public void adjustCurrentStateWithStringFromNetwork(string whichNewState)
        {
            if (whichNewState != currentApplicationState.ToString())
            {
                try
                {
                    currentApplicationState = (ApplicationState)Enum.Parse(typeof(ApplicationState), whichNewState);
                }
                catch (Exception e)
                {
                    logText("[APPCTRL] trying to update state to: " + whichNewState + " which is not a valid state.");
                }
            }
        }

        public void proceedToNextPresetFromNetwork(string whichNewState)
        {
            if (whichNewState != currentApplicationState.ToString()) // make sure theme didn't change
            {
                try
                {
                    currentApplicationState = (ApplicationState)Enum.Parse(typeof(ApplicationState), whichNewState);
                }
                catch (Exception e)
                {
                    logText("[APPCTRL] trying to update state to: " + whichNewState + " which is not a valid state.");
                }
            }
            else
            {
                goToNextPresetWithCurrentThemeFromNetwork();
            }
        }
        
        public void adjustCurrentStateWithString(string whichNewState)
        {
            if (whichNewState != currentApplicationState.ToString())
            {
                try
                {
                    currentApplicationState = (ApplicationState)Enum.Parse(typeof(ApplicationState), whichNewState);
                }
                catch (Exception e)
                {
                    logText("[APPCTRL] trying to update state to: " + whichNewState + " which is not a valid state.");
                }
            }
            /*{
                switch (whichNewState)
                {
                    case "allOn":
                        currentApplicationState = ApplicationState.allOn;
                        break;
                    case "allOff":
                        currentApplicationState = ApplicationState.allOff;
                        break;
                    case "newDay":
                        currentApplicationState = ApplicationState.newDay;
                        //currentLightTheme = "newDay";
                        //goToNextPresetWithCurrentThemeFromNetwork();
                        break;
                    case "peakDay":
                        currentLightTheme = "peakDay";
                        goToNextPresetWithCurrentThemeFromNetwork();
                        break;
                    case "lateDay":
                        currentLightTheme = "lateDay";
                        goToNextPresetWithCurrentThemeFromNetwork();
                        break;
                    case "restPeriod":
                        currentLightTheme = "restPeriod";
                        goToNextPresetWithCurrentThemeFromNetwork();
                        break;
                }
            }*/
        }
        
        public void updateAllTreeColorsTo(int whichColorChannel, Color whichColor0, Color whichColor1)
        {
            lightControl.updateAllTreeColorsTo(whichColorChannel, whichColor0, whichColor1);
        }
        
        public void updateIndividualTreeColorsTo(int whichTreeIndex, Color whichColor0, Color whichColor1)
        {
            lightControl.updateSpecificTreeColorsTo(whichTreeIndex, whichColor0, whichColor1);
        }

        public void updateSingleTreeTrunkColorsTo(int whichTreeIndex, Color whichColor0, Color whichColor1)
        {
            lightControl.updateTreeTrunkColorTo(whichTreeIndex, whichColor0, whichColor1);
        }

        public void updateSingleTreeCanopyColorsTo(int whichTreeIndex, Color whichColor0, Color whichColor1)
        {
            lightControl.updateTreeCanopyColorTo(whichTreeIndex, whichColor0, whichColor1);
        }

        public void updateSingleTreeGlobeColorTo(int whichTreeIndex, Color whichColor0)
        {
            lightControl.updateTreeGlobeColorTo(whichTreeIndex, whichColor0);
        }
        
        
        public void changeStateFromKeyboardTo(ApplicationState whichNewState) // allOff, allon, reactToPresence, testing  // letters 1, 2, 3, 4
        {
            currentApplicationState = whichNewState;
        }

        private void goToNextPresetWithCurrentThemeFromNetwork() // outside of the draw loop
        {
            /*
            currentTreeColorPresetIndex++;
            if (currentTreeColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
                currentTreeColorPresetIndex = 0;

            while (config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme != currentApplicationState.ToString()) // march through all themes until next light mode is selected
            {
                currentTreeColorPresetIndex += 1;
                if (currentTreeColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
                    currentTreeColorPresetIndex = 0;
            }*/

            doProceedToNextPresetOnNextUpdate = true;
        }
        
        private void goToNextPresetWithCurrentTheme() // outside of the draw loop
        {
            currentTreeColorPresetIndex++;
            if (currentTreeColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
                currentTreeColorPresetIndex = 0;

            while (config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme != currentApplicationState.ToString()) // march through all themes until next light mode is selected
            {
                currentTreeColorPresetIndex += 1;
                if (currentTreeColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
                    currentTreeColorPresetIndex = 0;
            }
        }


        private void doProceedToNextTheme() // new theme came in off draw cycle, we are catching up here
        {
            //currentApplicationState = ApplicationState.ambientAnimation;
            adjustCurrentStateWithString(config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme);
            
            //currentLightTheme = config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme;
            //treeColorEffectCtrl.changeColorSchemeTo(currentTreeColorPresetIndex);
            
            //modeFeedbackText.text = "PRESET: # "+ currentTreeColorPresetIndex +"\nTHEME: " + currentApplicationState.ToString();
            //modeFeedbackText.text += "\nNAME: "+ config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].name;
        }

        private void doProceedToNextPreset()
        {
            adjustTreeColorsToNewCurrentTheme();
        }

        public void adjustCurrentLightPresetFromKeyboard(bool doGoForward)
        {
            if (doGoForward)
                currentTreeColorPresetIndex++;
            else
                currentTreeColorPresetIndex--;

            if (currentTreeColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
                currentTreeColorPresetIndex = 0;
            else if (currentTreeColorPresetIndex < 0)
                currentTreeColorPresetIndex = config.treeColorPresets.treeColorPreset.Length - 1;

            //changeStateFromKeyboardTo(ApplicationControl.ApplicationState.ambientAnimation);
            //currentLightTheme = config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme;
            adjustCurrentStateWithString(config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].theme);

            //treeColorEffectCtrl.changeColorSchemeTo(currentTreeColorPresetIndex);

            //modeFeedbackText.text = "PRESET: # "+ currentTreeColorPresetIndex +"\nTHEME: " + currentLightTheme;
            //modeFeedbackText.text += "\nNAME: "+ config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].name;
        }
        
        private void adjustTreeColorsToNewCurrentTheme()
        {
            //if (currentTreeColorPresetIndex < 0) // special case on app launch (index = -1)
                goToNextPresetWithCurrentTheme();
            
            treeColorEffectCtrl.changeColorSchemeTo(currentTreeColorPresetIndex);
            
            modeFeedbackText.text = "PRESET: # "+ currentTreeColorPresetIndex +"\nTHEME: " + currentApplicationState.ToString();
            modeFeedbackText.text += "\nNAME: "+ config.treeColorPresets.treeColorPreset[currentTreeColorPresetIndex].name;

        }
        
        public void updateModeFeedbackToCustom()
        {
            modeFeedbackText.text = "PRESET: none (CUSTOM)";
        }
        
        public void startSparkleWaveFromKeyboard()
        {
            lightControl.startWaveGlobeSparkle();
        }

        public void adjustApplicationStateFromShowControl(ApplicationState whichNewState)
        {
            currentApplicationState = whichNewState;
        }

        
        #endregion control object interrops

               
        #region quitApp
        
        
        public void callQuit()
        {
            Application.Quit();
        }
        public void cleanUpOnQuit()
        {
            Debug.Log("[APPCTRL] cleaning up, calling it quits");
            logTextCtrl.onProgramExit();
            networkEventCtrl.onProgramExit();
            //sacnControl.onProgramExit();
            
            //transmitDataToDmx01.haltingProgram();
            
            /*
            if (sensorListenCtrl01!=null)
                sensorListenCtrl01.onProgramExit();
            */
            //for (int i = 0; i < numberOfArtNetUniverses; ++i)
            //{
                artnetCtrl.onProgramExit();
            //}
        }

        void OnApplicationQuit()
        {
            cleanUpOnQuit();
        }
        #endregion quitApp

    }
}

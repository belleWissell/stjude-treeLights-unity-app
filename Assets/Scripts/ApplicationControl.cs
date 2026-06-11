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
        
        //public int currentApplicationState = -1;

        //public GameObject lightControlObj;
        //private MonumentLightCtrl lightControl;
        
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
        //public GameObject[] artnetControlObj = new GameObject[numberOfArtNetUniverses];
        //private ArtnetCtrl[] artnetCtrl = new ArtnetCtrl[numberOfArtNetUniverses];
        public GameObject artnetControlObj;
        private ArtnetCtrl artnetCtrl;
        
        //public GameObject artnetFeedbackObj;
        //private ArtnetDataDisplayCtrl artnetDataDisplayCtrl;

        public TextMeshPro modeFeedbackText;

        /*
        public enum AnimalState
        {
            hidden,
            hinting,
            revealing
        }*/

        public ApplicationState currentApplicationState;
        private ApplicationState prevApplicationState;
        public enum ApplicationState
        {
            loading,
            allOn,
            ambientAnimation,
            reactToPresence,
            allOff,
            testing
        }

        public TreeColorMode currentTreeColorMode;
        public enum TreeColorMode
        {
            newDay,
            midDay,
            lateDay,
            solidTreesNewDay,
            solidTreesPeakDay,
            solidTreesLateDay
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
            
            //proxFeedbackCtrl = sensorProxFeedbackObj.GetComponent<ProxFeedbackCtrl>();
            
            artnetCtrl = artnetControlObj.GetComponent<ArtnetCtrl>();
            /*
            artnetCtrl[0] = artnetControlObj[0].GetComponent<ArtnetCtrl>();
            artnetCtrl[1] = artnetControlObj[1].GetComponent<ArtnetCtrl>();
            artnetCtrl[2] = artnetControlObj[2].GetComponent<ArtnetCtrl>();
            artnetCtrl[3] = artnetControlObj[3].GetComponent<ArtnetCtrl>();*/
            
            //feedbackAudioCtrl = audioCtrlObj.GetComponent<FeedbackAudioCtrl>();

            //artnetDataDisplayCtrl = artnetFeedbackObj.GetComponent<ArtnetDataDisplayCtrl>();
            
            //sacnControl = sacnControlObj.GetComponent<SACNCtrl>();
            
            //dmxLightCtrl = dmxCtrlObj.GetComponent<DMXLightCtrl>();
            
            //audioCtrl = audioControlObj.GetComponent<AudioCtrl>();
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

            
            
            artnetCtrl.initAndConnectArtNet();
            
            //sacnControl.initAndConnectSACN();
            

            //lightControl.assignActualNumberOfRegions(config.depthDataSettings.numberOfPanelsToTrack);
            treeColorEffectCtrl.init();
            /*
            for (i = 0; i < config.depthDataSettings.numberOfPanelsToTrack; ++i)
            {
                lightControl.assignRegionPositionFromProxFeedback(i, proxFeedbackCtrl.regionPosition[i]);
            }

            if (config.showControlSettings.doConnectToShowControl)
            {
                showControlCtrl.setNetworkSettingsAndInitConnection(config.showControlSettings.stringToSend, config.showControlSettings.ipAddress, config.showControlSettings.sendPort, config.showControlSettings.receivePort);
            }*/
            
            
            initComplete = true;

            /*
            if (config.showControlSettings.doConnectToShowControl)
            {
                showControlCtrl.retreiveCurrentModeFromShowControl();
            }
            else
            { */
                currentApplicationState = ApplicationState.ambientAnimation;
            //}
            
            
            
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

            if (initComplete)
            {
                //updateIndividualDetectionStatus();
                //if (currentApplicationState == ApplicationState.reactToPresence)
                //{
                //    checkForActivatedAreaAndAlertLights();
                //}
            }

        }


        /*
        public void setApplicationStateTo(int whichNewState)
        {
            
            /* /// 0 = attract
            ///     cycle through available animals
            ///     no sound
            /// 1 = someone is present
            ///     leave currently (in)active animal up
            ///     ramp up sound
            ///     listen for 
            /// 2 = someone activated a region
            ///     transition to active animal sequence
            ///  */
            /*
            currentApplicationState = whichNewState;
            textLog.logText("[APPCTRL] state changed to "+currentApplicationState, getConfigFromXML.debugMode);
        } */
        
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

        /*
        private void checkForActivatedAreaAndAlertLights()
        {
            bool isAnyRegionActive = false;
            for (int i = 0; i < config.depthDataSettings.numberOfPanelsToTrack; ++i)
            {
                if (sensorListenCtrl01.isRegionHot(i))
                {
                    isAnyRegionActive = true;
                    if (!lightControl.regionIsActivated[i])
                    {
                        lightControl.regionIsActivated[i] = true;
                        //lightControl.activateLightsInvicinityOf(i);
                    }
                }
                else
                {
                    if (lightControl.regionIsActivated[i])
                    {
                        lightControl.regionIsActivated[i] = false;
                        //lightControl.deactivateLightsInvicinityOf(i);
                    }
                }
            }

            if (!isAnyRegionActive)
            {
                lightControl.resumeAmbientAnimation();
            }
            else
            {
                lightControl.stopAmbientAnimation();
            }
        } */

        public void updateAllTreeColorsTo(int whichColorChannel, Color whichColor0, Color whichColor1)
        {
            lightControl.updateAllTreeColorsTo(whichColorChannel, whichColor0, whichColor1);
        }
        
        public void updateIndividualTreeColorsTo(int whichTreeIndex, Color whichColor0, Color whichColor1)
        {
            lightControl.updateSpecificTreeColorsTo(whichTreeIndex, whichColor0, whichColor1);
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

        //public void playSampleVids()
        //{
        //    videoTestObjCtrl.playAllAnims();
        //}

        private void adjustToNewApplicationState()
        {
            logTextCtrl.logText("[APPCTRL] switching state from " + prevApplicationState.ToString() + " to "+currentApplicationState.ToString(), config.debugMode);

            prevApplicationState = currentApplicationState;
            
            
            
            switch (currentApplicationState)
            {
                case ApplicationState.allOn:
                    lightControl.turnOnAllLights();
                    lightControl.stopAmbientAnimation();
                    //lightControl.turnOffAllLights();
                    //lightControl.stopLightTestCycle();
                    //lightControl.startLightAmbientWaves();
                    modeFeedbackText.text = "MODE: "+currentApplicationState.ToString();
                    break;
                case ApplicationState.testing:
                    lightControl.toggleAllLightboxesOrLightsOn();
                    lightControl.stopAmbientAnimation();
                    //lightControl.stopLightAmbientWaves();
                    break;
                case ApplicationState.allOff:
                    //lightControl.stopLightTestCycle();
                    lightControl.turnOffAllLights();
                    lightControl.stopAmbientGlobeSparkle();
                    //lightControl.stopLightAmbientWaves();
                    modeFeedbackText.text = "MODE: "+currentApplicationState.ToString();
                    break;
                case ApplicationState.ambientAnimation:
                    //lightControl.stopLightTestCycle();
                    //lightControl.fadeOutAllLightsIgnoringReactive();
                    //lightControl.launchAmbientAnimationFromTheStart();
                    //lightControl.stopLightAmbientWaves();
                    lightControl.startAmbientGlobeSparkle();
                    break;
                case ApplicationState.reactToPresence:
                    //lightControl.stopLightTestCycle();
                    lightControl.turnOffAllLights();
                    lightControl.fadeOutAllLightsIgnoringReactive();
                    //lightControl.resumeAmbientAnimation();
                    //lightControl.stopLightAmbientWaves();
                    break;
            }
        }

        /*
        public void testSendCommandToShowControl()
        {
            
            showControlCtrl.retreiveCurrentModeFromShowControl();
        }*/
        
        public void testLightTogglesFromKeyboard()
        {
            if (currentApplicationState != ApplicationState.testing)
                currentApplicationState = ApplicationState.testing;
            else
                lightControl.toggleAllLightboxesOrLightsOn();
        }
        
        public void changeStateFromKeyboardTo(ApplicationState whichNewState) // allOff, allon, reactToPresence, testing  // letters 1, 2, 3, 4
        {
            
            currentApplicationState = whichNewState;
            
        }

        public void switchColorModeFromKeyboardTo(TreeColorMode whichNewColorMode)
        {
            currentTreeColorMode = whichNewColorMode;
            
            treeColorEffectCtrl.adjustColorsTo(whichNewColorMode);

            switch (whichNewColorMode)
            {
                case TreeColorMode.newDay:
                    modeFeedbackText.text = "MODE: new day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
                case TreeColorMode.midDay:
                    modeFeedbackText.text = "MODE: mid day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
                case TreeColorMode.lateDay:
                    modeFeedbackText.text = "MODE: late day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
                case TreeColorMode.solidTreesNewDay:
                    modeFeedbackText.text = "MODE: solids new day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
                case TreeColorMode.solidTreesPeakDay:
                    modeFeedbackText.text = "MODE: solids peak day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
                case TreeColorMode.solidTreesLateDay:
                    modeFeedbackText.text = "MODE: solids Late day";
                    currentApplicationState = ApplicationState.ambientAnimation;
                    break;
            }

        }

        public void startSparkleWaveFromKeyboard()
        {
            lightControl.startWaveGlobeSparkle();
        }

        public void adjustApplicationStateFromShowControl(ApplicationState whichNewState)
        {
            currentApplicationState = whichNewState;
        }
        
        /*
        public void updateFromSensors(int whichRegion, int whichCode)
        {
            proxFeedbackCtrl.updateRegionText(whichRegion, whichCode);
            //lightControl.adjustHighlight(whichRegion, whichCode);
        }*/
        
        

        // **************************************
       
        // **************************************
        // used for real-time dragging
        public void updateVerticalDrag(float whichDist)
        {
            
        }

        public void updateHorzDrag(float whichDist)
        {
            
        }

        public void haltDrag()
        {
           
        }
        // **************************************
        
        // **************************************
        // used for simple dragging
        public void startVertDrag(bool goingDownward)
        {
        }
        
        public void startHorzDrag(bool goingRight)
        {
        }
        // **************************************
        
        
        
        public void buttonPressEvent(string whichButtonName)
        {
            logText("[CTRL] presse event on "+whichButtonName);
            
            switch (whichButtonName)
            {
                
            }
        }

        
        public void togglePerspectiveView()
        {
            if (currentlyInOrthographicView) // switch to perspective view
            {
                SceneCamera.orthographic = false;
                SceneCamera.farClipPlane = 5000;
                currentlyInOrthographicView = false;
            }
            else // switch to orthographic view
            {
                SceneCamera.orthographic = true;
                SceneCamera.farClipPlane = 3000;
                
                //SceneCamera.transform.position = SceneViewCameraControlVar.getCameraPosition();
                SceneCamera.transform.position = SceneViewCameraControlVar.getCameraOrbitZero();
                SceneCamera.transform.LookAt(SceneViewCameraControlVar.getCameraTarget());
                currentlyInOrthographicView = true;
            }

        }
        
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
        /*
        public void testLightsUp()
        {
            textLog.logText("[LIGHTS UP]", true);
            lightControl.fadeUpAllLights();
        }
        public void testLightsDown()
        {
            textLog.logText("[LIGHTS DOWN]", true);
            lightControl.fadeOutAllLights();
        }*/

        /*
        public void DMXcheckForUniverses()
        {
            textLog.logText("[UNiVERSE test]", true);
            //dmxLightCtrl.checkForUniverseFromKeyboard();

        }
        public void DMXsendTestData()
        {
            textLog.logText("[DatA test]", true);
            //dmxLightCtrl.sendTestDataFromKeyboard();

        }
        */
        
        /*
        public void setLightModeTo(int whichMode)
        {
            lightControl.adjustLightLevelsToMode(whichMode);
        }*/

        /*
        public void startRunOfShow()
        {
            lightControl.startRunOfShow();
            //feedbackAudioCtrl.startRunOfShow();
            
        }
        public void stopRunOfShow()
        {
            lightControl.stopRunOfShow();
            //feedbackAudioCtrl.stopRunOfShow();
            
        }*/


        /*
        public void openSquirrelPortal()
        {
            openAnimal(2);
        }
        public void closeSquirrelPortal()
        {
            closeAnimal(2);
        }  
        public void openDeerPortal()
        {
            openAnimal(5);
        }
        public void closeDeerPortal()
        {
            closeAnimal(5);
        }
        
        private void openAnimal(int whichAnimal)
        {
            animalCtrl.animateBlockIn(whichAnimal);
            titleFeedbackCtrl.animateInLabel(whichAnimal);
        }
        private void closeAnimal(int whichAnimal)
        {
            animalCtrl.animateBlockOut(whichAnimal);
            titleFeedbackCtrl.animateOutLabel(whichAnimal);
        }
        */
        
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
        public void onMouseOverSceneObject(int whichDataPoint, Vector3 whichPositionOnScreen)
        {
            //Debug.Log("[APPCTRL] detected mouse over "+whichDataPoint);
            /*if (VisualizationControl.isDataPointActive(whichDataPoint))
            {
                VisualizationControl.stopDataPointMotion(whichDataPoint);
                HudController.showDataFor(whichDataPoint, whichPositionOnScreen);
            }*/
        }
        public void onMouseOutOfSceneObject()
        {
            /*
            VisualizationControl.freeDataPoints();
            HudController.hideDataBox();*/
        }
        
        // ************************************************
        // Background (camera navigation) events

        public void onMouseDownOnBackground(float whichMouseX, float whichMouseY)
        {
            SceneViewCameraControlVar.mouseDn(whichMouseX, whichMouseY);
        }

        public void onMouseDragOnBackground(float whichMouseX, float whichMouseY)
        {
            SceneViewCameraControlVar.mouseMove(whichMouseX, whichMouseY);
        }

        public void onMouseUpOnBackground()
        {
            SceneViewCameraControlVar.mouseUp();
        }

        public void onMouseScrollOnBackground(float whichScrollAmnt)
        {
            SceneViewCameraControlVar.mouseWheel(whichScrollAmnt);
        }

        // ************************************************
        // UI events
        
        public void onMouseUpOnUI() // used mostly for drag events
        {
            //HudController.onMouseUpUI();
            //userInterfaceCtrl.onMouseOutUI();
        }
    
        public void onMouseHitUIButton(int whichButton)
        {
            //HudController.mouseDownButton(whichButton);
            //userInterfaceCtrl.mouseDownButton(whichButton);

        }
    
        public void onMouseOverUIButton(int whichButton)
        {
            //HudController.mouseOverButton(whichButton);
            //userInterfaceCtrl.mouseOverButton(whichButton);
        }
    
        public void onMouseOutOfUI()
        {
            //HudController.onMouseUpUI();
            //HudController.onMouseUpScroll();
            //userInterfaceCtrl.onMouseOutUI();

        }
    
        public void onMouseRolledOutOfUI()
        {
            //HudController.onMouseUpUI();
            //userInterfaceCtrl.onMouseOutUI();

        }
    
        // ************************************************
        // Scroll Bar events
    
        public void onMouseRolledOutOfScroll()
        {
            //HudController.onMouseUpScroll();
        }
    
        public void onMouseHitScrollBar(float whichXPos)
        {
            
            //HudController.updateScrollPosn(whichXPos);
        }
        
        // end of interactivity
        // ************************************************************************************************

        #endregion interactivity

        
        #region control object interrops
        


        
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

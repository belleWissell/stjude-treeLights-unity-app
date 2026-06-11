using System;
using System.Collections;
using System.Collections.Generic;
using AAMVC.Unity;
using AAMVC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace AAMVC.Input
{
    public class KeyboardControls : MonoBehaviour
    {

        // any and all recognized keys listed here
        private KeyCode[] codes =
        {
            KeyCode.Escape,
            KeyCode.O, KeyCode.R, KeyCode.D, KeyCode.C, KeyCode.Z,
            KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
            KeyCode.Space,
            KeyCode.F1, KeyCode.F2,
            KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5
        };

        private ApplicationControl appControl;
        private LogTextCtrl logTextCtrl;
        
        [TextArea()] [SerializeField] private string CommandReference;
        //public MessageProtocol.CustomMessageEvent MessageEvent;

        private void Awake()
        {
            appControl = ApplicationControl.Instance;
            logTextCtrl = LogTextCtrl.Instance;
        }

        // Start is called before the first frame update
        private void Start()
        {
            
            CommandReference = string.Join(", ", codes);
        }

        // Update is called once per frame
        void Update()
        {
            Keyboard kboard = Keyboard.current;
 
            if (kboard.anyKey.wasPressedThisFrame)
            {
                foreach (KeyControl k in kboard.allKeys)
                {
                    if (k.wasPressedThisFrame)
                    {
                        callLocalCommand2(k.keyCode); 
                        break;
                    }
                }
            }
        }
        
        private void callLocalCommand2(Key code)
        {
           logTextCtrl.logText("processing keypress " + code.ToString(), true);

            switch (code)
            {
                case Key.Escape:
                    appControl.cleanUpOnQuit();
                    appControl.callQuit();
                    
                    break;
                case Key.D:
                    appControl.toggleDebugGraphics();
                    
                    break;
                case Key.M:
                    appControl.toggleMouse();
                    
                    break;
                case Key.T:
                    appControl.toggleTextFeedbackWindow();
                    
                    break;
                case Key.A:
                    //appControl.changeStateFromKeyboardTo("ambient");
                    //appControl.toggleManualVsAutoDataFromKeyboard();
                    appControl.toggleSacnStreamFromKeyboard();
                    break;
                case Key.Z:
                    appControl.toggleArtnetConnection();
                    
                    break;

                case Key.S:
                    appControl.sendTestSacnData();
                    break;
                case Key.RightArrow:
                {
                    appControl.testLightTogglesFromKeyboard();
                    break;
                }
                /*case Key.LeftArrow:
                {
                    appControl.testSendCommandToShowControl();
                    break;
                }*/
                case Key.Digit1:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.newDay);
                    break;
                } 
                case Key.Digit2:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.midDay);
                    break;
                } 
                case Key.Digit3:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.lateDay);
                    break;
                } 
                case Key.Digit4:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.solidTreesNewDay);
                    break;
                } 
                case Key.Digit5:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.solidTreesPeakDay);
                    break;
                } 
                case Key.Digit6:
                {
                    appControl.switchColorModeFromKeyboardTo(ApplicationControl.TreeColorMode.solidTreesLateDay);
                    break;
                } 
                case Key.W:
                case Key.R:
                {
                    appControl.startSparkleWaveFromKeyboard();
                    break;
                } 
                case Key.Digit0:
                {
                    appControl.changeStateFromKeyboardTo(ApplicationControl.ApplicationState.allOff);
                    break;
                } 
                case Key.Digit9:
                {
                    appControl.changeStateFromKeyboardTo(ApplicationControl.ApplicationState.ambientAnimation);
                    break;
                } 
                /*case Key.K:
                {

                    break;
                }

                case Key.Digit5:
                    appControlObj.startRunOfShow();
                    break;
                case Key.Digit6:
                    appControlObj.stopRunOfShow();
                    break;
                case Key.Digit7:
                    appControlObj.setLightModeTo(0);
                    break;
                case Key.Digit8:
                    appControlObj.testLightsUp();
                    break;
                case Key.Digit9:
                    appControlObj.testLightsDown();
                    break;
                case Key.U:
                    appControlObj.DMXcheckForUniverses();
                    break;
                case Key.I:
                    appControlObj.DMXsendTestData();
                    break; */
                /*
                case Key.W:
                    appControlObj.toggleWaterBlockTest();
                    break;
                case Key.Digit0:
                    appControlObj.hideAllAnimals();
                    break;
                
                case Key.Digit4:
                    appControlObj.toggleAnimal(3);
                    break;
                case Key.Digit5:
                    appControlObj.toggleAnimal(4);
                    break;
                case Key.Digit6:
                    appControlObj.toggleAnimal(5);
                    break;*/
                default:
                    Debug.Log("Unmatched KeyPress");
                    break;
            }
        }

    }
    
    
}

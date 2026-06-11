using UnityEngine.Audio;
using UnityEngine;
using System; // for try catch
using System.Threading.Tasks; // for async
using UnityEngine.Networking; // for webrequest type loading
using System.IO; // for sorting out path issues
[System.Serializable]
public class AudioVoice
{
   private int myID = -1;
   private GameObject parent;
   
   public string audioID; // the name
   
   public AudioClip clip;

   [Range(0f, 1f)]
   public float volume;
   [Range(0.1f, 3f)]
   public float pitch;

   public bool voiceIsReady = false;

   private string voiceFileNameAndPath = null;

   [HideInInspector]
   public AudioSource source;

   public AudioVoice(int whichID, GameObject whichparent)
   {
      myID = whichID;
      parent = whichparent;
   }
   
   public void initVoiceAndLoadFile(string whichFileAndPath)
   {
      voiceFileNameAndPath = whichFileAndPath;
      loadClipAsync();
   }

   async void loadClipAsync()
   {
      clip = await LoadClip(makeAbsolutePath(voiceFileNameAndPath));

      if (clip != null)
      {
         voiceIsReady = true;
         
      }
   }
   
   async Task<AudioClip> LoadClip(string path)
   {
      AudioClip clipToReturn = null;
      using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
      {
         uwr.SendWebRequest();

         // wrap tasks in try/catch, otherwise it'll fail silently
         try
         {
            while (!uwr.isDone) await Task.Delay(5);

            if (uwr.isNetworkError || uwr.isHttpError) 
               Debug.Log($"audio] error loading audio file {uwr.error}");
            else
            {
               clip = DownloadHandlerAudioClip.GetContent(uwr);
            }
         }
         catch (Exception err)
         {
            System.Diagnostics.Debug.WriteLine("[audio] error loading audio file");
            Debug.Log($"[audio] error loading audio file {err.Message}, {err.StackTrace}");
            return null;
         }
      }
      return clipToReturn;
   }
   
   private string makeAbsolutePath(string file)
   {
      string result = file;
      //create folder if it does not exist
      string folder = Path.GetDirectoryName(result);
      if (folder == String.Empty)
      {
         //if so prepend current directory
         result = Directory.GetCurrentDirectory() + "\\" + file;
      }

      string removeDoubleSlash01 = result.Replace(@"\\", @"\");
      string removeDoubleSlash02 = removeDoubleSlash01.Replace(@"//", @"/");

      return removeDoubleSlash02;
   }
}

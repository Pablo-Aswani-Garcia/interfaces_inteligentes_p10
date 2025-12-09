using System.Diagnostics;
using UnityEngine;
using Whisper.Utils;
// Add the correct using directive for WhisperManager if it exists, for example:
using Whisper;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView; // <-- Change this to the actual namespace where WhisperManager is defined

public class react_to_voice : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public WhisperResult result;
        public  GameObject enemy;
        public string outputText;
        bool attack_triggered = false;
        bool flee_triggered = false;

    
        void Start()
        {
            whisper = gameObject.AddComponent<WhisperManager>();
            microphoneRecord = gameObject.AddComponent<MicrophoneRecord>();
            microphoneRecord.OnRecordStop += OnRecordStop;
            whisper.OnNewSegment += (segment) =>
            {
                outputText += segment + " ";
            };
            
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
               if (!microphoneRecord.IsRecording)
                {
                    microphoneRecord.StartRecord();
                } else
                {
                    microphoneRecord.StopRecord();
                }
            }
            if (result != null)
            {
                if ( outputText.ToLower().Contains("attack"))
                {
                    if (!attack_triggered)
                    {
                        attack_triggered = true;
                        flee_triggered = false;
                    }

                }
                else if (outputText.ToLower().Contains("flee"))
                {
                    if (!flee_triggered)
                    {
                        flee_triggered = true;
                        attack_triggered = false;

                    }
                }
                result = null;
            }
        }


        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            result = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
          if (result == null || outputText == null) 
                return;
            outputText = result.Result;

        }
    void FixedUpdate()
    {
        Vector3 direction = enemy.transform.position - transform.position;
        direction = direction.normalized;
        if (attack_triggered)
        {
            transform.Translate(direction * Time.deltaTime * 5);
            UnityEngine.Debug.Log("Attacking!");
        }
        if (flee_triggered)
        {
            transform.Translate(-direction * Time.deltaTime * 5);
        }
    }
}

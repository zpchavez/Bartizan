// using System;
using TowerFall;
using System;
using System.IO;
// using System.Net;
using SDL2;
using System.Diagnostics;

namespace Mod
{
  public class TrackerApiClient
  {
    private string apiUrl;
    private string apiKey;
    private bool isSetup;

    public TrackerApiClient()
    {
      this.isSetup = false;
      string trackerApiSettingsFile = Path.Combine (GetSavePath(), "tf-tracker-api.txt");
      if (File.Exists (trackerApiSettingsFile)) {
        string[] trackerApiSettings = File.ReadAllLines(trackerApiSettingsFile);
        if (trackerApiSettings.Length < 2) {
          TFGame.Log(new Exception("Invalid tf-tracker-api.txt contents"), false);
        }
        this.isSetup = true;
        this.apiUrl = trackerApiSettings[0];
        this.apiKey = trackerApiSettings[1];
      }
    }

    // private static void RespCallback(IAsyncResult ar)
    // {
    //   TFGame.Log(new Exception("Result Callback"), false);
    // }

    // This SHOULD be accessible from TFGame but isn't so I copy/pasted it
    public static string GetSavePath ()
    {
      string text = SDL.SDL_GetPlatform ();
      string result;
      if (text.Equals ("Linux")) {
        string text2 = Environment.GetEnvironmentVariable ("XDG_DATA_HOME");
        if (string.IsNullOrEmpty (text2)) {
          text2 = Environment.GetEnvironmentVariable ("HOME");
          if (string.IsNullOrEmpty (text2)) {
            result = ".";
            return result;
          }
          text2 += "/.local/share";
        }
        text2 += "/TowerFall";
        if (!Directory.Exists (text2)) {
          Directory.CreateDirectory (text2);
        }
        result = text2;
      } else if (text.Equals ("Mac OS X")) {
        string text2 = Environment.GetEnvironmentVariable ("HOME");
        if (string.IsNullOrEmpty (text2)) {
          result = ".";
        } else {
          text2 += "/Library/Application Support/TowerFall";
          if (!Directory.Exists (text2)) {
            Directory.CreateDirectory (text2);
          }
          result = text2;
        }
      } else {
        if (!text.Equals ("Windows")) {
          throw new Exception ("SDL2 platform not handled!");
        }
        result = AppDomain.CurrentDomain.BaseDirectory;
      }
      return result;
    }

    public bool IsSetup()
    {
      return this.isSetup;
    }

    public void SaveStats(TrackerMatchStats stats) {
      string payload = stats.ToJSON(this.apiKey).Replace("\"", "\\\"");
      this.MakeRequest("POST", payload);
    }

    public void MakeRequest(string method, string payload)
    {
      string trackerApiSettingsFile = Path.Combine (TrackerApiClient.GetSavePath(), "tf-tracker-api.txt");

      Process.Start(
        "/bin/bash",
        "-c \"curl '" + apiUrl + "matches' " +
        "-X" + method + " -H 'Content-Type: application/json' -H 'Accept: application/json' " +
        "--data-binary '" + payload + "' --compressed\""
      );
    }
  }
}
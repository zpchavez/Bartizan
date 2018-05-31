using Patcher;
using TowerFall;
using SDL2;
using System;
using System.IO;
using System.Net;

namespace Mod
{
  [Patch]
  public class MyVersusMatchResults : VersusMatchResults
  {
    static WebClient client = new WebClient();

    public MyVersusMatchResults (Session session, VersusRoundResults roundResults) : base(session, roundResults)
    {
      ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
      ServicePointManager.SecurityProtocol = (SecurityProtocolType) 3072; //TLS 1.2
    }

    private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
      return true;
    }

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

    private static void RespCallback(IAsyncResult ar)
    {
      TFGame.Log(new Exception("Result Callback"), false);
    }

    public override void Added ()
    {
      base.Added();

      string trackerApiSettingsFile = Path.Combine (GetSavePath(), "tf-tracker-api.txt");
      if (File.Exists (trackerApiSettingsFile)) {
        string[] trackerApiSettings = File.ReadAllLines(trackerApiSettingsFile);
        if (trackerApiSettings.Length < 2) {
          TFGame.Log(new Exception("Invalid tf-tracker-api.txt contents"), false);
          return;
        }
        string apiUrl = trackerApiSettings[0];
        string apiKey = trackerApiSettings[1];
        TrackerMatchStats stats = new TrackerMatchStats();

        stats.rounds = ((MySession)this.session).RoundsPlayedThisMatch;
        for (int index = 0; index < this.session.MatchStats.Length; index++) {
          if (TFGame.Players[index]) {
            stats.kills[index] = (int)this.session.MatchStats[index].Kills.Kills;
            stats.deaths[index] =
              (int)this.session.MatchStats[index].Deaths.Kills +
              (int)this.session.MatchStats[index].Deaths.SelfKills +
              (int)this.session.MatchStats[index].Deaths.TeamKills;
            stats.wins[index] = this.session.MatchStats[index].Won ? 1 : 0;
            stats.selfs[index] = (int)this.session.MatchStats[index].Kills.SelfKills;
            stats.teamKills[index] = (int)this.session.MatchStats[index].Kills.TeamKills;
            stats.revives[index] = (int)this.session.MatchStats[index].Revives;
          }
        }

        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        client.UploadString(apiUrl + "/matches", stats.ToJSON(apiKey));
      }
    }
  }
}

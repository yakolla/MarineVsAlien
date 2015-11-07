using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GPlusPlatform {

	GoogleAnalyticsV3 m_ga;
	static GPlusPlatform m_ins = null;
	static public GPlusPlatform Instance
	{
		get {
			if (m_ins == null)
			{
				m_ins = new GPlusPlatform();

				PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
					// enables saving game progress.
					.EnableSavedGames()
						.Build();
				
				PlayGamesPlatform.InitializeInstance(config);
				// recommended for debugging:
				PlayGamesPlatform.DebugLogEnabled = true;
				// Activate the Google Play Games platform
				PlayGamesPlatform.Activate();
			}

			return m_ins;
		}
	}

	public void Login(System.Action<bool> callback)
	{
		Social.localUser.Authenticate(callback);
	}

	public bool IsAuthenticated()
	{
		return PlayGamesPlatform.Instance.IsAuthenticated();
	}

	public void OpenGame(string filename, System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
		                                                    ConflictResolutionStrategy.UseLongestPlaytime, callback);
	}

	public void LoadGame(ISavedGameMetadata game, System.Action<SavedGameRequestStatus, byte[]> callback)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, callback);
	}

	public void SaveGame(ISavedGameMetadata game, byte[] savedData, System.TimeSpan totalPlaytime, Texture2D img, System.Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
		
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		
		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
				.WithUpdatedDescription("Saved game at " + System.DateTime.Now);

		if (img != null)
		{
			byte[] pngData = img.EncodeToPNG();
			builder = builder.WithUpdatedPngCoverImage(pngData);
		}
		
		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
		
	}

	public void ShowSavedGameBoard(uint maxNumToDisplay, System.Action<SelectUIStatus, ISavedGameMetadata> callback) {
		bool allowCreateNew = true;
		bool allowDelete = true;
		
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ShowSelectSavedGameUI("Select saved game",
		                                      maxNumToDisplay,
		                                      allowCreateNew,
		                                      allowDelete,
		                                      callback);
	}

	public void ShowLeaderboardUI()
	{
		Social.ShowLeaderboardUI();
	}

	public void ShowLeaderboardUI(string id)
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI(id);
	}

	public void ShowAchievementsUI()
	{
		Social.ShowAchievementsUI();
	}

	public void ReportProgress(string code, int progress, System.Action<bool> callback)
	{
		PlayGamesPlatform.Instance.ReportProgress(
			code, progress, (bool success) => {
			callback(success);
		});  
	}

	public void ReportScore(string code, long score, System.Action<bool> callback)
	{
		Social.ReportScore(score, code, (bool success) => {
			callback(success);
		}); 
	}

	public void InitAnalytics(GoogleAnalyticsV3 ga)
	{
		m_ga = ga;		
	}

	public void AnalyticsTrackEvent(string eventCategory, string eventAction, string eventLabel, int eventValue)
	{
		m_ga.LogEvent(eventCategory,eventAction, eventLabel,eventValue);
	}

	public void AnalyticsTrackScreen(string title)
	{
		m_ga.LogScreen(title);
	}
}

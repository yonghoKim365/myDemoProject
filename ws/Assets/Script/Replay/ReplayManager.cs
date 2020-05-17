using UnityEngine;
using System.Collections;
using System.IO;
using System;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.Text;
using ICSharpCode.SharpZipLib.Core;

public class ReplayManager  
{

	private static ReplayManager _instance = null;

	public static ReplayManager instance
	{
		get
		{
			if(_instance == null) _instance = new ReplayManager();            
			return _instance;
		}
	}

	// 기본적으로 쓸 메모리 스트림.
	MemoryStream _ms = new MemoryStream();

//	public bool useTempSaveEnctyption = true;

	public bool[] byte1 = new bool[8];
	public bool[] byte2 = new bool[8];
	
	byte[] tempByte = new byte[1];

	private int _seedNum = 0;

	public int nowFrame = 0;

	// 이어하기때 자동전투 상황인지.
	public bool isAutoPlayContinueGame = false;

	public int tempSaveCount = 0;


	public void init(int seedNum)
	{
		tempSaveCount = 0;

		BattleSimulator.instance.nowSeed = seedNum;
		//System.Buffer.BlockCopy
		_seedNum = seedNum;

		_ms.Close();
		_ms.Dispose();

		_ms = new MemoryStream();

		nowFrame = 0;

		for(int i = 0; i < 8; ++i)
		{
			byte1[i] = false;
			byte2[i] = false;
		}

		resetButtonState();
	}

	public bool[] skillButtons = new bool[3];
	public bool[] unitButtons = new bool[5];

	public bool changePlayer = false;
	public bool changePVPPlayer = false;

	public void resetMemoryStreamPosition()
	{
		if(_ms != null) _ms.Position = 0;
		else if(_replayData != null) _replayData.Position = 0;

		nowFrame = 0;
	}

	public void addData(bool hasButtonEvent, Player.MoveState moveState)
	{
		byte1[0] = hasButtonEvent;

		switch(moveState)
		{
		case Monster.MoveState.Backward:
			byte1[1] = true;
			byte1[2] = false;
			byte1[3] = false;
			break;
		case Monster.MoveState.Stop:
			byte1[1] = false;
			byte1[2] = true;
			byte1[3] = false;
			break;
		case Monster.MoveState.Forward:
			byte1[1] = false;
			byte1[2] = false;
			byte1[3] = true;
			break;
		}

		byte1[4] = GameManager.me.isAutoPlay;

		byte1[5] = changePlayer;
		byte1[6] = changePVPPlayer;

		_ms.Write(boolArrayToByteArray(byte1),0,1);

		if(byte1[0])
		{
			byte2[0] = unitButtons[0];
			byte2[1] = unitButtons[1];
			byte2[2] = unitButtons[2];
			byte2[3] = unitButtons[3];
			byte2[4] = unitButtons[4];
			byte2[5] = skillButtons[0];
			byte2[6] = skillButtons[1];
			byte2[7] = skillButtons[2];

			_ms.Write(boolArrayToByteArray(byte2),0,1);
		}
	}



	public void resetButtonState()
	{
		unitButtons[0] = false;
		unitButtons[1] = false;
		unitButtons[2] = false;
		unitButtons[3] = false;
		unitButtons[4] = false;
		
		skillButtons[0] = false;
		skillButtons[1] = false;
		skillButtons[2] = false;

		changePlayer = false;
		changePVPPlayer = false;

//		isAutoPlayContinueGame = false;
	}


	public void saveReplayData(string successType)
	{
		_ms.Position = 0;
		byte[] replayBuffer = new byte[_ms.Length];
		_ms.Read(replayBuffer, 0, (int)_ms.Length);

		Crc32 crc = new Crc32();
		
		string fileName = REPLAY_NAME;
		string zipPath = AssetBundleManager.getLocalFilePath() + fileName;
		
		try
		{
			//FileStream writer = File.Create(zipPath);//new FileStream(zipPath, FileMode.Create, FileAccess.Write);
			
			MemoryStream writer = new MemoryStream(); 
			
			ZipOutputStream zos = new ZipOutputStream(writer);
			zos.SetLevel(9);
			
			for(int i = 0; i < 2; ++i)
			{
				byte[] buffer;
				ZipEntry entry;
				
				if(i == 0)
				{
					buffer = replayBuffer;
					entry = new ZipEntry("r");
				}
				else
				{
					Util.stringBuilder.Length = 0;
					Util.stringBuilder.Append( GameManager.info.clientFullVersion ); // 게임 버전.
					Util.stringBuilder.Append(",");
					Util.stringBuilder.Append(GameManager.me.stageManager.isSurrenderGame?WSDefine.YES:WSDefine.NO); // 중도 포기한 게임인가?
					Util.stringBuilder.Append(",");
					Util.stringBuilder.Append((successType==WSDefine.GAME_SUCCESS)?WSDefine.YES:WSDefine.NO); // 이긴 게임인가?
					Util.stringBuilder.Append(",");
					Util.stringBuilder.Append(UIPlay.playerLeagueGrade);
					Util.stringBuilder.Append(",");
					Util.stringBuilder.Append(UIPlay.pvpleagueGrade.Get());
					
					buffer = System.Text.Encoding.UTF8.GetBytes(Util.stringBuilder.ToString());
					entry = new ZipEntry("t");
				}
				
				entry.DateTime = DateTime.Now;
				
				crc.Reset();
				crc.Update(buffer);
				entry.Crc = crc.Value;
				entry.Size = buffer.Length;
				zos.PutNextEntry(entry);
				zos.Write(buffer, 0, buffer.Length);
				zos.CloseEntry(); //new
			}
			
			zos.IsStreamOwner = false;
			zos.Finish();
			zos.Close();
			
//			Debug.LogError(writer.Length);
			
			writer.Position = 0;
			File.WriteAllBytes(zipPath, Util.encByte (writer.ToArray()));
			
			writer.Close();
			writer.Dispose();
			writer = null;
			
		}
		catch(Exception e)
		{
			
		}

		_ms.Close();
		_ms.Dispose();
		_ms = new MemoryStream();
	}





	public void tempSave(GamePlayerData gpdAtStartTime = null, GamePlayerData gpd2AtStartTime = null)
	{
		long position = _ms.Position;

		_ms.Position = 0;
		byte[] bytes = new byte[_ms.Length];
		_ms.Read(bytes, 0, (int)_ms.Length);

		saveTempZip(bytes);
		PlayerPrefs.SetString(TEMP_FILE_NAME,System.Text.UTF8Encoding.UTF8.GetString(bytes));

		// 헤더 저장 부분.
		Util.stringBuilder.Length = 0;
		Util.stringBuilder.Append( GameManager.info.clientFullVersion );
		Util.stringBuilder.Append( "," );
		Util.stringBuilder.Append( _seedNum );
		Util.stringBuilder.Append( "," );
		Util.stringBuilder.Append(GameManager.me.stageManager.nowRound.mode);
		Util.stringBuilder.Append(",");
		Util.stringBuilder.Append(GameManager.me.stageManager.nowRound.id);
		Util.stringBuilder.Append("," );

		switch(GameManager.me.stageManager.nowPlayingGameType)
		{
		case GameType.Mode.Hell:
			Util.stringBuilder.Append( RoundData.TYPE.HELL);
			Util.stringBuilder.Append("," );
			Util.stringBuilder.Append( HellModeManager.instance.roundIndex );
			break;

		case GameType.Mode.Epic:
			Util.stringBuilder.Append( RoundData.TYPE.EPIC);
			Util.stringBuilder.Append( ",");
//			Util.stringBuilder.Append( UIRoundClearPopup.rewardExp);
			Util.stringBuilder.Append( ",");
//			Util.stringBuilder.Append( UIRoundClearPopup.rewardGold);
			break;

//		case GameType.Mode.Challenge:
//			Util.stringBuilder.Append( RoundData.TYPE.CHALLENGE);
//			Util.stringBuilder.Append(",");
//			Util.stringBuilder.Append( GameManager.me.uiManager.uiPlay.challangeModeInfo.rank );
//			break;

		case GameType.Mode.Championship:
			Util.stringBuilder.Append( RoundData.TYPE.CHAMPIONSHIP);
			
			Util.stringBuilder.Append(",");

			if(gpdAtStartTime != null)
			{
				Util.stringBuilder.Append(gpdAtStartTime.serialize());

				if(gpd2AtStartTime != null)
				{
					Util.stringBuilder.Append(",Y,");
					Util.stringBuilder.Append(gpd2AtStartTime.serialize());
				}
				else
				{
					Util.stringBuilder.Append(",N");
				}

			}
			else 
			{
				if(GameManager.me.pvpPlayer == null || GameManager.me.pvpPlayer.playerData == null)
				{
					_ms.Position = position;
					return;
				}

				Util.stringBuilder.Append(GameManager.me.battleManager.pvpPlayers[0].playerData.serialize());


				if(GameManager.me.battleManager.pvpPlayers[1] != null)
				{
					Util.stringBuilder.Append(",Y,");
					Util.stringBuilder.Append(GameManager.me.battleManager.pvpPlayers[1].playerData.serialize());
				}
				else
				{
					Util.stringBuilder.Append(",N");
				}

			}



			break;
		case GameType.Mode.Friendly:
			Util.stringBuilder.Append( RoundData.TYPE.FRIENDLY);

			Util.stringBuilder.Append(",");

			if(gpdAtStartTime != null)
			{
				Util.stringBuilder.Append(gpdAtStartTime.serialize());

				if(gpd2AtStartTime != null)
				{
					Util.stringBuilder.Append(",Y,");
					Util.stringBuilder.Append(gpd2AtStartTime.serialize());
				}
				else
				{
					Util.stringBuilder.Append(",N");
				}
			}
			else
			{
				if(GameManager.me.pvpPlayer == null || GameManager.me.pvpPlayer.playerData == null)
				{
					_ms.Position = position;
					return;
				}

				Util.stringBuilder.Append(GameManager.me.battleManager.pvpPlayers[0].playerData.serialize());

				if(GameManager.me.battleManager.pvpPlayers[1] != null)
				{
					Util.stringBuilder.Append(",Y,");
					Util.stringBuilder.Append(GameManager.me.battleManager.pvpPlayers[1].playerData.serialize());
				}
				else
				{
					Util.stringBuilder.Append(",N");
				}

			}

			break;
		}

		string dataheader = Util.stringBuilder.ToString();
		Util.stringBuilder.Length = 0;
#if !UNITY_EDITOR

#endif
//		if(useTempSaveEnctyption)
		{
			File.WriteAllBytes(AssetBundleManager.getLocalFilePath()+TEMP_HEADER_FILE_NAME, Util.enc (dataheader) );

			byte[] encBa = Util.enc (dataheader);

			string saveHeader = Convert.ToBase64String(encBa);

//			Debug.LogError("saveHeader : " + saveHeader);

			PlayerPrefs.SetString(TEMP_HEADER_FILE_NAME, saveHeader);
		}
//		else
//		{
//			File.WriteAllText(AssetBundleManager.getLocalFilePath()+TEMP_HEADER_FILE_NAME, dataheader);
//			PlayerPrefs.SetString(TEMP_HEADER_FILE_NAME,dataheader);
//		}

		_ms.Position = position;

#if UNITY_EDITOR
		Log.saveFileLog();
#else
//		if(GameManager.isDebugBuild) Log.saveFileLog();
#endif
	}



	public string getSavedReplayData()
	{
		try
		{
			using (FileStream file = new FileStream(AssetBundleManager.getLocalFilePath()+REPLAY_NAME, FileMode.Open, System.IO.FileAccess.Read)) 
			{
				_ms = new MemoryStream();
				_ms.SetLength(file.Length);
				file.Read(_ms.GetBuffer(), 0, (int)file.Length);
				file.Close();
				_ms.Position = 0;
			}
			
			long position = _ms.Position;
			_ms.Position = 0;
			byte[] bytes = new byte[_ms.Length];
			_ms.Read(bytes, 0, (int)_ms.Length);
			_ms.Position = position;
			
			return Convert.ToBase64String( Util.decByte( bytes) );
		}
		catch(Exception e)
		{
#if UNITY_EDITOR
			Debug.LogError(e);
#endif
			return string.Empty;
		}
	}



	public int pvpSeed;
	public int replaySeed;

	public string continueVersion = "";

	public int continueSeed;
	public string continueMode;
	public string continueRoundId;
	public string continueGameType;
	public GamePlayerData continuePVPPlayerData, continuePVPPlayerData2;

	public int continueRank = 0;

	public const string TEMP_HEADER_FILE_NAME = "718205643PTEMEADERH467E85B1FE3";
	public const string TEMP_FILE_NAME =        "50495A7182EB1F6E05643467E83P12";
	public const string REPLAY_NAME =           "051TEM43467E850495A7182EB1FP13";

	public const string REPLAY_FILE_NAME = "r";
	public const string REPLAY_FILE_HEADER = "r2";

	public bool hasContinueGame()
	{
#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return false;
#endif

		bool has = ((File.Exists(AssetBundleManager.getLocalFilePath() + TEMP_FILE_NAME)) && (File.Exists(AssetBundleManager.getLocalFilePath() + TEMP_HEADER_FILE_NAME)));

		if(has)
		{
			if(loadContinueGameData() == false)
			{
				return false;
			}

			string[] h;
			string file1;
			string file2;

//			if(useTempSaveEnctyption)
			{
				file1 = Util.dec(File.ReadAllBytes(AssetBundleManager.getLocalFilePath()+TEMP_HEADER_FILE_NAME));
				file2 = PlayerPrefs.GetString(TEMP_HEADER_FILE_NAME);

				Byte[] ba = Convert.FromBase64String(file2);
				file2 = Util.dec(ba);
			}
//			else
//			{
//				file1 = (File.ReadAllText(AssetBundleManager.getLocalFilePath()+TEMP_HEADER_FILE_NAME));
//				file2 = PlayerPrefs.GetString(TEMP_HEADER_FILE_NAME);
//			}

			if(file2 == null || file1 != file2) return false;
			
			h = file1.Split(',');

			continueVersion = h[0];
			continueSeed = Convert.ToInt32(h[1]);
			continueMode = h[2];
			continueRoundId = h[3];
			continueGameType = h[4];

			if( string.IsNullOrEmpty(continueRoundId) == false && continueRoundId == "INTRO") return false;

			switch(continueMode)
			{
			case RoundData.MODE.PVP:

				try
				{
					continuePVPPlayerData = GamePlayerData.deserialize(h, true);
					DebugManager.instance.pvpPlayerData = continuePVPPlayerData;

					if(h[24] == "Y")
					{
						continuePVPPlayerData2 = GamePlayerData.deserialize(h, false);
						DebugManager.instance.pvpPlayerData2 = continuePVPPlayerData2;
					}
					else
					{
						DebugManager.instance.pvpPlayerData2 = null;
					}

				}
				catch
				{
					deleteTempFile();
					return false;
				}
				break;
			case RoundData.MODE.C_RUN:
			case RoundData.MODE.C_HUNT:
			case RoundData.MODE.C_SURVIVAL:
				int.TryParse(h[5], out continueRank);
				break;
			default:
				if(continueGameType == RoundData.TYPE.EPIC)
				{
//					int.TryParse(h[5], out UIRoundClearPopup.rewardExp);
//					int.TryParse(h[6], out UIRoundClearPopup.rewardGold);
				}
				else if(continueGameType == RoundData.TYPE.HELL)
				{
					int t = 0;
					int.TryParse(h[5], out t);
					HellModeManager.instance.continueHellWave = t;
				}
				break;
			}
		}

		return has;
	}


	public void deleteTempFile()
	{
		string file = AssetBundleManager.getLocalFilePath() + TEMP_FILE_NAME;

		if(File.Exists(file))
		{
			File.Delete(file);
		}

		file = AssetBundleManager.getLocalFilePath() + TEMP_HEADER_FILE_NAME;

		if(File.Exists(file))
		{
			File.Delete(file);
		}

		PlayerPrefs.DeleteKey(TEMP_FILE_NAME);
		PlayerPrefs.DeleteKey(TEMP_HEADER_FILE_NAME);
	}

	public bool isPVPReplayIsAttackerWinGame = false;
	public bool isPVPReplayIsSurrenderGame = false;
	public bool isPVPReplayIsAttackGame = false;


	public bool needReplayResultCheck = false;


	public string pvpReplayEnemyId = "";
	public string replayGameVersion = "";

	MemoryStream _replayData;
	public bool convertServerReplayData(string serverData)
	{
		Byte[] bytes = null;

		try
		{
			bytes = Convert.FromBase64String(serverData);
		}
		catch(Exception e)
		{
#if UNITY_EDITOR
			Debug.LogError(e.Message);
#endif
			return false;
		}

		try
		{
			_replayData = new MemoryStream();
			Stream stm = new MemoryStream(bytes);
			
			ZipInputStream zis = new ZipInputStream(stm);
			
			ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
			
			while ((ze = zis.GetNextEntry()) != null)
			{
				if (!ze.IsDirectory)
				{
//					Debug.Log(ze.Name);
					
					byte[] buffer = new byte[2048];
					int len;

					if(ze.Name != "t")
					{
						while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
						{
							_replayData.Write(buffer, 0, len);
						}
					}
					else // 얘는 리플레이 모드일때만 사용한다...
					{
						MemoryStream header = new MemoryStream();
						
						while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
						{
							header.Write(buffer, 0, len);
						}
						
						header.Position = 0;
						string headerData = System.Text.Encoding.UTF8.GetString(header.ToArray());
						string[] hd = headerData.Split(',');
						replayGameVersion = hd[0];
						isPVPReplayIsSurrenderGame = hd[1] == "1";
						isPVPReplayIsAttackerWinGame = hd[2] == "1";

						needReplayResultCheck = true;

						int.TryParse(hd[3], out UIPlay.playerLeagueGrade);

						Util.tryIntParseToXInt(hd[4], out UIPlay.pvpleagueGrade, 1);

#if UNITY_EDITOR
						Debug.LogError("replayGameVersion : " + replayGameVersion + "    pvpGrade: "  + hd[3]);
#endif

						header.Close();
						header.Dispose();
					}
				}
			}
			zis.Close(); // 입력받은 녀석은 지우고...
			zis.Dispose();
			zis = null;
			
			stm.Close();
			stm.Dispose();
			stm = null;
			
			_replayData.Position = 0; // 사용할 파일 스트림의 위치는 0으로...
		}
		catch(Exception e)
		{
			return false;
		}

		return true;
	}


	public bool loadZip(string fileName, bool setToReplayStream = false)
	{
		try
		{
			using (FileStream file = new FileStream(AssetBundleManager.getLocalFilePath()+fileName, FileMode.Open, System.IO.FileAccess.Read)) 
			{
				MemoryStream m = new MemoryStream();
				m.SetLength(file.Length);
				file.Read(m.GetBuffer(), 0, (int)file.Length);
				file.Close();
				m.Position = 0;
				
				ZipInputStream zis = new ZipInputStream(m); // zip 파일에 입력하고..
				
				if(setToReplayStream) _replayData = new MemoryStream();
				else _ms = new MemoryStream();
				
				ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
				
				while ((ze = zis.GetNextEntry()) != null)
				{
					if (!ze.IsDirectory)
					{
						byte[] buffer = new byte[2048];
						int len;

						if(ze.Name != "t")
						{
							while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
							{
								if(setToReplayStream) _replayData.Write(buffer, 0, len);
								else _ms.Write(buffer, 0, len);
								
							}
						}
					}
				}
				zis.Close(); // 입력받은 녀석은 지우고...
				zis.Dispose();
				zis = null;
				m.Close();
				m.Dispose();
				m = null;
				
				if(setToReplayStream) _replayData.Position = 0;
				else _ms.Position = 0; // 사용할 파일 스트림의 위치는 0으로...
			}


		}
		catch(Exception e)
		{
			return false;
		}

		return true;
	}


	public void loadReplayGameData()
	{
		_ms = new MemoryStream();
		_ms = _replayData;
	}


	public bool loadContinueGameData()
	{
		return loadZip(TEMP_FILE_NAME);
	}


	public void checkReplayResult(bool replayResultIsMyWinning)
	{
		if( GameManager.me.uiManager.uiPlay.btnReplayClose.gameObject.activeSelf == false && GameManager.me.isStartGame == false)
		{
			return;
		}

		if(isPVPReplayIsSurrenderGame)
		{
			if(replayResultIsMyWinning)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_SURREND"), playReplayResultAsWin);
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_SURREND"), playReplayResultAsLose);
			}

			return;
		}

		if(needReplayResultCheck)
		{
			// 공격게임.
			if(isPVPReplayIsAttackGame)
			{
				// 공격자가 이기면 나도 이겨야함.
				if(isPVPReplayIsAttackerWinGame == true && replayResultIsMyWinning == false)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_ERROR", Util.getUIText("WIN")), playReplayResultAsWin);
				}
				// 공격자가 지면 나도 져야함.
				else if(isPVPReplayIsAttackerWinGame == false && replayResultIsMyWinning == true)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_ERROR", Util.getUIText("LOSE")), playReplayResultAsLose);
				}
			}
			else // 방어게임
			{

				// 공격자가 이기면 나는 저야함.
				if(isPVPReplayIsAttackerWinGame == true && replayResultIsMyWinning == true)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_ERROR", Util.getUIText("WIN")), playReplayResultAsLose);
				}
				// 공격자가 지면 나는 이겨야함.
				else if(isPVPReplayIsAttackerWinGame == false && replayResultIsMyWinning == false)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_ERROR", Util.getUIText("LOSE")), playReplayResultAsWin);
				}
			}
		}

		GameManager.me.playReplayResult(replayResultIsMyWinning);
	}



	void playReplayResultAsWin()
	{
		GameManager.me.playReplayResult(true);
	}

	void playReplayResultAsLose()
	{
		GameManager.me.playReplayResult(false);
	}




	public bool getNextRecord()
	{
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Debug.LogError("_ms.Position : " + _ms.Position + "   _ms.Length: " + _ms.Length);
		}
#endif

		if(_ms.Position < _ms.Length)
		{
			if(GameManager.me.player != null)
			{
				_ms.Read(tempByte,0,1);
				booleanArrayFromByte(tempByte[0]);
				
				if(_results[1])
				{
					GameManager.me.player.moveState = Player.MoveState.Backward;
				}
				else if(_results[2])
				{
					GameManager.me.player.moveState = Player.MoveState.Stop;
				}
				else if(_results[3])
				{
					GameManager.me.player.moveState = Player.MoveState.Forward;
				}

				isAutoPlayContinueGame = _results[4];


				changePlayer = _results[5];
				changePVPPlayer = _results[6];


				if(_results[0])
				{
					_ms.Read(tempByte,0,1);
					booleanArrayFromByte(tempByte[0]);
					
					unitButtons[0] = _results[0];
					unitButtons[1] = _results[1];
					unitButtons[2] = _results[2];
					unitButtons[3] = _results[3];
					unitButtons[4] = _results[4];
					
					skillButtons[0] = _results[5];
					skillButtons[1] = _results[6];
					skillButtons[2] = _results[7];
				}
				else
				{
					unitButtons[0] = false;
					unitButtons[1] = false;
					unitButtons[2] = false;
					unitButtons[3] = false;
					unitButtons[4] = false;
					
					skillButtons[0] = false;
					skillButtons[1] = false;
					skillButtons[2] = false;
				}
			}

			return true;
		}

		if(GameManager.me.playMode == GameManager.PlayMode.replay)
		{
			if(isPVPReplayIsSurrenderGame)
			{
				needReplayResultCheck = false;

				// pvp 게임인데 도중 포기를 했다.
				// 1. 상대가 공격한 게임임. 그럼 상대가 진 것임. 
				// 2. 내가 공격한 게임임. 그럼 내가 진것임. 
				// 중도 포기했다는 것은 공격한 사람이 졌단거다.
				if((isPVPReplayIsAttackGame) )// && isPVPReplayIsAttackerWinGame) ||  (isPVPReplayIsAttackGame == false && isPVPReplayIsAttackerWinGame == false))
				{
					// 졌다고 표시한다.
					GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_FAILED;
				}
				else
				{
					// 이겼다고 표시한다.
					GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_SUCCESS;
				}
			}
			else
			{
				return true;
			}
		}
		else
		{
			GameManager.me.recordMode = GameManager.RecordMode.record;
		}

		return false;
	}


	private bool[] _results = new bool[8];
	public void booleanArrayFromByte(byte x) 
	{
		_results[0] = ((x & 0x01) != 0);
		_results[1] = ((x & 0x02) != 0);
		_results[2] = ((x & 0x04) != 0);
		_results[3] = ((x & 0x08) != 0);
		_results[4] = ((x & 0x10) != 0);
		_results[5] = ((x & 0x20) != 0);
		_results[6] = ((x & 0x40) != 0);
		_results[7] = ((x & 0x80) != 0);
	}

	public void check()
	{

	}

	// 1바이트에 8개의 boolean 배열을 때려박는다.
	byte[] arrayToByteArray = new byte[1];
	byte[] boolArrayToByteArray(bool[] bools)
	{
		int bitIndex = 0;

		arrayToByteArray[0] = 0;

		for (int i = 0; i < 8; ++i)
		{
			if (bools[i]) arrayToByteArray[0] |= (byte)(((byte)1) << bitIndex);
			++bitIndex;
		}
		
		return arrayToByteArray;
	}



	/*
	int BoolArrayToInt(bool[] bits)
	{
		int r = 0;
		for(int i = 0; i < 8; ++i) if(bits[i]) r |= 1 << (8 - i);
		return r;
	}
	*/
	
	/*
	byte[] boolArrayToByteArray(bool[] bools)
	{
		int len = bools.Length;
		int bytes = (len + 7) / 8;
		
		byte[] arr2 = new byte[bytes];
		
		int bitIndex = 0, byteIndex = 0;
		
		for (int i = 0; i < len; ++i)
		{
			if (bools[i])
			{
				arr2[byteIndex] |= (byte)(((byte)1) << bitIndex);
			}

			++bitIndex;

			if (bitIndex == 8)
			{
				bitIndex = 0;
				++byteIndex;
			}
		}
		
		return arr2;
	}
*/



	// 임시 파일 저장. 헤더는 따로 저장한다.
	void saveTempZip(byte[] data)
	{
		Crc32 crc = new Crc32();
		
		string zipPath = AssetBundleManager.getLocalFilePath() + TEMP_FILE_NAME;

		try
		{
			FileStream writer = File.Create(zipPath);//new FileStream(zipPath, FileMode.Create, FileAccess.Write);
			ZipOutputStream zos = new ZipOutputStream(writer);
			zos.SetLevel(9);
			
			byte[] buffer = data;
			
			ZipEntry entry = new ZipEntry(TEMP_FILE_NAME);
			entry.DateTime = DateTime.Now;

			entry.Size = _ms.Length;
			
			crc.Reset();
			crc.Update(buffer);
			
			entry.Crc = crc.Value;
			
			zos.PutNextEntry(entry);
			
			zos.Write(buffer, 0, buffer.Length);

			zos.Finish();
			zos.Close();
		}
		catch(Exception e)
		{

		}
	}





}

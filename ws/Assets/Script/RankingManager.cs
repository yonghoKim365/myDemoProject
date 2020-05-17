using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using epi;


public class RankingManager : MonoBehaviour {
	
	static RankingManager instance;
	public static RankingManager GetInstance(){
		return instance;
	}
	
	public ListGrid listGrid;
	public UIScrollView panel;
	public RankingPanel rankingPanel_pfb;
	
	
#if FUCK	
	
	public List<GameUser> rankers;

	void Awake(){
		instance = this;
		//FaceDownLoadTexture.init();
	}
	public void SetDrawRanking(){
		SetDrawRanking(true);
	}
	public SimpleFriendData[] GetRankerFive(){
		List<GameUser> rankersInGame = GAME_DATA.getFriendDistanceRankingWithoutMe() ;
		
		SimpleFriendData[] returnValue = new SimpleFriendData[5];
		for(int i=0;i<returnValue.Length;i++){
			returnValue[i] = new SimpleFriendData();
			returnValue[i].distance = -1;
		}
		
		int totalAvailableNum = 0;
		for(int i=rankersInGame.Count-1;i>=0;i--){
			if(rankersInGame[i].weekDistance>0){
				totalAvailableNum = i+1;
				break;
			}
		}
		
		if(totalAvailableNum>0){
			if(totalAvailableNum<5){
				for(int i=0;i<totalAvailableNum;i++){
					returnValue[i].userId = rankersInGame[rankersInGame.Count-i-1].userID;
					//returnValue[i].distance = rankersInGame[i].weekDistance*10;
					returnValue[i].distance = 20000*(i+1);
					returnValue[i].faceurl = rankersInGame[i].image_url;
				}
			}else{
				float dIndex = (float)totalAvailableNum/5f;
				int i=4;
				for(float tmpStartIndex=totalAvailableNum-1 ; tmpStartIndex>=0 ; tmpStartIndex-=dIndex){
					returnValue[i].userId = rankersInGame[(int)tmpStartIndex].userID;
					returnValue[i].faceurl = rankersInGame[(int)tmpStartIndex].image_url;
					returnValue[i].distance = rankersInGame[(int)tmpStartIndex].weekDistance*10;
					i--;
				}
			}
		}
		
		return returnValue;
	}
	public void LoadRankers(){
		rankers = GAME_DATA.getFriendRanking();
	}
	public int GetMyRanking(){
		string myId = LineManager.instance.debugUserId;
		for(int i=0;i<rankers.Count;i++){
			if(rankers[i].userID==myId){
				return i+1;
			}
		}
		return 0;
	}
	public void SetDrawRanking(bool isResetPos_p){
		
		return;
		
		LoadRankers();
		
		for(int i=0;i<listGrid.panels.Count;i++){
			if(i<rankers.Count){
				listGrid.panels[i].gameObject.SetActiveRecursively(true);
			}else{
				listGrid.panels[i].gameObject.SetActiveRecursively(false);
			}
		}
		
		List<object> rankers_obj = new List<object>();
		for(int i=0;i<rankers.Count;i++){
			
			rankers[i].weekOptionsList = new List<string>{"","","","","","",""};
			if(rankers[i].weekOptions!=null){
				string[] tmpstr1 = rankers[i].weekOptions.Split(',');
				for(int j=0;j<tmpstr1.Length;j++){
					string[] tmpstr2 = tmpstr1[j].Split (':');
					if(tmpstr2!=null){
						switch(tmpstr2[0]){
						case "C":
							rankers[i].weekOptionsList[0] = tmpstr2[1];
							break;
						case "R":
							rankers[i].weekOptionsList[1] = tmpstr2[1];
							break;
						case "P":
							rankers[i].weekOptionsList[2] = tmpstr2[1];
							break;
						case "CL":
							rankers[i].weekOptionsList[3] = tmpstr2[1];
							break;
						case "RL":
							rankers[i].weekOptionsList[4] = tmpstr2[1];
							break;
						case "RE":
							rankers[i].weekOptionsList[5] = tmpstr2[1];
							break;
						case "P2":
							rankers[i].weekOptionsList[6] = tmpstr2[1];
							break;
						}
					}
				}
			}
			rankers[i].bestOptionsList = new List<string>{"","","","","","",""};
			if(rankers[i].bestOptions!=null){
				string[] tmpstr1 = rankers[i].bestOptions.Split(',');
				for(int j=0;j<tmpstr1.Length;j++){
					string[] tmpstr2 = tmpstr1[j].Split (':');
					if(tmpstr2!=null){
						switch(tmpstr2[0]){
						case "C":
							rankers[i].bestOptionsList[0] = tmpstr2[1];
							break;
						case "R":
							rankers[i].bestOptionsList[1] = tmpstr2[1];
							break;
						case "P":
							rankers[i].bestOptionsList[2] = tmpstr2[1];
							break;
						case "CL":
							rankers[i].bestOptionsList[3] = tmpstr2[1];
							break;
						case "RL":
							rankers[i].bestOptionsList[4] = tmpstr2[1];
							break;
						case "RE":
							rankers[i].bestOptionsList[5] = tmpstr2[1];
							break;
						case "P2":
							rankers[i].bestOptionsList[6] = tmpstr2[1];
							break;
						}
					}
				}
			}
		
			rankers_obj.Add ((object)rankers[i]);
		}
		
		listGrid.setData(rankers_obj,isResetPos_p);
		
		if(isResetPos_p==true){
			panel.ResetPosition();
		}
		//listGrid.isLock=false;
	}
	
	public RankerData GetRankerData(string userId){
		RankerData returnValue = new RankerData();;
		for(int i=0;i<rankers.Count;i++){
			if(userId == rankers[i].userID){
				returnValue.userId = rankers[i].userID;
				returnValue.rank = i+1;
				returnValue.score = rankers[i].weekScore;
				returnValue.faceurl = rankers[i].image_url;
				returnValue.name = rankers[i].nickname;
			}
		}
		return returnValue;
	}
	/*
	public int GerRankUpStep(){
		
		LoadRankers();
		
		int lastRank = PlayerInfo.GetInstance().last_rank;
		int nowRank = GetMyRanking();
		if(lastRank>nowRank){
			return lastRank-nowRank;
		}else{
			return 0;
		}
	}
	
	public RankerData GetMyRankerData(){
		return GetRankerData(PlayerInfo.GetInstance().GetUserID());
	}
	public RankerData GetMyLoserRankerData(){
		string myId = PlayerInfo.GetInstance().GetUserID();
		int lastScore = GameData.GetTotalScore();
		RankerData returnValue = null;
		for(int i=rankers.Count-1;i>=0;i--){
			if(rankers[i].userID==myId){
				returnValue = GetRankerData(rankers[i+1].userID);
			}
		}
		
		return returnValue;
	}
	*/
	
#endif	
	
}


public class SimpleFriendData{
	public string userId;
	public float distance;
	public string faceurl;
	
	public SimpleFriendData(){
	}
}

public class RankerData{
	public string userId,name,faceurl;
	public int rank,score;
	public RankerData(){
	}
	
	
	
}
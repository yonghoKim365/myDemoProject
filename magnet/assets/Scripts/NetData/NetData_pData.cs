using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Sw;

using Kpro;

public partial class NetData
{
    #region :: 웹서버기반 데이터 클래스 모음 ::

    public class AccountInfo
    {
        public ulong user_idx;
        public string sns_id;
        public short sns_type;
        /// <summary> 마지막로그인썹 </summary>
        public int last_login_server;
        /// <summary> 마지막 로그인 </summary>
        public string last_login_date;
    }

    /// <summary> 기존의 것의 동기화를 맞추기 위해 삭제 안함.</summary>
    public class CharacterInfo
    {
        /// <summary> 캐릭터 인덱스 </summary>
        public ulong c_usn;
        /// <summary> 유닛ID </summary>
        public uint character_id;
        /// <summary> 닉네임 </summary>
        public string nickname;
        /// <summary> 슬롯번호 </summary>
        public short slot_no;
        /// <summary> 레벨 </summary>
        public ushort level;

        public uint CostumeId;
        public bool IsHideCostume;
        public uint Cloth;
        public uint Head;
        public uint Weapon;
        public uint SkillSetId;
        public CharacterInfo()
        {

        }

        public CharacterInfo(ulong c_usn, uint character_id, string nickname, short slot_no, ushort level, uint cloth, uint head, uint weapon, uint costume, uint skillSetId, bool isHideCos)
        {
            this.c_usn = c_usn;
            this.character_id = character_id;
            this.nickname = nickname;
            this.slot_no = slot_no;
			if (this.slot_no == 0){
				this.slot_no = 1;
				// 1,2,3 중 하나의 값을 갖는다. 0이면 안됨.
			}
            this.level = level;

            CostumeId = costume;
            IsHideCostume = isHideCos;
            Cloth = cloth;
            Head = head;
            Weapon = weapon;
            SkillSetId = skillSetId;
            if (SkillSetId <= 0)
            {
                switch (character_id)
                {
                    case 11000:
                        SkillSetId = 100;
                        break;
                    case 12000:
                        SkillSetId = 200;
                        break;
                    case 13000:
                        SkillSetId = 300;
                        break;
                }
            }
        }

        public void Set(CharacterInfo info)
        {
            c_usn = info.c_usn;
            character_id = info.character_id;
            nickname = info.nickname;
            slot_no = info.slot_no;
            level = info.level;

            CostumeId = info.CostumeId;
            IsHideCostume = info.IsHideCostume;
            Cloth = info.Cloth;
            Head = info.Head;
            Weapon = info.Weapon;
            SkillSetId = info.SkillSetId;
        }
    }

    public class CostumeInfo
    {
        /// <summary> 코스튬 인덱스 </summary>
        public ulong costume_idx;
        /// <summary> 캐릭터 인덱스 </summary>
        public ulong c_usn;
        /// <summary> 코스튬 아이디 </summary>
        public uint costume_id;
        /// <summary> 강화도 </summary>
        public ushort star_enchant;
        /// <summary> 승급 횟수 </summary>
        public ushort evolve_count;
        /// <summary> 장착 여부 [ 0 : 미장착, 1 : 장착 ] </summary>
        public short is_equip;
        /// <summary> 평타 스킬 레벨 </summary>
        public ushort skill0_level;
        /// <summary> 0번째 스킬레벨 </summary>
        public ushort skill1_level;
        /// <summary> 1번째 스킬레벨 </summary>
        public ushort skill2_level;
        /// <summary> 2번째 스킬레벨 </summary>
        public ushort skill3_level;
        /// <summary> 3번째 스킬레벨 </summary>
        public ushort skill4_level;
        public string option;//코스튬 옵션
        /// <summary> 등록일 </summary>
        //public string cc_rdt_dt;

        public void Set(CostumeInfo info)
        {
            costume_idx = info.costume_idx;
            c_usn = info.c_usn;
            costume_id = info.costume_id;
            star_enchant = info.star_enchant;
            is_equip = info.is_equip;
            skill0_level = info.skill0_level;
            skill1_level = info.skill1_level;
            skill2_level = info.skill2_level;
            skill3_level = info.skill3_level;
            skill4_level = info.skill4_level;
            //cc_rdt_dt = info.cc_rdt_dt;
        }
    }

    public class ItemInfo
    {
        /// <summary> 아이템 인덱스 </summary>
        public ulong item_idx;
        /// <summary> 아이템 아이디 </summary>
        public uint item_id;
        /// <summary> 아이템 타입 [ 1 : 장비, 2 : 소모성 ] </summary>
        public short item_type;
        /// <summary> 파트타입 </summary>
        public ushort parts_type;
        /// <summary> 강화 </summary>
        public ushort enchant;
        public ushort star_grade;//별 개수
        public ushort grade;//등급
        /// <summary> 아이템 보유량 </summary>
        public ushort amount;
        /// <summary> 장착여부 [ 0 : 미장착, 1 : 장착 ] </summary>
        public bool isMount;
        //public short i_enchant_ti;
        /// <summary> 옵션0 </summary>
        public string option_1;
        /// <summary> 옵션1 </summary>
        public string option_2;
        /// <summary> 옵션2 </summary>
        public string  option_3;
        /// <summary> 캐릭터 인덱스 </summary>
        //public ulong i_c_idx_bi;
        /*
        public void Set(ItemInfo info)
        {
            i_idx_bi = info.i_idx_bi;
            i_c_idx_bi = info.i_c_idx_bi;
            i_itemid_i = info.i_itemid_i;
            i_type_ti = info.i_type_ti;
            i_amount_i = info.i_amount_i;
            i_enchant_ti = info.i_enchant_ti;
            i_opt0_i = info.i_opt0_i;
            i_opt1_i = info.i_opt1_i;
            i_opt2_i = info.i_opt2_i;
        }
        */
    }

    /// (새로) 메일 기본 메시지
    public class EmailBaseInfo
    {
        public uint MaidId; //우편id
       // public long UserId;  //수신자 id
        public long SenderId;    //발신자 id
        public int MailType;    //우편차입
        public int IsReceive;   //0 미열람, 1 열람
        public ulong ReadTime;   // 열람한 시간
        public ulong SendTime;   //발송한 시간
        public ulong OverTime;   //유효기간 시간
         
        public EmailBaseInfo(uint idx/*, long uid*/, long sendid, int type,  int isReceive, ulong readTime, ulong sendTime, ulong overTime)
        {
            MaidId = idx;
          //  UserId = uid;
            SenderId = sendid;
            MailType = type;
            IsReceive = isReceive;
            ReadTime = readTime;
            SendTime = sendTime;
            OverTime = overTime;
        }
    }
    /// 메일 상세 정보
    public class EamilDetails
    {
        public uint MailId; //우편id
        public ulong ReadTime;  //열람한시간
        //public string Content;  //내용
        //public uint Cash;   //원보
        //public uint Gold;   //콜드
        //public uint Fame;   //성망
        //public uint Honor;  //명예
        //public uint Contribution;   //공헌
        //public uint RoyalBadge; //황성휘장
        //public uint LionBadge;  //사자왕 취장
        public int Count;   //첨부수량

        public List<EmailAttachmentInfo> emAttach;
       // public int Error;   //에러코드

        public EamilDetails(uint id, ulong readTime,/* string content, uint cash, uint gold, uint fame, uint honor, uint contri, uint royal, uint lion,*/ int count, List<EmailAttachmentInfo> emAtt/*, int error*/)
        {
            MailId = id;
            ReadTime = readTime;
            //Content = content;
            //Cash = cash;
            //Gold = gold;
            //Fame = fame;
            //Honor = honor;
            //Contribution = contri;
            //RoyalBadge = royal;
            //LionBadge = lion;
            Count = count;
            emAttach = emAtt;
          //  Error = error;
        }


    }
    // 단기 우편의 우편 첨부 파일정보
    public class EmailAttachmentInfo
    {
        public uint GoodType;  //물품타입
        public uint Id; 
        public uint Count; //물품수량

        public EmailAttachmentInfo(uint type, uint id, uint count)
        {
            GoodType = type;
            Id = id;
            Count = count;

            //if (Id <= 0)
            //{
            //    switch (GoodType)
            //    {
            //        case 1:
            //            GoodType = (uint)AssetType.Gold;
            //            break;//골드  
            //        case 2:
            //            GoodType = (uint)AssetType.Cash;
            //            break;//원보  
            //        case 3:
            //            GoodType = (uint)AssetType.Contribute;
            //            break;//공헌  
            //        case 4:
            //            GoodType = (uint)AssetType.Honor;
            //            break;//명예
            //        case 5:
            //            GoodType = (uint)AssetType.Badge;
            //            break;//황성휘장    
            //        case 6:
            //            GoodType = (uint)AssetType.LionBadge;
            //            break;//사자왕휘장  
            //        case 7:
            //            GoodType = (uint)AssetType.FAME;
            //            break;//성망        
            //        case 8:
            //            GoodType = (uint)AssetType.Energy;
            //            break;//체력
            //        case 9:
            //            GoodType = (uint)AssetType.Exp;
            //            break;//경험치      
            //        case 11:
            //            GoodType = (uint)AssetType.PartnerShard;
            //            break;//조각        
                        
            //        case 99: //공용
            //        case 12://장비아이템  
            //        case 10://유닛
            //        case 13://재료아이템  
            //        case 14:
            //            GoodType = 0;
            //            break;//칭호
            //    }
            //}
        }
    }

    // 일괄 수령 결과ㅣ
    public class EmailOneKeyFeatchInfo
    {
        uint unid;
        public EmailOneKeyFeatchInfo(uint id)
        {
            unid = id;
        }
    }

    // 일괄 삭제 결과ㅣ
    public class EmailDelInfo
    {
        uint unid;
        public EmailDelInfo(uint id)
        {
            unid = id;
        }
    }
    
    /// <summary> 서버에서 보내주는 재료 정보 </summary>
    public class MaterialData
    {
        public uint ItemId;
        public uint Amount;
        public byte AddValue;

        public MaterialData(uint id, uint amount)
        {
            ItemId = id;
            Amount = amount;
            AddValue = 0;
        }

        /// <summary> 강화용 </summary>
        public MaterialData(uint id, uint amount, byte addValue)
        {
            ItemId = id;
            Amount = amount;
            AddValue = addValue;
        }
    }

    /// <summary> 서버에서 보내주는 강화, 승급 아이템 재료 정보 </summary>
    public class UpgradeMaterialData
    {
        public UpgradeMaterialData(Enchant.EnchantInfo enchan)
        {
            if (0 < enchan.ItemIdx1)
                MaterialList.Add(new MaterialData(enchan.ItemIdx1, enchan.ItemValue1, 0));
            if (0 < enchan.ItemIdx2)
                MaterialList.Add(new MaterialData(enchan.ItemIdx2, enchan.ItemValue2, 0));
            if (0 < enchan.ItemIdx3)
                MaterialList.Add(new MaterialData(enchan.ItemIdx3, enchan.ItemValue3, 0));
            if (0 < enchan.ItemIdx4)
                MaterialList.Add(new MaterialData(enchan.ItemIdx4, enchan.ItemValue4, 0));
            if (0 < enchan.ItemIdx5)
                MaterialList.Add(new MaterialData(enchan.ItemIdx5, enchan.ItemValue5, 0));

            Price = enchan.CostGold;
            AddPrice = 0;
            //UpgradeValue = enchan.EnchantVlaue1;
            //UpgradePerValue = enchan.EnchantVlaue2;
        }

        public UpgradeMaterialData(Enchant.EvolveInfo evolve)
        {
            if(0 < evolve.ItemIdx1)
                MaterialList.Add(new MaterialData(evolve.ItemIdx1, evolve.ItemValue1, evolve.ItemAdd1) );
            if (0 < evolve.ItemIdx2)
                MaterialList.Add(new MaterialData(evolve.ItemIdx2, evolve.ItemValue2, evolve.ItemAdd2) );
            if (0 < evolve.ItemIdx3)
                MaterialList.Add(new MaterialData(evolve.ItemIdx3, evolve.ItemValue3, evolve.ItemAdd3) );

            Price = evolve.CostGold;
            AddPrice = (ulong)evolve.GoldAdd;
            //UpgradeValue = evolve.EvolveVlaue1;
            //UpgradePerValue = evolve.EvolveVlaue2;
        }

        public List<MaterialData> MaterialList = new List<MaterialData>();

        public ulong Price;//가격
        public ulong AddPrice;//추가량
        //public uint UpgradeValue;//정수
        //public float UpgradePerValue;//%
    }
    
    /// <summary> 클리어한 스테이지의 정보. </summary>
    public class ClearSingleStageData
    {
        public byte Clear_0;//1번째 클리어 했는지 0: 클리어하지 못함, 1: 클리어함
        public byte Clear_1;//2번째 클리어 했는지 0: 클리어하지 못함, 1: 클리어함
        public byte Clear_2;//3번째 클리어 했는지 0: 클리어하지 못함, 1: 클리어함
        public int DailyClearCount;//일일(금일) 몇번 클리어 했는지
        public int DailyResetCount;//일일(금일) 몇번 초기화 했는지
        public uint StageId;

        public ClearSingleStageData(uint stageId, int dailyClear, int dailyReset, byte clear_0, byte clear_1, byte clear_2)
        {
            Clear_0 = clear_0;
            Clear_1 = clear_1;
            Clear_2 = clear_2;

            DailyClearCount = dailyClear;
            DailyResetCount = dailyReset;
            StageId = stageId;
        }
    }

    /// <summary> 친구요청. </summary>
    public struct FriendRequest
    {
        public ulong sender_c_usn;           //요청자 c_usn
        public ulong receiver_c_usn;         //요청 받은 사람의 c_usn
        public string nickname;             //요청 받은 사람의닉네임

    }
      
    /// <summary> 친구수락. </summary>
    public struct FriendAccept
    {
        public ulong friend_c_usn;  //친구의 usn
        public string nickname;
    }

    /// <summary> 친구삭제. </summary>
    public struct FriendDelete
    {
        public ulong friend_c_usn;    //삭제한 친구 인덱스
        public uint remain_count;  //남은삭제횟수

    }

    /// <summary> 친구 검색 </summary>
    public struct FriendSearch
    {
        public ulong c_usn;
        public uint character_id;
        public string nickname;
        public uint level;
    }


    /// <summary> 하트보내기. </summary>
    public struct Heart
    {
        public ulong friend_c_usn;           //친구 캐릭터 c_usn
        public uint send_heart_cooltime;  //남은 시간(초)

    }

    /// <summary> 친구목록 정보. </summary>
    public struct FriendListData
    {
        public ulong friend_c_usn;               //친구 캐릭터 c_usn
        public uint character_id;               // 캐릭터 아이디
        public string nickname;                 //닉네임
        public uint level;
        public uint last_login_elapsed_time;  //마지막 로그인 시간으로 부터 경과한 시간(초),  
        public uint heart_send_cooltime;      //하트 전송 쿨타임(초)

        public void set(uint time)
        {
            heart_send_cooltime = time;
        }
    }

    /// <summary> 친구신청 받은 목록 정보. </summary>
    public struct ReceiveFriendData
    {
        public ulong sender_c_usn;           //신청자 c_usn
        public uint character_id;           // 캐릭터 아이디
        public string nickname;             //신청자 닉네임
        public uint level;
        public uint elapsed_time;         //경과 시간(초) 

    }



    /// <summary> 친구신청 보낸 목록 정보. </summary>
    public struct SendFriendData
    {
        public ulong receiver_c_usn;         //대상의 c_usn
        public uint character_id;           // 캐릭터 아이디
        public string nickname;             //대상의 닉네임
        public uint level;
        public uint elapsed_time;         //경과 시간(초) 

    }

    /// <summary> 추천친구 정보. </summary>
    public struct RecommandFriendData
    {
        public ulong c_usn;
        public uint character_id;           // 캐릭터 아이디
        public string nickname;
        public uint level;
        public uint costume_id;

        public int guildId;
        public int vipLevel;

        public bool isHideCostume;

        //public List<EquipItem> equip_item;
        public uint EquipHeadItemIdx;
        public uint EquipClothItemIdx;
        public uint EquipWeaponItemIdx;
        public uint SkillSetID;

        public uint Prefix;//접두
        public uint Sufffix;//접미
    }



    /// <summary> (새로) 친구 기본 정보. </summary>
    public class FriendBaseInfo
    {
        public bool IsLogin;                //로그인 여부
        public ulong ullRoleId;           // 친구 캐릭터 id
        public string szName;             // 친구 이름 
        public uint nLevel;              // 친구 레벨 
        public uint nLookId;            // 친구 초상화 id
        public uint BattlePower;        //전투력
        public ulong ullLoginTime;        // 마지막 로그인 시간 
        public ulong ullSendPowerTime;  //체력보낸시간

        public FriendBaseInfo(ulong id, string name, uint lv, uint lookId, ulong loginTime, ulong PowerTime, uint att, bool isLogin)
        {
            ullRoleId = id;
            szName = name;
            nLevel = lv;
            nLookId = lookId;
            ullLoginTime = loginTime;
            ullSendPowerTime = PowerTime;

            BattlePower = att;
            IsLogin = isLogin;
        }
    }
    /// <summary> (새로) 친구신청 기본정보. </summary>
    public class FriendRequestBaseInfo
    {
        public ulong ullRoleId;           // 친구 캐릭터 id
        public string szName;             // 친구 이름 
        public uint nLevel;              // 친구 레벨 
        public uint unRoleType;            // 캐릭터타입
        public ulong ullRequestTime;        // 캐릭터 신청 시간

        public FriendRequestBaseInfo(ulong id, string name, uint lv, uint RoleType, ulong reqTime)
        {
            ullRoleId = id;
            szName = name;
            nLevel = lv;
            unRoleType =  RoleType;
            ullRequestTime = reqTime;
        }
    }


    /// <summary> 장착 장비품 정보. </summary>
    public struct EquipItem
    {
        public uint item_id;         //장착아이템 아이디
        public ushort parts_type;    //장착 아이템 파츠타입
    }



    /// <summary>  캐릭터 정보 조회./// </summary>
    public struct CaracterInfoData
    {
        public List<equipItem> equip_item;
    }
    // 일단 착용아이템만 
    public struct equipItem
    {
        public ulong item_idx;          // 아이템 인덱스
        public uint item_id;           // 아이템 아이디
        public short item_type;          //아이템 타입
        public ushort part_type;        //파츠타입
        public ushort enchant;            //강화횟수
        public ushort star_enchant;       //별 강화횟수
        public ushort evolve_count;       //승급횟수
        public string option_1;           //옵션 1
        public string option_2;           //옵션 2
        public string option_3;           //옵션 3
        public string reg_date;         // 생성일


    }

    /// <summary>  상점 물품 정보 /// </summary>
    public class ShopItemInfoData
    {
        public uint Type;  //상점타입
        public ulong RefreshTimer; //최근 상점 자동 리셋시간
        public ulong ManualRefreshTimer;   // 최근 상점 수동 리셋시간
        public uint ManualRefreshCount;    //수동 리셋 횟수
        public uint itemCnt;   //상점 판매 물품 수량
        public List<NetData.ShopItemInfo>  shopInfo;  //상점의 모든 판매 물품

       public ShopItemInfoData(uint type, ulong refTimer, ulong manualTimer, uint refCnt, uint itemcnt, List<NetData.ShopItemInfo> info)
        {
            Type = type;
            RefreshTimer = refTimer;
            manualTimer = ManualRefreshTimer;
            ManualRefreshCount = refCnt;
            shopInfo = info;
        }
    }
    public class ShopItemInfo
    {
        public ulong Idx;    //0~8플래그 
        public uint GoodsId;    //판매물품id
        public uint Account;    //판매수량
        public uint DbIndex;    //데이터 테이블 증가 id

        public ShopItemInfo(ulong index, uint id, uint acc, uint dbidx)
        {
            Idx = index;
            GoodsId = id;
            Account = acc;
            DbIndex = dbidx;
        }
    }

    /// <summary> 랭킹데이터 </summary>
    public class RankInfo
    {
        public int Rank;
        public int RoleType;
        public int Level;
        public int VipLv;
        public long Data;
        public ulong Id;
        public string Name;
        public string GuildName;//길드 이름 받아오는 프로토콜에서 정리

        public RankInfo(int rank, ulong id, int type, string name, int lv, long data, int vip)
        {
            Rank = rank;
            Id = id;
            RoleType = type;
            Name = name;
            Level = lv;
            Data = data;
            VipLv = vip;
            GuildName = null;
        }
    }

    /// <summary>
    /// 난투장 방 정보
    /// </summary>
    public class MessInfo
    {
        public long Id;
        public int RoleNum; //인원
        public int RoleMaxNum;  //최대인원

        public MessInfo(long id, int num, int max)
        {
            Id = id;
            RoleNum = num;
            RoleMaxNum = max;
        }
    }

    ///<summary>난투장드랍아이템 정보 </summary>
    public class DropItem
    {
        public int Type;    //아이테타입
        public int Id;
        public int Amount;  //수량

        public DropItem(int type,int id,int amount)
        {
            Type = type;
            Id = id;
            Amount = amount;
        }
    }

    /// <summary>
    /// 길드 기본정보
    /// </summary>
    public struct GuildSimpleInfo
    {
        public uint Icon;
        public uint Id;
        public string Name;
        public string LeaderName;
       // public string Declaration;  //길드선언
        public uint Count;
        public uint guildLv;    //길드렙
        public uint JoinSet;    //2 자유가입/1 심사가입
        public ulong CreateTime;    //길드생성시간
        public uint Attack; //전투력
        public uint JoinLevel;  //가입레벨

        public GuildSimpleInfo(uint icon, uint id, string name, string leader,uint cnt, uint lv, uint join,ulong time ,uint attack, uint joinLv)
        {
            Icon = icon;
            Id = id;
            Name = name;
            LeaderName = leader;
            //Declaration = dec;
            Count = cnt;
            guildLv = lv;
            JoinSet = join;
            CreateTime = time;
            Attack = attack;
            JoinLevel = joinLv;
        }

    }

    /// <summary>
    /// 길드 상세정보
    /// </summary>
    public class GuildDetailedInfo
    {
        public uint Lv;
        public uint BlessLv;    //축원렙
        public uint ShopLv;
        public string Declaration;//선언
        public string Announce; //공고
        public ulong Bank;  //자금

        public GuildDetailedInfo(uint lv, uint bless, uint shop, string dec, string ann, ulong bank)
        {
            Lv = lv;
            BlessLv = bless;
            ShopLv = shop;
            Declaration = dec;
            Announce = ann;
            Bank = bank;
        }
    }

    /// <summary>
    /// 길드 구성원 정보
    /// </summary>
    public class GuildMemberInfo
    {
        public ulong Id;
        public string Name;
        public uint Type;
        public uint Lv;
        public uint position;//직위
        public ulong Contribution;    //공헌도
        public ulong LoginTime;//최근 로그인 일시
        public ulong LogountTime;   //최근 로그아웃 일시
        public uint Power;  //전투력
        public uint Online; //접속상태


        public GuildMemberInfo(ulong id, string name, uint type, uint lv, uint pos, ulong con, ulong login, ulong logout, uint power, uint online)
        {
            Id = id;
            Name = name;
            Type = type;
            Lv = lv;
            position = pos;
            Contribution = con;
            LoginTime = login;
            LogountTime = logout;
            Power = power;
            Online = online;
        }
    }

    /// <summary>
    /// 길드가입 신청자 정보
    /// </summary>
    public class ApplyRoleInfo
    {
        public ulong Id;
        public string Name;
        public uint Type;
        public uint Lv;
        public uint Power;  //전투력
        
        public ApplyRoleInfo(ulong id, string name, uint type, uint lv, uint power)
        {
            Id = id;
            Name = name;
            Type = type;
            Lv = lv;
            Power = power;
        }
    }

    /// <summary> 방입장한 유저 정보 </summary>
    public class RoomUserInfo
    {
        public ulong Id;
        public string Name;
        public uint Type;
        public uint Lv;
        public uint Power;  //전투력
        public int Slot;//방장의 경우 0으로하자

        public RoomUserInfo(ulong id, string name, uint type, uint lv, uint power, int slot)
        {
            Id = id;
            Name = name;
            Type = type;
            Lv = lv;
            Power = power;
            Slot = slot;
        }
    }


    /// <summary>
    /// 길드에서 자신의 정보 
    /// </summary>
    public class GuildSelfInfo
    {
        public uint Profession; //
        public uint Position;   //직위
        public ulong JoinTime;  //가입시간
        public uint ContriTotal;    //누적공헌도
        public uint ContriSpendTotal;   //누적공헌도 소모
        public uint DonateCoinTotal;    //유저의 누적 골드기부
        public uint DonateGemTotal;     //유저의 누적 골드원부

        
        public GuildSelfInfo(uint pro, uint pos, ulong time, uint contriTotal, uint contriSpendTotal, uint DonateCoin, uint DonateGem)
        {
            Profession = pro;
            Position = pos;
            JoinTime = time;
            ContriTotal = contriTotal;
            ContriSpendTotal = contriSpendTotal;
            DonateCoinTotal = DonateCoin;
            DonateGemTotal = DonateGem;
        }
    }
    /// <summary>
    /// 길드실시간정보 
    /// </summary>
    public class GuildStatusInfo
    {
        public uint Id;     
        public uint Type;   //1.길드가입 2.길드탈퇴 3.직위임명 4.길마되기 5.기부 6.축원 7.다른길드영지공격점령 8. 길드영지뺏김
        public string Name;  //캐릭닉네임 type )) 1->새길원 2->길탈자 3->피임명자 4->새로운길마 5->공헌자 6->축원자 
        public string Name2;    //누적공헌도 type )) 3-> v임명자
        public uint Param1;     // 3->직위 4->공헌타입 5-> 보상타입 
        public string Param2;    // 5-> 아이템Id
        public ulong Time;     //발생시간


        public GuildStatusInfo(uint id, uint type, string name, string name2, uint param1, string param2, ulong time)
        {
            Id = id;
            Type = type;
            Name = name;
            Name2 = name2;
            Param1 = param1;
            Param2 = param2;
            Time = time;
        }
    }

    // 업적단계정보
    public class AchieveLevelInfo
    {
        public uint Type;   //업적대분류(타입)
        public uint SubType;    //업적소분류
        public uint level;  //현재업적단계
        public uint Complete;
        public uint Fetch;  //수령여부 (1수령0미수령)

        public AchieveLevelInfo(uint type, uint sub, uint lv, uint complete, uint fetch)
        {
            Type = type;
            SubType = sub;
            level = lv;
            Complete = complete;
            Fetch = fetch;
        }

    }

    // 길드 단일퀘스트
    public class GuildTaskInfo
    {
        public uint Id;//길드 퀘스트 id
        public uint Type;
        public uint ValueTotal; //길드 퀘스트 클리어 누적치
        public uint Complete;   //클리어여부(0 미클리어, 1 클리어)

        public GuildTaskInfo(uint id, uint type, uint total, uint complete)
        {
            Id = id;
            Type = type;
            ValueTotal = total;
            Complete = complete;
        }
    }
    // 길드 개인퀘스트
    public class GuildUserTaskInfo
    {
        public uint Id;//길드 퀘스트 id
        public uint Type;
        public uint ValueTotal; //길드 퀘스트 클리어 누적치
        public uint Complete;   //클리어여부(0 미클리어, 1 클리어)
        public uint FetchRewrd; //보상획득여부(0 미획득. 1 획득)

        public GuildUserTaskInfo(uint id, uint type, uint total, uint complete, uint reward)
        {
            Id = id;
            Type = type;
            ValueTotal = total;
            Complete = complete;
            FetchRewrd = reward;
        }
    }
    
    public struct DailyCompleteData
    {
        public int CompleteCount;
        public int MaxCompleteCount;//0일 수 있음 0일 경우 Etc테이블에서 확인.

        public DailyCompleteData(int now, int max)
        {
            CompleteCount = now;
            MaxCompleteCount = max;
        }
        
    }

    /// <summary> 1:1차관 대상 유저 정보 </summary>
    public struct ArenaUserInfo
    {
        public long RoleId;                    // 캐릭터 ID 角色id
        public string Name;                      // 캐릭터 이름 角色名字
        public int Type;                       // 캐릭터 타입角色类型
        public int Level;                      // 레벨 等级
        public int VipLevel;                  // VIP 등급 VIP等级
        public int Attack;                 // 전투력 战斗力
        public int Rank;						// 순위 名次
        public ulong GuildId;               //길드 아이디

        public string GuildName;//Guild Id를 통해서 받아온 길드이름

        public ArenaUserInfo(long roleId, string name, int type, int lv, int vipLv, int att, int rank, ulong guildId)
        {
            RoleId = roleId;
            Name = name;
            Type = type;
            Level = lv;
            VipLevel = vipLv;
            Attack = att;
            Rank = rank;
            GuildId = guildId;

            GuildName = null;
        }

        public void SetGuildName(string guildName)
        {
            GuildName = guildName;
        }
    }

    /// <summary> 1:1차관 전투 기록 </summary>
    public struct ArenaFightInfo
    {
        public long RoleId;//캐릭터 아이디
        public long UllId;//고유 아이디
        public long TimeStamp;//타입스탬프??????
        public string UserName;//대상 이름
        public int CharType;//캐릭터 종류
        public int Level;
        public int BattlePoint;
        public int Ranking;//현재 랭킹
        public int WinRanking;//전투 후 상승할 랭킹//클라에선 사용안함
        public int Camp;//진영 1-공격측, 2-수비측// 클라에선 사용안함
        public int FightResult;//승패 표기 sw.FIGHT_RESULT

        public ArenaFightInfo(long roleId, long ullId, long timeStamp, string userName, int charType, int lv, int battlePoint, int ranking, int result, int winRanking, int camp)
        {
            RoleId = roleId;
            UllId = ullId;
            TimeStamp = timeStamp;
            UserName = userName;
            CharType = charType;
            Level = lv;
            BattlePoint = battlePoint;
            Ranking = ranking;
            FightResult = result;
            WinRanking = winRanking;
            Camp = camp;
        }
    }

    /// <summary> 채팅 데이터 </summary>
    public struct ChatData
    {
        public int ClassId;
        public int VipLv;
        public int Lv;
        public long UserUID;
        public long GuildId;
        public long WhisperUID;//귓속말 내가 아님 나일 경우 대상의 아이디 대상일 경우 자신의 아이디

        public string GuildName;
        public string UserName;
        public string Msg;

        public ChatData(string msg)
        {
            Msg = msg;
            GuildName = null;
            UserName = null;

            UserUID = 0;
            GuildId = 0;
            ClassId = 0;
            VipLv = 0;
            Lv = 0;
            WhisperUID = 0;
        }

        public void Init()
        {
            Msg = null;
            GuildName = null;
            UserName = null;

            UserUID = 0;
            GuildId = 0;
            ClassId = 0;
            VipLv = 0;
            Lv = 0;
            WhisperUID = 0;
        }
    }

    #endregion

    #region :: 게임 데이터 ::

    public class DropItemData
    {
        public uint LowDataId;
        public uint Amount;

        public DropItemData(uint lowDataId, uint amount, int type)
        {
            if (lowDataId <= 0)
                LowDataId = (uint)type;
            else
                LowDataId = lowDataId;

            Amount = amount;
        }
    }

    public struct SweepSlotData
    {
        public int GetGold;
        public int GetExp;
        public int SweepCount;

        public List<DropItemData> DropList;//4개
        public List<DropItemData> CardList;//2개

        public SweepSlotData(int gold, int exp, int count, List<DropItemData> drop, List<DropItemData> card)
        {
            GetGold = gold;
            GetExp = exp;
            SweepCount = count;

            DropList = drop;
            CardList = card;
        }
    }

    /// <summary> 클리어시 보상 데이터 </summary>
    public class RewardData
    {
        public int GetAsset;//특별 재화 획득

        public int GetCoin;
        public uint GetExp;

        public uint StageId;
        public uint SaveLevel;
        public uint SaveExp;
        public uint SaveMaxExp;

        public List<DropItemData> GetList = new List<DropItemData>();//획득한 아이템 리스트
        public List<DropItemData> CardList = new List<DropItemData>();//카드 보상 리스트
    }

    /// <summary> 서버에서 보내주는 정보(골드, 경험치던전) </summary>
    public class MonsterData
    {
        public uint LowDataId;
        public uint DropValue;

        public MonsterData(int id, int value)
        {
            LowDataId = (uint)id;
            DropValue = (uint)value;
        }
    }
    
    /// <summary> 마계의탑 랭킹 데이터 </summary>
    public struct TowerRankData
    {
        public byte RankNumber;
        public uint Seconds;
        public uint CharLowData;
        public ulong RoleId;
        public string Name;
    }

    /// <summary> 싱글 스테이지 별 보상 데이터 </summary>
    public struct StageStarRewardData
    {
        public int Value;
        public int ChapterID;
        public int BoxID;
        public int StageType;   //1 일반 2 하드

        public StageStarRewardData(int boxId, int rewardStar, int chapterId, int stageType)
        {
            BoxID = boxId;
            Value = rewardStar;
            ChapterID = chapterId;
            StageType = stageType;
        }
    }

    /// <summary> 방 정보(콜로세움, 멀티보스레이드) </summary>
    public struct RoomData
    {
        public uint DungeonId;//해당 던전
        public long RoomId;//방 고유 번호
        //public ulong OwnerId;//방장
        public bool IsLeader;
        public RoomUserInfo Owner;
        public List<RoomUserInfo> UserList;
    }

    /// <summary> 콜로세움 클리어 정보 </summary>
    public struct ColosseumData
    {
        public uint StageId;//마지막까지 깬 스테이지 아이디
        public uint SelectId;
        
        public ColosseumData(uint last)//, int count, int maxCount)
        {
            StageId = 0;
            SelectId = 0;
            List<DungeonTable.ColosseumInfo> list = _LowDataMgr.instance.GetLowDataColosseumList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].RequireStageId != last)
                    continue;

                StageId = list[i].StageId;
                SelectId = list[i].StageId;//현재 진행해야 하는 스테이지
                break;
            }
            
            if(StageId == 0)//모든 스테이지 클리어이다.
            {
                StageId = list[list.Count-1].StageId+1;
                SelectId = list[list.Count-1].StageId+1;//현재 진행해야 하는 스테이지
            }
        }

        public bool IsPossible
        {
            get
            {
                if (SelectId <= StageId+1)
                    return true;

                return false;
            }
        }
    }

    public class StatusData
    {
        public bool IsOwn;//보유중인지
        public bool IsEquip;//장착중인지 Position == 1
        public int Id;// 서버 아이디
        public int Lv;// 레벨
        public int Point;//
        public uint LowDataId;//Status 테이블 아이디

        public StatusData(bool own, bool isEquip, int id, int lv, int point, uint lowDataId)
        {
            IsOwn = own;
            IsEquip = isEquip;
            Id = id;
            Lv = lv;
            Point = point;
            LowDataId = lowDataId;
        }
    }

    public class SkillSetData
    {
        public bool IsEquip;
        public uint SkillSetId;
        public uint[] SkillLevel;
        public uint[] SkillId;
        public uint StatusId;//해당 스킬의 신분 아이디

        public SkillSetData(bool equip, uint[] lv, uint[] id, uint setId, uint status)
        {
            IsEquip = equip;
            SkillLevel = lv;
            SkillId = id;
            SkillSetId = setId;
            StatusId = status;
        }
    }

    public class PassiveData
    {
        public bool IsEquip;
        public int PassiveId;
        public uint Level;
        public uint StatusId;//해당 스킬의 신분 아이디

        public PassiveData(bool equip, int id, uint lv,uint status)
        {
            IsEquip = equip;
            Level = lv;
            PassiveId = id;
            StatusId = status;
        }
    }

    #endregion

    #region :: PlayerInfo - 황비홍 ::

    public _UserInfo _userInfo = new _UserInfo();

    public _UserInfo GetUserInfo()
    {
        return _userInfo;
    }

    
    //인게임 재도전 시 이벤트 팝업 관련 된 내용 스킵
    public bool bEventPopupCheck = true;
    
    /// <summary> 유저의 재산들을 AssetType에 맞게 넣는다. </summary>
    public void SetAsset(AssetType assetType, ulong value)
    {
        if (!_userInfo._AssetDic.ContainsKey(assetType))
        {
            Debug.LogError(string.Format("unDefined AssetType! {0}", assetType));
            _userInfo._AssetDic.Add(assetType, value);
            //return;
        }
        else
            _userInfo._AssetDic[assetType] = value;

        UIMgr.instance.RefreshTopMenuCash(assetType);
    }

    /// <summary> 유저의 재산들을 AssetType에 맞게 준다. </summary>
    public ulong GetAsset(AssetType assetType)
    {
        if (!_userInfo._AssetDic.ContainsKey(assetType))
        {
            Debug.LogError(string.Format("unDefined AssetType! {0}",assetType));
            return 0;
        }

        return _userInfo._AssetDic[assetType];
    }
        
    public uint UserLevel { get { return _userInfo._Level; } }
    public string Nickname {  get { return _userInfo._charName; } }



    

    // 새로 친구의 장착정보 
    public FriendFullInfo friendFullInfo = new FriendFullInfo();

    public FriendFullInfo GetFriendFullInfo()
    {
        return friendFullInfo;
    }

    public class FriendFullInfo
    {
        public ulong RoldId;    //캐릭터 id
        public uint EqupipCount;    //장비수량


        //유저의 장비품
        Dictionary<ePartType, _ItemData> _equipList = new Dictionary<ePartType, _ItemData>();

        //유저의 아이템 리스트
        List<_ItemData> _itemList = new List<_ItemData>();

        //Dictionary<ePartType, _ItemData> _equipList = new Dictionary<ePartType, _ItemData>();   //유저의 장비품
        //List<_CostumeData> _costumeList = new List<_CostumeData>();         //유저의 코스튬 리스트

        public List<_EquipmentInfo> cEquipmentInfo;
        public List<_CostumeInfo> cCostumeInfo;
        public uint CostumeCount;   //코스튬수량

        public void ClearData()
        {
            // 초기화
            EqupipCount = 0;
            CostumeCount = 0;

            if (cCostumeInfo != null)
                cCostumeInfo.Clear();

            if (_itemList!=null)
                _itemList.Clear();
            if(_equipList!=null)
                _equipList.Clear();
        }


        /// <summary>
        /// 해당 파트타입에 맞는 아이템 넘겨준다.
        /// </summary>
        /// <param name="partType"></param>
        /// <returns></returns>
        public _ItemData GetEquipParts(ePartType partType)
        {
            _ItemData itemData = null;
            if (_equipList.TryGetValue(partType, out itemData))
                return itemData;

            return null;
        }

        /// <summary> 아이템중 고유아이디가 맞고 타입이 같은 녀석을 가져온다 </summary>
        public _ItemData GetItemDataForIndexAndType(ulong itemindex, byte itemType)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex != itemindex)
                    continue;

                if ((byte)eItemType.EQUIP == itemType && _itemList[i].IsEquipItem())
                    return _itemList[i];
                else if ((byte)eItemType.USE == itemType && _itemList[i].IsUseItem())
                    return _itemList[i];
            }

            return null;
        }
        /*
        //장비아이템용
        public _ItemData CreateEquipItem(ulong itemIndex, uint itemDataIndex, ushort enchant, ushort grade, ushort minorGrade, uint Attack)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex == itemIndex)
                {
                    //동일한게 있나 일단 검색
                    return null;
                }
            }

            _ItemData itemdata = new _ItemData(itemIndex, itemDataIndex, enchant, grade, minorGrade, Attack);
            _itemList.Add(itemdata);
            return itemdata;
        }
        */
        /*
        public bool ItemEquip(ulong itemindex)
        {
            _ItemData item = GetItemDataForIndexAndType(itemindex, (byte)eItemType.EQUIP);

            if (item != null)
            {
                //장비품인가
                if (item._itemType == (byte)eItemType.EQUIP)
                {
                    Item.EquipmentInfo itemlowdata = _LowDataMgr.instance.GetLowDataEquipItemInfo(item._equipitemDataIndex);

                    //실제로 있는 아이템인가 
                    if (itemlowdata != null)
                    {
                        //부위가 어디인가 해당하는 부위에 아이템이 있을경우 해당 아이템을 해제하고 장착해야함
                        if (_equipList.ContainsKey((ePartType)itemlowdata.UseParts))
                        {
                            //장착중 - 이전아이템 제거
                            //이제 인벤토리에 장착중인 아이템도 표시하므로 밑의 코드는 주석처리해둠
                            //_ItemData invenAdditem = CreateEquipItem(_equipList[(ePartType)itemlowdata.UseParts]._itemIndex,
                            //                                            _equipList[(ePartType)itemlowdata.UseParts]._equipitemDataIndex,
                            //                                            _equipList[(ePartType)itemlowdata.UseParts]._enchant,
                            //                                            _equipList[(ePartType)itemlowdata.UseParts]._Grade,
                            //                                            _equipList[(ePartType)itemlowdata.UseParts]._MinorGrade,
                            //                                            _equipList[(ePartType)itemlowdata.UseParts]._Attack);

                            ////스탯복사
                            //List<ItemAbilityData> statList = _equipList[(ePartType)itemlowdata.UseParts].StatList;
                            //int count = statList.Count;
                            //_equipList.Remove((ePartType)itemlowdata.UseParts);
                            //for (int i = 0; i < count; i++)
                            //{
                            //    ItemAbilityData abilityData = statList[i];
                            //    invenAdditem.StatList.Add(abilityData);
                            //}

                            ////////////////////////////////////////////////////////////////////////////////
                            ////// invenAdditem -> 장착중이었다가 해제된 아이템 여기서 인벤에 추가해준다.
                            ///////////////////////////////////////////////////////////////////////////////

                            //장착중인 아아이템 삭제
                            // _equipList.Remove((ePartType)itemlowdata.UseParts);
                            
                            //새롭게 아이템 장착
                            _equipList.Add((ePartType)itemlowdata.UseParts, item);

                            //인벤에 있는 아이템 삭제
                            //_itemList.Remove(item);
                        }
                        else
                        {
                            //장착중이아님
                            _equipList.Add((ePartType)itemlowdata.UseParts, item);

                            //장착된 아이템은 인벤토리의 아이템리스트에서 삭제
                           // _itemList.Remove(item);
                        }
                    }

                }
            }


            return false;
        }
        */
        /// <summary>
        /// 코스튬의 LowData를 빼온다
        /// </summary>
        /// <returns></returns>
        public Item.CostumeInfo GetLowData(int idx)
        {
            return _LowDataMgr.instance.GetLowDataCostumeInfo((uint)cCostumeInfo[idx].unType);
        }
        /// <summary>
        /// 캐릭터의 LowData를 빼온다
        /// </summary>
        /// <returns></returns>
        public Item.CostumeInfo GetLowDataChar(int idx)
        {
            return _LowDataMgr.instance.GetLowDataCostumeInfo((uint)cCostumeInfo[idx].unType);
        }
        /// <summary>
        /// 코스튬의 이름을 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public string GetLocName(int idx)
        {
            Item.CostumeInfo costumInfo = GetLowData(idx);
            string costumeName = _LowDataMgr.instance.GetStringItem(costumInfo.NameId);

            return costumeName;
        }

        /// <summary>
        /// 장착아이템의 LowData를 빼온다
        /// </summary>
        /// <returns></returns>
        public Item.EquipmentInfo GetLowDataEquio(int idx)
        {
            return _LowDataMgr.instance.GetLowDataEquipItemInfo((uint)idx);
        }


    }

    public class _EquipmentInfo
    {
        public int unId;                      // id
        public int unType;                        // 타입
        public int unPosition;                  // 장비 슬롯 위치
        public int unEnchantTime;             // 강화 횟수 
        public int unEvolveStar;              // 승급 별수
        public int unEvolveTime;              // 승급 횟수
        public int unRandomOption1;               // 랜덤 속성 타입1
        public int unRandomValue1;                // 랜덤 속성 수치1
        public int unRandomOption2;               // 랜덤 속성 타입2
        public int unRandomValue2;               // 랜덤 속성 수치2
        public int unBasicValue;				// 고정 속성 수치

        public _EquipmentInfo(int id, int type, int pos, int enchantTime, int evolStar, int evolTime, int op1, int val1, int op2, int val2, int basicVal )
        {
            unId = id;
            unType = type;
            unPosition = pos;
            unEnchantTime = enchantTime;
            unEvolveStar = evolStar;
            unEvolveTime = evolTime;
            unRandomOption1 = op1;
            unRandomValue1 = val1;
            unRandomOption2 = op2;
            unRandomValue2 = val2;
            unBasicValue = basicVal;
        }
    }
    public class _CostumeInfo
    {
        public int unId;                      // id
        public int unType;                        //타입
        public int unPosition;                   //장비 슬롯 위치
        public int unEvolveStar;             //승급 별수
        public int unEvolveTime;              // 승급횟수
        public List<int> unToken;              // 보석리스트(4개)
        public List<int> unSkillLevel;              // 스킬레벨리스트(5개)

        public _CostumeInfo(int id, int type, int pos, int evolStar, int enchantTime)
        {
            unId = id;
            unType = type;
            unPosition = pos;
            unEvolveStar = evolStar;
            unEvolveTime = enchantTime;

        }    
    }

    /// <summary> 셋트아이템 정보 </summary>
    public class SetItemData
    {
        public uint LowDataId;//Table EquipmentSet 아이디
        public bool IsMount;// 장착 여부

        public SetItemData(uint id, bool mount)
        {
            LowDataId = id;
            IsMount = mount;
        }
    }
    
    //코스튬 정보는 서버에 맞게 수정해주는게 좋다고 생각함
    public class _UserInfo
    {
        public ulong _AccountUUID;
        
        public ulong _charUUID;                 //서버에서 받는 내가 선택한 캐릭터의 uuid
        public uint _userCharIndex;            //유저가 선택한 캐릭터 인덱스
        public string _charName;                //서버에서 받는 내가 선택한 캐릭터의 이름
        public uint _Level;                    //유저 캐릭터의 레벨
        public uint _VipLevel;                    //유저 캐릭터의 vip 레벨
        public ulong _exp;                      //유저 캐릭터의 경험치
        public uint _TotalAttack;                //총합계 전투력
        public uint _BaseAttack;                 //캐릭터 빈몸의 전투력
        public uint _GuildId;                    //길드id
        public uint _LeftTitle;                 //접두
        public uint _RightTitle;                //접미
        public ulong _Contribution;               //공헌 

        public int ArenaRanking;                //1:1차관 랭킹정보
        public int TowerFloor;                  //마계의탑 현재 층수

        public Dictionary<AssetType, ulong> _AssetDic = new Dictionary<AssetType, ulong>();//유저 보유 자산들
        public Dictionary<EtcID, DailyCompleteData> DungeonDailyInfoDic = new Dictionary<EtcID, DailyCompleteData>();

        //유저의 장비품
        Dictionary<ePartType, _ItemData> _equipList = new Dictionary<ePartType, _ItemData>();

        //유저의 코스튬 리스트
        List<_CostumeData> _costumeList = new List<_CostumeData>();

        //유저의 아이템 리스트
        List<_ItemData> _itemList = new List<_ItemData>();

        //유저의 파트너 리스트
        List<_PartnerData> _partnerList = new List<_PartnerData>();

        //셋트아이템 정보(보유한것임)
        List<SetItemData> _SetItemList = new List<SetItemData>();

        //유저가 클리어한 스테이지 정보
        public Dictionary<uint, ClearSingleStageData> ClearSingleStageDic = new Dictionary<uint, ClearSingleStageData>();//uint : SingleTabiel ID
        public List<StageStarRewardData> StageStarReward = new List<StageStarRewardData>();
        public List<StageStarRewardData> HardStageStarReward = new List<StageStarRewardData>();
        public List<SkillSetData> _SKillSetList = new List<SkillSetData>();
        public List<PassiveData> _PassiveList = new List<PassiveData>();

        //유저가 가진 업적의 리스트
        public List<Mission> _MissionList = new List<Mission>();

        public ColosseumData _ColosseumData;//콜로세움 데이터 정보

        public bool isHideCostum;// true면 코스튬 감춘다.

        //public int MultyRaidDailyCount;//멀티보스 레이드 일일 클리어 카운트
        //public int MultyRaidDailyMaxCount;//최대 횟수

        public _UserInfo()
        {
            
        }

        /// <summary> 유저 정보 클리어. </summary>
        public void ClearData()
        {
            _SKillSetList.Clear();
            _PassiveList.Clear();

            _equipList.Clear();
            _costumeList.Clear();
            _itemList.Clear();
            _partnerList.Clear();
            _SetItemList.Clear();

            _MissionList.Clear();

            ClearSingleStageDic.Clear();
            StageStarReward.Clear();

            _AssetDic.Clear();
            DungeonDailyInfoDic.Clear();
            SetTitleLowDataId(0, 0);
            
            ArenaRanking = 0;

            SocialPanel.TalkHistory.Clear();
            SingleGameState.lastSelectStageId = 0;//지워놔야한다
        }

        //캐릭터 Table 인덱스
        public uint GetCharIdx()
        {
            return _userCharIndex;
        }

        public void SetAccountID(ulong accountId)
        {
            _AccountUUID = accountId;
        }

        //계정 고유 아이디
        public ulong GetAccountUUID()
        {
            return _AccountUUID; // return 0, do not use
        }
        //캐릭터 고유 아이디
        public ulong GetCharUUID()
        {
            return _charUUID;
        }

        /// <summary> 가방의 줌 </summary>
        public List<_ItemData> GetItemList()
        {
            return _itemList;
        }

        public void SetLevelExp(uint level, ulong exp)
        {
            _Level = level;
            _exp = exp;
        }

        public void SetTitleLowDataId(uint left, uint right)
        {
            _LeftTitle = left;
            _RightTitle = right;
        }


		// 장착된 총 아이템+costume 의 전투력 수치리턴
		// 캐릭터의 전투력을 계산한다. 
		// 그런데 캐릭터의 전투력은 서버로부터 받기 때문에 사실 따로 계산할 필요가 없다. 아마 예전 함수인듯.
		// 마을의 전투력을 갱신하는 기능때문에 여러곳에서 콜하는것 같다. 
		// 2017.9.12. kyh
		// # battlePoint total BattlePoint #getpcbattlepoint
		public uint RefreshTotalAttackPoint(bool isTown = true)
		{
			//uint TotalAttack = TotalAttack;
//			_TotalAttack = _BaseAttack;
//			
//			var enumerator = _equipList.GetEnumerator();
//			while (enumerator.MoveNext())
//			{
//				Debug.Log(" enumerator.Current.Value._Attack :"+enumerator.Current.Value._Attack);
//				_TotalAttack += enumerator.Current.Value._Attack;
//			}  

			//전투력 계산에서 코스튬을 제외한다. 20171103 kyh
			//ItemAbilityData CostumeAbility = GetEquipCostume ().AbilityData;
			//float gradeCalcedCostumeAbilityValue = _LowDataMgr.instance.GetCostumeAbilityValue (GetEquipCostume ()._Grade, GetEquipCostume ()._MinorGrade, CostumeAbility.Value);
			//float costumeBattlePoint = _LowDataMgr.instance.GetLowDataCostumeBattlePoint (CostumeAbility.Ability, gradeCalcedCostumeAbilityValue);
			//_TotalAttack += (uint)costumeBattlePoint;

			Dictionary<AbilityType, float> playerStats = NetData.instance.CalcPlayerStats();

			var statEnum = playerStats.GetEnumerator ();
			float _tot = 0f;
			while (statEnum.MoveNext()) {
				float v = _LowDataMgr.instance.GetLowDataBattlePoint(statEnum.Current.Key, statEnum.Current.Value);
				//Debug.Log(" ability :"+statEnum.Current.Key+", val:"+v);
				_tot += v;
			}

			_TotalAttack = (uint)NetData.instance.myRoundToInt2 (_tot);

			if(isTown && TownState.TownActive)
			{
				UIBasePanel townPanel = UIMgr.GetTownBasePanel();
				if (townPanel != null)
					(townPanel as TownPanel).SetUserTotalAtt(_TotalAttack);
			}
			
			return _TotalAttack;
		}



		//PMsgRoleInfoS

		public void SetPlayCharInfo(PMsgRoleInfoS pMsgRoleInfo){

			_charUUID = (ulong)pMsgRoleInfo.UllRoleId;
			_Level = (uint)pMsgRoleInfo.UnLevel;
			_VipLevel = (uint)pMsgRoleInfo.UnVipLevel;
			_exp =(ulong)pMsgRoleInfo.UllExp;
			_Contribution = (ulong)pMsgRoleInfo.UllContribution;
			_userCharIndex = (uint)pMsgRoleInfo.UnType;
			_charName = pMsgRoleInfo.SzName;
			_TotalAttack = (uint)pMsgRoleInfo.UnAttack;
			_BaseAttack = (uint)pMsgRoleInfo.UnBaseAttack;
			_GuildId = (uint)pMsgRoleInfo.UnGuildId;
			
			isHideCostum = pMsgRoleInfo.UnCostumeShowFlag == (int)COSTUME_FLAG_TYPE.COSTUME_FLAG_HIDE;
		}
        
        public void SetPlayCharInfo(ulong charIdx, uint level, uint viplv, ulong exp, ulong attend, string lastAttend, string charName, uint userCharIndex, uint BaseAttack , uint guildId, ulong contri, bool isHideCostume)
        {
            _charUUID = charIdx;
            _Level = level;
            _VipLevel = viplv;
            _exp = exp;
            _Contribution = contri;
            /*
            _energy = energy;
            _gold = gold;
            _cash = cash;
            */
            _userCharIndex = userCharIndex;
            _charName = charName;

            _BaseAttack = BaseAttack;
            _GuildId = guildId;

            isHideCostum = isHideCostume;
        }

		public void SetPlayCharAsset(PMsgRoleInfoS pMsgRoleInfo){

			if (0 < _AssetDic.Count)
			{
				Debug.LogWarning("is 'AssetDic' already setting");
				_AssetDic.Clear();
			}
			
			_AssetDic.Add(AssetType.Gold, (ulong)pMsgRoleInfo.UllCoins);
			_AssetDic.Add(AssetType.Cash, (ulong)pMsgRoleInfo.UnGem);
			_AssetDic.Add(AssetType.Energy, (ulong)pMsgRoleInfo.UnPower);
			_AssetDic.Add(AssetType.FAME, (ulong)pMsgRoleInfo.UllFame);
			_AssetDic.Add(AssetType.Contribute, (ulong)pMsgRoleInfo.UllContribution);
			_AssetDic.Add(AssetType.Badge, (ulong)pMsgRoleInfo.UllRoyalBadge);
			_AssetDic.Add(AssetType.Honor, (ulong)pMsgRoleInfo.UllHonor);
			_AssetDic.Add(AssetType.Heart, 0);
			_AssetDic.Add(AssetType.LionBadge, (ulong)pMsgRoleInfo.UllLionKingBadge);
		}
		// 

        //유저 재화 셋팅 최초 접속시만 사용.
        public void SetPlayCharAsset(ulong gold, ulong cash, ulong energy, ulong fame, ulong contribution, ulong badge, ulong honor, ulong heart, ulong lion)
        {
            if (0 < _AssetDic.Count)
            {
                Debug.LogWarning("is 'AssetDic' already setting");
                _AssetDic.Clear();
            }

            _AssetDic.Add(AssetType.Gold, gold);
            _AssetDic.Add(AssetType.Cash, cash);
            _AssetDic.Add(AssetType.Energy, energy);
            _AssetDic.Add(AssetType.FAME, fame);
            _AssetDic.Add(AssetType.Contribute, contribution);
            _AssetDic.Add(AssetType.Badge, badge);
            _AssetDic.Add(AssetType.Honor, honor);
            _AssetDic.Add(AssetType.Heart, heart);
            _AssetDic.Add(AssetType.LionBadge, lion);
        }

        public void GetCompleteCount(EtcID type, ref int now, ref int max)
        {
            DailyCompleteData data;
            if( !DungeonDailyInfoDic.TryGetValue(type, out data))
            {
                now = 0;
                max = _LowDataMgr.instance.GetEtcTableValue<int>(type);
            }
            else
            {
                now = data.CompleteCount;
                max = data.MaxCompleteCount;
            }

        }

        public void SetCompleteCount(EtcID type, int now, int max)
        {
            DailyCompleteData data;
            if (!DungeonDailyInfoDic.TryGetValue(type, out data))
            {
                if (max == 0)
                    max = _LowDataMgr.instance.GetEtcTableValue<int>(type);

                DungeonDailyInfoDic.Add(type, new DailyCompleteData(now, max) );
            }
            else
            {
                if (max == 0)
                    max = data.MaxCompleteCount;

                data.CompleteCount = now;
                data.MaxCompleteCount = max;
                DungeonDailyInfoDic[type] = data;
            }
        }

        /// <summary>
        /// 임시로 쓰일 아이템 장착/해제 스크립트 서버가 붙으면 네트워크로 처리
        /// </summary>
        /// <param name="itemindex">아이템의 고유인덱스</param>
        public bool ItemEquip(ulong itemindex)
        {
            _ItemData item = GetItemDataForIndexAndType(itemindex, (byte)eItemType.EQUIP);

            if(item != null)
            {
                //장비품인가
                if (item._itemType == (byte)eItemType.EQUIP )
                {
                    Item.EquipmentInfo itemlowdata = _LowDataMgr.instance.GetLowDataEquipItemInfo(item._equipitemDataIndex);

                    //실제로 있는 아이템인가 
                    if(itemlowdata != null )
                    {
                        //부위가 어디인가 해당하는 부위에 아이템이 있을경우 해당 아이템을 해제하고 장착해야함
                        if(_equipList.ContainsKey((ePartType)itemlowdata.UseParts))
                        {
                            //장착중인 아아이템 삭제
                            _equipList.Remove((ePartType)itemlowdata.UseParts);
                            
                            //새롭게 아이템 장착
                            _equipList.Add((ePartType)itemlowdata.UseParts, item);
                        }
                        else
                        {
                            //장착중이아님
                            _equipList.Add((ePartType)itemlowdata.UseParts, item);
                        }
                    }
                        
                }
            }

            return false;
        }

        /// <summary>
        /// 임시로 쓰일 아이템 해제 스크립트 서버가 붙으면 네트워크로 처리
        /// </summary>
        public bool ItemUnEquip(ePartType partID)
        {
            if (_equipList.ContainsKey(partID))
            {
                _equipList.Remove(partID);
                return true;
            }

            return false;
        }

        public bool ItemUnEquip(ulong itemindex)
        {
            var enumerator = _equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //invenAdditem.Stats.Add(enumerator.Current.Key, enumerator.Current.Value);

                //맞는 아이템 찾기
                if( enumerator.Current.Value._itemIndex == itemindex)
                {
                    ePartType partID = enumerator.Current.Key;

                    if (ItemUnEquip(partID))
                        return true;
                }
            }

            return false;
        }

        public bool isHaveItem(ulong itemIndex)
        {
            //장착된쪽에 있는가
            var enumerator = _equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //invenAdditem.Stats.Add(enumerator.Current.Key, enumerator.Current.Value);

                //맞는 아이템 찾기
                if (enumerator.Current.Value._itemIndex == itemIndex)
                {
                    return true;
                }
            }

            //그냥 들고있는 쪽에 있는가
            _ItemData item = GetItemDataForIndexAndType(itemIndex, (byte)eItemType.EQUIP);

            if (item != null)
            {
                //있기는 있다
                return true;
            }

            //아예없다
            return false;
        }

        public bool isEquipItem(ulong itemIndex)
        {
            var enumerator = _equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //invenAdditem.Stats.Add(enumerator.Current.Key, enumerator.Current.Value);

                //맞는 아이템 찾기
                if (enumerator.Current.Value._itemIndex == itemIndex)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary> 아이템중 고유아이디가 맞고 타입이 같은 녀석을 가져온다 </summary>
        public _ItemData GetItemDataForIndexAndType(ulong itemindex, byte itemType)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex != itemindex)
                    continue;

                if ((byte)eItemType.EQUIP == itemType && _itemList[i].IsEquipItem() )
                    return _itemList[i];
                else if ((byte)eItemType.USE == itemType && _itemList[i].IsUseItem())
                    return _itemList[i];
            }

            return null;
        }
        
        //현재 장착중인 파츠의 아이템 정보를 가져온다
        public Item.EquipmentInfo GetEquipPartsLowData(ePartType part)
        {
            if (_equipList.ContainsKey(part))
            {
                return _equipList[part].GetEquipLowData();
            }

            //옷의 정보가 없을수가 없는데..
            return null;
        }

        //현재 장착중인 아이템중 unID로 아이템을 찾는다
        public _ItemData GetEquipPartsForItemID(ulong itemIndex)
        {
            var enumerator = _equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //맞는 아이템 찾기
                if (enumerator.Current.Value._itemIndex == itemIndex)
                {
                    return enumerator.Current.Value;
                }
            }

            return null;
        }

        //현재 장착중인 파츠의 low정보를 가져온다
        public _ItemData GetEquipParts(ePartType part)
        {
            if (_equipList.ContainsKey(part))
            {
                return _equipList[part];
            }

            //옷의 정보가 없을수가 없는데..
            return null;
        }

        //현재 장착중인 코스튬의 정보를 가져온다
        public _CostumeData GetEquipCostume()
        {
            for (int i = 0; i < _costumeList.Count; i++)
            {
                if (_costumeList[i]._isEquip)
                {
                    return _costumeList[i];
                }
            }

            return null;
        }
        /// <summary>
        /// 장착중인 파트너를 뱉어준다.
        /// </summary>
        /// <param name="slotNumber">원하는 포지션의 파트너 1~2</param>
        /// <returns></returns>
        public _PartnerData GetEquipPartner(int slotNumber)
        {
            int loopCount = _partnerList.Count;
            for(int i=0; i < loopCount; i++)
            {
                _PartnerData data = _partnerList[i];
                //if (data._isEquip && data._SlotNumber.CompareTo(slotNumber) == 0)
                if (data._isEquip && data._SlotNumber == slotNumber )
                    return data;
            }
            //장착중인 파트너가 없다?
            return null;
        }

        /// <summary> 장착중인 파트너들의 서버 아이디 </summary>
        public ulong[] GetEquipPartnersIdx()
        {
            _PartnerData par_0 = GetEquipPartner(1);
            _PartnerData par_1 = GetEquipPartner(2);

            ulong[] idx = new ulong[] {
                par_0 == null ? 0 : par_0._partnerIndex,
                par_1 == null ? 0 : par_1._partnerIndex
            };

            return idx;
        }

        //costumeIndex의 코스튬을 장착한다
        public void EquipCostume(ulong costumeIndex)
        {
            for (int i = 0; i < _costumeList.Count; i++)
            {
                if (_costumeList[i]._costumeIndex == costumeIndex)
                {
                    _costumeList[i].EquipCostume();
                }
                else
                {
                    _costumeList[i].unEquipCostume();
                }
            }
        }
        
        public _CostumeData CreateCostume(ulong costumeIndex, uint costumeDataIndex, ushort grade, ushort minorGrade, ushort[] skillLevel, bool isEquip, bool isOwn)
        {
            for (int i = 0; i < _costumeList.Count; i++)
            {
                if (_costumeList[i]._costumeIndex == costumeIndex)
                {
                    Debug.Log("ASSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                    //동일한게 있나 일단 검색
                    return null;
                }
            }

            _CostumeData cosdata = new _CostumeData(costumeIndex, costumeDataIndex, grade, minorGrade, skillLevel, isEquip, isOwn);
            _costumeList.Add(cosdata);
            return cosdata;
        }

        public _PartnerData CreatePartner(ulong partnerIndex, ushort partnerDataIndex, uint nowLevel, int slotNumber, ushort MinorGrade, ushort Enchant, ulong nowExp, ushort buffResetCount, uint attack, bool isOwn)//, ushort grade
        {
            for (int i = 0; i < _partnerList.Count; i++)
            {
                if (_partnerList[i]._partnerIndex == partnerIndex)
                {
                    //동일한게 있나 일단 검색
                    return null;
                }
            }

            _PartnerData pardata = new _PartnerData(partnerIndex, partnerDataIndex, nowLevel, slotNumber,/* MinorGrade, Enchant, */nowExp/*, buffResetCount*/, attack, isOwn);//grade, 
            _partnerList.Add(pardata);

            return pardata;
        }

        //장비아이템용
        public _ItemData CreateEquipItem(ulong itemIndex, uint itemDataIndex, ushort enchant, ushort grade, ushort minorGrade, uint Attack)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex == itemIndex)
                {
                    //동일한게 있나 일단 검색
                    return null;
                }

                if (!_itemList[i].IsEquipItem())
                    continue;

                Item.EquipmentInfo info = _itemList[i].GetEquipLowData();
                Item.EquipmentInfo info2 = _LowDataMgr.instance.GetLowDataEquipItemInfo(itemDataIndex);
                if (info.EquipSetId == info2.EquipSetId && info2.UseParts == info.UseParts )//동일한 데이터다
                {
                    if (info.Grade < info2.Grade)
                    {
                        _itemList[i]._enchant = enchant;
                        _itemList[i]._equipitemDataIndex = itemDataIndex;
                        _itemList[i]._Attack = Attack;
                    }

                    return null;
                }
            }

            _ItemData itemdata = new _ItemData(itemIndex, itemDataIndex, enchant, grade, minorGrade, Attack);
            _itemList.Add(itemdata);
            return itemdata;
        }

        // 친구장비아이템용
        public _ItemData CreateFriendEquipItem(ulong itemIndex, uint itemDataIndex, ushort enchant, ushort grade, ushort minorGrade, uint Attack)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex == itemIndex)
                {
                    //동일한게 있나 일단 검색
                    return null;
                }
            }

            _ItemData itemdata = new _ItemData(itemIndex, itemDataIndex, enchant, grade, minorGrade, Attack);
            return itemdata;
        }

        /// <summary> 파트너의 데이터아이디로 찾아서 삭제한다. </summary>
        public bool RemovePartnerForDataID(uint id)
        {
            int parCount = _partnerList.Count;
            for(int i=0; i < parCount; i++)
            {
                if (_partnerList[i]._partnerDataIndex != id)
                    continue;

                _partnerList.RemoveAt(i);
                return true;
            }

            return false;
        }

        //사용아이템용
        public _ItemData CreateUseItem(ulong itemIndex, uint itemDataIndex, ushort count, bool isNew)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i]._itemIndex == itemIndex)
                {
                    //동일한게 있나 일단 검색
                    return null;
                }
            }

            _ItemData itemdata = new _ItemData(itemIndex, itemDataIndex, 0, count, isNew);
            _itemList.Add(itemdata);
            return itemdata;
        }

        /// <summary> 코스튬의 테이블 아이디로 찾음. </summary>
        /// <param name="costumeId">코스튬 테이블 아이디</param>
        public _CostumeData GetCostumeDataForLowDataID(uint lowDataId)
        {
            for (int i = 0; i < _costumeList.Count; i++)
            {
                if (_costumeList[i]._costmeDataIndex != lowDataId)
                    continue;

                return _costumeList[i];
            }

            return null;
        }

        /// <summary> 코스튬의 Index로 찾음. </summary>
        public _CostumeData GetCostumeForIndex(ulong index)
        {
            for (int i = 0; i < _costumeList.Count; i++)
            {
                if (_costumeList[i]._costumeIndex != index)
                    continue;

                return _costumeList[i];
            }

            return null;
        }

        /// <summary> 코스튬 리스트 </summary>
        public List<_CostumeData> GetCostumeList()
        {
            return _costumeList;
        }

        //특정 아이템인덱스인 아이템을 가져옴 - 조각 카운트 같은거 찾을때 씀
        public List<_ItemData> GetItemListForItemID(uint itemIndex, byte type)
        {
            List<_ItemData> list = new List<_ItemData>();
            for(int i=0;i< _itemList.Count;i++)
            {
                if(type == (byte)eItemType.EQUIP)
                {
                    //장비품이면
                    if(_itemList[i]._equipitemDataIndex == itemIndex)
                    {
                        list.Add(_itemList[i] );
                    }
                }
                else if (type == (byte)eItemType.USE)
                {
                    //소비품이면
                    if (_itemList[i]._useitemDataIndex == itemIndex)
                    {
                        list.Add(_itemList[i] );
                    }
                }
            }

            return list.Count == 0 ? null : list;
        }

        public _ItemData GetItemForItemID(uint itemIndex, byte type)
        {
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (type == (byte)eItemType.EQUIP)
                {
                    //장비품이면
                    if (_itemList[i]._equipitemDataIndex == itemIndex)
                    {
                        return _itemList[i];
                    }
                }
                else if (type == (byte)eItemType.USE)
                {
                    //소비품이면
                    if (_itemList[i]._useitemDataIndex == itemIndex)
                    {
                        return _itemList[i];
                    }
                }
            }

            return null;
        }

        /// <summary> 아이템의 총 개수를 리턴 </summary>
        public int GetItemCountForItemId(uint itemIndex, byte type)
        {
            int totalCount = 0;
            List<_ItemData> list = GetItemListForItemID(itemIndex, type);
            if (list == null || list.Count == 0)
                return 0;
            
            for (int i = 0; i < list.Count; i++)
                totalCount += list[i].Count;

            return totalCount;
        }

        /// <summary>
        /// 파트너 리스트
        /// </summary>
        /// <returns></returns>
        public List<_PartnerData> GetPartnerList()
        {
            return _partnerList;
        }

        /// <summary>
        /// 파트너 장착
        /// </summary>
        /// <param name="partnerData">장착 시키고자 하는 파트너 데이터</param>
        /// <param name="slotNumber">장착시키고자 하는 슬롯 번호</param>
        public void EquipPartner(_PartnerData partnerData, int slotNumber)
        {
            //기존꺼 해제 후 장착
            _PartnerData prevPar = GetEquipPartner(slotNumber);
            if (prevPar != null)
                prevPar.unEquipItem();

            PlayerPrefs.SetInt(string.Format("E_Par_{0}", partnerData._partnerIndex), slotNumber);
            partnerData.EquipItem(slotNumber);
        }
        /// <summary>
        /// 파트너 비어있는 슬롯 번호 뱉어줌
        /// 0은 비어있는 슬롯이 없다는 것. 1 or 2
        /// </summary>
        /// <returns></returns>
        public int GetEmptyPartnerSlot()
        {
            int emptySlot = GetEquipPartner(1) == null ? 1 : (GetEquipPartner(2) == null ? 2 : 0);
            return emptySlot;
        }
        /// <summary>
        /// 파트너 장착 해제
        /// </summary>
        /// <param name="slotNum">해제하고자 하는 슬롯</param>
        public void UnEquipPartner(int slotNum)
        {
            _PartnerData partnerData = GetEquipPartner(slotNum);
            if(partnerData == null)
                return;

            PlayerPrefs.DeleteKey(string.Format("E_Par_{0}", partnerData._partnerIndex) );
            partnerData.unEquipItem();
        }

        /// <summary> 파트너 테이블 아이디로 찾는다. </summary>
        public _PartnerData GetPartnerForDataID(uint dataID)
        {
            int listCount = _partnerList.Count;
            for (int i=0; i < listCount; i++)
            {
                _PartnerData data = _partnerList[i];
                if (data._partnerDataIndex != dataID)
                    continue;

                return data;
            }

            return null;
        }

        /// <summary> 해당 타입이 이미 존재하는지 </summary>
        public bool GetPartnerForType(int type)
        {
            int listCount = _partnerList.Count;
            for (int i = 0; i < listCount; i++)
            {
                _PartnerData data = _partnerList[i];
                Partner.PartnerDataInfo info = data.GetLowData();
                if (info.Type != type)
                    continue;

                return true;
            }
            
            return false;
        }

        /// <summary> 파트너 고유 아이디로 찾는다. </summary>
        public _PartnerData GetPartnerForIdx(ulong idx)
        {
            int listCount = _partnerList.Count;
            for (int i = 0; i < listCount; i++)
            {
                _PartnerData data = _partnerList[i];
                if (data._partnerIndex != idx)
                    continue;

                return data;
            }

            return null;
        }
        
        /// <summary>
        /// Type에 맞는 아이템 뱉어준다.
        /// None일 경우 모든 리스트 줌.
        /// </summary>
        /// <param name="itemType">원하는 타입</param>
        /// <param name="useType">itemType이 USE일 경우 세부 분류가 있다. 조각의 경우 PARTNER_SHARD, COSTUME_SHARD 둘중 아무거나 넣어도 됨</param>
        /// <returns></returns>
        public List<_ItemData> GetTypeItemList(eItemType itemType, AssetType useType = AssetType.None)
        {
            if (itemType == eItemType.NONE)
                return _itemList;

            List<_ItemData> typeList = new List<_ItemData>();
            int itemCount = _itemList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                _ItemData data = _itemList[i];
                if (data._itemType != (byte)itemType)
                {
                    if(itemType == eItemType.EQUIP && data.IsUseItem() )
                    {
                        AssetType type = (AssetType)data.GetUseLowData().Type;
                        if (type != AssetType.Jewel)
                            continue;
                    }
                    else
                        continue;
                }
                else if (useType != AssetType.None && data.IsUseItem())//사용 아이템일 경우 세부 분류가 있다. 그것을 검사.
                {
                    Item.ItemInfo useLowData = data.GetUseLowData();
                    switch (useType)//예외처리.
                    {
                        case AssetType.CostumeShard://조각의 경우 2개가 있는데 하나로 본다.
                        case AssetType.PartnerShard:
                            if (useLowData.Type != (byte)AssetType.CostumeShard
                                && useLowData.Type != (byte)AssetType.PartnerShard)
                                continue;

                            break;
                        case AssetType.Jewel:
                            if (useLowData.Type != (byte)AssetType.Jewel)
                                continue;

                            break;

                        case AssetType.Material://재료, 위에 정의 되어 있지 않은 것들은 전부 재료임
                            if (useLowData.Type == (byte)AssetType.CostumeShard ||
                                useLowData.Type == (byte)AssetType.PartnerShard ||
                                useLowData.Type == (byte)AssetType.Jewel )
                                continue;

                                break;
                        
                        default:
                            Debug.LogError("not define enum error now type = " + useType);
                            continue;
                    }
                }

                typeList.Add(data);
            }

            return typeList;
        }

        /// <summary> 장비 아이템 이 아이템이 </summary>
        public bool IsMountEquipItem(ePartType partType, ulong itemIdx)
        {
            _ItemData mountItem = null;
            if(_equipList.TryGetValue(partType, out mountItem) )
            {
                if (mountItem._itemIndex == itemIdx)
                    return true;
            }

            return false;
        }

        /// <summary> 플레이 중인 캐릭터의 현제 경험치와 Max경험치를 준다.</summary>
        public void GetCurrentAndMaxExp(ref uint curExp, ref uint maxExp)
        {
            Level.LevelInfo levelLowData = _LowDataMgr.instance.GetLowDataCharLevel(_Level);

            curExp = (uint)_exp;
            maxExp = levelLowData.Exp;
        }

        /// <summary> 소비아이템 삭제. </summary>
        public bool RemoveUseTypeItem(ulong itemIdx)
        {
            int loopCount = _itemList.Count;
            for(int i=0; i < loopCount; i++)
            {
                _ItemData useItem = _itemList[i];
                if (!useItem.IsUseItem())//소비가 아니고
                    continue;
                else if ( useItem._itemIndex != itemIdx)//아이디가 다르면 무시
                    continue;

                _itemList.Remove(useItem);
                return true;
            }

            ///에러인지는 모르겠다. 그냥 못찾음
            Debug.Log("<color=green>not found useTypeItem</color> " + itemIdx);
            return false;
        }

        /// <summary> 장비아이템 삭제. </summary>
        public bool RemoveEquipTypeItem(ulong itemIdx)
        {
            int loopCount = _itemList.Count;
            for (int i = 0; i < loopCount; i++)
            {
                _ItemData equipItem = _itemList[i];
                if (!equipItem.IsEquipItem())
                    continue;
                else if (equipItem._itemIndex != itemIdx)
                    continue;

                _itemList.Remove(equipItem);
                return true;
            }

            ///에러인지는 모르겠다. 그냥 못찾음
            Debug.Log("<color=green>not found equipTypeItem</color> " + itemIdx);
            return false;
        }

        /// <summary> 착용중인 아이템의 인덱스를 검사해서 찾는다. </summary>
        public _ItemData GetEquipItemForIdx(ulong itemIdx)
        {
            _ItemData itemData = null;
            var enumerator = _equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value._itemIndex != itemIdx)
                    continue;

                itemData = enumerator.Current.Value;
                break;
            }

            return itemData;
        }


        //일퀘나 업적중 보상을 받을수 있는것이 있는가
        public bool MissionCompletCheck()
        {
            for (int i = 0; i < _MissionList.Count; i++)
            {
                if (_MissionList[i].isComplet())
                {
                    return true;
                }
            }

            return false;
        }

        public Mission GetMission(uint missionID)
        {
            for (int i = 0; i < _MissionList.Count; i++)
            {
                if (_MissionList[i]._MissionInfo.MissionID == missionID)
                {
                    return _MissionList[i];
                }
            }

            return null;
        }
             
        //미션 타입
        //1: 특정인스턴스 클리어
        //2: 길드퀘스트 클리어(길드퀘스트의 경우 인스턴스가 아니므로 따로 분리)
        //3: 골드구매
        //4: 체력사용
        //5: 몬스터 처치
        public void MissionUpdate(MissionType type, byte SubType, uint value)
        {
            List<uint> missionList = new List<uint>();
            for(int i=0;i< _MissionList.Count;i++)
            {
                if( !_MissionList[i].isComplet() && !_MissionList[i].isClear() && _MissionList[i]._MissionInfo.MissionType == (byte)type)
                {
                    //서브타입이 0일경우 무시
                    if(_MissionList[i]._MissionInfo.MissionSubType != 0 )
                    {
                        if(_MissionList[i]._MissionInfo.MissionSubType != SubType )
                        {
                            continue;
                        }
                    }

                    //0이면 그냥 시작 0이아니면 이전업적을 클리어 했는지 확인
                    if (_MissionList[i]._MissionInfo.beforeMission != 0)
                    {
                        Mission beforeMission = GetMission(_MissionList[i]._MissionInfo.beforeMission);

                        if (beforeMission == null)
                            continue;

                        if (!beforeMission.isClear())
                            continue;
                    }

                    missionList.Add(_MissionList[i]._MissionInfo.MissionID);
                }
            }

            if(missionList.Count > 0 )
            {
                StringBuilder info = new StringBuilder();

                for (int i = 0; i < missionList.Count; i++)
                {
                    if (string.IsNullOrEmpty(info.ToString()))
                        info.Append(string.Format("{0}", missionList[i].ToString()));
                    else
                        info.Append(string.Format(",{0}", missionList[i].ToString()));
                }


                //HttpSender.instance.ReqMissionUpdate( info.ToString(), value, () =>
                //{
                    //당연같겠지...

                //});
            }            
        }

        public void SortPartnerList()
        {
            _partnerList.Sort(SortPartnerList);
        }

        int SortPartnerList(_PartnerData a, _PartnerData b)
        {
            if (a._isOwn && b._isOwn)
            {
                //둘다 보유하고있을때체크
                uint aGrade = a.CurQuality, bGrade = b.CurQuality;

                if (a == b)
                    return 0;

                if (aGrade < bGrade)
                    return 1;
                else if (aGrade == bGrade)
                {
                    if (a._partnerDataIndex < b._partnerDataIndex)
                        return 1;
                    else
                        return -1;
                }
                else
                    return -1;
            }
            else if (a == b)
                return 0;
            else if (a._isOwn && !b._isOwn)
            {
                if (b.NowShard >= b._needShard)
                    return 1;
                return -1;
            }
            else if (!a._isOwn && b._isOwn)
            {
                if (a.NowShard >= a._needShard)
                    return -1;

                return 1;
            }
            else
            {
                //둘다 보유안하고잇음 , 소환가능한것을 앞으로
                if (a.NowShard >= a._needShard && b.NowShard < b._needShard) //a가능 b불가
                    return -1;
                else if (a.NowShard >= a._needShard && b.NowShard >= b._needShard)   //a가능 b가능 -> 인덱스순
                {
                    if (a._partnerDataIndex < b._partnerDataIndex)
                        return -1;
                    return 1;
                }
                else if (a.NowShard < a._needShard && b.NowShard < b._needShard)    // a불가 b불가 
                {
                    //둘다 소환불가능하면 인덱스순으로 
                    if (a._partnerDataIndex < b._partnerDataIndex)
                        return -1;

                    return 1;
                }
                else    //a불가 b가능?
                    return 1;
            }


            //if (!a._isOwn && !b._isOwn)
            //{
            //    //둘다 보유안하고잇음 , 소환가능한것을 앞으로
            //    if (a.NowShard >= a._needShard && b.NowShard < b._needShard)
            //        return -1;
            //    else if (a.NowShard >= a._needShard && b.NowShard >= b._needShard)
            //        return 0;
            //    else if (a.NowShard < a._needShard && b.NowShard < b._needShard)
            //    {
            //        //둘다 소환불가능하면 인덱스순으로 
            //        if (a._partnerDataIndex < b._partnerDataIndex)
            //            return -1;

            //        return 1;
            //    }
            //    else
            //        return 1;
            //}
            //else if (a == b)
            //    return 0;
            //else if (!a._isOwn && b._isOwn)
            //{
            //    if (a.NowShard >= a._needShard)
            //        return -1;

            //    return 1;
            //}
            //else if (a._isOwn && !b._isOwn)
            //{
            //    if (b._needShard >= b.NowShard)
            //        return 1;
            //    return -1;
            //}
            //else
            //{
            //    //둘다 보유하고있을때체크
            //    uint aGrade = a.CurQuality, bGrade = b.CurQuality; ;

            //    if (aGrade < bGrade)
            //        return 1;
            //    else if (aGrade == bGrade)
            //    {
            //        if (a._partnerDataIndex < b._partnerDataIndex)
            //            return 1;
            //        else
            //            return -1;
            //    }
            //    else
            //        return -1;
            //}




            //if (!a._isOwn && !b._isOwn)
            //{
            //    if (a._partnerDataIndex < b._partnerDataIndex)
            //        return -1;

            //    return 1;
            //}
            //else if (a == b)
            //    return 0;
            //else if (!a._isOwn && b._isOwn)
            //    return 1;
            //else if (a._isOwn && !b._isOwn)
            //    return -1;
            //else
            //{

            //    uint aGrade = a.CurQuality, bGrade = b.CurQuality; ;

            //    if (aGrade < bGrade)
            //        return 1;
            //    else if (aGrade == bGrade)
            //    {
            //        if (a._partnerDataIndex < b._partnerDataIndex)
            //            return 1;
            //        else
            //            return -1;
            //    }
            //    else
            //        return -1;
            //}


        }

        public List<_ItemData> GetEquipItemList()
        {
            List<_ItemData> list = new List<_ItemData>();
            for (int i=0; i < (int)ePartType.PART_MAX; i++)
            {
                _ItemData item = null;
                if (!_equipList.TryGetValue((ePartType)i, out item))
                    continue;

                list.Add(item);
            }

            return list;
        }

        /// <summary> 보유중인 셋트아이템 등록 </summary>
        public void AddSetItemData(SetItemData setData)
        {
            if (IsOwnSetItemData(setData.LowDataId))
                return;

            _SetItemList.Add(setData);

            if(1 < _SetItemList.Count)
            {
                _SetItemList.Sort((a, b) => {
                    if (a.LowDataId < b.LowDataId)
                        return -1;

                    return 1;
                });
            }
        }

        /// <summary> 장착중인 셋트아이템 </summary>
        public SetItemData GetMountSetItem()
        {
            for (int i = 0; i < _SetItemList.Count; i++)
            {
                if (_SetItemList[i].IsMount)
                    return _SetItemList[i];
            }

            return null;
        }

        /// <summary> 셋트아이템 장착 변경 </summary>
        public void ChangeMountSetItem(uint newMountId)
        {
            SetItemData prevSet = GetMountSetItem();
            if(prevSet != null)
                prevSet.IsMount = false;
            
            SetItemData mountSet = GetOwnSetItemData(newMountId);
            if(mountSet != null)
                mountSet.IsMount = true;

            Item.EquipmentSetInfo equipSetInfo = _LowDataMgr.instance.GetItemSetLowData(newMountId);
            for(int i=0; i < _itemList.Count; i++)
            {
                if ( !_itemList[i].IsEquipItem() )
                    continue;

                Item.EquipmentInfo info = _itemList[i].GetEquipLowData();
                if (info.EquipSetId != equipSetInfo.Id)
                    continue;

                Debug.Log("Equip ITem " + _itemList[i].EquipPartType + " " + _itemList[i]._itemIndex);
                ItemEquip(_itemList[i]._itemIndex);
            }

            RefreshTotalAttackPoint();
        }

        /// <summary> 장착중인 셋트아이템 </summary>
        public bool IsMountSetItem(uint lowDataId)
        {
            for (int i = 0; i < _SetItemList.Count; i++)
            {
                if (_SetItemList[i].LowDataId.Equals(lowDataId))
                    return _SetItemList[i].IsMount;
            }

            return false;
        }

        /// <summary> 보유중인 셋트아이템인지 </summary>
        public bool IsOwnSetItemData(uint lowDataId)
        {
            for(int i=0; i < _SetItemList.Count; i++)
            {
                if (_SetItemList[i].LowDataId.Equals(lowDataId) )
                    return true;
            }

            return false;
        }

        /// <summary> 보유중인 셋트아이템인지 </summary>
        public SetItemData GetOwnSetItemData(uint lowDataId)
        {
            for (int i = 0; i < _SetItemList.Count; i++)
            {
                if (_SetItemList[i].LowDataId.Equals(lowDataId))
                    return _SetItemList[i];
            }

            return null;
        }

        public List<SetItemData> GetOwnSetItemData()
        {
            return _SetItemList;
        }

        /// <summary> 장착중인 스킬셋 넘겨준다. </summary>
        public SkillSetData GetEquipSKillSet()
        {
            for(int i=0; i < _SKillSetList.Count; i++)
            {
                if (_SKillSetList[i].IsEquip)
                    return _SKillSetList[i];
            }

            return null;
        }

        /// <summary> 아이디에 맞는 스킬셋 넘겨준다 </summary>
        public SkillSetData GetSkillSetData(uint lowDataId)
        {
            for (int i = 0; i < _SKillSetList.Count; i++)
            {
                if (_SKillSetList[i].SkillSetId.Equals(lowDataId))
                    return _SKillSetList[i];
            }

            return null;//보유중인 것이 아니다.
        }

        /// <summary> 장착중인 패시브 넘겨준다. </summary>
        public PassiveData GetEquipPassive()
        {
            for (int i = 0; i < _PassiveList.Count; i++)
            {
                if (_PassiveList[i].IsEquip)
                    return _PassiveList[i];
            }

            return null;
        }

        /// <summary> 아이디에 맞는 패시브 넘겨준다 </summary>
        public PassiveData GetPassiveData(int lowDataId)
        {
            for (int i = 0; i < _PassiveList.Count; i++)
            {
                if (_PassiveList[i].PassiveId.Equals(lowDataId))
                    return _PassiveList[i];
            }

            return null;//보유중인 것이 아니다.
        }
    }
    #endregion

    #region :: Mission(일퀘/업적) - 황비홍 ::
    public class Mission
    {
        public MissionTable.MissionInfo _MissionInfo;
        public uint _MissionValue; //횟수가 몇번인지 _MissionInfo.MissionValue 만큼 하면 완료받기 직전의 상태와 동일하다
        public bool _MissionClear; //현재 미션이 완료되었는지(보상까지 받은상태)

        public void InitMission(MissionTable.MissionInfo MissionInfo)
        {
            _MissionInfo = MissionInfo;

        }

        //두개를 분리해야될수도
        public void SetMissionValue(uint MissionValue, bool MissionClear)
        {
            _MissionValue = MissionValue;
            _MissionClear = MissionClear;
        }

        public void SetMissionClear()
        {
            _MissionClear = true;
            _MissionValue = _MissionInfo.MissionValue;
        }

        public bool isComplet()
        {
            //완료한 상태인가 리턴
            if (_MissionInfo.MissionValue <= _MissionValue)
                return true;

            return false;
        }

        //완전하게 완료한 상태인가 -> 보상을 받은상태
        public bool isClear()
        {
            return _MissionClear;
        }

    }    

    #endregion

    #region :: Costume - 황비홍 ::
    /// <summary>
    /// 실제 게임에쓰일 코스튬데이터 
    /// </summary>
    public class _CostumeData
    {
        public ulong _costumeIndex;         //서버에서 받아온 코스튬의 ID
        public uint _costmeDataIndex;       //LowData의 코스튬 인덱스
        public ushort _Grade;             //성급 10회당 1씩 오르는 것
        public ushort _MinorGrade;          //성급(서버에서 받아와야함) - 코스튬은 아이템과 다르게 성급채워서 등급올리는 시스템은 없다.

        public uint _needShard;            //최대 조각의 갯수

        public ushort[] _skillLevel;

        public bool _isEquip;   //착용중인지
        public bool _isOwn;     //보유중인지
        public ItemAbilityData AbilityData = new ItemAbilityData();

        //장착된 보석의 정보
        //public Dictionary<ushort, _ItemData> _equipJewelList = new Dictionary<ushort, _ItemData>();
        public uint[] _EquipJewelLowId = new uint[SystemDefine.MaxJewelCount];

        public _CostumeData(ulong costumeIndex, uint costmeDataIndex, ushort grade, ushort MinorGrade, ushort[] skillLevel, bool isEquip, bool isOwn)
        {
            _costumeIndex = costumeIndex;
            _costmeDataIndex = costmeDataIndex;
            _Grade = grade;
            _MinorGrade = MinorGrade;

            //_equipJewelList.Clear();
            int arrLength = _EquipJewelLowId.Length;
            for (int i=0; i < arrLength; i++)
            {
                _EquipJewelLowId[i] = 0;
            }

            _skillLevel = skillLevel;

            Item.CostumeInfo lowData = GetLowData();
            if (lowData != null)
            {
                _needShard = lowData.NeedShardValue;

                Item.ItemValueInfo valueInfo = _LowDataMgr.instance.GetLowDataItemValueInfo(lowData.BasicOptionIndex);
                AbilityData.Ability = (AbilityType)valueInfo.OptionId;
                AbilityData.Value = valueInfo.BasicValue;//* 0.1f
            }

            _isEquip = isEquip;
            _isOwn = isOwn;
        }
        //착용
        public bool EquipCostume()
        {
            _isEquip = true;
            return true;
        }
        //해제
        public bool unEquipCostume()
        {
            _isEquip = false;
            return true;
        }
        //사용할수 있냐
        public bool IsUse()
        {
            if (_isOwn)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// 코스튬의 LowData를 빼온다
        /// </summary>
        /// <returns></returns>
        public Item.CostumeInfo GetLowData()
        {
            return _LowDataMgr.instance.GetLowDataCostumeInfo(_costmeDataIndex);
        }
        /// <summary>
        /// 코스튬의 이름을 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public string GetLocName()
        {
            Item.CostumeInfo costumInfo = GetLowData();
            string costumeName = _LowDataMgr.instance.GetStringItem(costumInfo.NameId );

            return costumeName;
        }
        /// <summary>
        /// Costume의 Description을 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            Item.CostumeInfo costumInfo = GetLowData();
            string desc = _LowDataMgr.instance.GetStringItem(costumInfo.DescriptionId);
             
            return desc;
        }
        /// <summary>
        /// 아이콘을 뱉어준다
        /// </summary>
        /// <returns></returns>
        public string GetIconName()
        {
            Item.CostumeInfo costumInfo = GetLowData();

            return _LowDataMgr.instance.GetLowDataIcon(costumInfo.Icon );
        }
        /*
        /// <summary> 스킬 리스트를 넘겨준다. </summary>
        public List<uint> GetSkillList()
        {
            Item.CostumeInfo costumInfo = GetLowData();
            List<uint> list = new List<uint>();
            list.Add(System.Convert.ToUInt32(costumInfo.skill0[0]));
            list.Add(costumInfo.skill1);
            list.Add(costumInfo.skill2);
            list.Add(costumInfo.skill3);
            list.Add(costumInfo.skill4);

            return list;
        }
        */
        /// <summary> etcTable에 있는 것을 참고해야 하지만 하드코딩을 하는 결과 뿐이여서 따로 뺌.</summary>
        public ushort MaxJewelSlot
        {
            get {
                int slot_01 = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.CostumeOpenSlot1);
                int slot_02 = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.CostumeOpenSlot2);
                int slot_03 = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.CostumeOpenSlot3);
                int slot_04 = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.CostumeOpenSlot4);
                if (slot_01 <= _Grade && _Grade < slot_02)//테스트를 해야하니 0등급일 때에도 슬롯 1개를줌
                    return 1;
                else if (slot_02 <= _Grade && _Grade < slot_03)
                    return 2;
                else if (slot_03 <= _Grade && _Grade < slot_04)
                    return 3;
                else if (slot_04 <= _Grade)
                    return 4;

                return 0;
            }
        }
    }
    #endregion

    #region :: Item - 황비홍 ::

    public class ItemAbilityData
    {
        public AbilityType Ability;
        public float Value;

        public ItemAbilityData() {
        }

        public ItemAbilityData(int type, float value) {
            Ability = (AbilityType)type;
            Value = value;
        }
    }

    public class _ItemData
    {
        public byte _itemType;              //아이템의 타입 사용아이템인지 장비아이템인지
        public bool IsNewItem;              //신규 아이템인지.
        private ushort _count;               //use아이템일경우의 갯수 (사용아이템일경우 갯수가 지정되어있지만 장비아이템의 경우 0)
        public ushort _enchant;             //장비 아이템일경우 강화 등급(서버에서 받아와야한다 사용아이템은 없겠지)
        public ushort _MinorGrade;          //장비 아이템의 성급(성급을 맥스로 채우면 등급이 올라감)
        public uint _Attack;                 //전투력
        public uint _equipitemDataIndex;  //LowData의 Equip아이템 인덱스
        public uint _useitemDataIndex;    //LowData의 use아이템 인덱스
        public ulong _itemIndex;             //서버에서 받아온 아이템의 ID

        //스탯들 - 어차피 클라이언트는 서버에서 보내주는 스탯들로 채우면됨
        //public Dictionary<AbilityType, float> Stats = new Dictionary<AbilityType, float>();
        public List<ItemAbilityData> StatList = new List<ItemAbilityData>();
        public _ItemData(ulong itemIndex, uint itemDataIndex, ushort enchant, ushort Grade, ushort MinorGrade, uint Attack )
        {
            _itemIndex = itemIndex;
            _equipitemDataIndex = itemDataIndex;
            _count = 0;
            _itemType = (byte)eItemType.EQUIP;
            _enchant = enchant;
            //_Grade = Grade;
            _MinorGrade = MinorGrade;
            _Attack = Attack;

            IsNewItem = false;//얘하고는 별로 관계없는 변수 무조건 false
            //_isEquip = false;
        }

        public _ItemData(ulong itemIndex, uint itemDataIndex, ushort enchant, ushort count, bool isNew)
        {
            _itemIndex = itemIndex;
            _useitemDataIndex = itemDataIndex;
            _count = count;
            _itemType = (byte)eItemType.USE;
            _enchant = 0;
            IsNewItem = isNew;

            Item.ItemInfo useLowData = GetUseLowData();
            if(useLowData != null && 0 < useLowData.OptionType)
            {
                ItemAbilityData data = new ItemAbilityData();
                data.Ability = (AbilityType)useLowData.OptionType;
                data.Value = useLowData.value;

                StatList.Add(data);
            }
            
        }

        public _ItemData(_ItemData target)
        {
            _itemIndex = target._itemIndex;
            _equipitemDataIndex = target._equipitemDataIndex;
            _useitemDataIndex = target._useitemDataIndex;
            _itemType = target._itemType;
            _count = target._count;
            _enchant = target._enchant;
            //_Grade = target._Grade;
            _MinorGrade = target._MinorGrade;
            StatList = target.StatList;
        }

        public ushort Count {
            set {
                _count = value;
            }
            get {
                if(_useitemDataIndex.Equals(599002))//휘장
                {
                    return (ushort)instance.GetAsset(AssetType.Badge);//(ushort)(_count+(ushort)instance.GetAsset(AssetType.Badge));
                }
                else if (_useitemDataIndex.Equals(599003))//성망
                {
                    return (ushort)instance.GetAsset(AssetType.FAME);// (ushort)(_count +(ushort)instance.GetAsset(AssetType.FAME));
                }

                return _count;
            }
        }

        public ushort _Grade {
            get {
                if (IsEquipItem())
                    return GetEquipLowData().Grade;
                else
                    return GetUseLowData().Grade;
            }
        }               //장비 아이템의 등급
        /// <summary>
        /// 장착가능 장비아이템의 LowData를 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public Item.EquipmentInfo GetEquipLowData()
        {
            if ( !IsEquipItem() )
                return null;

            return _LowDataMgr.instance.GetLowDataEquipItemInfo(_equipitemDataIndex);
        }
        /// <summary>
        /// 사용아이템의 LowData를 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public Item.ItemInfo GetUseLowData()
        {
            if (IsEquipItem() )
                return null;

            return _LowDataMgr.instance.GetUseItem(_useitemDataIndex);
        }

        /// <summary>
        /// 장착가능한 장비아이템인지 판별해주는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsEquipItem()
        {
            if (0 < _equipitemDataIndex && _itemType == (byte)eItemType.EQUIP )
                return true;

            return false;
        }

        /// <summary>
        /// 장착가능한 장비아이템인지 판별해주는 함수
        /// </summary>
        /// <returns></returns>
        public bool IsUseItem()
        {
            if (0 < _useitemDataIndex && _itemType == (byte)eItemType.USE)
                return true;

            return false;
        }

        /// <summary>
        /// 아이템의 이름을 준다.
        /// </summary>
        /// <returns></returns>
        public string GetLocName()
        {
            uint nameId = 0;
            if(IsEquipItem() )
            {
                Item.EquipmentInfo equipLowData = GetEquipLowData();
                nameId = equipLowData.NameId;
            }
            else
            {
                Item.ItemInfo useLowData = GetUseLowData();
                nameId = useLowData == null ? 0 : useLowData.NameId;
            }

            string locName = _LowDataMgr.instance.GetStringItem(nameId);
            return locName;
        }
        /*
        /// <summary>
        /// 장비 셋트 데이터를 줌 장비만임
        /// </summary>
        /// <returns></returns>
        public Item.EquipmentSetInfo GetEquipSetLowData()
        {
            if (!IsEquipItem())
                return null;

            return _LowDataMgr.instance.GetItemSetLowData(_equipitemDataIndex);
        }
        */
        /// <summary>
        /// 아이템의 판매가격을 준다
        /// </summary>
        /// <returns></returns>
        public ulong GetPrice()
        {
            ulong price = 0;

            if (IsEquipItem())
            {
                Item.EquipmentInfo equipLowData = GetEquipLowData();
                price = equipLowData.SellPrice;
            }
            else
            {
                Item.ItemInfo useLowData = GetUseLowData();
                price = useLowData.SellPrice;
            }

            return price;
        }

        /// <summary>
        /// 파트 타입 뱉어준다
        /// </summary>
        public ePartType EquipPartType
        {
            get {
                if (!IsEquipItem())
                    return ePartType.NONE;

                Item.EquipmentInfo eLowData = GetEquipLowData();
                if(eLowData == null)
                    return ePartType.NONE;

                return (ePartType)eLowData.UseParts;
            }
        }
        
    }
    #endregion

    #region :: Partner - 황비홍 ::
    //파트너 액티브 스킬데이터
    public class _PartnerActiveSkillData
    {
        public byte _skillLevel;
        public uint _skillIndex;

        public _PartnerActiveSkillData(uint SkillIndex, byte Level)
        {
            _skillLevel = Level;
            _skillIndex = SkillIndex;
        }
    }

    public class _PartnerData
    {
        public int _SlotNumber;             //파트너의 슬롯 번호(서버에서 받아와야한다) -1은 미착용 0 or 1 임.
        public ulong _partnerIndex;           //서버에서 받아온 파트너의 ID
        public uint _NowLevel;               //파트너의 현재 레벨(서버에서 받아와야한다)
        public uint _MaxLevel;               //파트너의 최대 레벨 (등급에따라다름)
        //public ushort _MinorGrade;           //파트너의 성급(서버에서 받아와야한다)   
        //public ushort _Enchant;              //파트너의 강화등급(서버에서 받아와야한다)   
        public uint _Attack;                    //파트너의 공격력
        public uint CurQuality  //현재등급(성급과동일..?)
        {
            get {
                return GetLowData().Quality;
            }
        }

        private ulong NowExp;//현재 경험치
        public ulong _NowExp//현재 경험치 계산함.
        {
            set {
                NowExp = value;
            }
            get
            {
                //if (1 < _NowLevel)
                //{
                    //PartnerLevel.PartnerLevelInfo lvLowData = _LowDataMgr.instance.GetLowDataPartnerLevel(_NowLevel - 1);
                    //return NowExp- lvLowData.Expoverlab;
                //}
                //else//1레벨
                    return NowExp;
            }
        }

        public uint _MaxExp//경험치 최대 량.
        {
            get {
                PartnerLevel.PartnerLevelInfo lvLowData = _LowDataMgr.instance.GetLowDataPartnerLevel(_NowLevel);
                if (lvLowData == null)
                    return 0;

                return lvLowData.Exp;
            }
        }

        public ushort _partnerDataIndex;     //LowData의 파트너 인덱스
        //public ushort _nowShard;             //현재 조각의 갯수
        public ushort _needShard;            //최대 조각의 갯수

        //액티브스킬리스트
        public Dictionary<ushort, _PartnerActiveSkillData> ActiveSkillList = new Dictionary<ushort, _PartnerActiveSkillData>();

        public bool _isEquip;               //장착중인가
        public bool _isOwn;                 //보유중인가

        //버프스킬 인덱스도 여기서 넣어줘야 하나...
        public _PartnerData(ulong partnerIndex, ushort partnerDataIndex, uint nowLevel, int slotNumber/*, ushort MinorGrade, ushort Enchant*/, ulong exp, /*ushort buffResetCount,*/uint attack, bool isOwn)//, ushort grade
        {
            _partnerIndex = partnerIndex;
            _partnerDataIndex = partnerDataIndex;

            _NowLevel = nowLevel;
            //_Grade = grade;
            _SlotNumber = slotNumber <= 0 ? PlayerPrefs.GetInt(string.Format("E_Par_{0}", _partnerIndex), -1) : slotNumber;
            //_Enchant = Enchant;
            NowExp = exp;
            _Attack = attack;

            Partner.PartnerDataInfo lowData = GetLowData();
            if (lowData == null)
                return;

            _needShard = lowData.NeedShardValue;
           // _MinorGrade = isOwn ? MinorGrade : lowData.Quality;

            _MaxLevel = (uint)GetMaxEvolveLevel(lowData.Quality);

            if ( 0 < _SlotNumber)
                _isEquip = true;
            else
                _isEquip = false;

            _isOwn = isOwn;
        }

        public _PartnerActiveSkillData GetBuffSkillToSlot(ushort slot)
        {
            if(ActiveSkillList.ContainsKey(slot))
            {
                //return BuffSkill[EquipBuffSkill[slot]];
                return ActiveSkillList[slot];
            }

            return null;
        }

        public void EquipBuffSkillData(ushort slot, _PartnerActiveSkillData buffSkillData)
        {
            if (ActiveSkillList.ContainsKey(slot))
            {
                ActiveSkillList[slot] = buffSkillData;
            }
            else
            {
                ActiveSkillList.Add(slot, buffSkillData);
            }
        }

        public void ActivePartner()
        {
            _isOwn = true;
        }

        public bool EquipItem(int slotNumber)
        {
            _isEquip = true;
            _SlotNumber = slotNumber;
            return true;
        }

        public bool unEquipItem()
        {
            _isEquip = false;
            _SlotNumber = -1;
            return true;
        }

        /// <summary>
        /// 현재 조각 개수를 리턴해준다
        /// </summary>
        public int NowShard
        {
            get
            {
                //나중에 수정 - 임시로 _needShard값이랑 맞춰준다
                //return _needShard;

                Partner.PartnerDataInfo lowData = GetLowData();
                int totalCount = NetData.instance.GetUserInfo().GetItemCountForItemId(lowData.ShardIdx, (byte)eItemType.USE);
                return totalCount;
            }
        }
        
        /// <summary>
        /// 파트너의 LowData를 빼온다
        /// </summary>
        /// <returns></returns>
        public Partner.PartnerDataInfo GetLowData()
        {
            return _LowDataMgr.instance.GetPartnerInfo(_partnerDataIndex);
        }
        /// <summary>
        /// 파트너의 이름을 뱉어준다.
        /// </summary>
        /// <returns></returns>
        public string GetLocName()
        {
            Partner.PartnerDataInfo info = GetLowData();
            string name = _LowDataMgr.instance.GetStringUnit(info.NameId);

            return name;
        }
        /// <summary>
        /// 파트너의 Class Type(PartnerClassType)
        /// </summary>
        /// <returns></returns>
        public string GetClassType()
        {
            string typeName = null;
            Partner.PartnerDataInfo info = GetLowData();
            switch((PartnerClassType)info.Class )
            {
                case PartnerClassType.Attack:
                    typeName = "Img_MarkAttack02";
                    break;
                case PartnerClassType.Buff:
                    typeName = "Img_MarkBuff02";
                    break;
                case PartnerClassType.Defence:
                    typeName = "Img_MarkShield02";
                    break;
                case PartnerClassType.None:
                    break;
            }

            return typeName; 
        }
        /// <summary>
        /// 파트너의 아이콘을 준다.
        /// </summary>
        /// <returns></returns>
        public string GetIcon()
        {
            Partner.PartnerDataInfo info = GetLowData();
            return info.PortraitId;
        }

        /// <summary>
        /// 파트너의 등급을출력
        /// </summary>
        /// <returns></returns>
        public string GetGradeName()
        {
            string gradeName = "";
            switch (CurQuality)
            {
                case 1: //일반
                    gradeName = _LowDataMgr.instance.GetStringCommon(1145);
                    break;
                case 2://우수
                    gradeName = _LowDataMgr.instance.GetStringCommon(1146);
                    break;
                case 3://희귀
                    gradeName = _LowDataMgr.instance.GetStringCommon(1147);
                    break;
                case 4://고대
                    gradeName = _LowDataMgr.instance.GetStringCommon(1148);
                    break;
                case 5://전설
                    gradeName = _LowDataMgr.instance.GetStringCommon(1149);
                    break;
                case 6://무쌍
                    gradeName = _LowDataMgr.instance.GetStringCommon(1150);
                    break;
            }

            return gradeName;
        }

        /// <summary>
        /// 파트너의 승급최대레벨을 준다
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public int GetMaxEvolveLevel(int grade)
        {
            int level = 0;
            switch (grade)
            {
                case 1:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner1GradeMaxLevel);
                    break;
                case 2:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner2GradeMaxLevel);
                    break;
                case 3:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner3GradeMaxLevel);
                    break;
                case 4:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner4GradeMaxLevel);
                    break;
                case 5:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner5GradeMaxLevel);
                    break;
                case 6:
                    level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner6GradeMaxLevel);
                    break;

            }
            return level;
        }


        /// <summary>
        /// 스킬 리스트를 준다.
        /// </summary>
        /// <returns></returns>
        public List<uint> GetSkillIdList()
        {
            Partner.PartnerDataInfo info = GetLowData();
            List<uint> skillList = new List<uint>() {
                info.skill0,//평타
                info.skill1,//스킬1
                info.skill2,//스킬2
                info.skill3,//스킬3
                info.skill4 //필살기 스킬
            };

            return skillList;
        }

        /// <summary>
        /// 필살기 스킬 아이콘 뱉어주기.
        /// </summary>
        /// <returns></returns>
        public string GetStrongSkillIcon()
        {
            List<uint> skillList = GetSkillIdList();
            SkillTables.ActionInfo acLowData = _LowDataMgr.instance.GetSkillActionLowData(skillList[4]);

            if (acLowData == null)
            {
                Debug.LogError("not found strong skill LowData error! " + skillList[4] );
                return null;
            }

            return _LowDataMgr.instance.GetLowDataIcon(acLowData.Icon );
        }

        /// <summary>
        /// 스킬 아이콘들
        /// </summary>
        /// <param name="skillIcon_0">평타</param>
        /// <param name="skillIcon_1">액티브 스킬 1</param>
        /// <param name="skillIcon_2">액티브 스킬 2</param>
        /// <param name="skillIcon_3">액티브 스킬 3</param>
        public void GetSkillIcons(ref string skillIcon_0, ref string skillIcon_1, ref string skillIcon_2, ref string skillIcon_3)
        {
            List<uint> skillList = GetSkillIdList();
            SkillTables.ActionInfo acLowData_0 = _LowDataMgr.instance.GetSkillActionLowData(skillList[0]);
            SkillTables.ActionInfo acLowData_1 = _LowDataMgr.instance.GetSkillActionLowData(skillList[1]);
            SkillTables.ActionInfo acLowData_2 = _LowDataMgr.instance.GetSkillActionLowData(skillList[2]);
            SkillTables.ActionInfo acLowData_3 = _LowDataMgr.instance.GetSkillActionLowData(skillList[3]);

            if (acLowData_0 != null)
                skillIcon_0 = _LowDataMgr.instance.GetLowDataIcon(acLowData_0.Icon );
            if (acLowData_1 != null)
                skillIcon_1 = _LowDataMgr.instance.GetLowDataIcon(acLowData_1.Icon );
            if (acLowData_2 != null)
                skillIcon_2 = _LowDataMgr.instance.GetLowDataIcon(acLowData_2.Icon);
            if (acLowData_3 != null)
                skillIcon_3 = _LowDataMgr.instance.GetLowDataIcon(acLowData_3.Icon);
        }

        /// <summary> 값 복사. </summary>
        public _PartnerData(_PartnerData p)
        {
            _partnerIndex = p._partnerIndex;
            _partnerDataIndex = p._partnerDataIndex;
            _NowLevel = p._NowLevel;
            //_Grade = p._Grade;
            _SlotNumber = p._SlotNumber;
            //_MinorGrade = p._MinorGrade;
            //_Enchant = p._Enchant;
            _NowExp = p.NowExp;

            _needShard = p._needShard;
            _MaxLevel =(uint)GetMaxEvolveLevel(p.GetLowData().Quality);// p._MaxLevel;
            _Attack = p._Attack;

            _isEquip = p._isEquip;
            _isOwn = p._isOwn;
        }

        /// <summary> 원본 경험치. 계산이 아무것도 안되어 있음 </summary>
        public ulong GetOriginalNowExp()
        {
            return NowExp;
        }

        /// <summary> 파트너 소환했을때 실데이터로 변경해준다. </summary>
        public void SetSpawnPartnerData(ulong partnerIdx, uint level, ulong exp, ushort enchant, ushort minorGrade, ushort buffResetCount, uint Attack)//, ushort grade
        {
            _partnerIndex = partnerIdx;
            _NowLevel = level;
            _NowExp = exp;
            //_Enchant = enchant;
            //_MinorGrade = minorGrade;
            //_Grade = grade;
            _isOwn = true;
            _Attack = Attack;

            _SlotNumber = PlayerPrefs.GetInt(string.Format("E_Par_{0}", partnerIdx), -1);
            if (0 < _SlotNumber)
                _isEquip = true;
        }
    }
    #endregion

    

    
	

    #region :: Skill - 황비홍 ::
    public class _SkillDataGrop
    {
        public SkillData[] normalAttackData;
        public SkillData[] skillData;

        public Dictionary<uint, ActionInfo> ActionDic;
        public Dictionary<uint, List<AbilityData>> AbilityDic;

        public ActionInfo GetAction(uint skillID)
        {
            if (ActionDic.ContainsKey(skillID))
            {
                return ActionDic[skillID];
            }

            return null;
        }

        public ushort GetAbilityCount(uint skillID)
        {
            if (AbilityDic.ContainsKey(skillID))
            {
                return (ushort)AbilityDic[skillID].Count;
            }

            return 0;
        }

        public List<AbilityData> GetAbilityList(uint skillID)
        {
            if (AbilityDic.ContainsKey(skillID))
            {
                return AbilityDic[skillID];
            }

            return null;
        }

        public AbilityData GetAbility(uint skillID, uint notiIdx)
        {
            if (AbilityDic.ContainsKey(skillID))
            {
                for (int i = 0; i < AbilityDic[skillID].Count; i++)
                {
                    if (AbilityDic[skillID][i].notiIdx == notiIdx)
                    {
                        return AbilityDic[skillID][i];
                    }
                }
            }

            return null;
        }

        //적용때마다 검색해서 가져오는데 그냥 저장을 해놓는게 나을려나
        public SkillTables.BuffInfo GetBuff(uint BuffIdx, uint skillIdx)
        {
            SkillTables.BuffInfo nBuffData = new SkillTables.BuffInfo();
            SkillTables.BuffInfo _BuffData = _LowDataMgr.GetBuffData(BuffIdx);
            if (_BuffData == null)
                return null;
            
            nBuffData.baseFactor = _BuffData.baseFactor;
            nBuffData.buffDisplay = _BuffData.buffDisplay;
            nBuffData.buffType = _BuffData.buffType;
            nBuffData.descrpition = _BuffData.descrpition;
            nBuffData.startType = _BuffData.startType;
            //nBuffData.durationTime = _BuffData.durationTime;
            nBuffData.effect = _BuffData.effect;
            nBuffData.CastingEffect = _BuffData.CastingEffect;
            //nBuffData.eventValue1 = _BuffData.eventValue1;
            //nBuffData.eventValue2 = _BuffData.eventValue2;
            nBuffData.factorRate = _BuffData.factorRate;
            nBuffData.icon = _BuffData.icon;
            nBuffData.Indx = _BuffData.Indx;
            nBuffData.name = _BuffData.name;
            nBuffData.overLapCount = _BuffData.overLapCount;
            //nBuffData.Rate = _BuffData.Rate;
            nBuffData.tic = _BuffData.tic;
            nBuffData.buffAbility = _BuffData.buffAbility;

            //내가 가진 스킬정보중 스킬레벨을 찾아오자.

            int SkillLevel = -1;
            //평타에도 일단 체크
            for(int i=0;i< normalAttackData.Length;i++)
            {
                if(normalAttackData[i] != null)
                {
                    if(normalAttackData[i]._SkillID == skillIdx)
                    {
                        SkillLevel = normalAttackData[i]._SkillLevel;
                        break;
                    }
                }
            }

            //스킬에서 체크
            if(SkillLevel != -1)
            {
                for (int i = 0; i < skillData.Length; i++)
                {
                    if (skillData[i] != null)
                    {
                        if (skillData[i]._SkillID == skillIdx)
                        {
                            SkillLevel = skillData[i]._SkillLevel;
                            break;
                        }
                    }
                }
            }

            //if(SkillLevel != -1)
            //{
            //    SkillTables.SkillLevelInfo skillLevelInfo = _LowDataMgr.GetSkillLevelData(skillIdx, (byte)SkillLevel);

            //    if(skillLevelInfo != null)
            //    {
            //        //nBuffData.factorRate = skillLevelInfo.factorRate[0].ToString();
            //        nBuffData.durationTime = skillLevelInfo.durationTime;
            //    }
            //}

            return nBuffData;
        }
    }
    #endregion

    #region :: 로그인시 캐릭터 리스트
    /// <summary>
    /// 로그인했을때 생성한 캐릭터 정보 가지고있을 데이터
    /// </summary>
    public struct CreateCharData
    {
        public ulong charIdx;//고유 아이디
        public ulong charTableIdx;//테이블 아이디
        public ulong charSlot;//생성한 슬롯 번호

        public string charNick;//캐릭터 네임

        public ulong co_Idx;//착용한 코스튬 고유 아이디
        public ulong co_TableIdx;//착용한 코스튬 테이블 아이디
        
        public void Empty()
        {
            charIdx = 0;
            charTableIdx = 0;
            charSlot = 0;

            charNick = null;

            co_Idx = 0;
            co_TableIdx = 0;
        }
    }

    /// <summary>
    /// 스테이지 별등급 클리어 조건 데이터
    /// </summary>
    public class StageClearData
    {
        public ClearQuestType Type;
        public uint Value;
        public int CurValueCount;
        public bool IsClear;

        public StageClearData(ClearQuestType type, uint value)
        {
            Type = type;
            Value = value;

            switch (Type)
            {
                case ClearQuestType.HP_PERCENT://시작부터 성공
                case ClearQuestType.NO_DIE_PARTNER:
                case ClearQuestType.TIME_LIMIT:
                case ClearQuestType.MINIMUM_HIT:
                    IsClear = true;
                    break;

                default://시작부터 실패
                    IsClear = false;
                    break;
            }
        }

        public string GetTypeString()
        {
            string str = null;
            switch (Type)
            {
                case ClearQuestType.HP_PERCENT:
                    str = string.Format(_LowDataMgr.instance.GetStringStageData(8000), Value);
                    break;
                case ClearQuestType.NO_DIE_PARTNER:
                    str = _LowDataMgr.instance.GetStringStageData(8001);//string.Format("没有伙伴死亡");//,Value
                    break;
                case ClearQuestType.STAGE_CLEAR:
                    str = _LowDataMgr.instance.GetStringStageData(8003);//string.Format("{0}", Type.ToString() );
                    break;
                case ClearQuestType.TIME_LIMIT:
                    str = string.Format(_LowDataMgr.instance.GetStringStageData(8002), Value);
                    break;

                case ClearQuestType.MAX_DAMAGE:
                    str = string.Format(_LowDataMgr.instance.GetStringStageData(8004), Value);
                    break;
                case ClearQuestType.MINIMUM_HIT:
                    str = string.Format(_LowDataMgr.instance.GetStringStageData(8005), Value);
                    break;
                case ClearQuestType.UNEQUIP_PARTNER:
                    str = string.Format(_LowDataMgr.instance.GetStringStageData(8006), Value);
                    break;
            }
             
            return str;
        }
        
        public bool CheckClearQuest(ClearQuestType checkType, float value)
        {
            bool clear = false;
            switch (checkType)
            {
                case ClearQuestType.HP_PERCENT:
                    if (!IsClear)//한번실패는 쭉 실패
                        return false;

                    float par = (float)Value * 0.01f;
                    if (value < par)
                        clear = false;
                    else
                        clear = true;
                    break;

                case ClearQuestType.NO_DIE_PARTNER:
                    if (!IsClear)//한번실패는 쭉 실패
                        return false;

                    if (Value < value)
                        clear = false;
                    else
                        clear = true;
                    break;

                case ClearQuestType.STAGE_CLEAR:
                    if (value != 1)
                        clear = false;
                    else
                        clear = true;

                    break;

                case ClearQuestType.TIME_LIMIT:
                    if (!IsClear)//한번실패는 쭉 실패
                        return false;

                    if (Value < value)
                        clear = false;
                    else
                        clear = true;

                    break;

                case ClearQuestType.UNEQUIP_PARTNER:
                    if (Value < value)//뭔가를 장착했다는 소리겠지
                        clear = false;
                    else
                        clear = true;

                    break;

                case ClearQuestType.MAX_DAMAGE:
                    if (IsClear)//한번 클리어면 계속 클리어상태
                        return true;

                    if (value < Value)//데미지 모자름
                        clear = false;
                    else
                        clear = true;

                    break;

                case ClearQuestType.MINIMUM_HIT:
                    if (!IsClear)//한번실패는 쭉 실패
                        return false;

                    if (Value <= value)//n 회 이상 피격시 실패
                        clear = false;
                    else
                        clear = true;
                    break;
                    
            }

            return clear;
        }

        /// <summary> 조건부 값이 증가하는 타입 </summary>
        public bool CheckCondition(ClearQuestType checkType, int add)
        {
            if (Type != checkType)
                return false;//이녀석은 이 조건이 아님
            else if (Value <= CurValueCount)//이미 조건부가 충족(망했음) 체크하지말자
                return false;//이녀석은 이 조건이 맞으나 이미 충족(망했음)

            CurValueCount += add;
            IsClear = CheckClearQuest(Type, CurValueCount);

            return true;//이녀석은 이 조건이 맞음 결과는 모름
        }

    }

    #endregion
}

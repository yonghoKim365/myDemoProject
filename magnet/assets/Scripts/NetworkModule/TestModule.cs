using UnityEngine;
using System.Collections;
using Sw;

public class TestModule : Immortal<TestModule>
{

	// Use this for initialization
	void Start () {
        //NetworkClient.instance.ConnectServer("52.78.243.250", 7201, (state) =>
        //{
        //    switch (state)
        //    {
        //        case Core.Net.ConnectState.Success:
        //            //연결성공
        //            break;

        //        case Core.Net.ConnectState.Error:
        //            //에러코드
        //            break;

        //        case Core.Net.ConnectState.Close:
        //            //에러코드
        //            break;
        //    }
        //});
    }

    int m_serverID;
    int m_unCode;
    string m_IP;
    int m_port;

    public void SetUnCode(int ServerID, string IP, int port, int unCode)
    {
        m_serverID = ServerID;
        m_unCode = unCode;
        m_IP = IP;
        m_port = port;
    }

    long m_selectID;
    public void SetSelectID(long selectID)
    {
        m_selectID = selectID;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 00, 200, 50), "SendPMsgServerListC"))
        {
            NetworkClient.instance.SendPMsgServerListC();
        }

        if (GUI.Button(new Rect(0, 60, 200, 50), "SendPMsgLoginC"))
        {
            //NetworkClient.instance.SendPMsgLoginC(1, "signedkey", 1);
        }

        if (GUI.Button(new Rect(0, 120, 200, 50), "SendPMsgGameLoginC"))
        {
            NetworkClient.instance.ConnectGameServer(m_IP, m_port, (state) =>
            {
                switch (state)
                {
                    case Core.Net.ConnectState.Success:
                        //연결성공
                        NetworkClient.instance.SendPMsgGameLoginC(1, 1, 65835, m_unCode, 1, 0);
                        break;

                    case Core.Net.ConnectState.Error:
                        //에러코드
                        break;

                    case Core.Net.ConnectState.Close:
                        //에러코드
                        break;
                }
            });
        }

        if(GUI.Button(new Rect(0,180, 200,50), "SendPMsgRoleCreateNewC"))
        {
            NetworkClient.instance.SendPMsgRoleCreateNewC("testUser", 11000, 1);
        }

        if (GUI.Button(new Rect(0, 240, 200, 50), "SendPMsgRoleDeleteC"))
        {
            NetworkClient.instance.SendPMsgRoleDeleteC( m_selectID );
        }

        if (GUI.Button(new Rect(0, 300, 200, 50), "SendPMsgRoleSelectC"))
        {
            NetworkClient.instance.SendPMsgRoleSelectC( m_selectID );
        }
        /*
        if (GUI.Button(new Rect(0, 360, 200, 50), "SendPMsgTalkCS"))
        {
            NetworkClient.instance.SendPMsgTalkCS((int)TALK_CHANNEL_TYPE.TALK_CHANNEL_WORLD, 0, "", "testmessage", 0);
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

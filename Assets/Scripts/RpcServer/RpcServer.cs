

using Plc.Http;
using Plc.WebServerRequest;
using UnityEngine;
using UnityEngine.Networking;
using HotFixDll.ComingFish.ResourcesTool;
namespace Plc.Rpc
{
	[RequireComponent(typeof(ServerGetMsg))]
	[RequireComponent(typeof(ServerSendMsg))]
	public class RpcServer : MonoBehaviour
	{
		private const short userMsg = 64;
		[HideInInspector]
		public static RpcServer Instance;

		[HideInInspector]
		public ServerGetMsg serverGetMsg;
		[HideInInspector]
		public ServerSendMsg serverSendMsg;
		public WebClientManage webClientManage;
		public HttpMgr httpMgr;
		public LogRecord logRecord;
		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Debug.LogError("Rpcserver instance error");
			}
			SetupServer("9000");
			serverGetMsg = GetComponent<ServerGetMsg>();
			serverSendMsg = GetComponent<ServerSendMsg>();
		}

		void OnApplicationQuit()
		{
			ServerUnregisterHandler();
		}

		/// <summary>
		/// 建立服务器
		/// </summary>
		public void SetupServer(string _port)
		{
			if (!NetworkServer.active)
			{
				ShowMsg("setup server");
				ServerRegisterHandler();
				NetworkServer.Listen(int.Parse(_port));

				if (NetworkServer.active)
				{
					ShowMsg("Server setup ok.");
				}
			}
		}

		/// <summary>
		/// 停止服务器端
		/// </summary>
		public void ShutdownServer()
		{
			if (NetworkServer.active)
			{
				ServerUnregisterHandler();
				NetworkServer.DisconnectAll();
				NetworkServer.Shutdown();

				if (!NetworkServer.active)
				{
					ShowMsg("shut down server");
				}
			}
		}


		/// <summary>
		/// 服务器端有客户端连入事件
		/// </summary>
		/// <param name="netMsg">Net message.</param>
		private void OnServerConnected(NetworkMessage netMsg)
		{
			ShowMsg("One client connected to server" + netMsg.conn.address);
		}

		/// <summary>
		/// 服务器端有客户端断开事件
		/// </summary>
		/// <param name="netMsg">Net message.</param>
		private void OnServerDisconnected(NetworkMessage netMsg)
		{
			ShowMsg("One client connected from server");
			serverGetMsg.ServerDeletePort(netMsg);
		}

		/// <summary>
		/// 服务器端错误事件
		/// </summary>
		/// <param name="netMsg">Net message.</param>
		private void OnServerError(NetworkMessage netMsg)
		{
			ServerUnregisterHandler();
			ShowMsg("Server error");
		}

		/// <summary>
		/// 显示信息
		/// </summary>
		/// <param name="Msg">Message.</param>
		private void ShowMsg(string Msg)
		{
			//info.text = Msg + "\n\r" + info.text;
			Debug.Log(Msg);
		}

		/// <summary>
		/// server send msg to all type client 
		/// </summary>
		/// <param name="_plcMsg">msg body</param>
		public void SendToAllClient(PLCMsg _plcMsg)
		{
			if (NetworkServer.active)
			{
				NetworkServer.SendToAll(userMsg, _plcMsg);
			}
		}

		/// <summary>
		/// server send msg to this id client
		/// </summary>
		/// <param name="id">_netMsg.conn.connectionId </param>
		/// <param name="_plcMsg">msg body</param>
		public void SendToClientById(int id, PLCMsg _plcMsg)
		{
			if (NetworkServer.active)
			{
				NetworkServer.SendToClient(id, userMsg, _plcMsg);
			}
		}

		/// <summary>
		/// 服务器端收到信息事件
		/// </summary>
		/// <param name="netMsg">Net message.</param>
		private void ServerGet(NetworkMessage netMsg)
		{

			serverGetMsg.RpcServerGetMsg(netMsg);
		}

		/// <summary>
		/// 服务器端注册事件
		/// </summary>
		private void ServerRegisterHandler()
		{
			NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnected);
			NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnected);
			NetworkServer.RegisterHandler(MsgType.Error, OnServerError);
			NetworkServer.RegisterHandler(userMsg, ServerGet);
		}

		/// <summary>
		/// 服务器端注销事件
		/// </summary>
		private void ServerUnregisterHandler()
		{
			NetworkServer.UnregisterHandler(MsgType.Connect);
			NetworkServer.UnregisterHandler(MsgType.Disconnect);
			NetworkServer.UnregisterHandler(MsgType.Error);
			NetworkServer.UnregisterHandler(userMsg);
		}
	}
}
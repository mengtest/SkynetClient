﻿using Skynet.DotNetClient;
using Skynet.DotNetClient.Utils.Signals;
using UnityEngine;


public class TestSkynetClient : MonoBehaviour 
{
	private TestLoginTcp _login;
	private const string Protocol = "tcp"; // "ws"; //"tcp";

	private TestGateTcp _gateTcp;
	private TestGateWs _gateWs;
	private TestGateUdp _gateUdp;

	public void Start () 
	{
		SkynetLogger.Error(Channel.NetDevice, "++++++Skynet Client Start++++");

		_login = new TestLoginTcp (Protocol);
		_login.Run(ProcessLoginResp);
	}

	private void ProcessLoginResp(int code, AuthPackageResp resp)
	{
		_login.DisConnect();

		if(code == 200)
		{
			if (Protocol == "tcp")
			{
				_gateTcp = new TestGateTcp();
				_gateTcp.Run(resp);
			}
			else
			{
				_gateWs = new TestGateWs();
				_gateWs.Run(resp);
			}
			
			Signals.Get<UdpSignal>().AddListener(SignalUdp);
		}

		_login = null;
	}

	private void SignalUdp(BattleSession session)
	{
		_gateUdp = new TestGateUdp();
		_gateUdp.Run(session);
	}
	
	private void OnDestroy()
	{
		if (_login != null)
		{
			_login.DisConnect();
			_login = null;
		}

		if (_gateTcp != null)
		{
			_gateTcp.DisConnect();
			_gateTcp = null;
		}

		if (_gateWs != null)
		{
			_gateWs.DisConnect();
			_gateWs = null;
		}

		if (_gateUdp != null)
		{
			_gateUdp.DisConnect();
			_gateUdp = null;
		}
		
		SkynetLogger.Error(Channel.NetDevice, "++++++Skynet Client Destroy++++");
	}
}

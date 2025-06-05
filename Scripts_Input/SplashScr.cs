
using System.Diagnostics;

public class SplashScr : mScreen
{
	public static int splashScrStat;

	private bool isCheckConnect;

	private bool isSwitchToLogin;

	public static int nData = -1;

	public static int maxData = -1;

	public static SplashScr instance;

	public static Image imgLogo;

	public SplashScr()
	{
		instance = this;
	}

	public static void loadSplashScr()
	{
		splashScrStat = 0;
	}

	public override void update()
	{
		if (splashScrStat == 30 && !isCheckConnect)
		{
			UnityEngine.Debug.Log( splashScrStat );
            UnityEngine.Debug.Log(isCheckConnect);
            isCheckConnect = true;
			if (Rms.loadRmsInt("serverchat") != -1)
			{
				GameScr.isPaintChatVip = ((Rms.loadRmsInt("serverchat") == 0) ? true : false);
			}
			if (Rms.loadRmsInt("isPlaySound") != -1)
			{
				GameCanvas.isPlaySound = Rms.loadRmsInt("isPlaySound") == 1;
			}
			if (GameCanvas.isPlaySound)
			{
				SoundMn.gI().loadSound(TileMap.mapID);
			}
			SoundMn.gI().getStrOption();
			ServerListScreen.loadIP();
		}
		splashScrStat++;
		ServerListScreen.updateDeleteData();
	}

	public static void loadIP()
	{
		if (Rms.loadRmsInt("svselect") == -1)
		{
			int num = 0;
			if (mResources.language > 0)
			{
				for (int i = 0; i < mResources.language; i++)
				{
					num += ServerListScreen.lengthServer[i];
				}
			}
			if (ServerListScreen.serverPriority == -1)
			{
				ServerListScreen.ipSelect = num + Res.random(0, ServerListScreen.lengthServer[mResources.language]);
			}
			else
			{
				ServerListScreen.ipSelect = ServerListScreen.serverPriority;
			}
			Rms.saveRmsInt("svselect", ServerListScreen.ipSelect);
			GameMidlet.IP = ServerListScreen.address[ServerListScreen.ipSelect];
			GameMidlet.PORT = ServerListScreen.port[ServerListScreen.ipSelect];
			mResources.loadLanguague(ServerListScreen.language[ServerListScreen.ipSelect]);
			LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
			GameCanvas.connect();
		}
		else
		{
			ServerListScreen.ipSelect = Rms.loadRmsInt("svselect");
			if (ServerListScreen.ipSelect > ServerListScreen.nameServer.Length - 1)
			{
				ServerListScreen.ipSelect = ServerListScreen.serverPriority;
				Rms.saveRmsInt("svselect", ServerListScreen.ipSelect);
			}
			GameMidlet.IP = ServerListScreen.address[ServerListScreen.ipSelect];
			GameMidlet.PORT = ServerListScreen.port[ServerListScreen.ipSelect];
			mResources.loadLanguague(ServerListScreen.language[ServerListScreen.ipSelect]);
			LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
			GameCanvas.connect();
		}
	}

	public override void paint(mGraphics g)
	{
		if (nData != -1)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.gI().paintLogo(g,GameCanvas.w / 2, GameCanvas.h / 2 - 24);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.h / 2 + 24, g);
			mFont.tahoma_7b_white.drawString(g, mResources.downloading_data + nData * 100 / maxData + "%", GameCanvas.w / 2, GameCanvas.h / 2, 2);
		}
		else if (splashScrStat >= 30)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.hh, g);
			if (ServerListScreen.cmdDeleteRms != null)
			{
				mFont.tahoma_7_white.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
			}
		}
	}

	public static void loadImg()
	{
		
	}
}

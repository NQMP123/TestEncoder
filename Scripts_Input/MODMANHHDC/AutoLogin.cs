using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MODMANHHDC
{
    public class AutoLogin
    {
		public static bool IsLogin;
        public static string acc;
        public static string pass;
        public static int server = -1;

		public static IEnumerator loadAccount()
		{
			if (GameCanvas.loginScr == null)
			{
				GameCanvas.loginScr = new LoginScr();
			}

			acc = acc != null && acc != string.Empty ? acc : GameCanvas.loginScr.tfUser.getText();
			pass = pass != null && pass != string.Empty ? pass : GameCanvas.loginScr.tfPass.getText();
			server = server != -1 ? server : ServerListScreen.ipSelect;

			if (acc != string.Empty && pass != string.Empty && server != -1)
			{
				yield return new WaitForSecondsRealtime(2);
				if (GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen)
				{

					if (server != ServerListScreen.ipSelect)
					{
						GameCanvas.serverScr.perform(100 + server, null);
                        yield return new WaitForSecondsRealtime(0.5f);
					}

					if (Session_ME.gI().isConnected() == false)
						GameCanvas.connect();

					if (GameCanvas.loginScr == null)
					{
						GameCanvas.loginScr = new LoginScr();
					}
					GameCanvas.loginScr.switchToMe();

					Service.gI().login(acc, pass, GameMidlet.VERSION, (sbyte)0);
					IsLogin = false;
				}
			}
			else
			{
				if ((GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen) &&
					Rms.loadRmsString("acc") != string.Empty)
				{
					GameCanvas.serverScreen.perform(3, null);
				}
			}
		}
	}
}

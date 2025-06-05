using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MODMANHHDC
{
    public class QLTKPanel : mScreen, IActionListener
    {
        public static myCommand qltkCmd;

        static QLTKPanel()
        {
            qltkCmd = new myCommand("Auto Manager", GameCanvas.serverScreen, 999, null);
            qltkCmd.w = 76;
            qltkCmd.x = 5; ;
            qltkCmd.y = GameCanvas.h - qltkCmd.h - 5;
        }

        private bool isInputAccount;
        private bool isSelectSv;
        private bool isEditAccount;
        private Scroll scroll;

        private int w;
        private int h;
        private int x;
        private int y;
        private int itemH = 30;
        public int bColor = 15115520;
        public List<Account> accounts = new List<Account>();
        private TField userName;
        private TField password;
        private myCommand back;
        private myCommand ok;
        public myCommand server;
        private myCommand btnAdd, btnEdit, btnBack;
        private int mainSelect = 0;
        private int numw, numh;
        private MyVector vecServer = new MyVector();
        private int accountSelect = -1;
        private WaitTimer wait = new WaitTimer();

        private static QLTKPanel instance;
        public static QLTKPanel gI()
        {
            if (instance == null)
            {
                instance = new QLTKPanel();
            }
            return instance;
        }
        public QLTKPanel()
        {
            scroll = new Scroll();

            userName = new TField();
            userName.setIputType(TField.INPUT_TYPE_ANY);
            userName.name = "Tài Khoản";
            userName.width = 160;
            userName.height = mScreen.ITEM_HEIGHT + 2;

            password = new TField();
            password.setIputType(TField.INPUT_TYPE_PASSWORD);
            password.name = "Mật Khẩu";
            password.width = 160;
            password.height = mScreen.ITEM_HEIGHT + 2;

            back = new myCommand("Trở Lại", this, 1, null);
            back.w = 76;

            ok = new myCommand("OK", this, 2, null);
            ok.w = 76;

            server = new myCommand("Vui Lòng Chọn Máy Chủ", this, 3, "");
            server.setType();

            this.w = GameCanvas.w - 50;
            this.h = GameCanvas.h - 70;
            this.x = (GameCanvas.w - this.w) / 2;
            this.y = GameCanvas.hh - this.h / 2;
            load();
            scroll.setStyle(accounts.Count, itemH, x, y, this.w, this.h, true, 1);

            btnAdd = new myCommand("Thêm", this, 5, null);
            btnAdd.w = 76;
            btnAdd.x = x;
            btnAdd.y = GameCanvas.h - btnAdd.h;

            btnEdit = new myCommand("Sửa", this, 6, null);
            btnEdit.w = 76;
            btnEdit.x = GameCanvas.hw - btnEdit.w / 2;
            btnEdit.y = GameCanvas.h - btnAdd.h;

            btnBack = new myCommand("Trở Lại", this, 7, null);
            btnBack.w = 76;
            btnBack.x = this.x + this.w - btnEdit.w;
            btnBack.y = GameCanvas.h - btnAdd.h;
        }
        public override void paint(mGraphics g)
        {
            GameCanvas.paintBGGameScr(g);
            g.translate(-g.getTranslateX(), -g.getTranslateY());

            if (isSelectSv)
            {
                for (int i = 0; i < vecServer.size(); i++)
                {
                    if (vecServer.elementAt(i) != null)
                    {
                        ((myCommand)vecServer.elementAt(i)).paint(g);
                    }
                }
                back.x = btnBack.x;
                back.y = btnBack.y;
                back.paint(g);
            }else if (isInputAccount)
                paintInputAccount(g);
            else 
                paintListAccount(g);
            
            base.paint(g);
        }
        private int paintButton(mGraphics g, string text, int x, int y, int size, int alignText, int alignBtn = 0, bool isFocus = false)
        {
            Image[] btns = isFocus ? myCommand.btnsF : myCommand.btns;

            int w = (int)btns[1].getWidth();
            
            int maxS = Mathf.Max(size, w);
            int num = maxS % w;
            int widthbtn = w * 2 + num + Mathf.Clamp(maxS - w * 2, 0, Mathf.Abs(maxS - w * 2));

            if(alignBtn == mFont.RIGHT)
            {
                x -= widthbtn;
            }else if(alignBtn == mFont.CENTER)
            {
                x -= widthbtn / 2;
            }

            g.drawImage(btns[0], x, y, 0);

            for (int i = w; i < maxS - w; i += w)
            {
                g.drawImage(btns[1], x + i, y, 0);
            }

            
            if(num > 0)
            {
                g.drawRegion(btns[1], 0, 0, num, btns[1].getHeight(), 0, x + maxS - num, y, 0);
            }

            g.drawImage(btns[2], x + maxS, y, 0);

            int dx = alignText == mFont.CENTER ? widthbtn / 2 : 10;
            mFont.tahoma_7b_white.drawString(g, text, x + dx, y + 6, alignText);
            return widthbtn;
        }
        private void paintListAccount(mGraphics g)
        {
            PopUp.paintPopUp(g, x, y - 5, w, h + 5, -1, true);
            g.translate(x, y);
            g.setClip(0, 0, this.w, this.h);
            g.translate(scroll.cmx, -scroll.cmy);
            for (int i = 0; i < accounts.Count; i++)
            {
                int itemH = i * this.itemH;
                int size = w * 4 / 8;
                string nameSv = accounts[i].ipserver.Split(":")[0];
                int w1 = paintButton(g, accounts[i].username + " - " + nameSv, 12, itemH, size, mFont.LEFT, isFocus: accountSelect == i);

                paintButton(g, "Xóa", w - 140, itemH, 32, mFont.CENTER);

                paintButton(g, "Đăng Nhập", w - 75, itemH, 32, mFont.CENTER);
            }
            g.reset();

            btnAdd.paint(g);
            btnEdit.paint(g);
            btnBack.paint(g);
        }

        private void paintInputAccount(mGraphics g)
        {
            int h = 170;
            int w = 250;
            int x = GameCanvas.w / 2 - w / 2;
            int y = GameCanvas.hh - h / 2;
            PopUp.paintPopUp(g, x, y, w, h, -1, true);
            
            userName.x = GameCanvas.w / 2 - userName.width / 2;
            userName.y = y + 50;

            
            password.x = GameCanvas.w / 2 - password.width / 2;
            password.y = userName.y + userName.height + 10;
            

            server.x = userName.x;
            server.y = password.y + password.height + 10;

            back.x = userName.x + userName.width - back.w;
            back.y = y + h - 30;

            ok.x = userName.x;
            ok.y = y + h - 30;

            userName.paint(g);
            password.paint(g);
            GameCanvas.resetTrans(g);
            back.paint(g);
            ok.paint(g);
            server.paint(g);
        }

        public override void update()
        {

            GameScr.cmx++;
            if (GameScr.cmx > GameCanvas.w * 3 + 100)
            {
                GameScr.cmx = 100;
            }
            if (isSelectSv)
            {
                for (int i = 0; i < vecServer.size(); i++)
                {
                    Command command = (Command)vecServer.elementAt(i);
                    if (command != null && command.isPointerPressInside())
                    {
                        command.performAction();
                    }
                }

                return;
            }

            if (isInputAccount)
            {
                userName.update();
                password.update();
                return;
            }

            scroll.updatecm();
            
            base.update();
        }

        public override void updateKey()
        {
            if (isSelectSv)
            {
                if (back.isPointerPressInside())
                    isSelectSv = false;
                updateSelectSv();
                
                return;
            }

            if (isInputAccount)
            {
                if (GameCanvas.keyPressed[Key.FIRE])
                {
                    GameCanvas.clearKeyPressed();
                    if (userName.isFocus)
                    {
                        userName.setFocus(false);
                        password.setFocusWithKb(true);
                    }
                    else if (password.isFocus)
                    {
                        password.setFocus(false);
                        userName.setFocusWithKb(true);
                    }
                }

                if (back.isPointerPressInside())
                    back.performAction();
                else if (ok.isPointerPressInside())
                    ok.performAction();
                else if (server.isPointerPressInside())
                    server.performAction();
                GameCanvas.clearKeyPressed();
                return;
            }

            if (btnAdd.isPointerPressInside())
                btnAdd.performAction();

            if (btnEdit.isPointerPressInside())
                btnEdit.performAction();

            if (btnBack.isPointerPressInside())
                btnBack.performAction();

            if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerClick && !GameCanvas.isPointerMove)
            {
                accountSelect = (scroll.cmtoY + GameCanvas.py - y) / itemH;
                if (accountSelect >= accounts.Count)
                {
                    accountSelect = -1;
                }
                if(accountSelect != -1)
                {
                    if (GameCanvas.isPointerHoldIn(this.x + w - 140, this.y + accountSelect * itemH - scroll.cmy, 64, 24))
                    {
                        GameCanvas.clearAllPointerEvent();
                        SoundMn.gI().buttonClick();
                        accounts.RemoveAt(accountSelect);
                        save();
                        return;
                    }
                    if (GameCanvas.isPointerHoldIn(this.x + w - 75, this.y + accountSelect * itemH - scroll.cmy, 64, 24))
                    {
                        GameCanvas.clearAllPointerEvent();
                        SoundMn.gI().buttonClick();
                        Account itemSelect = accounts[accountSelect];
                        AutoLogin.acc = itemSelect.username;
                        AutoLogin.pass = itemSelect.password;
                        AutoLogin.server = 0;
                        this.perform(8, null);
                        return;
                    }
                }
                return;
            }


            

            if (scroll.cmyLim > 0)
            {
                ScrollResult result = scroll.updateKey();
                if (scroll.cmy < 0 && result.isDowning == false)
                {

                    scroll.cmy -= scroll.cmy / 2;
                }
                else if (scroll.cmy > scroll.cmyLim && result.isDowning == false)
                {
                    scroll.cmy -= (scroll.cmy - scroll.cmyLim + 6) / 2;
                }
            }

            GameCanvas.clearKeyPressed();
        }

        public override void keyPress(int keyCode)
        {
            if (userName.isFocus)
                userName.keyPressed(keyCode);
            if (password.isFocus)
                password.keyPressed(keyCode);
        }

        private void updateSelectSv()
        {
            int num = mainSelect % numw;
            int num2 = mainSelect / numw;
            if (GameCanvas.keyPressed[4])
            {
                if (num > 0)
                {
                    mainSelect--;
                }
                GameCanvas.keyPressed[4] = false;
            }
            else if (GameCanvas.keyPressed[6])
            {
                if (num < numw - 1)
                {
                    mainSelect++;
                }
                GameCanvas.keyPressed[6] = false;
            }
            else if (GameCanvas.keyPressed[2])
            {
                if (num2 > 0)
                {
                    mainSelect -= numw;
                }
                GameCanvas.keyPressed[2] = false;
            }
            else if (GameCanvas.keyPressed[8])
            {
                if (num2 < numh - 1)
                {
                    mainSelect += numw;
                }
                GameCanvas.keyPressed[8] = false;
            }
            if (mainSelect < 0)
            {
                mainSelect = 0;
            }
            if (mainSelect >= vecServer.size())
            {
                mainSelect = vecServer.size() - 1;
            }
            if (GameCanvas.keyPressed[5])
            {
                ((Command)vecServer.elementAt(num)).performAction();
                GameCanvas.keyPressed[5] = false;
            }
            GameCanvas.clearKeyPressed();
        }

        public override void switchToMe()
        {
            SoundMn.gI().stopAll();
            

            vecServer.removeAllElements();
            for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
            {
                string ip = ServerListScreen.nameServer[i] + ":" + ServerListScreen.address[i] + ":" + ServerListScreen.port[i] + ":0,0,0";
                myCommand command = new myCommand(ServerListScreen.nameServer[i], this, 100 + i, ip);
                command.w = 76;
                vecServer.addElement(command);
            }
            sort();
            base.switchToMe();
        }

        private void sort()
        {
            mainSelect = 0;
            string sv = (string)server.p;
            for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
            {
                if (ServerListScreen.nameServer[i] != null && sv.StartsWith(ServerListScreen.nameServer[i]))
                {
                    mainSelect = i;
                    break;
                }
            }

            int w2c = 5;
            int wc = 76;
            int hc = mScreen.cmdH;
            numw = 2;
            if (GameCanvas.w > 3 * (wc + w2c))
            {
                numw = 3;
            }
            if (vecServer.size() < 3)
            {
                numw = 2;
            }
            numh = vecServer.size() / numw + ((vecServer.size() % numw != 0) ? 1 : 0);
            for (int i = 0; i < vecServer.size(); i++)
            {
                Command command = (Command)vecServer.elementAt(i);
                if (command != null)
                {
                    int num = GameCanvas.hw - numw * (wc + w2c) / 2;
                    int x = num + i % numw * (wc + w2c);
                    int num2 = GameCanvas.hh - numh * (hc + w2c) / 2;
                    int y = num2 + i / numw * (hc + w2c);
                    command.x = x;
                    command.y = y;
                }
            }
        }

        private void save()
        {
            try
            {
                
                JArray jar = new JArray();
                for (int i = 0; i < accounts.Count; i++)
                {
                    Account acc = accounts[i];
                    JObject job = new JObject();
                    job.Add("id", acc.id);
                    job.Add("username", acc.username);
                    job.Add("password", acc.password);
                    job.Add("ip", acc.ipserver);
                    jar.Add(job);
                }
                Rms.saveRmsString("listAccount", jar.ToString());
            }
            catch (Exception e)
            {

                Debug.LogError("err save acc " + e.Message);
            }
        }

        private void load()
        {
            try
            {
                accounts.Clear();
                string s = Rms.loadRmsString("listAccount") != null ? Rms.loadRmsString("listAccount") : "[]";

                JArray jar = JArray.Parse(s);

                for (int i = 0; i < jar.Count; i++)
                {
                    JObject job = (JObject)jar[i];
                    accounts.Add(new Account(job.Value<int>("id"), job.Value<string>("username"), job.Value<string>("password"), job.Value<string>("ip")));
                }

                //for (int i = 0; i < 20; i++)
                //{
                //    accounts.Add(new Account(i, "Account " + i, "a", "Server1:1:1:0,0,0"));
                //}
            }
            catch (Exception e)
            {

                Debug.LogException(e);
            }
        }

        public void perform(int idAction, object p)
        {
            if(idAction == 1)
            {
                isEditAccount = false;
                isInputAccount = false;
            }
            else if(idAction == 2)
            {
                isInputAccount = false;

                try
                {
                    if (isEditAccount)
                    {
                        if (accountSelect >= 0)
                        {
                            accounts[accountSelect].username = userName.getText();
                            accounts[accountSelect].password = password.getText();

                            accounts[accountSelect].ipserver = (string)server.p;
                        }
                        isEditAccount = false;
                    }
                    else
                    {
                        int id = accounts.Count == 0 ? 0 : accounts[accounts.Count - 1].id + 1;

                        string ip = (string)server.p;
                        Debug.Log("ip =>>>>>>>>>>" + ip);
                        accounts.Add(new Account(id, userName.getText(), password.getText(), ip));
                    }
                    save();
                    Debug.Log("okokok");
                    scroll.clear();
                    scroll.setStyle(accounts.Count, itemH, x, y , this.w, this.h, true, 1);

                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else if(idAction == 3)
            {
                isSelectSv = true;
            }
            else if(idAction >= 100)
            {
                string sv = (string)p;
                Debug.Log(sv);
                server.caption = sv.Split(":")[0];
                server.p = sv;
                isSelectSv = false;
            }
            if(idAction == 5)
            {
                isInputAccount = true;
            }
            if(idAction == 6)
            {
                if(accountSelect >= 0)
                {
                    Account account = accounts[accountSelect];
                    isInputAccount = true;
                    isEditAccount = true;
                    userName.setText(account.username);
                    password.setText(account.password);
                    server.caption = account.ipserver.Split(':')[0];
                    server.p = account.ipserver;
                }
            }
            if(idAction == 7)
            {
                GameCanvas.serverScreen.switchToMe();
            }
            if(idAction == 8)
            {   
                if(accountSelect >= 0)
                {
                    AutoLogin.IsLogin = true;
                    Session_ME.gI().close();
                    ServerListScreen.getServerList(accounts[accountSelect].ipserver);
                    Rms.saveRmsInt("svselect", 0);
                    ServerListScreen.ipSelect = 0;
                    GameCanvas.connect();
                    Main.main.StartCoroutine(AutoLogin.loadAccount());
                }
            }
        }
    }
}

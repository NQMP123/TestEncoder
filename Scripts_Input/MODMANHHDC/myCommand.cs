using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MODMANHHDC
{
    public class myCommand : Command
    {
        public static Image[] btns, btnsF;

        static myCommand()
        {
            Texture2D texture = Resources.Load<Texture2D>("btn1");
            int h = 24 * mGraphics.zoomLevel;
            int w = h * texture.width / texture.height;
            texture = Resize(texture, w, h);
            Image btnMain = Image.createImage(texture.EncodeToPNG());

            btns = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                btns[i] = Image.createImage(btnMain, i * (w / 3), 0, w / 3, h, 0);
            }
            texture = Resources.Load<Texture2D>("btn2");
            texture = Resize(texture, w, h);
            btnMain = Image.createImage(texture.EncodeToPNG());
            btnsF = new Image[3];
            for (int i = 0; i < 3; i++)
            {
                btnsF[i] = Image.createImage(btnMain, i * (w / 3), 0, w / 3, h, 0);
            }
        }
        public static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }

        public myCommand(string caption, IActionListener actionListener, int action, object p) 
            : base(caption, actionListener, action, p)
        {
        }

        public void paint(mGraphics g)
        {
            if (caption != string.Empty)
            {
                if (!isFocus)
                {
                    paintBtn(btns[0], btns[1], btns[2], x, y, w, g);
                }
                else
                {
                    paintBtn(btnsF[0], btnsF[1], btnsF[2], x, y, w, g);
                }
            }
            int num = ((type != 1) ? (x + 38) : (x + hw));
            if (!isFocus)
            {
                mFont.tahoma_7b_dark.drawString(g, caption, num, y + 7, 2);
            }
            else
            {
                mFont.tahoma_7b_green2.drawString(g, caption, num, y + 7, 2);
            }
        }

        public void paintBtn(Image img0, Image img1, Image img2, int x, int y, int size, mGraphics g)
        {
            for (int i = (int)img0.getWidth(); i <= size - img0.getWidth() - img2.getWidth(); i += (int)img1.getWidth())
            {
                g.drawImage(img1, x + i, y, 0);
            }
            int num = size % (int)img1.getWidth();
            if (num > 0)
            {
                g.drawRegion(img1, 0, 0, num, 24, 0, x + size - (int)img1.getWidth() - num, y, 0);
            }
            g.drawImage(img0, x, y, 0);
            g.drawImage(img2, x + size - img2.getWidth(), y, 0);
        }
    }
}

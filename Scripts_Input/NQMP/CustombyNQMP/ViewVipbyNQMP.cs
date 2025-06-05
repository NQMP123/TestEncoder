public class ViewVipbyNQMP 
{
    public static bool vip = true;
    public static Image imgVip = GameCanvas.loadImage("/mainImage/imgvip.png");
    static int w = GameCanvas.w;
    static int h = GameCanvas.h;
    public static Command cmd = new Command("", (int)(w * 0.93), (int)(h * 0.125));
    
    public static void update()
    {
        //if (cmd.img != imgVip)
        //{
        //    cmd.img = imgVip;
        //}
        //if (cmd.isPointerPressInside()) 
        //{
        //    Service.gI().sendVip();
            
        //    GameCanvas.clearAllPointerEvent();
        //}
    }
   
    public static void paint(mGraphics g)
    {
        //if (!vip)
        //{
        //    return;
        //}
        //if(imgVip == null) 
        //{
        //    return;
        //}
        //cmd.paint(g);   
    }
}
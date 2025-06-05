public class MainCustom
{
    public static void update()
    {
        ViewBossbyNQMP.update();
        ViewVipbyNQMP.update();
        TamBao.update();
    }
    public static void paint(mGraphics g)
    {
        ViewBossbyNQMP.paint(g);
        ViewVipbyNQMP.paint(g);
        TamBao.paint(g);
    }
}

public class MovePoint
{
	public int xEnd;

	public int yEnd;

	public int dir;

	public int cvx;

	public int cvy;

	public int status;

	public MovePoint(int xEnd, int yEnd, int act, int dir)
	{
		this.xEnd = xEnd;
		this.yEnd = yEnd;
		this.dir = dir;
		status = act;
	}
    public MovePoint() { }

    public MovePoint(int xEnd, int yEnd)
	{
		this.xEnd = xEnd;
		this.yEnd = yEnd;
	}
    // ctor tiện dụng (gọi lại ctor mặc định)
    public MovePoint(int x, int y, sbyte status, sbyte dir) : this()
    {
        Init(x, y, status, dir);
    }

    // hàm khởi tạo lại cho ObjectPool
    public void Init(int x, int y, sbyte status, sbyte dir)
    {
        xEnd = x; yEnd = y;
        this.status = status; this.dir = dir;
    }
}

public class Account {
    public int id;
    public string username;
    public string password;
    public string ipserver;
    public Account(int id, string u, string p, string ipServer)
    {
        this.id = id;
        this.username = u;
        this.password = p;
        this.ipserver = ipServer;
    }
}

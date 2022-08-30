
public class User
{
    public string Nickname;
    public string UserID;
    public bool Online;

    public User(string name, string ID, bool online)
    {
        this.Nickname = name;
        this.UserID = ID;
        this.Online = online;
    }


}

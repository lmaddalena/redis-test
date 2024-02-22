namespace RedisTest;

internal class Member
{
    public string Username { get; set; }
    public string Name  { get; set; }   

    public string GetKey()
    {
        return "_" + this.Username + "_";
    }
}
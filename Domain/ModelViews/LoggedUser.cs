namespace Identity.Domain.ModelViews;

public struct LoggedUser
{
    public SimpleUser User { get;set;}
    public string Token {get;set;}
}
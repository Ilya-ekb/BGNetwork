using System.Net.Http;
using Core.ObjectsSystem;

public class GamepadClient : BaseDroppable
{
    private static HttpClient client;
    public GamepadClient()
    {
        client = new HttpClient();
    }
}
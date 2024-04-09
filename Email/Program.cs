namespace Email;

public class Program
{ 
    public static void Main()
    {
        var receiver = new MessageReceiver();
        receiver.ReceiveMessage();
        
        Console.WriteLine("Press 'q' to quit.");
        while (Console.Read() != 'q')
        {
        }
    }
}


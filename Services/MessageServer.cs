using MessTals.Data;
using Microsoft.EntityFrameworkCore;

namespace MessTals.Services;

public class MessageServer
{
    public Dictionary<string, Action<Message>> ConnectedUsers = new ();
    private readonly IDbContextFactory<AppDbContext> _factory;

    public MessageServer(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    private async Task<User> CreateNew(string name)
    {
        await using AppDbContext context = await _factory.CreateDbContextAsync();
        User user = new User()
        {
            Name = name,
            Messages = new List<Message>()
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private Task Notify(Message message)
    {
        try
        {
            if (ConnectedUsers.ContainsKey(message.Receiver.Name)) 
                ConnectedUsers[message.Receiver.Name].Invoke(message);
        }
        catch (NullReferenceException)
        {
            ConnectedUsers.Remove(message.Receiver.Name);
        }
        finally
        {
            ConnectedUsers[message.Sender].Invoke(message);
        }

        return Task.CompletedTask;
    }

    private bool NameCompare(string name1, string name2) =>
        string.Equals(name1, name2, StringComparison.CurrentCultureIgnoreCase);
    
    public async Task SendMessage(string sender, string receiver, string content)
    {
        await using AppDbContext db = await _factory.CreateDbContextAsync();
        User _receiver = (await db.Users.ToListAsync())
                         .FirstOrDefault(user => NameCompare(user.Name, receiver))
                         ?? await CreateNew(receiver);
        Message message = new Message()
        {
            Content = content,
            Sender = sender,
            Receiver = _receiver,
            Posted = DateTime.Now
        };
        
        await db.Messages.AddAsync(message);
        await db.SaveChangesAsync();
        await Notify(message);
    }
    
    public async Task<User> ConnectUser(Action<Message> update, string name)
    {
        await using AppDbContext context = await _factory.CreateDbContextAsync();
        User user = (await context.Users.Include(u => u.Messages)
                        .ToListAsync()).FirstOrDefault(u => NameCompare(u.Name, name)) 
                    ?? await CreateNew(name);
        user.Messages.AddRange(await context.Messages.Where(message => message.Sender == user.Name).ToListAsync());
        if (!ConnectedUsers.ContainsKey(name)) ConnectedUsers.Add(name, update);
        return user;
    }

    public async Task<List<User>> GetUsers()
    {
        await using AppDbContext context = await _factory.CreateDbContextAsync();
        return await context.Users.ToListAsync();
    }
}
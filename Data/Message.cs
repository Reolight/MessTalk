using System.ComponentModel.DataAnnotations;

namespace MessTals.Data;

public class Message
{
    [Required, Key] public int Id { get; set; }
    [Required] public string Content { get; set; }
    public DateTime Posted { get; set; }
    public string Sender { get; set; }
    public User Receiver { get; set; }
}
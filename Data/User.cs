using System.ComponentModel.DataAnnotations;

namespace MessTals.Data;

public class User
{
    [Required, Key] public string Name { get; set; }
    public List<Message> Messages { get; set; }
}
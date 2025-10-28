namespace TodoTimeLineApp_Backend.NET.Models;

public class Todo
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCurrent { get; set; }
}

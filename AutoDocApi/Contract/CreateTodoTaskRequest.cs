namespace AutoDocApi.Contract;

public class CreateTodoTaskRequest
{
    public string Title { get; set; }= default!;
    public DateTime DueDate { get; set; }
}

public class UpdateTodoTaskRequest
{
    public string Title { get; set; } = default!;
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = default!;
}
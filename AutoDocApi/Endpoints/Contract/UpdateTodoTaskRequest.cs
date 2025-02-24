namespace AutoDocApi.Contract;

public record struct UpdateTodoTaskRequest(string Title, DateTime DueDate, string Status);

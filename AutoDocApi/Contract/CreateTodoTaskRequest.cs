namespace AutoDocApi.Contract;

public record struct CreateTodoTaskRequest(string Title, DateTime DueDate);

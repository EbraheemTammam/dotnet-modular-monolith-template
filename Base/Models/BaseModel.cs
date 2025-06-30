namespace Base.Models;

public abstract class BaseModel
{
    public Guid Id { get; protected set; }
}

public abstract class LoggedBaseModel : BaseModel
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

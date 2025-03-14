namespace BxHelpers.Models.Events;

public class BaseEvent
{
    public string Hash { get; set; }
    
    public BaseEvent()
    {
        Hash = BxHash.Generate();
    }

    public bool Validate()
    {
        return BxHash.Validate(Hash);
    }
}
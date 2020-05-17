using System.Collections;

public interface IEventHandler
{
    bool IsEnabled { get; set; }
    void OnEvent(IBaseEvent evt);
}

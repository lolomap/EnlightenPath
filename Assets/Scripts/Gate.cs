using Events;
using Zenject;

public class Gate : Pit
{
    [Inject] private EventBus _eventBus;
    
    protected override void OnFall(Player player)
    {
        _eventBus.Win.RaiseEvent();
    }
}
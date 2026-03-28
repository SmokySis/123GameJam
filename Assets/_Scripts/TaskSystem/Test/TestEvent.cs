using EventSystem;

namespace TaskSystem.Test
{
    public readonly struct GameStartedEvent : IGameEvent
    {
    }
    public readonly struct GameEndEvent : IGameEvent { }
    public readonly struct APPOpenEvent : IGameEvent { }
}
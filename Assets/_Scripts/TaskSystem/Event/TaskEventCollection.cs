using EventSystem;

namespace TaskSystem.Event
{
    public readonly struct FirstTaskEvent : IGameEvent { }
    public readonly struct AskGPTAndFinishHomeWorkEvent : IGameEvent { }
    public readonly struct MakePPTFinishEvent : IGameEvent { }
    public readonly struct MakeWordProposalFinishEvent : IGameEvent { }
    public readonly struct StartLabReportEvent : IGameEvent { }
    public readonly struct EndLabReportEvent : IGameEvent { }
    public readonly struct StartResumeEvent : IGameEvent { }
    public readonly struct EndResumeEvent : IGameEvent { }
}

using CitizenFX.Core;

public class BxBaseScript : BaseScript
{
    new public static EventHandlerDictionary EventHandlers = null!;
    new public static StateBag GlobalState = null!;
    new public static ExportDictionary Exports = null!;
    new public static PlayerList Players = null!;

    #if CLIENT
    new public static Player LocalPlayer = null!;
    #endif

    public BxBaseScript()
    {
        EventHandlers = base.EventHandlers;
        GlobalState = base.GlobalState;
        Exports = base.Exports;
        Players = base.Players;
        
        #if CLIENT
        LocalPlayer = base.LocalPlayer;
        #endif
    }
}
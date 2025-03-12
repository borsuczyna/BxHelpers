using CitizenFX.Core;

public class BxBaseScript : BaseScript
{
    new public static EventHandlerDictionary EventHandlers;
    new public static StateBag GlobalState;
    new public static ExportDictionary Exports;
    new public static PlayerList Players;

    #if CLIENT
    new public static Player LocalPlayer;
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
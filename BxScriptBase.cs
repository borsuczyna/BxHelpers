using CitizenFX.Core;

public class BxBaseScript : BaseScript
{
    new private static EventHandlerDictionary EventHandlers;
    new private static StateBag GlobalState;
    new private static ExportDictionary Exports;
    new private static PlayerList Players;

    #if CLIENT
    new private static Player LocalPlayer;
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
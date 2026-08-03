namespace nba_mvc.Models
{
    public class ActionEvent : BaseId
    {
        public Game Game { get; set; }
        public Player Player { get; set; }
        public Team Team { get; set; }
        public int Quarter { get; set; }
        public TimeSpan GameTime { get; set; }
        public EventType EventType { get; set; }
    }

    public enum EventType
    {
        TwoPointShot,
        ThreePointShot,
        TwoPointMiss,
        ThreePointMiss,
        Assist,
        ReboundOff,
        ReboundDef,
        Steal,
        Block,
        Turnover,
        Foul,
        FreeThrowMiss,
        FreeThrowMade,
        SubstituteIn,
        SubstituteOut,
        FoulReceived,
    }
}
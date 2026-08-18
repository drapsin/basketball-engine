namespace nba_mvc.Dtos.Stats
{
    public class ShootingSplitDto
    {
        public int Made { get; set; }
        public int Attempted { get; set; }
        public double Percentage => Attempted == 0 ? 0 : Math.Round((double)Made / Attempted * 100, 1);
    }
}
namespace UFCTeams.Models
{
    public class Fighter
    {
        public string Name { get; set; }
        public string WeightClass { get; set; }
        public bool IsChampion { get; set; }
        public int Wins { get; set; }

        public Fighter(string name, string weightClass, bool isChampion, int wins)
        {

            Name = name;
            WeightClass = weightClass;
            IsChampion = isChampion;
            Wins = wins;
        }
    }
}

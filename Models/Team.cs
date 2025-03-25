using System;
using System.Collections.Generic;

namespace UFCTeams.Models
{
    public class Team
    {
        public string TeamName { get; set; }
        public List<Fighter> Fighters { get; set; }
        public static List<Team> SavedTeams { get; private set; } = new List<Team>();
        public string Coach { get; set; }

        public static Team Create(string teamName, Fighter fighter1, Fighter fighter2, string coach)
        {

            Team newTeam = new Team
            {
                TeamName = teamName,
                Fighters = new List<Fighter> { fighter1, fighter2 },
                Coach = coach
            };

            SavedTeams.Add(newTeam);
            return newTeam;
        }

        public static bool Delete(string teamName)
        {
            var teamToRemove = SavedTeams.Find(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase));
            if (teamToRemove != null)
            {
                SavedTeams.Remove(teamToRemove);
                return true;
            }
            return false;
        }
    }
}

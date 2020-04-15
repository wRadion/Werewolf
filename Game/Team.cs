using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Werewolf.Game
{
    public class Team : IEquatable<Team>
    {
        private static readonly Dictionary<int, Team> allTeams;
        public static Team GetTeamById(int id)
        {
            if (!allTeams.ContainsKey(id)) throw new KeyNotFoundException();
            return allTeams[id];
        }

        public static Team Village;
        public static Team Werewolf;
        public static Team Lover;

        static Team()
        {
            allTeams = new Dictionary<int, Team>();

            Village = new Team(0, "Village", Color.FromRgb(32, 160, 32));
            Werewolf = new Team(1, "Loup-garou", Color.FromRgb(160, 32, 32));
            Lover = new Team(2, "Amoureux", Color.FromRgb(192, 128, 192));
        }

        public int Id { get; }
        public string Name { get; }
        public Color Color { get; }

        public Team(int id, string name, Color color)
        {
            Id = id;
            Name = name;
            Color = color;

            allTeams.Add(id, this);
        }

        public bool Equals(Team other) => Id.Equals(other.Id);
    }
}

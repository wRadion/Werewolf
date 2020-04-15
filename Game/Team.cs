using System;
using System.Windows.Media;

namespace Werewolf.Game
{
    public class Team : IEquatable<Team>
    {
        public static Team Village = new Team("Village", Color.FromRgb(32, 160, 32));
        public static Team Werewolf = new Team("Loup-garou", Color.FromRgb(160, 32, 32));
        public static Team Lover = new Team("Amoureux", Color.FromRgb(192, 128, 192));

        public string Name { get; }
        public Color Color { get; }

        public Team(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        public bool Equals(Team other) => Name.Equals(other.Name);
    }
}

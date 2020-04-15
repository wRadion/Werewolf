using System;

namespace Werewolf.Game
{
    public class Role : IEquatable<Role>
    {
        public static Role Cupid = new Role("Cupidon", Team.Lover, true);
        public static Role Hunter = new Role("Chasseur", Team.Village, true);
        public static Role LittleGirl = new Role("Petite Fille", Team.Village, true);
        public static Role Seer = new Role("Voyante", Team.Village, true);
        public static Role Villager = new Role("Villageois", Team.Village, false);
        public static Role Werewolf = new Role("Loup-garou", Team.Werewolf, false);
        public static Role Witch = new Role("Sorcière", Team.Village, true);

        public string Name { get; }
        public Team DefaultTeam { get; }
        public bool IsUnique { get; }

        public Role(string name, Team defaultTeam, bool isUnique)
        {
            Name = name;
            DefaultTeam = defaultTeam;
            IsUnique = isUnique;
        }

        public override string ToString() => Name;

        public bool Equals(Role other) => Name.Equals(other.Name);
    }
}

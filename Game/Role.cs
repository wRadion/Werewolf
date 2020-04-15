using System;
using System.Linq;
using System.Collections.Generic;

namespace Werewolf.Game
{
    public class Role : IEquatable<Role>
    {
        private static readonly Dictionary<int, Role> allRoles;
        public static Role GetRoleById(int id)
        {
            if (!allRoles.ContainsKey(id)) throw new KeyNotFoundException();
            return allRoles[id];
        }
        public static Role[] GetAllRoles() => allRoles.Values.ToArray();

        public static Role Cupid;
        public static Role Hunter;
        public static Role LittleGirl;
        public static Role Seer;
        public static Role Villager;
        public static Role Werewolf;
        public static Role Witch;

        static Role()
        {
            allRoles = new Dictionary<int, Role>();

            Cupid = new Role(0, "Cupidon", Team.Village, true);
            Hunter = new Role(1, "Chasseur", Team.Village, true);
            LittleGirl = new Role(2, "Petite Fille", Team.Village, true);
            Seer = new Role(3, "Voyante", Team.Village, true);
            Villager = new Role(4, "Villageois", Team.Village, false);
            Werewolf = new Role(5, "Loup-garou", Team.Werewolf, false);
            Witch = new Role(6, "Sorcière", Team.Village, true);
        }

        public int Id { get; }
        public string Name { get; }
        public Team DefaultTeam { get; }
        public bool IsUnique { get; }

        public Role(int id, string name, Team defaultTeam, bool isUnique)
        {
            Id = id;
            Name = name;
            DefaultTeam = defaultTeam;
            IsUnique = isUnique;

            allRoles.Add(id, this);
        }

        public override string ToString() => Name;

        public bool Equals(Role other) => Id.Equals(other.Id);
    }
}

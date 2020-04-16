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

            Cupid = new Role(0, "Cupidon", Team.Village, true,
                "Lors de la première nuit, vous choisirez deux personnes. Ces deux personnes seront amoureuses, c'est-à-dire " +
                "que si l'une des personne meurt pendant la nuit ou le jour, l'autre personne se suicidera par chagrin. Et si" +
                "les deux amoureux survivent ensemble jusqu'à la fin, ils gagneront.");
            Hunter = new Role(1, "Chasseur", Team.Village, true,
                "Lorsque vous mourez, vous tuez une personne de votre choix.");
            LittleGirl = new Role(2, "Petite Fille", Team.Village, true,
                "Pendant la nuit, lorsque les loup-garous sont réveillés, vous pouvez voir les messages qu'ils s'envoient.");
            Seer = new Role(3, "Voyante", Team.Village, true,
                "Chaque nuit, vous pouvez découvrir le rôle d'un joueur de votre choix.");
            Villager = new Role(4, "Villageois", Team.Village, false,
                "Vous n'avez pas de compétence particulière.");
            Werewolf = new Role(5, "Loup-garou", Team.Werewolf, false,
                "Pendant la nuit, vous pouvez discuter avec les autres loup-garous et voter pour une personne. La personne " +
                "qui aura le plus de vote sera désigné comme cible et mourra le lendemain matin.");
            Witch = new Role(6, "Sorcière", Team.Village, true,
                "Pendant la nuit, vous avez le choix entre 3 actions : utiliser la potion de guérison; elle permet " +
                "d'empêcher la cible des loup-garous d'être tuée, utiliser la potion d'empoisonnement pour tuer la personne" +
                "de votre choix, ou ne rien faire. Vous n'avez qu'une seule potion de chaque pour toute la partie.");
        }

        public int Id { get; }
        public string Name { get; }
        public Team DefaultTeam { get; }
        public bool IsUnique { get; }
        public string Description { get; }

        public Role(int id, string name, Team defaultTeam, bool isUnique, string description)
        {
            Id = id;
            Name = name;
            DefaultTeam = defaultTeam;
            IsUnique = isUnique;
            Description = description;

            allRoles.Add(id, this);
        }

        public override string ToString() => Name;

        public bool Equals(Role other) => Id.Equals(other.Id);
    }
}

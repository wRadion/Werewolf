using System.Collections.Generic;
using System.Linq;

using Werewolf.Network;

namespace Werewolf.Game
{
    public class Game
    {
        #region Singleton
        private static Game _instance = null;
        public static Game Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Game();
                return _instance;
            }
        }
        #endregion Singleton

        private readonly List<Player> _players;
        private readonly List<Role> _roles;

        private Game()
        {
            _players = new List<Player>();
            _roles = new List<Role>();
        }

        public void AddPlayer(User user) => _players.Add(new Player(user));
        public void RemovePlayer(string name) => _players.RemoveAll((p) => p.Name == name);
        public void RemovePlayer(User user) => RemovePlayer(user.Name);

        public void AddRole(Role role) => _roles.Add(role);
        public void RemoveRole(Role role) => _roles.Remove(role);
        public bool ContainsRole(Role role) => _roles.Any((r) => r.Name == role.Name);
        public List<Role> GetRoles() => new List<Role>(_roles);

        public bool ValidateRoles()
        {
            int villagersCount = _roles.Count((r) => r.DefaultTeam == Team.Village);
            int werewolvesCount = _roles.Count((r) => r.DefaultTeam == Team.Werewolf);

            return villagersCount > werewolvesCount && werewolvesCount >= 1;
        }
    }
}

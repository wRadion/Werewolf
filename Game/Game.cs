using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Werewolf.Game.Exceptions;
using Werewolf.Network;
using Werewolf.Network.Events;

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
                if (!Server.Instance.Started) throw new InvalidOperationException("Game cannot be used in Clients");
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
        public List<Player> GetPlayers() => _players;

        public void AddRole(Role role) => _roles.Add(role);
        public void RemoveRole(Role role) => _roles.Remove(role);
        public bool ContainsRole(Role role) => _roles.Any((r) => r.Name == role.Name);
        public List<Role> GetRoles() => new List<Role>(_roles);

        public void SendEvent<TEventArgs>(TEventArgs args) where TEventArgs : ServerToClientEventArgs
        {
            foreach (Player player in _players)
                player.User.SendEvent(args);
        }

        public void SendEvent<TEventArgs>(TEventArgs args, Role role) where TEventArgs : ServerToClientEventArgs
        {
            foreach (Player player in _players.Where((p) => p.Role == role))
                player.User.SendEvent(args);
        }

        public void SendEvent<TEventArgs>(TEventArgs args, Tag tag) where TEventArgs : ServerToClientEventArgs
        {
            foreach (Player player in _players.Where((p) => p.IsTagged(tag)))
                player.User.SendEvent(args);
        }

        public void ValidateRoles()
        {
            int villagersCount = _roles.Count((r) => r.DefaultTeam == Team.Village);
            int werewolvesCount = _roles.Count((r) => r.DefaultTeam == Team.Werewolf);
            int rolesCount = _roles.Count;
            int playersCount = _players.Count;

            //if (werewolvesCount < 1) throw new NotEnoughWerewolfException();
            //if (werewolvesCount >= villagersCount) throw new TooMuchWerewolfException();
            //if (rolesCount < playersCount) throw new NotEnoughRolesException();
            //if (rolesCount > playersCount) throw new TooMuchRolesException();
        }

        public void AssignRolesRandomly()
        {
            Random rand = new Random();
            List<Role> roles = new List<Role>(_roles);

            foreach (Player player in _players)
            {
                int randomIndex = rand.Next(0, roles.Count);
                player.Role = roles[randomIndex];
                roles.RemoveAt(randomIndex);
            }
        }

        public async void StartGameLoop()
        {
            for (int i = 15; i >= -1; --i)
            {
                SendEvent(new TimerUpdatedEventArgs(i));
                await Task.Delay(1000);
            }
        }
    }
}

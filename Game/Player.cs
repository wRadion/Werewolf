using System;
using Werewolf.Network;

namespace Werewolf.Game
{
    public class Player : IEquatable<Player>
    {
        public User User { get; }
        private Role _role;
        private Tag _tag;
        private Team _overrideTeam;

        public string Name => User.Name;
        public Role Role
        {
            get => _role;
            set => _role = value;
        }
        public Team Team => _overrideTeam ?? _role.DefaultTeam;

        public Player(User user)
        {
            User = user;
            _role = null;
            _tag = Werewolf.Game.Tag.NONE;
            _overrideTeam = null;
        }

        public void Tag(Tag tag)
        {
            _tag |= tag;

            if (tag == Werewolf.Game.Tag.LOVER)
                _overrideTeam = Team.Lover;
        }
        public void RemoveTag(Tag tag) => _tag ^= tag;
        public bool IsTagged(Tag tag) => _tag.HasFlag(tag);

        public bool Equals(Player other) => Name.Equals(other.Name);
    }
}

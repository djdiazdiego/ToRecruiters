using Core.BaseModels;
using System;
using System.Collections.Generic;

namespace PlayerHub.Domain
{
    public sealed class Player : AggregateRoot<Guid>
    {
        private readonly List<Skill> _skills = new List<Skill>();

        public string Name { get; private set; } = null!;
        public PositionValue Position { get; private set; }

        public IReadOnlyCollection<Skill> Skills => _skills;

        public static Player New(
           string name,
           PositionValue position,
           IEnumerable<Skill> skills)
        {
            var player = new Player
            {
                Name = name,
                Position = position
            };

            player._skills.AddRange(skills);
            player.SetCreationDate();

            return player;
        }

        public void Update(
           string name,
           PositionValue position,
           IEnumerable<Skill> skills)
        {
            Name = name;
            Position = position;

            _skills.Clear();
            _skills.AddRange(skills);

            SetLastUpdateDate();
        }
    }
}

using Core.BaseModels;

namespace PlayerHub.Domain
{
    public sealed class Skill : Enumeration<SkillValue>
    {
        public Skill() : base()
        {
        }

        public Skill(SkillValue value, string name) : base(value, name)
        {
            SetCreationDate();
        }

        public int Value => Id.GetHashCode();
    }
}

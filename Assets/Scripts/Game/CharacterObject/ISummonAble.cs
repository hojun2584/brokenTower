

namespace Hojun
{

    public interface IAttackAble 
    {
        public void Attack(IHitAble hitObject);
    }

    public interface IHitAble
    {
        public void Hit(float hitObject);
    }

    public interface IDeadAble
    {
        public void Dead();
    }

    public interface IMoveAble
    {
        public void Move();
    }

    public interface IflightAble
    {
        public void Flight();
    }



}
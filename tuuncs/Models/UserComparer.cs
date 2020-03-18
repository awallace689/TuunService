using System.Collections.Generic;
namespace tuuncs.Models
{
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User first, User second)
        {
            return first.Username == second.Username ? true : false;
        }

        public int GetHashCode(User track)
        {
            return track.Username.GetHashCode();
        }
    }
}
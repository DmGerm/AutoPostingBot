using AutoMapper;
using AutoPost_Bot.Models;
using AutoPost_Bot.Users;

namespace AutoPost_Bot.MappingProfiles
{
    public class PasswordHashResolver : IValueResolver<UserEditModel, UserModel, byte[]>
    {
        private readonly IUsersRepo _repo;

        public PasswordHashResolver(IUsersRepo repo)
        {
            _repo = repo;
        }

        public byte[] Resolve(UserEditModel source, UserModel destination, byte[] destMember, ResolutionContext context)
        {
            if (source.PasswordChanged == false || string.IsNullOrEmpty(source.Password))
            {
                destination.PasswordSalt = source.PasswordSalt;
                return destination.PasswordHash;
            }
            _repo.GeneratePasswordHash(source.Password, out var salt);
            destination.PasswordSalt = salt;
            return destination.PasswordHash;
        }
    }
}

using Mapster;
using pyttogpanne_api.Features.Users;
using pyttogpanne_api.Models;

namespace pyttogpanne_api.Profiles
{
    public class MappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<InviteUserDto, User>()
                .Map(dest => dest.UserName, src => src.Email)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Ignore(dest => dest.Id!);
        }
    }
}

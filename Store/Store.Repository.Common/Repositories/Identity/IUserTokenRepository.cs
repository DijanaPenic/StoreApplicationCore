using Store.Model.Common.Models.Identity;

namespace Store.Repository.Common.Repositories.Identity
{
    public interface IUserTokenRepository : IIdentityRepository<IUserToken, IUserTokenKey>
    {
    }
}
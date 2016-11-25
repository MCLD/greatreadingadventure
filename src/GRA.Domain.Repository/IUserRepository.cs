namespace GRA.Domain.Repository
{
    public interface IUserRepository : IRepository<Model.User>
    {
        void SetUserPassword(int currentUserId, string password);
        void AddRole(int currentUserId, int userId, int roleId);
        Model.User GetByUsername(string username);
        Model.AuthenticationResult AuthenticateUser(string username, string password);
    }
}

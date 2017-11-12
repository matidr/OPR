namespace IUserService
{
    public interface IUserService
    {
        void AddUser(string name, string password);
        void EditUser(string name, string password);
        void DeleteUser(string username);
        string ListUsers();
    }
}

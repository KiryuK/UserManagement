using Application;

namespace WebAPI.Services;

public class GraphUserService : IUserService
{
    // Hier kommt später der GraphServiceClient rein

    public async Task<List<User>> SearchAsync(string term)
    {
        // Später: _graphClient.Users.GetAsync(...)
        throw new NotImplementedException("Graph kommt bald!");
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        // Später: _graphClient.Users[id].GetAsync()
        throw new NotImplementedException("Graph kommt bald!");
    }
}
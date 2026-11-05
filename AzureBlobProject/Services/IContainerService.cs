namespace AzureBlobProject.Services
{
    public interface IContainerService
    {
        Task<List<string>> GetAllCotainerAndBlobs();
        Task<List<string>> GetAllContainer();
        Task CreateContainer(string containerName);
        Task DeleteContainer(string containerName);
    }
}

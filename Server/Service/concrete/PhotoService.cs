using Server.model.habit;

namespace Server.service;

public interface IPhotoService
{
    Task<string?> UploadProfilePicture(string sessionKey, IFormFile photo);

    Task<string?> GetProfilePicture(string sessionKey);
}
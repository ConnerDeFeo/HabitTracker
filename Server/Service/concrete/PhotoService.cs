using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using MongoDB.Driver;
using Server.model.user;
using Server.service.utils;

namespace Server.service.concrete;

public class PhotoService(IAmazonS3 s3Client, IMongoDatabase _database)
{
    private readonly IMongoCollection<User> _users = _database.GetCollection<User>("Users");
    private readonly IAmazonS3 _s3Client = s3Client;
    private readonly string _bucketName = "habit-tracker-photos";

    /// <summary>
    /// Uploads a profile photo to the S3 bucket.
    /// The photo is stored in the format "profile_photos/{userId}/{uniqueFileName}.{fileExtension}".
    /// </summary>
    /// <param name="sessionKey">session key of the given user</param>
    /// <param name="file">photo being uploaded</param>
    /// <returns></returns>
    public async Task<bool> UploadProfilePhoto(string sessionKey, IFormFile file)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        //File must exist and not be empty, user must be logged in
        if (file is null || file.Length == 0 || user is null)
            return false;

        string key = $"profilePhotos/{user.Id}";
        //send as stream
        using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        //Upload the new photo, transfer utility is used to handle poltentially larger files
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = key,
            BucketName = _bucketName,
            ContentType = file.ContentType
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        //This is where the front end can find the photo
        return true;
    }

}
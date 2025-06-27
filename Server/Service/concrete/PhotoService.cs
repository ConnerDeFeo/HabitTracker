
using Amazon.S3;
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
    public async Task<string?> UploadProfilePhoto(string sessionKey, IFormFile file)
    {
        var userId = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        //File must exist and not be empty, user must be logged in
        if (file == null || file.Length == 0 || userId == null)
            return null;

        var key = $"profile_photos/{userId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = key,
            BucketName = _bucketName,
            ContentType = file.ContentType,
            CannedACL = S3CannedACL.PublicRead // Optional: controls access level
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        string url = $"https://{_bucketName}.s3.amazonaws.com/{key}";
        return url;
    }
}
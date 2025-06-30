
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
    public async Task<string?> UploadProfilePhoto(string sessionKey, IFormFile file)
    {
        User? user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        //File must exist and not be empty, user must be logged in
        if (file is null || file.Length == 0 || user is null)
            return null;

        var key = $"profilePhotos/{Guid.NewGuid()}";

        //send as stream
        using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        //Delete the previous profile picture uploaded
        if (user.ProfilePhotoKey is not null)
        { 
            DeleteObjectRequest deleteRequest = new()
            {
                BucketName = _bucketName,
                Key = user.ProfilePhotoKey
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

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
        string url = $"https://{_bucketName}.s3.amazonaws.com/{key}";
        await _users.UpdateOneAsync(
            u => u.SessionKey == sessionKey,
            Builders<User>.Update.Set(u => u.ProfilePhotoKey, key)
        );

        return url;
    }

    public async Task<string?> GetProfilePhoto(string sessionKey)
    {
        var user = await UserUtils.GetUserBySessionKey(sessionKey, _users);
        if (user == null || string.IsNullOrEmpty(user.ProfilePhotoKey))
            return null;

        return $"https://{_bucketName}.s3.amazonaws.com/{user.ProfilePhotoKey}";
    }
}
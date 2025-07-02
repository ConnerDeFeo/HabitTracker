namespace Test.service;
using Server.service.utils;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class TestPasswordHasher
{
    [Fact]
    public void TestHashing(){
        string hashedPassword = PasswordHasher.HashPassword("TestPassword");

        Assert.True(PasswordHasher.VerifyPassword("TestPassword", hashedPassword));
    }
}
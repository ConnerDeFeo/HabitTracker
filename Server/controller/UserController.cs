namespace Server.controller;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Server.service;


[Route("api/users")]
[ApiController]
public class UserController(MongoDbService mongoService) : ControllerBase
{
    private readonly MongoDbService _mongoService = mongoService;
}
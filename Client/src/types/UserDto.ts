type UserDto = {
    username: string,
    dateCreated: string,
    id: string
    friends: Record<string,string | undefined>,
    friendRequests: Record<string,string | undefined>,
    friendRequestsSent: string[]
}

export default UserDto;
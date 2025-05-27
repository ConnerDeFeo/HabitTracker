
//This covers the hold screen with a blur and puts the . . . for the user
const Waiting = ()=>{
    return(
        <div className="absolute h-[100%] w-[100%] right-0 backdrop-blur-[1.5px] align-center">
            <p className="text-center my-auto mt-[40vh] text-9xl">. . .</p>
        </div>
    );
}

export default Waiting;
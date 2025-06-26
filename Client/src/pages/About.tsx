const About = ()=>{
    return(
        <div className="grid w-[90%] mx-auto mb-10">
            <p className="text-6xl text-center my-10">Tech Stack</p>
            <div className="grid gap-y-5 md:flex">
                <img src="./mongo.jpg" alt="mongo" className="h-40 w-70 m-auto"/>
                <img src="./AspNet.svg" alt="asp net" className="h-40 w-70 m-auto"/>
                <img src="./reactTs.png" alt="react ts" className="h-40 w-70 m-auto"/>
            </div>
            <p className="text-6xl text-center my-10">Hosting Services</p>
            <div className="grid md:flex">
                <img src="./MongoAtlas.webp" alt="mongo atlas" className="h-40 w-40 m-auto"/>
                <img src="./beanstalk.webp" alt="beanstalk" className="h-60 w-80 m-auto"/>
                <img src="./amplify.png" alt="amplify" className="h-40 w-40 m-auto"/>
            </div>
        </div>
    );
}

export default About;
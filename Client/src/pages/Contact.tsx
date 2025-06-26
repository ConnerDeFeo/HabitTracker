import { useState } from "react";
import Input from "../components/Input";

const Contact = ()=>{
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");

    const textSizing = "text-4xl";
    const infoSizing = "text-3xl";

    return(
        <div className="my-10">
            <div className="grid w-[85%] min-w-[300px] mx-auto gap-y-7">
                <p className="text-6xl">Reach Out!</p>
                <Input value={name} updateValue={setName} placeholder="name" type="text"/>
                <Input value={email} updateValue={setEmail} type="email" placeholder="email"/>
                <textarea className="habitBorder p-3 h-25 resize-none" placeholder="message" value={message} onChange={(e)=>setMessage(e.target.value)}/>
            </div>
            <div className="w-[85%] border-y-3 mx-auto mt-10 md:h-40 md:flex md:items-center">
                <div className="md:w-[90%] mx-auto grid grid-cols-2 gap-y-2 md:flex md:justify-between text-center">
                    <div className="col-start-1 mx-auto">
                        <p className={textSizing}>Phone</p>
                        <p className={infoSizing}>315-879-7067</p>
                    </div>
                    <div className="row-start-2 col-span-full mx-auto">
                        <p className={textSizing}>Website</p>
                        <div className="flex">
                            <a href="https://connerdefeo.com" target="_blank" className={`${infoSizing} font-hand crossOut text-blue-500`}>connerdefeo.com</a>
                        </div>
                    </div>
                    <div className="col-start-2 row-start-1 mx-auto">
                        <p className={textSizing}>Email</p>
                        <p className={infoSizing}>jjd2843@rit.edu</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Contact;
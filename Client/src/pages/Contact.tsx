import { useState } from "react";
import Input from "../components/General/Input";
import emailjs from 'emailjs-com';

const Contact = ()=>{
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [message, setMessage] = useState("");

    const textSizing = "text-4xl";
    const infoSizing = "text-3xl";

    async function sendEmail(e:React.FormEvent<HTMLFormElement>){
        e.preventDefault();
        const form = e.target as HTMLFormElement;
        if (!form) 
            return;
        try {
            await emailjs.sendForm(
                'service_ho7d22t',
                'template_blol2zp',
                form,
                'dHmN6Poxt2OKm6_FU'
            );
            window.location.reload();
        } catch (error) {
            console.error('Email error:', error);
        }
    }

    return(
        <div className="my-10">
            <form className="grid w-[85%] min-w-[300px] max-w-[760px] mx-auto gap-y-7" onSubmit={sendEmail}>
                <p className="text-6xl">Reach Out!</p>
                <Input value={name} updateValue={setName} placeholder="name" type="text" name="from_name"/>
                <Input value={email} updateValue={setEmail} type="email" placeholder="email" name="from_email"/>
                <textarea className="habitBorder p-3 h-25 resize-none" placeholder="message" name="message" value={message} onChange={(e)=>setMessage(e.target.value)}/>
                <input type="submit" className="w-30 ml-auto color-black border border-black font-hand bg-black text-white rounded-2xl cursor-pointer py-1 text-4xl "/>
            </form>
            <div className="w-[85%] border-y-3 mx-auto mt-10 md:h-40 md:flex md:items-center">
                <div className="md:w-[90%] mx-auto grid grid-cols-2 gap-y-2 md:flex md:justify-between text-center my-2">
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
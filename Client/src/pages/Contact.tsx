import { useState } from "react";
import Input from "../components/Input";

const Contact = ()=>{
    const [name, setName] = useState("")
    return(
        <div>
            <div className="grid w-[85%] min-w-[300px] border mx-auto">
                <p className="text-6xl">Reach Out!</p>
            </div>
            <div>

            </div>
        </div>
    );
}

export default Contact;
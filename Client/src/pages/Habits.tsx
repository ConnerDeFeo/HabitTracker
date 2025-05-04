import { useEffect, useState } from "react";
import Container from "../components/Container";

const Habits = ()=>{
    const [habits,setHabits] = useState();

    useEffect(()=>{
        const fetchHabits = async ()=>{

        }
        fetchHabits();
    },[])

    return(
        <Container content={
            <>
                <div className="grid grid-cols-2 grid-rows-2 text-center gap-4">
                    <div>1</div>
                    <div>1</div>
                    <div>1</div>
                    <div>1</div>
                </div>
                <button>Testing</button>
            </>
        }/>
            
    );
}

export default Habits;
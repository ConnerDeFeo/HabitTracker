import { useEffect } from "react";

const Schedule = ()=>{
    const days = ["Sun", "Mon","Tues","Wed","Thur","Fri","Sat"];

    useEffect(()=>{
        const fetchMonth = async ()=>{
            const yyyyMM: string = new Date().toISOString().split('T')[0].substring(0,7);

        }

        fetchMonth();
    },[])

    return(
        <div className="flex justify-between max-w-[75%] mx-auto mt-5">   
            {days.map((day)=><p className="text-4xl" key={day}>{day}</p>)}

        </div>
    );
}

export default Schedule;
import DateInfo from "../types/DateInfo";

//Some comonly needed formatin and date adjustment functions
const DateService={
    padZero: (num: number): string => String(num).padStart(2, '0'),
    increaseMonth: (date: DateInfo): DateInfo =>{
        const newMonth = date.month+1;
        let newYear = date.year;

        if(newMonth/12>=1)
            newYear+=1;

        return {
            day:date.day,
            month:newMonth%12,
            year:newYear
        }
    },
    decreaseMonth: (date: DateInfo): DateInfo =>{
        let newMonth = date.month-1;
        let newYear = date.year;

        if(newMonth<0){
            newMonth=11;
            newYear-=1;
        }

        return {
            day:date.day,
            month:newMonth,
            year:newYear
        }
    },
    increaseDay: (date: DateInfo): DateInfo =>{
        const newDate = new Date(date.year,date.month,date.day);
        newDate.setDate(newDate.getUTCDate()+1);

        return{
            day: newDate.getUTCDate(),
            month: newDate.getUTCMonth(),
            year: newDate.getUTCFullYear()
        }
    },
    decreaseDay: (date: DateInfo): DateInfo =>{
        const newDate = new Date(date.year,date.month,date.day);
        newDate.setDate(newDate.getUTCDate()-1);

        return{
            day: newDate.getUTCDate(),
            month: newDate.getUTCMonth(),
            year: newDate.getUTCFullYear()
        }
    },
    getUtcToday:():Date=>{
        const now = new Date();
        return new Date(Date.UTC(
            now.getUTCFullYear(),
            now.getUTCMonth(),
            now.getUTCDate()
        ));
    }
}

export default DateService;